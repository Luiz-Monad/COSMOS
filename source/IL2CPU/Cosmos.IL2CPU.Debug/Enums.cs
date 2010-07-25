﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmos.Compiler.Debug {
    // Messages from Guest to Host
    public enum MsgType: byte { 
        Noop = 0
        , TracePoint = 1
        , Message = 2
        , BreakPoint = 3
        , Error = 4
        , Pointer = 5
        // This is sent once on start up. The first call to debug stub sends this. 
        // Host can then respond with a series of set breakpoints etc, ie ones that were set before running.
        , Ready = 6
    }
    
    // Messages from Host to Guest
    public enum Command : byte {
        TraceOff = 1, TraceOn = 2
        // Break command is also for continuing from breakstate.
        , Break = 3
        , Step = 4
        , BreakOnAddress = 5
    }
}
