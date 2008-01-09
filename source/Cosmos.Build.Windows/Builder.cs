﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Cosmos.Build.Windows {
    public class Builder {
        //TODO: Fix this - config file? Package format?
        protected const string mBuildPath = @"s:\source\il2cpu\Build\";
        protected string mToolsPath;
        protected string mISOPath;
        protected string mAsmPath;

        public Builder() {
            mToolsPath = mBuildPath + @"Tools\";
            mISOPath = mBuildPath + @"ISO\";
            mAsmPath = mToolsPath + @"asm\";
        }

        protected void RemoveFile(string aPathname) {
            if (File.Exists(aPathname)) {
                File.Delete(aPathname);
            }
        }

        protected void Call(string aEXEPathname, string aArgLine, string aWorkDir, bool aWait) {
            var xStartInfo = new ProcessStartInfo();
            xStartInfo.FileName = aEXEPathname;
            xStartInfo.Arguments = aArgLine;
            xStartInfo.WorkingDirectory = aWorkDir;
            xStartInfo.UseShellExecute = false;
            xStartInfo.RedirectStandardError = true;
            xStartInfo.RedirectStandardOutput = true;
            var xProcess = Process.Start(xStartInfo);
            if (aWait) {
                if (!xProcess.WaitForExit(60 * 1000) || xProcess.ExitCode != 0) {
                    //TODO: Fix
                    //throw new Exception("Call failed");
                    Console.WriteLine("Error during call");
                    Console.Write(xProcess.StandardOutput.ReadToEnd());
                    Console.Write(xProcess.StandardError.ReadToEnd());
                }
            }
        }

        protected void MakeISO() {
            RemoveFile(mBuildPath + "cosmos.iso");
            RemoveFile(mISOPath + "output.bin");
            File.Move(mBuildPath + "output.bin", mISOPath + "output.bin");
            // From TFS its read only, and one of the utils doesnt like that
            File.SetAttributes(mISOPath + "isolinux.bin", FileAttributes.Normal);

            // Call(mToolsPath + @"mkisofs.exe", "-R -b syslinux/isolinux.bin -no-emul-boot -boot-load-size 4 -boot-info-table -o Cosmos.iso files", mISOPath, true);
            Call(mToolsPath + @"mkisofs.exe", @"-R -b isolinux.bin -no-emul-boot -boot-load-size 4 -boot-info-table -o ..\Cosmos.iso .", mISOPath, true);
        }

        public void Compile() {
            if (!Directory.Exists(mAsmPath)) {
                Directory.CreateDirectory(mAsmPath);
            }
            var xTarget = System.Reflection.Assembly.GetEntryAssembly();
            IL2CPU.Program.Main(new string[] {@"-in:" + xTarget.Location
                , "-plug:" + mToolsPath + @"Cosmos.Kernel.Plugs\Cosmos.Kernel.Plugs.dll"
                , "-platform:nativex86", "-asm:" + mAsmPath}
                );

            RemoveFile(mBuildPath + "output.obj");
            Call(mToolsPath + @"nasm\nasm.exe", String.Format("-g -f elf -F stabs -o \"{0}\" \"{1}\"", mBuildPath + "output.obj", mAsmPath + "main.asm"), mBuildPath, true);

            RemoveFile(mBuildPath + "output.bin");
            Call(mToolsPath + @"cygwin\ld.exe", String.Format("-Ttext 0x500000 -Tdata 0x200000 -e Kernel_Start -o \"{0}\" \"{1}\"", "output.bin", "output.obj"), mBuildPath, true);
            RemoveFile(mBuildPath + "output.obj");
        }

        public void BuildKernel() {
        }

        public enum Target { ISO, PXE, QEMU, QEMU_GDB, VMWare, VMWarePXE, VirtualPC, VirtualPCPXE };
        public void Build(Target aType) {
            Compile();

            switch (aType) {
                case Target.ISO:
                    MakeISO();
                    break;

                case Target.PXE:
                    RemoveFile(Path.Combine(mBuildPath, @"PXE\Boot\output.obj"));
                    File.Move(mBuildPath + @"output.obj", mBuildPath + @"PXE\Boot\output.obj");
                    // *Must* set working dir so tftpd32 will set itself to proper dir
                    Call(mBuildPath + @"tftpd32.exe", "", mBuildPath + @"pxe\", false);
                    break;

                case Target.QEMU:
                case Target.QEMU_GDB:
                    MakeISO();
                    RemoveFile(mISOPath + "serial-debug.txt");
                    //Call(mCosmosPath + @"tools\qemu\qemu.exe", @"-L . -cdrom ..\..\build\Cosmos\ISO\Cosmos.iso -boot d -hda ..\..\build\Cosmos\ISO\C-drive.img -serial " + "\"" + @"file:..\..\build\Cosmos\ISO\serial-debug.txt" + "\"" + " -S -s", mCosmosPath + @"tools\qemu\", aType == Target.QEMU);

                    if (aType == Target.QEMU_GDB) {
                      //  Call(mCosmosPath + @"tools\gdb\bin\gdb.exe"
                      //      , mBuildPath + @"ISO\files\output.obj" + " --eval-command=\"target remote:1234\" --eval-command=\"b _CODE_REQUESTED_BREAK_\" --eval-command=\"c\""
                      //      , mCosmosPath + @"tools\qemu\", true);
                    }
                    break;

                case Target.VMWare:
                    break;

                case Target.VMWarePXE:
                    break;

                case Target.VirtualPC:
                    break;

                case Target.VirtualPCPXE:
                    break;

            }

        }
    }
}
