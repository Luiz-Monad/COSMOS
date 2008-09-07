﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Indy.IL2CPU;
using System.Windows.Threading;

namespace Cosmos.Build.Windows {
    public partial class OptionsWindow : Window {
        [DllImport("user32.dll")]
        public static extern int ShowWindow(int Handle, int showState);

        [DllImport("kernel32.dll")]
        public static extern int GetConsoleWindow();

        protected Block mOptionsBlockPrefix;
        protected Builder mBuilder = new Builder();

        public static void Display() {
            int xConsoleWindow = GetConsoleWindow();
            ShowWindow(xConsoleWindow, 0);

            var xOptionsWindow = new OptionsWindow();
            bool xDoBuild = true;
            var xShowOptions = xOptionsWindow.chbxShowOptions.IsChecked.Value;
            // If the user doenst have the option to auto show, then look
            // for control key pressed
            if (!xShowOptions) {
                // We should use the WPF Keyboard.IsKeyDown, but it does not work here.
                // It appears that it gets initialized at some point later
                // or after a WPF window is shown, but it does not work here for sure
                // so instead we have to us an extern.
                xShowOptions = KeyState.IsKeyDown(System.Windows.Forms.Keys.RControlKey)
                    || KeyState.IsKeyDown(System.Windows.Forms.Keys.LControlKey);
            }
            if (xShowOptions) {
                xDoBuild = xOptionsWindow.ShowDialog().Value;
            }
            if (xDoBuild) {
                ShowWindow(xConsoleWindow, 1);
                xOptionsWindow.DoBuild();

                //Debug Window is only displayed if Qemu + Debug checked, or if other VM + Debugport selected
                bool xIsQemu = xOptionsWindow.rdioQEMU.IsChecked.Value;
                bool xUseQemuDebug = xOptionsWindow.cmboDebugMode.SelectedIndex > 0;
                if (((xIsQemu & xUseQemuDebug) | (!xIsQemu & (xOptionsWindow.mComPort > 0))) && xOptionsWindow.mDebugMode != DebugModeEnum.None) {
                    var xDebugWindow = new DebugWindow();
                    if (xOptionsWindow.mDebugMode == DebugModeEnum.Source) {
                        var xLabelByAddressMapping = ObjDump.GetLabelByAddressMapping(xOptionsWindow.mBuilder.BuildPath + "output.bin",
                                                                                      xOptionsWindow.mBuilder.ToolsPath + @"cygwin\objdump.exe");
                        var xSourceMappings = SourceInfo.GetSourceInfo(xLabelByAddressMapping,
                                                                       xOptionsWindow.mBuilder.BuildPath + "Tools/asm/debug.cxdb");
                        xDebugWindow.SetSourceInfoMap(xSourceMappings);
                    } else {
                        throw new Exception("Debug mode not supported: " + xOptionsWindow.mDebugMode);
                    }
                    xDebugWindow.ShowDialog();
                }
            }
        }

        protected void TargetChanged(object aSender, RoutedEventArgs e) {
            spnlDebugger.Visibility = Visibility.Visible;
            wpnlDebugPort.Visibility = Visibility.Visible;
            if (aSender == rdioQEMU) {
                wpnlDebugPort.Visibility = Visibility.Collapsed;
            }
            spnlQEMU.Visibility = aSender == rdioQEMU ? Visibility.Visible : Visibility.Collapsed;
            spnlVPC.Visibility = aSender == rdioVPC ? Visibility.Visible : Visibility.Collapsed;
            spnlISO.Visibility = aSender == rdioISO ? Visibility.Visible : Visibility.Collapsed;
            spnlPXE.Visibility = aSender == rdioPXE ? Visibility.Visible : Visibility.Collapsed;
            spnlUSB.Visibility = aSender == rdioUSB ? Visibility.Visible : Visibility.Collapsed;
            spnlVMWare.Visibility = aSender == rdioVMWare ? Visibility.Visible : Visibility.Collapsed;
        }

        public OptionsWindow() {
            InitializeComponent();

            Loaded += delegate(object sender, RoutedEventArgs e) {
                this.Activate();
            };

            butnBuild.Click += new RoutedEventHandler(butnBuild_Click);
            butnCancel.Click += new RoutedEventHandler(butnCancel_Click);

            rdioQEMU.Checked += new RoutedEventHandler(TargetChanged);
            rdioVMWare.Checked += new RoutedEventHandler(TargetChanged);
            rdioVPC.Checked += new RoutedEventHandler(TargetChanged);
            rdioISO.Checked += new RoutedEventHandler(TargetChanged);
            rdioPXE.Checked += new RoutedEventHandler(TargetChanged);
            rdioUSB.Checked += new RoutedEventHandler(TargetChanged);

            tblkBuildPath.Text = mBuilder.BuildPath;
            tblkISOPath.Text = mBuilder.BuildPath + "Cosmos.iso";

            var xDrives = System.IO.Directory.GetLogicalDrives();
            foreach (string xDrive in xDrives) {
                var xType = new System.IO.DriveInfo(xDrive);
                if (xType.IsReady) {
                    if ((xType.DriveType == System.IO.DriveType.Removable) && xType.DriveFormat.StartsWith("FAT")) {
                        cmboUSBDevice.Items.Add(xDrive);
                    }
                }
            }

            cmboDebugPort.Items.Add("Disabled");
            // MtW: for now, leave COM1 out, as COM1 is used by the Cosmos kernel to output debug messages
            // Kudzu: Need to configure that too....
            //cmboDebugPort.Items.Add("COM1");
            cmboDebugPort.SelectedIndex = cmboDebugPort.Items.Add("COM2");
            cmboDebugPort.Items.Add("COM3");
            cmboDebugPort.Items.Add("COM4");
            //cmboDebugPort.Items.Add("Ethernet 1");
            //cmboDebugPort.Items.Add("Ethernet 2");
            //cmboDebugPort.Items.Add("Ethernet 3");
            //cmboDebugPort.Items.Add("Ethernet 4");

            cmboDebugMode.SelectedIndex = cmboDebugMode.Items.Add("None");
            cmboDebugMode.Items.Add("IL");
            cmboDebugMode.Items.Add("Source");

            foreach (string xNIC in Enum.GetNames(typeof(Builder.QemuNetworkCard))) {
                cmboNetworkCards.Items.Add(xNIC);
            }
            foreach (string xSoundCard in Enum.GetNames(typeof(Builder.QemuAudioCard))) {
                cmboAudioCards.Items.Add(xSoundCard);
            }
            LoadSettingsFromRegistry();
        }

        private void butnBuild_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }

        private DebugModeEnum mDebugMode;
        private byte mComPort;

        protected void DoBuild() {
            SaveSettingsToRegistry();

            mComPort = (byte)cmboDebugPort.SelectedIndex;
            if (mComPort > 3) {
                throw new Exception("Debug port not supported yet!");
            }
            mComPort++;
            string xDebugMode = (string)cmboDebugMode.SelectedValue;
            mDebugMode = DebugModeEnum.None;
            if (xDebugMode == "IL") {
                mDebugMode = DebugModeEnum.IL;
                throw new NotSupportedException("Debug mode IL isn't supported yet, use Source instead.");
            } else if (xDebugMode == "Source") {
                mDebugMode = DebugModeEnum.Source;
                mComPort = 1;
            } else if (xDebugMode == "None") {
                mDebugMode = DebugModeEnum.None;
            } else {
                throw new Exception("Selected debug mode not supported!");
            }

            if (chbxCompileIL.IsChecked.Value) {
                var xMainWindow = new MainWindow();
                xMainWindow.Show();
                if (xMainWindow.PhaseBuild(mBuilder, mDebugMode, mComPort) == false) {
                    return;
                }
            }
            if (rdioQEMU.IsChecked.Value) {
                mBuilder.MakeQEMU(chbxQEMUUseHD.IsChecked.Value,
                                  chbxQEMUUseGDB.IsChecked.Value,
                                  mDebugMode != DebugModeEnum.None,
                                  mDebugMode != DebugModeEnum.None,
                                  chckQEMUUseNetworkTAP.IsChecked.Value,
                                  cmboNetworkCards.SelectedValue,
                                  cmboAudioCards.SelectedValue);
            } else if (rdioVMWare.IsChecked.Value) {
                mBuilder.MakeVMWare(rdVMWareServer.IsChecked.Value);
            } else if (rdioVPC.IsChecked.Value) {
                mBuilder.MakeVPC();
            } else if (rdioISO.IsChecked.Value) {
                mBuilder.MakeISO();
            } else if (rdioPXE.IsChecked.Value) {
                mBuilder.MakePXE();
            } else if (rdioUSB.IsChecked.Value) {
                mBuilder.MakeUSB(cmboUSBDevice.Text[0]);
            }
        }

        private void butnCancel_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }

        protected const string mRegKey = @"Software\Cosmos\User Kit";

        protected void SaveSettingsToRegistry() {
            using (var xKey = Registry.CurrentUser.CreateSubKey(mRegKey)) {
                string xTarget = "QEMU";
                if (rdioVMWare.IsChecked.Value) {
                    xTarget = "VMWare";
                } else if (rdioVPC.IsChecked.Value) {
                    xTarget = "VPC";
                } else if (rdioISO.IsChecked.Value) {
                    xTarget = "ISO";
                } else if (rdioPXE.IsChecked.Value) {
                    xTarget = "PXE";
                } else if (rdioUSB.IsChecked.Value) {
                    xTarget = "USB";
                }
                xKey.SetValue("Target", xTarget);

                // Misc
                xKey.SetValue("Show Options Window", chbxShowOptions.IsChecked.Value, RegistryValueKind.DWord);
                //Force checkbox to be on, was chckCompileIL.IsChecked.Value,
                xKey.SetValue("Compile IL", true, RegistryValueKind.DWord);
                              
                xKey.SetValue("Debug Port", cmboDebugPort.Text);
                xKey.SetValue("Debug Mode", cmboDebugMode.Text);

                // QEMU
                xKey.SetValue("Use GDB", chbxQEMUUseGDB.IsChecked.Value, RegistryValueKind.DWord);
                xKey.SetValue("Create HD Image", chbxQEMUUseHD.IsChecked.Value, RegistryValueKind.DWord);
                xKey.SetValue("Use network TAP", chckQEMUUseNetworkTAP.IsChecked.Value, RegistryValueKind.DWord);
                xKey.SetValue("Network Card",
                              cmboNetworkCards.Text,
                              RegistryValueKind.String);
                xKey.SetValue("Audio Card",
                              cmboAudioCards.Text,
                              RegistryValueKind.String);
                // VMWare
                string xVMWareVersion = string.Empty;
                if (rdVMWareServer.IsChecked.Value) {
                    xVMWareVersion = "VMWare Server";
                } else if (rdVMWareWorkstation.IsChecked.Value) {
                    xVMWareVersion = "VMWare Workstation";
                }
                xKey.SetValue("VMWare Version", xVMWareVersion);

                // USB
                if (cmboUSBDevice.SelectedItem != null) {
                    xKey.SetValue("USB Device", cmboUSBDevice.Text);
                }
            }
        }

        private void LoadSettingsFromRegistry() {
            using (var xKey = Registry.CurrentUser.CreateSubKey(mRegKey)) {
                string xBuildType = (string)xKey.GetValue("Target",
                                                          "QEMU");
                switch (xBuildType) {
                    case "QEMU":
                        rdioQEMU.IsChecked = true;
                        break;
                    case "VMWare":
                        rdioVMWare.IsChecked = true;
                        break;
                    case "VPC":
                        rdioVPC.IsChecked = true;
                        break;
                    case "ISO":
                        rdioISO.IsChecked = true;
                        break;
                    case "PXE":
                        rdioPXE.IsChecked = true;
                        break;
                    case "USB":
                        rdioUSB.IsChecked = true;
                        break;
                }

                // Misc
                chbxShowOptions.IsChecked = ((int)xKey.GetValue("Show Options Window", 1) != 0);
                chbxCompileIL.IsChecked = ((int)xKey.GetValue("Compile IL", 1) != 0);
                
                cmboDebugPort.SelectedIndex = cmboDebugPort.Items.IndexOf(xKey.GetValue("Debug Port",
                                                                                        ""));
                if (cmboDebugPort.SelectedIndex == -1) {
                    cmboDebugPort.SelectedIndex = 0;
                }
                cmboDebugMode.SelectedIndex = cmboDebugMode.Items.IndexOf(xKey.GetValue("Debug Mode",
                                                                                        ""));
                if (cmboDebugMode.SelectedIndex == -1) {
                    cmboDebugMode.SelectedIndex = 0;
                }

                // QEMU
                chbxQEMUUseGDB.IsChecked = ((int)xKey.GetValue("Use GDB", 0) != 0);
                chbxQEMUUseHD.IsChecked = ((int)xKey.GetValue("Create HD Image", 0) != 0);
                chckQEMUUseNetworkTAP.IsChecked = ((int)xKey.GetValue("Use network TAP",
                                                                      0) != 0);
                cmboNetworkCards.SelectedIndex = cmboNetworkCards.Items.IndexOf(xKey.GetValue("Network Card",
                                                                                              Builder.QemuNetworkCard.rtl8139.ToString()));
                cmboAudioCards.SelectedIndex = cmboAudioCards.Items.IndexOf(xKey.GetValue("Audio Card",
                                                                                          Builder.QemuAudioCard.es1370.ToString()));
                // VMWare
                string xVMWareVersion = (string)xKey.GetValue("VMWare Version", "VMWare Server");
                switch (xVMWareVersion) {
                    case "VMWare Server":
                        rdVMWareServer.IsChecked = true;
                        break;
                    case "VMWare Workstation":
                        rdVMWareWorkstation.IsChecked = true;
                        break;
                }

                // USB
                cmboUSBDevice.SelectedIndex = cmboUSBDevice.Items.IndexOf(xKey.GetValue("USB Device", ""));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}