﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Indy.IL2CPU.Assembler;
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

		public virtual void Initialize(Assembler.Assembler aAssembler, Func<TypeReference, TypeDefinition> aTypeResolver) {
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
			InitializePlugMethodsList(aAssembler, aTypeResolver);
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
		private SortedList<string, MethodDefinition> mPlugMethods;

		private static string GetMethodDefinitionFullName(MethodReference aSelf) {
			StringBuilder sb = new StringBuilder(aSelf.ReturnType.ReturnType.FullName + " " + aSelf.DeclaringType.FullName + "." + aSelf.Name);
			sb.Append("(");
			if (aSelf.Parameters.Count > 0) {

				foreach (ParameterDefinition xParam in aSelf.Parameters) {
					sb.Append(xParam.ParameterType.FullName);
					sb.Append(",");
				}
			}
			return sb.ToString().TrimEnd(',') + ")";
		}

		private void InitializePlugMethodsList(Assembler.Assembler aAssembler, Func<TypeReference, TypeDefinition> aTypeResolver) {
			System.Diagnostics.Debugger.Break();
			if (mPlugMethods != null) {
				throw new Exception("PlugMethods list already initialized!");
			}
			mPlugMethods = new SortedList<string, MethodDefinition>();
			PlugScopeEnum xNotWantedScope;
			if (aAssembler.InMetalMode) {
				xNotWantedScope = PlugScopeEnum.NonMetalOnly;
			} else {
				xNotWantedScope = PlugScopeEnum.MetalOnly;
			}
			foreach (AssemblyDefinition xAssemblyDef in GetPlugAssemblies()) {
				foreach (ModuleDefinition xModuleDef in xAssemblyDef.Modules) {
					foreach (TypeDefinition xType in (from item in xModuleDef.Types.Cast<TypeDefinition>()
													  where item.CustomAttributes.Cast<CustomAttribute>().Count(x => x.Constructor.DeclaringType.FullName == typeof(PlugAttribute).FullName && (x.Fields["Scope"] == null || (PlugScopeEnum)x.Fields["Scope"] == xNotWantedScope)) != 0
													  select item)) {
						CustomAttribute xPlugAttrib = (from item in xType.CustomAttributes.Cast<CustomAttribute>()
													   where item.Constructor.DeclaringType.FullName == typeof(PlugAttribute).FullName
													   select item).First();
						TypeReference xTypeRef = xModuleDef.TypeReferences.Cast<TypeReference>().First(x => (x.FullName + ", " + x.Scope.ToString()) == (string)xPlugAttrib.Fields["Target"]);
						if (xTypeRef == null) {
							throw new Exception("TypeRef for '" + (string)xPlugAttrib.Fields["ReplaceType"] + "' not found!");
						}
						TypeDefinition xReplaceTypeDef = aTypeResolver(xTypeRef);
						foreach (MethodDefinition xMethod in (from item in xType.Methods.Cast<MethodDefinition>()
															  where item.CustomAttributes.Cast<CustomAttribute>().Count(x => x.Constructor.DeclaringType.FullName == typeof(PlugMethodAttribute).FullName && (x.Fields["Scope"] == null || (PlugScopeEnum)x.Fields["Scope"] != xNotWantedScope)) != 0
															  select item)) {
							CustomAttribute xPlugMethodAttrib = (from item in xMethod.CustomAttributes.Cast<CustomAttribute>()
																 where item.Constructor.DeclaringType.FullName == typeof(PlugMethodAttribute).FullName
																 select item).First();
							string xSignature = (string)xPlugMethodAttrib.Fields["Signature"];
							if (!String.IsNullOrEmpty(xSignature)) {
								mPlugMethods.Add(xSignature, xMethod);
								continue;
							}
							string xStrippedSignature = GetMethodDefinitionFullName(xMethod).Replace(xType.FullName, "");
							foreach (MethodDefinition xOrigMethodDef in xReplaceTypeDef.Methods) {
								string xOrigStrippedSignature = GetMethodDefinitionFullName(xOrigMethodDef).Replace(xReplaceTypeDef.FullName, "");
								if (xOrigStrippedSignature == xStrippedSignature) {
									mPlugMethods.Add(Label.GenerateLabelName(xOrigMethodDef), xMethod);
								}
							}
						}
					}
				}
			}
			Console.Write(new String('-', Console.WindowWidth));
			Console.WriteLine("Recognized Plug methods:");
			foreach (string s in mPlugMethods.Keys) {
				Console.WriteLine(s);
			}
			Console.Write(new String('-', Console.WindowWidth));
		}

		public virtual Type GetOpForCustomMethodImplementation(string aName) {
			return null;
		}

		protected virtual IList<AssemblyDefinition> GetPlugAssemblies() {
			List<AssemblyDefinition> xResult = new List<AssemblyDefinition>();
			xResult.Add(AssemblyFactory.GetAssembly(typeof(OpCodeMap).Assembly.Location));
			return xResult;
		}

		public MethodReference GetCustomMethodImplementation(string aOrigMethodName, bool aInMetalMode) {
			if (mPlugMethods.ContainsKey(aOrigMethodName)) {
				return mPlugMethods[aOrigMethodName];
			}
			return null;
		}

		[Obsolete("Try to use the GetPlugAssemblies infrastructure!")]
		public virtual MethodReference GetCustomMethodImplementation_Old(string aOrigMethodName, bool aInMetalMode) {
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