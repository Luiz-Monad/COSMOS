using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using Orvid.Graphics;

namespace StructTest
{
    public class Kernel : Sys.Kernel
    {
        public Kernel()
        {
            base.ClearScreen = true;
        }

        protected override void BeforeRun()
        {
            Console.WriteLine("Hello, and welcome to the TestOS by Orvid.");
            Console.WriteLine("To see a list of supported commands, type '?'");
        }


        public void DoRun()
        {
            this.Run();
        }

        protected override void Run()
        {
            //Monitor m = new Monitor();
            //byte r = 255;
            //sbyte r = -128;
            //char r = '\u0127';
            //ushort r = 65535;
            //short r = -32768;
            //uint r = 4294967295;
            //int r = -2147483648;
            //ulong r = 18446744073709551615;
            //long r = 9223372036854775807;
            float r = 123.7f;
            //double r = 123.7;
            Console.WriteLine("Just one more test.");
            Console.WriteLine("Raw Byte: " + r.ToString());
            Console.WriteLine("Byte->Byte: " + (unchecked((byte)r)).ToString());
            Console.WriteLine("Byte->SByte: " + (unchecked((sbyte)r)).ToString());
            Console.WriteLine("Byte->Char: " + (unchecked((char)r)).ToString());
            Console.WriteLine("Byte->UShort: " + (unchecked((ushort)r)).ToString());
            Console.WriteLine("Byte->Short: " + (unchecked((short)r)).ToString());
            Console.WriteLine("Byte->UInt: " + (unchecked((uint)r)).ToString());
            Console.WriteLine("Byte->Int: " + (unchecked((int)r)).ToString());
            Console.WriteLine("Byte->ULong: " + (unchecked((ulong)r)).ToString());
            Console.WriteLine("Byte->Long: " + (unchecked((long)r)).ToString());
            Console.WriteLine("Byte->Float: " + (unchecked((float)r)).ToString());
            Console.WriteLine("Byte->Double: " + (unchecked((double)r)).ToString());
            Console.WriteLine("Byte->Decimal: " + (unchecked((decimal)r)).ToString());
            while (true)
            {

            }
        }
    }
}
