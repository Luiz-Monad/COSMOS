﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmos.IL2CPU.ILOpCodes {
  public class OpInt64 : ILOpCode {
    public readonly UInt64 Value;

    public OpInt64(Code aOpCode, UInt64 aValue)
      : base(aOpCode) {
      Value = aValue;
    }
  }
}