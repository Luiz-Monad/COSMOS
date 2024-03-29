﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Cosmos.Core
{
    public class INTs
    {
        #region Enums
        // TODO: Protect IRQs like memory and ports are
        // TODO: Make IRQs so they are not hookable, and instead release high priority threads like FreeBSD (When we get threading)
        public enum EFlagsEnum : uint
        {
            Carry = 1,
            Parity = 1 << 2,
            AuxilliaryCarry = 1 << 4,
            Zero = 1 << 6,
            Sign = 1 << 7,
            Trap = 1 << 8,
            InterruptEnable = 1 << 9,
            Direction = 1 << 10,
            Overflow = 1 << 11,
            NestedTag = 1 << 14,
            Resume = 1 << 16,
            Virtual8086Mode = 1 << 17,
            AlignmentCheck = 1 << 18,
            VirtualInterrupt = 1 << 19,
            VirtualInterruptPending = 1 << 20,
            ID = 1 << 21
        }

        [StructLayout(LayoutKind.Explicit, Size = 0x68)]
        public struct TSS
        {
            [FieldOffset(0)]
            public ushort Link;
            [FieldOffset(4)]
            public uint ESP0;
            [FieldOffset(8)]
            public ushort SS0;
            [FieldOffset(12)]
            public uint ESP1;
            [FieldOffset(16)]
            public ushort SS1;
            [FieldOffset(20)]
            public uint ESP2;
            [FieldOffset(24)]
            public ushort SS2;
            [FieldOffset(28)]
            public uint CR3;
            [FieldOffset(32)]
            public uint EIP;
            [FieldOffset(36)]
            public EFlagsEnum EFlags;
            [FieldOffset(40)]
            public uint EAX;
            [FieldOffset(44)]
            public uint ECX;
            [FieldOffset(48)]
            public uint EDX;
            [FieldOffset(52)]
            public uint EBX;
            [FieldOffset(56)]
            public uint ESP;
            [FieldOffset(60)]
            public uint EBP;
            [FieldOffset(64)]
            public uint ESI;
            [FieldOffset(68)]
            public uint EDI;
            [FieldOffset(72)]
            public ushort ES;
            [FieldOffset(76)]
            public ushort CS;
            [FieldOffset(80)]
            public ushort SS;
            [FieldOffset(84)]
            public ushort DS;
            [FieldOffset(88)]
            public ushort FS;
            [FieldOffset(92)]
            public ushort GS;
            [FieldOffset(96)]
            public ushort LDTR;
            [FieldOffset(102)]
            public ushort IOPBOffset;
        }

        [StructLayout(LayoutKind.Explicit, Size = 512)]
        public struct MMXContext
        {
        }

        [StructLayout(LayoutKind.Explicit, Size = 80)]
        public struct IRQContext
        {
            [FieldOffset(0)]
            public unsafe MMXContext* MMXContext;

            [FieldOffset(4)]
            public uint EDI;

            [FieldOffset(8)]
            public uint ESI;

            [FieldOffset(12)]
            public uint EBP;

            [FieldOffset(16)]
            public uint ESP;

            [FieldOffset(20)]
            public uint EBX;

            [FieldOffset(24)]
            public uint EDX;

            [FieldOffset(28)]
            public uint ECX;

            [FieldOffset(32)]
            public uint EAX;

            [FieldOffset(36)]
            public uint Interrupt;

            [FieldOffset(40)]
            public uint Param;

            [FieldOffset(44)]
            public uint EIP;

            [FieldOffset(48)]
            public uint CS;

            [FieldOffset(52)]
            public EFlagsEnum EFlags;

            [FieldOffset(56)]
            public uint UserESP;
        }
        #endregion
        
        private static IRQDelegate[] mIRQ_Handlers = new IRQDelegate[256];

        // We used to use:
        //Interrupts.IRQ01 += HandleKeyboardInterrupt;
        // But at one point we had issues with multi cast delegates, so we changed to this single cast option.
        // [1:48:37 PM] Matthijs ter Woord: the issues were: "they didn't work, would crash kernel". not sure if we still have them..
        public static void SetIntHandler(byte aIntNo, IRQDelegate aHandler)
        {
            mIRQ_Handlers[aIntNo] = aHandler;
        }
        public static void SetIrqHandler(byte aIrqNo, IRQDelegate aHandler)
        {
            SetIntHandler((byte)(0x20 + aIrqNo), aHandler);
        }

        private static void IRQ(uint irq, ref IRQContext aContext)
        {
            var xCallback = mIRQ_Handlers[irq];
            if (xCallback != null)
            {
                HMI.GCMonitor();
                xCallback(ref aContext);
                HMI.GCFreeAll();
            }
        }

        public static void HandleInterrupt_Default(ref IRQContext aContext)
        {
            if (aContext.Interrupt >= 0x20 && aContext.Interrupt <= 0x2F)
            {
                if (aContext.Interrupt >= 0x28)
                {
                    Global.PIC.EoiSlave();
                }
                else
                {
                    Global.PIC.EoiMaster();
                }
            }
        }

        public delegate void IRQDelegate(ref IRQContext aContext);
        public delegate void ExceptionInterruptDelegate(ref IRQContext aContext, ref bool aHandled);

        #region Default Interrupt Handlers

        //IRQ 0 - System timer. Reserved for the system. Cannot be changed by a user.
        public static void HandleInterrupt_20(ref IRQContext aContext)
        {
            IRQ(0x20, ref aContext);  
            Global.PIC.EoiMaster();
        }

        //public static IRQDelegate IRQ01;
        //IRQ 1 - Keyboard. Reserved for the system. Cannot be altered even if no keyboard is present or needed.
        public static void HandleInterrupt_21(ref IRQContext aContext)
        {

            IRQ(0x21, ref aContext);
            Global.PIC.EoiMaster();
        }
        public static void HandleInterrupt_22(ref IRQContext aContext)
        {

            IRQ(0x22, ref aContext);
            Global.PIC.EoiMaster();
        }
        public static void HandleInterrupt_23(ref IRQContext aContext)
        {

            IRQ(0x23, ref aContext);
            Global.PIC.EoiMaster();
        }
        public static void HandleInterrupt_24(ref IRQContext aContext)
        {

            IRQ(0x24, ref aContext);
            Global.PIC.EoiMaster();
        }
        public static void HandleInterrupt_25(ref IRQContext aContext)
        {
            IRQ(0x25, ref aContext);
            Global.PIC.EoiMaster();
        }
        public static void HandleInterrupt_26(ref IRQContext aContext)
        {

            IRQ(0x26, ref aContext);
            Global.PIC.EoiMaster();
        }
        public static void HandleInterrupt_27(ref IRQContext aContext)
        {

            IRQ(0x27, ref aContext);
            Global.PIC.EoiMaster();
        }

        public static void HandleInterrupt_28(ref IRQContext aContext)
        {

            IRQ(0x28, ref aContext);
            Global.PIC.EoiSlave();
        }
        //IRQ 09 - (Added for AMD PCNet network card)
        //public static IRQDelegate IRQ09;

        public static void HandleInterrupt_29(ref IRQContext aContext)
        {
            IRQ(0x29, ref aContext);
            Global.PIC.EoiSlave();
        }

        //IRQ 10 - (Added for VIA Rhine network card)
        //public static IRQDelegate IRQ10;

        public static void HandleInterrupt_2A(ref IRQContext aContext)
        {
            IRQ(0x2A, ref aContext);
            Global.PIC.EoiSlave();
        }

        //IRQ 11 - (Added for RTL8139 network card)
        //public static IRQDelegate IRQ11;

        public static void HandleInterrupt_2B(ref IRQContext aContext)
        {
            IRQ(0x2B, ref aContext);
            Global.PIC.EoiSlave();
        }

        public static void HandleInterrupt_2C(ref IRQContext aContext)
        {

            IRQ(0x2C, ref aContext);
            Global.PIC.EoiSlave();
        }


        public static void HandleInterrupt_2D(ref IRQContext aContext)
        {
            IRQ(0x2D, ref aContext);
            Global.PIC.EoiSlave();
        }
        //IRQ 14 - Primary IDE. If no Primary IDE this can be changed
        public static void HandleInterrupt_2E(ref IRQContext aContext)
        {
            IRQ(0x2E, ref aContext);
            Global.PIC.EoiSlave();
        }
        //IRQ 15 - Secondary IDE
        public static void HandleInterrupt_2F(ref IRQContext aContext)
        {
            IRQ(0x2F, ref aContext);
            Global.PIC.EoiSlave();
        }

        public static event IRQDelegate Interrupt30;
        // Interrupt 0x30, enter VMM
        public static void HandleInterrupt_30(ref IRQContext aContext)
        {
            if (Interrupt30 != null)
            {
                Interrupt30(ref aContext);
            }
        }

        public static void HandleInterrupt_35(ref IRQContext aContext)
        {
            Global.Dbg.SendMessage("Interrupts", "Interrupt 35 handler");
            aContext.EAX *= 2;
            aContext.EBX *= 2;
            aContext.ECX *= 2;
            aContext.EDX *= 2;
        }

        public static void HandleInterrupt_40(ref IRQContext aContext)
        {
            IRQ(0x40, ref aContext);
        }
        public static void HandleInterrupt_41(ref IRQContext aContext)
        {
            IRQ(0x41, ref aContext);
        }
        public static void HandleInterrupt_42(ref IRQContext aContext)
        {
            IRQ(0x42, ref aContext);
        }
        public static void HandleInterrupt_43(ref IRQContext aContext)
        {
            IRQ(0x43, ref aContext);
        }
        public static void HandleInterrupt_44(ref IRQContext aContext)
        {
            IRQ(0x44, ref aContext);
        }
        public static void HandleInterrupt_45(ref IRQContext aContext)
        {
            IRQ(0x45, ref aContext);
        }
        public static void HandleInterrupt_46(ref IRQContext aContext)
        {
            IRQ(0x46, ref aContext);
        }
        public static void HandleInterrupt_47(ref IRQContext aContext)
        {
            IRQ(0x47, ref aContext);
        }
        public static void HandleInterrupt_48(ref IRQContext aContext)
        {
            IRQ(0x48, ref aContext);
        }
        public static void HandleInterrupt_49(ref IRQContext aContext)
        {
            IRQ(0x49, ref aContext);
        }

        #endregion

        #region CPU Exceptions

        public static IRQDelegate GeneralProtectionFault;

        public static void HandleInterrupt_00(ref IRQContext aContext)
        {
            HandleException(aContext.EIP, "Divide by zero", "EDivideByZero", ref aContext);
        }

        public static void HandleInterrupt_01(ref IRQContext aContext)
        {
            HandleException(aContext.EIP, "Debug Exception", "Debug Exception", ref aContext);
        }

        public static void HandleInterrupt_02(ref IRQContext aContext)
        {
            HandleException(aContext.EIP, "Non Maskable Interrupt Exception", "Non Maskable Interrupt Exception", ref aContext);
        }

        public static void HandleInterrupt_03(ref IRQContext aContext)
        {
            HandleException(aContext.EIP, "Breakpoint Exception", "Breakpoint Exception", ref aContext);
        }

        public static void HandleInterrupt_04(ref IRQContext aContext)
        {
            HandleException(aContext.EIP, "Into Detected Overflow Exception", "Into Detected Overflow Exception", ref aContext);
        }

        public static void HandleInterrupt_05(ref IRQContext aContext)
        {
            HandleException(aContext.EIP, "Out of Bounds Exception", "Out of Bounds Exception", ref aContext);
        }

        public static void HandleInterrupt_06(ref IRQContext aContext)
        {
            HandleException(aContext.EIP, "Invalid Opcode", "EInvalidOpcode", ref aContext);
        }

        public static void HandleInterrupt_07(ref IRQContext aContext)
        {
            HandleException(aContext.EIP, "No Coprocessor Exception", "No Coprocessor Exception", ref aContext);
        }

        public static void HandleInterrupt_08(ref IRQContext aContext)
        {
            HandleException(aContext.EIP, "Double Fault Exception", "Double Fault Exception", ref aContext);
        }

        public static void HandleInterrupt_09(ref IRQContext aContext)
        {
            HandleException(aContext.EIP, "Coprocessor Segment Overrun Exception", "Coprocessor Segment Overrun Exception", ref aContext);
        }

        public static void HandleInterrupt_0A(ref IRQContext aContext)
        {
            HandleException(aContext.EIP, "Bad TSS Exception", "Bad TSS Exception", ref aContext);
        }

        public static void HandleInterrupt_0B(ref IRQContext aContext)
        {
            HandleException(aContext.EIP, "Segment Not Present", "Segment Not Present", ref aContext);
        }

        public static void HandleInterrupt_0C(ref IRQContext aContext)
        {
            HandleException(aContext.EIP, "Stack Fault Exception", "Stack Fault Exception", ref aContext);
        }
        public static void HandleInterrupt_0D(ref IRQContext aContext)
        {
            if (GeneralProtectionFault != null)
            {
                GeneralProtectionFault(ref aContext);
            }
            else
            {
                HandleException(aContext.EIP, "General Protection Fault", "GPF", ref aContext);
            }
        }

        public static void HandleInterrupt_0E(ref IRQContext aContext)
        {
            HandleException(aContext.EIP, "Page Fault Exception", "Page Fault Exception", ref aContext);
        }

        public static void HandleInterrupt_0F(ref IRQContext aContext)
        {
            HandleException(aContext.EIP, "Unknown Interrupt Exception", "Unknown Interrupt Exception", ref aContext);
        }
   
        public static void HandleInterrupt_10(ref IRQContext aContext)
        {
            HandleException(aContext.EIP, "x87 Floating Point Exception", "Coprocessor Fault Exception", ref aContext);
        }

        public static void HandleInterrupt_11(ref IRQContext aContext)
        {
            HandleException(aContext.EIP, "Alignment Exception", "Alignment Exception", ref aContext);
        }

        public static void HandleInterrupt_12(ref IRQContext aContext)
        {
            HandleException(aContext.EIP, "Machine Check Exception", "Machine Check Exception", ref aContext);
        }
        public static void HandleInterrupt_13(ref IRQContext aContext)
        {
            HandleException(aContext.EIP, "SIMD Floating Point Exception", "SIMD Floating Point Exception", ref aContext);
        }


        #endregion

        private static void HandleException(uint aEIP, string aDescription, string aName, ref IRQContext ctx)
        {
          // At this point we are in a very unstable state.
          // Try not to use any Cosmos routines, just
          // report a crash dump.
            const string SysFault = " *** System Fault ***  ";
            const string xHex = "0123456789ABCDEF";
            uint xPtr = ctx.EIP;

            // we're printing exception info to the screen now:
            // 0/0: x
            // 1/0: exception number in hex
            unsafe
            {
                byte* xAddress = (byte*)0xB8000;
                xAddress[0] = (byte)' ';
                xAddress[1] = 0x0C;
                xAddress[2] = (byte)'*';
                xAddress[3] = 0x0C;
                xAddress[4] = (byte)'*';
                xAddress[5] = 0x0C;
                xAddress[6] = (byte)'*';
                xAddress[7] = 0x0C;
                xAddress[8] = (byte)' ';
                xAddress[9] = 0x0C;
                xAddress[10] = (byte)'C';
                xAddress[11] = 0x0C;
                xAddress[12] = (byte)'P';
                xAddress[13] = 0x0C;
                xAddress[14] = (byte)'U';
                xAddress[15] = 0x0C;
                xAddress[16] = (byte)' ';
                xAddress[17] = 0x0C;
                xAddress[18] = (byte)'E';
                xAddress[19] = 0x0C;
                xAddress[20] = (byte)'x';
                xAddress[21] = 0x0C;
                xAddress[22] = (byte)'c';
                xAddress[23] = 0x0C;
                xAddress[24] = (byte)'e';
                xAddress[25] = 0x0C;
                xAddress[26] = (byte)'p';
                xAddress[27] = 0x0C;
                xAddress[28] = (byte)'t';
                xAddress[29] = 0x0C;
                xAddress[30] = (byte)'i';
                xAddress[31] = 0x0C;
                xAddress[32] = (byte)'o';
                xAddress[33] = 0x0C;
                xAddress[34] = (byte)'n';
                xAddress[35] = 0x0C;
                xAddress[36] = (byte)' ';
                xAddress[37] = 0x0C;
                xAddress[38] = (byte)'x';
                xAddress[39] = 0x0C;
                xAddress[40] = (byte)xHex[(int)((ctx.Interrupt >> 4) & 0xF)];
                xAddress[41] = 0x0C;
                xAddress[42] = (byte)xHex[(int)(ctx.Interrupt & 0xF)];
                xAddress[43] = 0x0C;
                xAddress[44] = (byte)' ';
                xAddress[45] = 0x0C;
                xAddress[46] = (byte)'*';
                xAddress[47] = 0x0C;
                xAddress[48] = (byte)'*';
                xAddress[49] = 0x0C;
                xAddress[50] = (byte)'*';
                xAddress[51] = 0x0C;
                xAddress[52] = (byte)' ';
                xAddress[53] = 0x0C;
            }

          // lock up
            while (true)
            {
            }
        }

        // This is to trick IL2CPU to compile it in
        //TODO: Make a new attribute that IL2CPU sees when scanning to force inclusion so we dont have to do this.
        // We dont actually need to cal this method
        public static void Dummy()
        {
            // Compiler magic
            bool xTest = false;
            if (xTest)
            {
                unsafe
                {
                    var xCtx = new IRQContext();
                    HandleInterrupt_Default(ref xCtx);
                    HandleInterrupt_00(ref xCtx);
                    HandleInterrupt_01(ref xCtx);
                    HandleInterrupt_02(ref xCtx);
                    HandleInterrupt_03(ref xCtx);
                    HandleInterrupt_04(ref xCtx);
                    HandleInterrupt_05(ref xCtx);
                    HandleInterrupt_06(ref xCtx);
                    HandleInterrupt_07(ref xCtx);
                    HandleInterrupt_08(ref xCtx);
                    HandleInterrupt_09(ref xCtx);
                    HandleInterrupt_0A(ref xCtx);
                    HandleInterrupt_0B(ref xCtx);
                    HandleInterrupt_0C(ref xCtx);
                    HandleInterrupt_0D(ref xCtx);
                    HandleInterrupt_0E(ref xCtx);
                    HandleInterrupt_0F(ref xCtx);
                    HandleInterrupt_10(ref xCtx);
                    HandleInterrupt_11(ref xCtx);
                    HandleInterrupt_12(ref xCtx);
                    HandleInterrupt_13(ref xCtx);
                    HandleInterrupt_20(ref xCtx);
                    HandleInterrupt_21(ref xCtx);
                    HandleInterrupt_22(ref xCtx);
                    HandleInterrupt_23(ref xCtx);
                    HandleInterrupt_24(ref xCtx);
                    HandleInterrupt_25(ref xCtx);
                    HandleInterrupt_26(ref xCtx);
                    HandleInterrupt_27(ref xCtx);
                    HandleInterrupt_28(ref xCtx);
                    HandleInterrupt_29(ref xCtx);
                    HandleInterrupt_2A(ref xCtx);
                    HandleInterrupt_2B(ref xCtx);
                    HandleInterrupt_2C(ref xCtx);
                    HandleInterrupt_2D(ref xCtx);
                    HandleInterrupt_2E(ref xCtx);
                    HandleInterrupt_2F(ref xCtx);
                    HandleInterrupt_30(ref xCtx);
                    HandleInterrupt_35(ref xCtx);
                    HandleInterrupt_40(ref xCtx);
                    HandleInterrupt_41(ref xCtx);
                    HandleInterrupt_42(ref xCtx);
                    HandleInterrupt_43(ref xCtx);
                    HandleInterrupt_44(ref xCtx);
                    HandleInterrupt_45(ref xCtx);
                    HandleInterrupt_46(ref xCtx);
                    HandleInterrupt_47(ref xCtx);
                    HandleInterrupt_48(ref xCtx);
                    HandleInterrupt_49(ref xCtx);
                }
            }
        }

    }
}