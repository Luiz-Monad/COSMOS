using System;

namespace Cosmos.IL2CPU.Profiler.IL
{
	[Cosmos.IL2CPU.OpCode(ILOp.Code.Stind_I1)]
	public class Stind_I1: ILOpProfiler
	{



		#region Old code
		// using System;
		// using System.IO;
		// 
		// 
		// using CPU = Indy.IL2CPU.Assembler.X86;
		// 
		// namespace Indy.IL2CPU.IL.X86 {
		// 	[Cosmos.IL2CPU.OpCode(ILOp.Code.Stind_I1)]
		// 	public class Stind_I1: ILOpProfiler {
		// 		public Stind_I1(ILReader aReader, MethodInformation aMethodInfo)
		// 			: base(aReader, aMethodInfo) {
		// 		}
		// 		public override void DoAssemble() {
		// 			Stind_I.Assemble(Assembler, 1);
		// 		}
		// 	}
		// }
		#endregion
	}
}
