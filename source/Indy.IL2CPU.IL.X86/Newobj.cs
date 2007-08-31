using System;
using System.IO;
using Mono.Cecil.Cil;
using CPU = Indy.IL2CPU.Assembler;

namespace Indy.IL2CPU.IL.X86 {
	public class Newobj: IL.Newobj {
		/// <summary>
		/// This overload takes the label name of the ctor
		/// </summary>
		/// <param name="aCtor"></param>
		public void Assemble(string aCtor) {
			new CPU.JumpAlways(aCtor);
		}

		public override void Assemble(Instruction aInstruction) {
			throw new NotImplementedException("This file has been autogenerated and not been changed afterwards!");
		}
	}
}