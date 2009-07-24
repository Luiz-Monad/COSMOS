﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmos.IL2CPU.ILOpCodes {
  public class InlineVar : ILOpCode {
    public readonly UInt16 Value;

    public InlineVar(Code aOpCode, UInt16 aValue)
      : base(aOpCode) {
      Value = aValue;
    }
  }
}
