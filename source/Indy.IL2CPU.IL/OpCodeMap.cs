﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Indy.IL2CPU.IL;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Indy.IL2CPU.IL {
	public abstract class OpCodeMap {
		protected readonly SortedList<Code, Type> mMap = new SortedList<Code, Type>();

		protected OpCodeMap() {
			MethodHeaderOp = GetMethodHeaderOp();
			MethodFooterOp = GetMethodFooterOp();
			PInvokeMethodBodyOp = GetPInvokeMethodBodyOp();
			CustomMethodImplementationProxyOp = GetCustomMethodImplementationProxyOp();
			CustomMethodImplementationOp = GetCustomMethodImplementationOp();
			InitVmtImplementationOp = GetInitVmtImplementationOp();
			MainEntryPointOp = GetMainEntryPointOp();
		}

		protected abstract Assembly ImplementationAssembly {
			get;
		}

		protected abstract Type GetMethodHeaderOp();
		protected abstract Type GetMethodFooterOp();
		protected abstract Type GetPInvokeMethodBodyOp();
		protected abstract Type GetCustomMethodImplementationProxyOp();
		protected abstract Type GetCustomMethodImplementationOp();
		protected abstract Type GetInitVmtImplementationOp();
		protected abstract Type GetMainEntryPointOp();

		public virtual void Initialize(Assembler.Assembler aAssembler) {
			foreach (Type t in (from item in ImplementationAssembly.GetTypes()
								where item.IsSubclassOf(typeof(Op)) && item.GetCustomAttributes(typeof(OpCodeAttribute), true).Length > 0
								select item)) {
				object[] xAttribs = t.GetCustomAttributes(typeof(OpCodeAttribute), true);
				try {
					mMap.Add(((OpCodeAttribute)xAttribs[0]).OpCode, t);
				} catch {
					Console.WriteLine("Was adding op " + ((OpCodeAttribute)xAttribs[0]).OpCode);
					throw;
				}
			}
		}

		public Type GetOpForOpCode(Code code) {
			if (!mMap.ContainsKey(code)) {
				throw new NotSupportedException("OpCode '" + code + "' not supported!");
			}
			return mMap[code];
		}

		public readonly Type MethodHeaderOp;
		public readonly Type MethodFooterOp;
		public readonly Type PInvokeMethodBodyOp;
		public readonly Type CustomMethodImplementationProxyOp;
		public readonly Type CustomMethodImplementationOp;
		public readonly Type InitVmtImplementationOp;
		public readonly Type MainEntryPointOp;

		public virtual Type GetOpForCustomMethodImplementation(string aName) {
			return null;
		}

		public virtual MethodReference GetCustomMethodImplementation(string aOrigMethodName, bool aInMetalMode) {
			return null;
		}

		public virtual bool HasCustomAssembleImplementation(MethodInformation aMethod, bool aInMetalMode) {
			return false;
		}

		public virtual void DoCustomAssembleImplementation(bool aInMetalMode, Assembler.Assembler aAssembler, MethodInformation aMethodInfo) {
		}

		public virtual void PostProcess(Assembler.Assembler aAssembler) {
		}
	}
}