﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using Cosmos.Debug.Common;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace Cosmos.VS.Windows
{
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// 
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]

    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]

    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]

    // This attribute registers a tool window exposed by this package.
    [ProvideToolWindow(typeof(AssemblyTW))]
    [ProvideToolWindow(typeof(RegistersTW))]
    [ProvideToolWindow(typeof(StackTW))]
    [ProvideToolWindow(typeof(InternalTW))]

    [Guid(GuidList.guidCosmos_VS_WindowsPkgString)]
    public sealed class Cosmos_VS_WindowsPackage : Package
    {
        Queue<byte> mCommand;
        Queue<byte[]> mMessage;
        System.Timers.Timer mTimer = new System.Timers.Timer(100);
        /// <summary>A pipe server that will receive responses from the AD7Process</summary>
        Cosmos.Debug.Common.PipeServer mPipeDown;

        private StateStorer mStateStorer;
        public StateStorer StateStorer
        {
            get
            {
                return mStateStorer;
            }
        }

        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        public Cosmos_VS_WindowsPackage()
        {
            mCommand = new Queue<byte>();
            mMessage = new Queue<byte[]>();

            // There are a lot of threading issues in VSIP, and the WPF dispatchers do not work.
            // So instead we use a stack and a timer to poll it for data.
            mTimer.AutoReset = true;
            mTimer.Elapsed += new System.Timers.ElapsedEventHandler(ProcessMessage);
            mTimer.Start();

            mPipeDown = new Cosmos.Debug.Common.PipeServer(Pipes.DownName);
            mPipeDown.DataPacketReceived += new Action<byte, byte[]>(PipeThread_DataPacketReceived);
            mPipeDown.Start();

            mStateStorer = new StateStorer();
        }

        private ToolWindowPane2 FindWindow(Type aWindowType)
        {
            try
            {

            }
            catch (System.Reflection.TargetInvocationException e)
            {
                Exception innerException = e.InnerException;
                Global.OutputPane.OutputString(innerException.Message + "\r\n");
                Global.OutputPane.OutputString(innerException.StackTrace + "\r\n");
            }
            // Get the instance number 0 of this tool window.
            // Our windows are single instance so this instance will be the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            var xWindow = FindToolWindow(aWindowType, 0, true);
            if ((xWindow == null) || (xWindow.Frame == null))
            {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }
            return xWindow as ToolWindowPane2;
        }

        private void ShowWindow(Type aWindowType)
        {
            var xWindow = FindWindow(aWindowType);
            var xFrame = (IVsWindowFrame)xWindow.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(xFrame.Show());
        }

        private void UpdateWindow(Type aWindowType, string aTag, byte[] aData)
        {
            System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal,
                (Action)delegate()
                {
                    var xWindow = FindWindow(aWindowType);
                    xWindow.UserControl.Package = this;
                    xWindow.UserControl.Update(aTag, aData);
                }
            );
        }

        private void ShowWindowAssembly(object aCommand, EventArgs e)
        {
            System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal,
                (Action)delegate()
                {
                    ShowWindow(typeof(AssemblyTW));
                }
            );
        }

        private void ShowWindowInternal(object aCommand, EventArgs e)
        {
            System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal,
                (Action)delegate()
                {
                    ShowWindow(typeof(InternalTW));
                }
            );
        }

        private void ShowWindowRegisters(object aCommand, EventArgs e)
        {
            System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal,
                (Action)delegate()
                {
                    ShowWindow(typeof(RegistersTW));
                }
            );
        }

        private void ShowWindowStack(object aCommand, EventArgs e)
        {
            System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal,
                (Action)delegate()
                {
                    ShowWindow(typeof(StackTW));
                }
            );
        }

        private void ShowWindowAll(object aCommand, EventArgs e)
        {
            ShowWindowAssembly(aCommand, e);
            ShowWindowRegisters(aCommand, e);
            ShowWindowStack(aCommand, e);
            // Dont show Internal Window, most Cosmos users wont use it.
        }

        private void AddCommand(OleMenuCommandService aMcs, uint aCmdID, EventHandler aHandler)
        {
            // Create the command for the assembly tool window
            var xCmdID = new CommandID(GuidList.guidCosmosMenu, (int)aCmdID);
            var xMenuCmd = new MenuCommand(aHandler, xCmdID);
            aMcs.AddCommand(xMenuCmd);
        }

        // Initialization of the package; this method is called right after the package is sited, so this is the place
        // where you can put all the initilaization code that rely on services provided by VisualStudio.
        protected override void Initialize()
        {
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            var xMcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (xMcs != null)
            {
                AddCommand(xMcs, PkgCmdIDList.cmdidCosmosAssembly, ShowWindowAssembly);
                AddCommand(xMcs, PkgCmdIDList.cmdidCosmosRegisters, ShowWindowRegisters);
                AddCommand(xMcs, PkgCmdIDList.cmdidCosmosStack, ShowWindowStack);
                AddCommand(xMcs, PkgCmdIDList.cmdidCosmosInternal, ShowWindowInternal);
                AddCommand(xMcs, PkgCmdIDList.cmdidCosmosShowAll, ShowWindowAll);
            }

            // Create Cosmos output pane
            var xDTE = (EnvDTE80.DTE2)Package.GetGlobalService(typeof(EnvDTE.DTE));
            var xPane = xDTE.ToolWindows.OutputWindow.OutputWindowPanes;
            Global.OutputPane = xPane.Add("Cosmos");
            Global.OutputPane.OutputString("Debugger windows loaded.\r\n");
        }

        void ProcessMessage(object sender, EventArgs e)
        {
            byte xCmd;
            byte[] xMsg;
            while (true)
            {
                lock (mCommand)
                {
                    if (mCommand.Count == 0)
                    {
                        break;
                    }
                    xCmd = mCommand.Dequeue();
                    xMsg = mMessage.Dequeue();
                }

                switch (xCmd)
                {
                    case Debugger2Windows.Noop:
                        break;

                    case Debugger2Windows.Stack:
                        System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal,
                            (Action)delegate()
                            {
                                UpdateWindow(typeof(StackTW), "STACK", xMsg);
                            }
                        );
                        break;

                    case Debugger2Windows.Frame:
                        System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal,
                            (Action)delegate()
                            {
                                UpdateWindow(typeof(StackTW), "FRAME", xMsg);
                            }
                        );
                        break;

                    case Debugger2Windows.Registers:
                        System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal,
                            (Action)delegate()
                            {
                                UpdateWindow(typeof(RegistersTW), null, xMsg);
                            }
                        );
                        break;

                    case Debugger2Windows.Quit:
                        //Close();
                        break;

                    case Debugger2Windows.AssemblySource:
                        UpdateWindow(typeof(AssemblyTW), null, xMsg);
                        break;

                    case Debugger2Windows.PongVSIP:
                        UpdateWindow(typeof(InternalTW), null, Encoding.UTF8.GetBytes("Pong from VSIP"));
                        break;

                    case Debugger2Windows.PongDebugStub:
                        UpdateWindow(typeof(InternalTW), null, Encoding.UTF8.GetBytes("Pong from DebugStub"));
                        break;

                    case Debugger2Windows.OutputPane:
                        System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal,
                          (Action)delegate()
                        {
                            Global.OutputPane.OutputString(System.Text.Encoding.UTF8.GetString(xMsg));
                        }
                        );
                        break;

                    case Debugger2Windows.OutputClear:
                        System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal,
                          (Action)delegate()
                        {
                            Global.OutputPane.Clear();
                            StateStorer.ClearState();
                        }
                        );
                        break;
                }
            }
        }

        void PipeThread_DataPacketReceived(byte aCmd, byte[] aMsg)
        {
            lock (mCommand)
            {
                mCommand.Enqueue(aCmd);
                mMessage.Enqueue(aMsg);
            }
        }


        public void StoreAllStates()
        {
            var cWindow = FindWindow(typeof(StackTW));
            byte[] aData = cWindow.UserControl.GetCurrentState();
            StateStorer.StoreState("StackTW", aData == null ? null : (byte[])aData.Clone());

            cWindow = FindWindow(typeof(RegistersTW));
            aData = cWindow.UserControl.GetCurrentState();
            StateStorer.StoreState("RegistersTW", aData == null ? null : (byte[])aData.Clone());
        }
        public void RestoreAllStates()
        {
            var cWindow = FindWindow(typeof(StackTW));
            byte[] aData = StateStorer.RetrieveState(StateStorer.CurrLineId, "StackTW");
            cWindow.UserControl.SetCurrentState(aData == null ? null : (byte[])aData.Clone());
            
            cWindow = FindWindow(typeof(RegistersTW));
            aData = StateStorer.RetrieveState(StateStorer.CurrLineId, "RegistersTW");
            cWindow.UserControl.SetCurrentState(aData == null ? null : (byte[])aData.Clone());
        }
    }
}