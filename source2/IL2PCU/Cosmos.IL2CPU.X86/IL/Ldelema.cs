using System;
using CPUx86 = Cosmos.IL2CPU.X86;
using Indy.IL2CPU;
namespace Cosmos.IL2CPU.X86.IL
{
    [Cosmos.IL2CPU.OpCode( ILOpCode.Code.Ldelema )]
    public class Ldelema : ILOp
    {
        public Ldelema( Cosmos.IL2CPU.Assembler aAsmblr )
            : base( aAsmblr )
        {
        }

        public static void Assemble( Assembler aAssembler, uint aElementSize )
        {
            
            aAssembler.Stack.Pop();
            aAssembler.Stack.Pop();
            aAssembler.Stack.Push( 4, typeof( uint ) );
            new CPUx86.Pop { DestinationReg = CPUx86.Registers.EAX };
            new CPUx86.Move { DestinationReg = CPUx86.Registers.EDX, SourceValue = aElementSize };
            new CPUx86.Multiply { DestinationReg = CPUx86.Registers.EDX };
            new CPUx86.Add { DestinationReg = CPUx86.Registers.EAX, SourceValue = ( uint )( ObjectImpl.FieldDataOffset + 4 ) };
            new CPUx86.Pop { DestinationReg = CPUx86.Registers.EDX };
            new CPUx86.Add { DestinationReg = CPUx86.Registers.EDX, SourceReg = CPUx86.Registers.EAX };
            new CPUx86.Push { DestinationReg = CPUx86.Registers.EDX };
        }

        public override void Execute( MethodInfo aMethod, ILOpCode aOpCode )
        {
            var xSize = Assembler.Stack.Pop();

            Assemble( Assembler, ( uint )xSize.Size );
        }


        // using System;
        // using System.IO;
        // 
        // 
        // using CPU = Cosmos.IL2CPU.X86;
        // using CPUx86 = Cosmos.IL2CPU.X86;
        // using Cosmos.IL2CPU.X86;
        // using Indy.IL2CPU.Compiler;
        // 
        // namespace Indy.IL2CPU.IL.X86 {
        // 	[OpCode(OpCodeEnum.Ldelema)]
        // 	public class Ldelema: Op {
        //         private Type mType;
        // 		public Ldelema(ILReader aReader, MethodInformation aMethodInfo)
        // 			: base(aReader, aMethodInfo) {
        // 			mType= aReader.OperandValueType;
        // 		}
        // 
        // 		public static void Assemble(CPU.Assembler aAssembler, uint aElementSize) {
        // 			aAssembler.Stack.Pop();
        // 			aAssembler.Stack.Pop();
        // 			aAssembler.Stack.Push(new StackContent(4, typeof(uint)));
        //             new CPUx86.Pop { DestinationReg = CPUx86.Registers.EAX };
        //             new CPUx86.Move { DestinationReg = CPUx86.Registers.EDX, SourceValue = aElementSize };
        // 			new CPUx86.Multiply{DestinationReg=CPUx86.Registers.EDX};
        //             new CPUx86.Add { DestinationReg = CPUx86.Registers.EAX, SourceValue = (uint)(ObjectImpl.FieldDataOffset + 4) };
        //             new CPUx86.Pop { DestinationReg = CPUx86.Registers.EDX };
        //             new CPUx86.Add { DestinationReg = CPUx86.Registers.EDX, SourceReg = CPUx86.Registers.EAX };
        //             new CPUx86.Push { DestinationReg = CPUx86.Registers.EDX };
        // 		}
        // 
        // 		public override void DoAssemble() {
        //             var xElementSize = GetService<IMetaDataInfoService>().SizeOfType(mType);
        // 			Assemble(Assembler, xElementSize);
        // 		}
        // 	}
        // }

    }
}
