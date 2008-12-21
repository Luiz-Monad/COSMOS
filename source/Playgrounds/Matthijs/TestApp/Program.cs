﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Indy.IL2CPU.Assembler.X86;
using System.IO;
using System.Reflection;
using Indy.IL2CPU.Assembler.X86.X;
using Indy.IL2CPU.Tests.AssemblerTests.X86;

namespace TestApp {
    class Program {
        class Renderer : Y86 {
            public void DoRender() {
                new global::Indy.IL2CPU.Assembler.X86.ShiftLeft { SourceValue = 30, DestinationValue = 65, DestinationIsIndirect = true, Size = 8 };
                new global::Indy.IL2CPU.Assembler.X86.ShiftLeft { SourceValue = 30, DestinationValue = 650, DestinationIsIndirect = true, Size = 8 };
                new global::Indy.IL2CPU.Assembler.X86.ShiftLeft { SourceValue = 30, DestinationValue = 650000, DestinationIsIndirect = true, Size = 8 };
                new global::Indy.IL2CPU.Assembler.X86.ShiftLeft { SourceValue = 300, DestinationValue = 65, DestinationIsIndirect = true, Size = 8 };
                new global::Indy.IL2CPU.Assembler.X86.ShiftLeft { SourceValue = 300, DestinationValue = 650, DestinationIsIndirect = true, Size = 8 };
                new global::Indy.IL2CPU.Assembler.X86.ShiftLeft { SourceValue = 300, DestinationValue = 650000, DestinationIsIndirect = true, Size = 8 };
                new global::Indy.IL2CPU.Assembler.X86.ShiftLeft { SourceValue = 300000, DestinationValue = 65, DestinationIsIndirect = true, Size = 8 };
                new global::Indy.IL2CPU.Assembler.X86.ShiftLeft { SourceValue = 300000, DestinationValue = 650, DestinationIsIndirect = true, Size = 8 };
                new global::Indy.IL2CPU.Assembler.X86.ShiftLeft { SourceValue = 300000, DestinationValue = 650000, DestinationIsIndirect = true, Size = 8 };
            }
        }
        static void Main(string[] args) {
            try {
                var xAsm = new Assembler();
                xAsm.Initialize();
                //xAsm.DataMembers.Add(new Indy.IL2CPU.Assembler.DataMember("TestData", new byte[] { 65, 66, 67, 68, 69, 70, 71, 72, 73, 74 }));
                xAsm.Instructions.Clear();
                xAsm.DataMembers.Clear();
                var xRenderer = new Renderer();
                xRenderer.DoRender();
                if (!Directory.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                                                                                         "Output"))) {
                    Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                        "Output"));
                }
                using (var xOutput = new StreamWriter(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                                                                                         "Output"),
                                                                            "TheOutput.asm"))) {
                    xAsm.FlushText(xOutput);
                }
                using (Stream xOutput = new FileStream(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                                                                                         "Output"),
                                                                            "TheOutput.bin"), FileMode.Create)) {
                    xAsm.FlushBinary(xOutput, 0x200000);
                }
                
                //TestCodeGenerator.Execute();

                //InvalidOpcodeTester.Initialize();
                //InvalidOpcodeTester.ExecuteSingle(typeof(Move), 0);
                //InvalidOpcodeTester.GenerateHtml(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                //                                                                         "Output.html"));
                //InvalidOpcodeTester.GenerateXml(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                //                                                                         "Output.xml"));
            }catch(InvalidOpcodeTester.AbortException){
                InvalidOpcodeTester.GenerateHtml(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                                                                                         "Output.html"));
                InvalidOpcodeTester.GenerateXml(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                                                                                         "Output.xml"));
            } catch (Exception E) { Console.WriteLine(E.ToString()); } 
            finally {
                Console.WriteLine("Finished");
                Console.ReadLine();
                                    Console.ReadLine();
            }
        }
    }
}