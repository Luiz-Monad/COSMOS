using System;
using Mono.Cecil.Cil;

namespace Indy.IL2CPU.IL {
	[OpCode(Code.Volatile)]
	public abstract class Volatile: Op {
	}
}