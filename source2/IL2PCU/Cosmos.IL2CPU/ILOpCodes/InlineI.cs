﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmos.IL2CPU.ILOpCodes {
  public class InlineI : ILOpCode {
    public readonly UInt32 Value;

    public InlineI(Code aOpCode, UInt32 aValue)
      : base(aOpCode) {
      Value = aValue;
    }
  }
}
