﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Indy.IL2CPU.Assembler.X86
{
	/// <summary>
	/// Represents the JBE opcode
	/// </summary>
	[OpCode(0xFFFFFFFF, "jbe")]
	public class JumpIfBelowOrEqual : JumpBase
	{
		public JumpIfBelowOrEqual(string aAddress)
			: base(aAddress)
		{
		}
		public override string ToString()
		{
			return "jbe " + Address;
		}
	}
}