﻿using System;


namespace Cosmos.Kernel {
	class ConsoleDrv {
		public static void Main() {
			Console.WriteLine("This is Indy OS...");
			//int xDivider = 0;
			Console.WriteLine("Done");
			Console.WriteLine("Testing IDT");
			TestIDT();
		}

		public static void TestIDT() {
		}
	}
}
