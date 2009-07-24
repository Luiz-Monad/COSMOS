using System;

namespace Cosmos.IL2CPU.X86.IL
{
	[Cosmos.IL2CPU.OpCode(ILOpCode.Code.Mul)]
	public class Mul: ILOpX86
	{
		public Mul(ILOpCode aOpCode):base(aOpCode)
		{
		}

		#region Old code
		// using System;
		// using System.IO;
		// 
		// 
		// using CPU = Indy.IL2CPU.Assembler.X86;
		// 
		// namespace Indy.IL2CPU.IL.X86 {
		// 	[OpCode(OpCodeEnum.Mul)]
		// 	public class Mul: Op {
		// 	    private string mNextLabel;
		// 	    private string mCurLabel;
		// 	    private uint mCurOffset;
		// 	    private MethodInformation mMethodInformation;
		// 		public Mul(ILReader aReader, MethodInformation aMethodInfo)
		// 			: base(aReader, aMethodInfo) {
		// 		    mMethodInformation = aMethodInfo;
		// 		    mCurOffset = aReader.Position;
		// 		    mCurLabel = IL.Op.GetInstructionLabel(aReader);
		//             mNextLabel = IL.Op.GetInstructionLabel(aReader.NextPosition);
		// 		}
		// 		public override void DoAssemble() {
		// 			Multiply(Assembler, GetServiceProvider(),
		//                 mCurLabel, mMethodInformation, mCurOffset, mNextLabel);
		// 		}
		// 	}
		// }
		#endregion Old code
	}
}
