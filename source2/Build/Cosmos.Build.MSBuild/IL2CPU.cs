﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Reflection;
using Cosmos.Compiler.Assembler;
using Cosmos.Compiler.Assembler.X86;
using System.IO;
using Cosmos.Build.Common;
using Microsoft.Win32;
using Cosmos.IL2CPU.X86;
using Cosmos.IL2CPU;

namespace Cosmos.Build.MSBuild
{
    public class IL2CPU : AppDomainIsolatedTask
    {
        private static bool mFirstTime = true;
        public IL2CPU()
        {
//            CheckFirstTime();

            
        }

        private static void CheckFirstTime()
        {
            if (mFirstTime)
            {
//                mFirstTime = false;
                var xSearchDirs = new List<string>();
                xSearchDirs.Add(Path.GetDirectoryName(typeof(IL2CPU).Assembly.Location));

                using (var xReg = Registry.LocalMachine.OpenSubKey("Software\\Cosmos", false))
                {
                    var xPath = (string)xReg.GetValue(null);
                    xSearchDirs.Add(xPath);
                    xSearchDirs.Add(Path.Combine(xPath, "Kernel"));
                }
                mSearchDirs = xSearchDirs.ToArray();

                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

                Assembly.LoadWithPartialName("Cosmos.Sys");
                Assembly.LoadWithPartialName("Cosmos.Hardware");
                Assembly.LoadWithPartialName("Cosmos.Kernel");
                Assembly.LoadWithPartialName("Cosmos.Sys.FileSystem");
            }
        }

        private static string[] mSearchDirs = new string[0];

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var xShortName = args.Name;
            if (xShortName.Contains(','))
            {
                xShortName = xShortName.Substring(0, xShortName.IndexOf(','));
                // TODO: remove following statement if it proves unnecessary
                if (xShortName.Contains(','))
                {
                    throw new Exception("Algo error");
                }
            }
            foreach (var xDir in mSearchDirs)
            {
                var xPath = Path.Combine(xDir, xShortName + ".dll");
                if (File.Exists(xPath))
                {
                    return Assembly.LoadFrom(xPath);
                }
                xPath = Path.Combine(xDir, xShortName + ".exe");
                if (File.Exists(xPath))
                {
                    return Assembly.LoadFrom(xPath);
                }
            }
            if (mStaticLog != null)
            {
                mStaticLog("Assembly '" + args.Name + "' not resolved!");
            }
            return null;
        }

        private static Action<string> mStaticLog = null;

        private void DoInitTypes()
        {
            var xType = typeof(Cosmos.Hardware.Plugs.FCL.System.Console);
            Log.LogMessage("Console plug loaded: {0}", xType.FullName);
            xType = typeof(Cosmos.Sys.Plugs.Deboot);
            Log.LogMessage("Deboot plug loaded: {0}", xType.FullName);
            xType = typeof(Cosmos.Kernel.Plugs.ArrayListImpl);
            Log.LogMessage("ArrayList plug loaded: {0}", xType.FullName);
        }

        #region properties
        [Required]
        public string InputAssembly
        {
            get;
            set;
        }

        [Required]
        public string DebugMode{
            get;
            set;
        }

        public string TraceAssemblies
        {
            get;
            set;
        }

        public byte DebugCom
        {
            get;
            set;
        }

        [Required]
        public bool UseNAsm
        {
            get;
            set;
        }

        public string DebugSymbolsFile
        {
            get;
            set;
        }

        public string LogFile
        {
            get;
            set;
        }

        [Required]
        public string OutputFile
        {
            get;
            set;
        }

        public ITaskItem[] References
        {
            get;
            set;
        }

        #endregion

        private bool Initialize()
        {
            CheckFirstTime();            
            DoInitTypes();
            // load searchpaths:
            Log.LogMessage("SearchPath: '{0}'", References);
            if (References!=null)
            {
                var xSearchPaths = new List<string>(mSearchDirs);
                foreach (var xRef in References)
                {
                    if (xRef.MetadataNames.OfType<string>().Contains("FullPath"))
                    {
                        var xDir = Path.GetDirectoryName(xRef.GetMetadata("FullPath"));
                        if (!xSearchPaths.Contains(xDir))
                        {
                            xSearchPaths.Add(xDir);
                        }
                    }
                }
                mSearchDirs = xSearchPaths.ToArray();
            }
            if (String.IsNullOrEmpty(DebugMode))
            {
                mDebugMode = Cosmos.Build.Common.DebugMode.None;
            }
            else
            {
                if (!Enum.GetNames(typeof(DebugMode)).Contains(DebugMode, StringComparer.InvariantCultureIgnoreCase))
                {
                    Log.LogError("Invalid DebugMode specified");
                    return false;
                }
                mDebugMode = (DebugMode)Enum.Parse(typeof(DebugMode), DebugMode);
            }
            if (String.IsNullOrEmpty(TraceAssemblies))
            {
                mTraceAssemblies = Cosmos.Build.Common.TraceAssemblies.User;
            }
            else
            {
                if (!Enum.GetNames(typeof(TraceAssemblies)).Contains(TraceAssemblies, StringComparer.InvariantCultureIgnoreCase))
                {
                    Log.LogError("Invalid TraceAssemblies specified");
                    return false;
                }
                mTraceAssemblies = (TraceAssemblies)Enum.Parse(typeof(TraceAssemblies), TraceAssemblies);
            }
            AppDomain.CurrentDomain.AppendPrivatePath(Path.GetDirectoryName(InputAssembly));
            return true;
        }


        private DebugMode mDebugMode = Cosmos.Build.Common.DebugMode.None;
        private TraceAssemblies mTraceAssemblies = Cosmos.Build.Common.TraceAssemblies.All;
        private void LogTime(string message)
        {
            //
        }
        public override bool Execute()
        {
            try
            {
                Log.LogMessage("Executing IL2CPU on assembly");
                if (!Initialize())
                {
                    return false;
                }

                LogTime("Engine execute started");
                Assembly xEntryAsm;
                if (Path.IsPathRooted(InputAssembly))
                {
                    xEntryAsm = Assembly.LoadFrom(InputAssembly);
                }
                else
                {
                    xEntryAsm = Assembly.Load(Path.GetFileNameWithoutExtension(InputAssembly));
                }
                var xInitMethod = xEntryAsm.EntryPoint.DeclaringType.GetMethod("Init", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                var xAsm = new AppAssemblerNasm(DebugCom);
                xAsm.DebugMode = mDebugMode;
                xAsm.TraceAssemblies = mTraceAssemblies;
#if OUTPUT_ELF
                xAsm.EmitELF = true;
#endif

                var xNasmAsm = (AssemblerNasm)xAsm.Assembler;
                xAsm.Assembler.Initialize();
                using (var xScanner = new ILScanner(xAsm))
                {
                    xScanner.TempDebug += x => Log.LogMessage(x);
                    if (!String.IsNullOrEmpty(LogFile))
                    {
                        xScanner.EnableLogging(LogFile);
                    }
                    xScanner.Execute(xInitMethod);

                    using (var xOut = new StreamWriter(OutputFile, false))
                    {
                        if (!String.IsNullOrEmpty(DebugSymbolsFile))
                        {
                            xNasmAsm.FlushText(xOut);
                            xAsm.WriteDebugSymbols(DebugSymbolsFile);
                        }
                        else
                        {
                            xAsm.Assembler.FlushText(xOut);
                        }
                    }
                }
                LogTime("Engine execute finished");
                return true;
            }
            catch (Exception E)
            {
                Log.LogMessage("Loaded assemblies: ");
                foreach (var xAsm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Log.LogMessage(xAsm.Location);
                }
                Log.LogErrorFromException(E, true);
                return false;
            }
        }
    }
}