﻿using Cosmos.IL2CPU.Plugs;
using Assembler=Cosmos.IL2CPU.Assembler;
using CPUAll = Cosmos.IL2CPU;
using CPUx86 = Cosmos.IL2CPU.X86;
using Cosmos.IL2CPU.X86;

namespace Cosmos.Sys.Plugs.Assemblers{
    public class ShutDown : AssemblerMethod
    {
        public override void AssembleNew(object aAssembler, object aMethodInfo) {
          //ACPI Way...ONLY QEMu And Boschs
          new CPUx86.Move {
            DestinationReg = CPUx86.Registers.DX,
            SourceValue = 0xB004
          };
          new CPUx86.Move {
            DestinationReg = CPUx86.Registers.AX,
            SourceValue = 0x0 | 0x2000
          };
          new CPUx86.Out {
            DestinationReg = CPUx86.Registers.AX,
            Size = (byte)CPUx86.Instruction.InstructionSize.Word
          };
          //TODO: ACPI Way...see http://forum.osdev.org/viewtopic.php?t=16990
        }
    }
}
