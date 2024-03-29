﻿// Guids.cs
// MUST match guids.h
using System;

namespace Cosmos.VS.XSharp {
  static class GuidList {
    public const string guidCosmos_VS_XSharpPkgString = "e2ce86d3-fb0b-43ad-938a-5bcdd087ea2d";
    public const string guidCosmos_VS_XSharpCmdSetString = "a9722019-958f-476e-926a-79f4f32f8530";

    public static readonly Guid guidCosmos_VS_XSharpCmdSet = new Guid(guidCosmos_VS_XSharpCmdSetString);
  };
}