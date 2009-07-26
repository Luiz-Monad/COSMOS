using System;

namespace Cosmos.IL2CPU.X86.IL
{
	[Cosmos.IL2CPU.OpCode(ILOpCode.Code.Conv_Ovf_I8)]
	public class Conv_Ovf_I8: ILOp
	{
		public Conv_Ovf_I8(ILOpCode aOpCode):base(aOpCode)
		{
		}

		#region Old code
		// using System;
		// 
		// using CPUx86 = Indy.IL2CPU.Assembler.X86;
		// using Indy.IL2CPU.Assembler;
		// 
		// namespace Indy.IL2CPU.IL.X86 {
		// 	[OpCode(OpCodeEnum.Conv_Ovf_I8)]
		// 	public class Conv_Ovf_I8: Op {
		//         private string mNextLabel;
		// 	    private string mCurLabel;
		// 	    private uint mCurOffset;
		// 	    private MethodInformation mMethodInformation;
		// 		public Conv_Ovf_I8(ILReader aReader, MethodInformation aMethodInfo)
		// 			: base(aReader, aMethodInfo) {
		//              mMethodInformation = aMethodInfo;
		// 		    mCurOffset = aReader.Position;
		// 		    mCurLabel = IL.Op.GetInstructionLabel(aReader);
		//             mNextLabel = IL.Op.GetInstructionLabel(aReader.NextPosition);
		// 		}
		// 		public override void DoAssemble() {
		//             var xSource = Assembler.StackContents.Pop();
		//             switch (xSource.Size)
		//             {
		//                 case 1:
		//                 case 2:
		//                 case 4:
		//                     new CPUx86.Pop { DestinationReg = CPUx86.Registers.EAX };
		//                     new CPUx86.SignExtendAX { Size = 32 };
		//                     new CPUx86.Push { DestinationReg = CPUx86.Registers.EDX };
		//                     new CPUx86.Push { DestinationReg = CPUx86.Registers.EAX };
		//                     break;
		//                 case 8:
		//                     new CPUx86.Noop();
		//                     break;
		//                 default:
		//                     EmitNotImplementedException(Assembler, GetServiceProvider(), "Conv_Ovf_I8: SourceSize " + xSource + " not supported!", mCurLabel, mMethodInformation, mCurOffset, mNextLabel);
		//                     break;
		//             }
		//             Assembler.StackContents.Push(new StackContent(8, true, false, true));
		// 		}
		// 	}
		// }
		#endregion Old code
	}
}
