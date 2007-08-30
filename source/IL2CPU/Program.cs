﻿using System;
using System.IO;
using Indy.IL2CPU;
using Indy.IL2CPU.IL.X86;

namespace IL2CPU {
	public class Program {
		public static void Main(string[] args) {
			try {
				Engine e = new Engine();
				using (FileStream fs = new FileStream(@"output", FileMode.Create)) {
					using (BinaryWriter br = new BinaryWriter(fs)) {
						e.Execute("HelloWorld.exe", typeof (Noop).Assembly.GetName().ToString(), br);
					}
				}
			} catch (Exception E) {
				Console.WriteLine(E.ToString());
			}
			Console.ReadLine();
		}
	}
}