using System;

namespace Cosmos.IL2CPU.X86.IL
{
	[Cosmos.IL2CPU.OpCode(ILOpCode.Code.Stind_Ref)]
	public class Stind_Ref: ILOp
	{
		public Stind_Ref(ILOpCode aOpCode):base(aOpCode)
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
		// 	[OpCode(OpCodeEnum.Stind_Ref)]
		// 	public class Stind_Ref: Op {
		// 		public Stind_Ref(ILReader aReader, MethodInformation aMethodInfo)
		// 			: base(aReader, aMethodInfo) {
		// 		}
		// 		public override void DoAssemble() {
		// 			Stind_I.Assemble(Assembler, 4);
		// 		}
		// 	}
		// }
		#endregion Old code
	}
}
