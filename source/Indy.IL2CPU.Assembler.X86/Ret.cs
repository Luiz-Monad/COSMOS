﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Indy.IL2CPU.Assembler.X86 {
	public class Ret: Instruction{
		public override string ToString() {
			return "ret";
		}
	}
}