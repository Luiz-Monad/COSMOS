﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RsenkTest.Commands
{
    abstract class CommandBase
    {
        protected List<CommandBase> parameters;
        public List<CommandBase> Parameters
        {
            get
            {
                return parameters;
            }
        }
        public abstract string Name { get; }
        public abstract string Summary { get; }
        public abstract void Help();
        public abstract void Execute(params ParameterBase[] args);
    }
}
