﻿using System;
using System.Linq;

namespace Indy.IL2CPU.Assembler {
	public class Literal: Instruction {
		public readonly string Data;

		public Literal(string aData) {
			Data = aData;
		}

		public override string ToString() {
			return Data;
		}
	}
}