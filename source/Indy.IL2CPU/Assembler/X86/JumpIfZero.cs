﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Indy.IL2CPU.Assembler.X86 {
	/// <summary>
	/// Represents the JZ opcode
	/// </summary>
    [OpCode("jz")]
	public class JumpIfZero: JumpBase {
	}
}