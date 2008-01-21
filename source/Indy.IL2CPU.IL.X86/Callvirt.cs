using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using CPU = Indy.IL2CPU.Assembler;
using CPUx86 = Indy.IL2CPU.Assembler.X86;

namespace Indy.IL2CPU.IL.X86 {
	[OpCode(Code.Callvirt, true)]
	public class Callvirt: Op {
		private readonly int mMethodIdentifier;
		private readonly string mNormalAddress;
		private readonly string mMethodDescription;
		private readonly int mThisOffset;
		private readonly int mArgumentCount;
		private readonly int mReturnSize;
		private readonly string mLabelName;
		private readonly MethodInformation mMethodInfo;
		public Callvirt(Instruction aInstruction, MethodInformation aMethodInfo)
			: base(aInstruction, aMethodInfo) {
			mLabelName = GetInstructionLabel(aInstruction);
			mMethodInfo = aMethodInfo;
			int xThisOffSet = (from item in aMethodInfo.Locals
							   select item.Offset + item.Size).LastOrDefault();
			MethodReference xMethod = aInstruction.Operand as MethodReference;
			if (xMethod == null) {
				throw new Exception("Unable to determine Method!");
			}
			MethodDefinition xMethodDef = Engine.GetDefinitionFromMethodReference(xMethod);
			mMethodDescription = CPU.Label.GenerateLabelName(xMethodDef);
			MethodInformation xTheMethodInfo = Engine.GetMethodInfo(xMethodDef, xMethodDef, mMethodDescription, null);
			if (xMethodDef.IsStatic || !xMethodDef.IsVirtual) {
				Engine.QueueMethod(xMethodDef);
				mNormalAddress = CPU.Label.GenerateLabelName(xMethodDef);
				mReturnSize = xTheMethodInfo.ReturnSize;
				return;
			}
			mMethodIdentifier = Engine.GetMethodIdentifier(xMethodDef);
			Engine.QueueMethodRef(VTablesImplRefs.GetMethodAddressForTypeRef);
			mArgumentCount = xTheMethodInfo.Arguments.Length;
			mReturnSize = xTheMethodInfo.ReturnSize;
			mThisOffset = xTheMethodInfo.Arguments[0].Offset;
		}

		public override void DoAssemble() {
			Action xEmitCleanup = delegate() {
				foreach (MethodInformation.Argument xArg in mMethodInfo.Arguments) {
					new CPUx86.Add("esp", xArg.Size.ToString());
				}
			};
			if (!String.IsNullOrEmpty(mNormalAddress)) {
				new CPUx86.Call(mNormalAddress);
			} else {
				if (Assembler.InMetalMode) {
					throw new Exception("Virtual methods not supported in Metal mode! (Called method = '" + mMethodDescription + "')");
				}
				//Assembler.Add(new CPUx86.Pop("eax"));
				//Assembler.Add(new CPUx86.Pushd("eax"));
				EmitCompareWithNull(Assembler, mMethodInfo, "[esp + 0x" + mThisOffset.ToString("X") + "]", mLabelName+ "_AfterNullRefCheck", xEmitCleanup);
				new CPU.Label(mLabelName + "_AfterNullRefCheck");
				new CPUx86.Move(CPUx86.Registers.EAX, "[esp + 0x" + mThisOffset.ToString("X") + "]");
				new CPUx86.Pushd(CPUx86.Registers.AtEAX);
				new CPUx86.Pushd("0" + mMethodIdentifier.ToString("X") + "h");
				new CPUx86.Call(CPU.Label.GenerateLabelName(VTablesImplRefs.GetMethodAddressForTypeRef));
				new CPUx86.Call(CPUx86.Registers.EAX);
			}
			if (!Assembler.InMetalMode) {
				new CPUx86.Test("ecx", "2");
				new CPUx86.JumpIfNotEquals(MethodFooterOp.EndOfMethodLabelNameException);
			}
			new CPU.Comment("Argument Count = " + mArgumentCount.ToString());
			for (int i = 0; i < mArgumentCount; i++) {
				Assembler.StackSizes.Pop();
			}
			if (mReturnSize == 0) {
				return;
			}
			if (mReturnSize <= 4) {
				new CPUx86.Pushd(CPUx86.Registers.EAX);
				Assembler.StackSizes.Push(mReturnSize);
				return;
			}
			if (mReturnSize <= 8) {
				new CPUx86.Pushd(CPUx86.Registers.EBX);
				new CPUx86.Pushd(CPUx86.Registers.EAX);
				Assembler.StackSizes.Push(mReturnSize);
				return;
			}
		}
	}
}