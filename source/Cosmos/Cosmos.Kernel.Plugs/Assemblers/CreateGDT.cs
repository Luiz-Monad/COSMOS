﻿using System;
using Indy.IL2CPU.Assembler;
using Indy.IL2CPU.Assembler.X86;
using Indy.IL2CPU.Plugs;
using Assembler=Indy.IL2CPU.Assembler.Assembler;
using CPUx86 = Indy.IL2CPU.Assembler.X86;
using CPUNative = Indy.IL2CPU.Assembler.X86;
using System.Collections.Generic;

namespace Cosmos.Kernel.Plugs.Assemblers {
	public class CreateGDT : AssemblerMethod {
		public override void Assemble(Assembler aAssembler) {
			string xFieldName = "_NATIVE_GDT_Contents";
			string xFieldData
				// Null Segment
				= "0,0,0,0,0,0,0,0"
				// Code Segment
				+ ", 0xFF, 0xFF, 0, 0, 0, 0x99, 0xCF, 0"
				// Data Segment
				+ ", 0xFF,0xFF,0,0,0,0x93,0xCF,0"
				// ES
				+ ", 0xFF,0xFF,0,0,0,0x93,0xCF,0"
				// FS
				+ ", 0xFF,0xFF,0,0,0,0x93,0xCF,0"
				// GS
				+ ", 0xFF,0xFF,0,0,0,0x93,0xCF,0"
				// SS
				+ ", 0xFF,0xFF,0,0,0,0x93,0xCF,0";
			//aAssembler.DataMembers.Add(new KeyValuePair<string, DataMember> (aAssembler.CurrentGroup,new DataMember(xFieldName, "db", xFieldData)));
			//xFieldName = "_NATIVE_GDT_Pointer";
			////xFieldData = "0x17, (_NATIVE_GDT_Contents and 0xFFFF), (_NATIVE_GDT_Contents shr 16)";
			//aAssembler.DataMembers.Add(new KeyValuePair<string, DataMember> (aAssembler.CurrentGroup,new DataMember(xFieldName, "dw", "0x37,0,0")));

            aAssembler.DataMembers.Add(new KeyValuePair<string, DataMember>(aAssembler.CurrentGroup, new DataMember(xFieldName, "db", xFieldData)));
            aAssembler.DataMembers.Add(new KeyValuePair<string, DataMember>(aAssembler.CurrentGroup, new DataMember("_NATIVE_GDT_Pointer", "dw", "0x37,0,0")));

			new CPUx86.Move(Registers.EAX, "_NATIVE_GDT_Pointer");
			new CPUx86.Move("dword [_NATIVE_GDT_Pointer + 2]", "_NATIVE_GDT_Contents");

            //Memory Protection
            //new CPUx86.Move(Registers.EAX, "Kernel_Start");
            //new CPUx86.ShiftRight(Registers.EAX, "12");
            //new CPUx86.Move(Registers.AX, "[" + xFieldName + " + 10]");
            //new CPUx86.ShiftRight(Registers.EAX, "16");
            //new CPUx86.Move(Registers.AL, "[" + xFieldName + " + 12]");
            //new CPUx86.Move(Registers.EAX, "(_end_data - Kernel_Start)");
            //new CPUx86.ShiftRight(Registers.EAX, "12");
            //new CPUx86.Move(Registers.AX, "[" + xFieldName + " + 8]");
            //new CPUx86.ShiftRight(Registers.EAX, "16");
            ////Note; the uppto part of the limit is skipped
            ////new CPUx86.Move(Registers.AL, "[" + xFieldName + " + 12]");
            

			new Label(".RegisterGDT");
			new CPUNative.Lgdt(Registers.AtEAX);
			new CPUx86.Move(Registers.AX, "0x10");
			new CPUx86.Move("ds", Registers.AX);
            new CPUx86.Move("es", Registers.AX);
            new CPUx86.Move("fs", Registers.AX);
            new CPUx86.Move("gs", Registers.AX);
            new CPUx86.Move("ss", Registers.AX);				 
            // Force reload of code segement
			new CPUx86.Jump("0x8:flush__GDT__table");
			new Label("flush__GDT__table");
		}
	}
}
