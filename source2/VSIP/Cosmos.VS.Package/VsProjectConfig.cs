﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Project;

namespace Cosmos.VS.Package {

  public class VsProjectConfig : ProjectConfig {

    public VsProjectConfig(ProjectNode project, string configuration) : base(project, configuration) { }

  }

}
