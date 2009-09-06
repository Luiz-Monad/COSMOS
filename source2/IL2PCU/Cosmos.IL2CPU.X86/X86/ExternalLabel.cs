﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmos.IL2CPU.X86
{
    public class ExternalLabel: Instruction
    {
        public ExternalLabel(string aName):base()
        {
            Name = aName;
        }

        public string Name
        {
            get;
            set;
        }

        public override void WriteText( Cosmos.IL2CPU.Assembler aAssembler, System.IO.TextWriter aOutput )
        {
            aOutput.Write("extern ");
            aOutput.Write(Name);
        }
    }
}