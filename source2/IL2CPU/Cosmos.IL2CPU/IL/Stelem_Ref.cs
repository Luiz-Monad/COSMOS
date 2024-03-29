using System;
using CPUx86 = Cosmos.Assembler.x86;
using Cosmos.Assembler;
using Cosmos.IL2CPU.IL.CustomImplementations.System;

namespace Cosmos.IL2CPU.X86.IL {
  [Cosmos.IL2CPU.OpCode(ILOpCode.Code.Stelem_Ref)]
  public class Stelem_Ref: ILOp {
    public Stelem_Ref(Cosmos.Assembler.Assembler aAsmblr)
      : base(aAsmblr) {
    }
    public static void Assemble(Cosmos.Assembler.Assembler aAssembler, uint aElementSize, MethodInfo aMethod, ILOpCode aOpCode) {
      // stack - 3 == the array
      // stack - 2 == the index
      // stack - 1 == the new value
      uint xStackSize = aElementSize;
      if (xStackSize % 4 != 0) {
        xStackSize += 4 - xStackSize % 4;
      }
      aAssembler.Stack.Pop();
      aAssembler.Stack.Pop();
      aAssembler.Stack.Pop();
      //new CPUx86.Push { DestinationReg = CPUx86.Registers.ESP, DestinationIsIndirect = true, DestinationDisplacement = (int)(xStackSize + 4) };

      //new CPUx86.Call { DestinationLabel = MethodInfoLabelGenerator.GenerateLabelName(GCImplementationRefs.DecRefCountRef) };

      new CPUx86.Mov { DestinationReg = CPUx86.Registers.EBX, SourceReg = CPUx86.Registers.ESP, SourceIsIndirect = true, SourceDisplacement = (int)xStackSize }; // the index
      new CPUx86.Mov { DestinationReg = CPUx86.Registers.ECX, SourceReg = CPUx86.Registers.ESP, SourceIsIndirect = true, SourceDisplacement = (int)xStackSize + 4 }; // the index

      new CPUx86.Add { DestinationReg = CPUx86.Registers.ECX, SourceValue = (uint)(ObjectImpl.FieldDataOffset + 4) };

      new CPUx86.Push { DestinationValue = aElementSize };
	  aAssembler.Stack.Push(4, typeof(int));
      new CPUx86.Push { DestinationReg = CPUx86.Registers.EBX };
	  aAssembler.Stack.Push(4, typeof(int));


      //Multiply( aAssembler, aServiceProvider, aCurrentLabel, aCurrentMethodInfo, aCurrentOffset, aNextLabel );
      (new Mul(aAssembler)).Execute(aMethod, aOpCode);

      new CPUx86.Push { DestinationReg = CPUx86.Registers.ECX };
	  aAssembler.Stack.Push(4, typeof(int));

      //Add( aAssembler, aServiceProvider, aCurrentLabel, aCurrentMethodInfo, aCurrentOffset, aNextLabel );
      (new Add(aAssembler)).Execute(aMethod, aOpCode);

      new CPUx86.Pop { DestinationReg = CPUx86.Registers.ECX };
      aAssembler.Stack.Pop();
      for (int i = (int)(aElementSize / 4) - 1; i >= 0; i -= 1) {
        new Comment(aAssembler, "Start 1 dword");
        new CPUx86.Pop { DestinationReg = CPUx86.Registers.EBX };
        new CPUx86.Mov { DestinationReg = CPUx86.Registers.ECX, DestinationIsIndirect = true, SourceReg = CPUx86.Registers.EBX };
        new CPUx86.Add { DestinationReg = CPUx86.Registers.ECX, SourceValue = 4 };
      }
      switch (aElementSize % 4) {
        case 1: {
            new Comment(aAssembler, "Start 1 byte");
            new CPUx86.Pop { DestinationReg = CPUx86.Registers.EBX };
            new CPUx86.Mov { DestinationReg = CPUx86.Registers.ECX, DestinationIsIndirect = true, SourceReg = CPUx86.Registers.BL };
            break;
          }
        case 2: {
            new Comment(aAssembler, "Start 1 word");
            new CPUx86.Pop { DestinationReg = CPUx86.Registers.EBX };
            new CPUx86.Mov { DestinationReg = CPUx86.Registers.ECX, DestinationIsIndirect = true, SourceReg = CPUx86.Registers.BX };
            break;
          }
        case 0: {
            break;
          }
        default:
          throw new Exception("Remainder size " + (aElementSize % 4) + " not supported!");

      }
      new CPUx86.Add { DestinationReg = CPUx86.Registers.ESP, SourceValue = 0x8 };
    }
    public override void Execute(MethodInfo aMethod, ILOpCode aOpCode) {
      Assemble(Assembler, 4, aMethod, aOpCode);
    }
  }
}