﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace Cosmos.Debug.Common {
  public class DebugConnectorSerial : DebugConnectorStream {
    private SerialPort mPort;

    public DebugConnectorSerial(string aPort) {
      mPort = new SerialPort(aPort);
      mPort.BaudRate = 115200;
      mPort.Parity = Parity.None;
      mPort.DataBits = 8;
      mPort.StopBits = StopBits.One;
      mPort.Open();
      Start(mPort.BaseStream);
    }

  }
}
