﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmos.IL2CPU.X86.x87
{
    [OpCode("fist")]
    public class IntStore : InstructionWithDestinationAndSize
    {
        public static void InitializeEncodingData(Instruction.InstructionData aData)
        {
            aData.EncodingOptions.Add(new InstructionData.InstructionEncodingOption
            {
                OpCode = new byte[] { 0xDF },
                NeedsModRMByte = true,
                InitialModRMByteValue = 2,
                DestinationMemory = true,
                DestinationImmediate = false,
                DestinationReg = null,
                AllowedSizes = InstructionSizes.Word,
                DefaultSize = InstructionSize.Word
            });
            aData.EncodingOptions.Add(new InstructionData.InstructionEncodingOption
            {
                OpCode = new byte[] { 0xDB },
                NeedsModRMByte = true,
                InitialModRMByteValue = 2,
                DestinationMemory = true,
                DestinationImmediate = false,
                DestinationReg = null,
                AllowedSizes = InstructionSizes.DWord,
                DefaultSize = InstructionSize.DWord
            });
        }
    }
}
