using System;

namespace Cosmos.IL2CPU.X86.IL
{
    [Cosmos.IL2CPU.OpCode( ILOpCode.Code.Ldelem_U2 )]
    public class Ldelem_U2 : ILOp
    {
        public Ldelem_U2( Cosmos.IL2CPU.Assembler aAsmblr )
            : base( aAsmblr )
        {
        }

        public override void Execute( MethodInfo aMethod, ILOpCode aOpCode )
        {
            Ldelem_Ref.Assemble( Assembler, 2 );
        }


        // using System;
        // using System.Collections.Generic;
        // using System.IO;
        // using CPU = Cosmos.IL2CPU.X86;
        // using CPUx86 = Cosmos.IL2CPU.X86;
        // 
        // namespace Indy.IL2CPU.IL.X86 {
        //     [OpCode(OpCodeEnum.Ldelem_U2)]
        //     public class Ldelem_U2 : Op {
        //         
        //         //public static void ScanOp(ILReader aReader,
        //         //                          MethodInformation aMethodInfo,
        //         //                          SortedList<string, object> aMethodData) {
        //         //    Engine.RegisterType(typeof(ushort));
        //         //}
        // 
        //         public Ldelem_U2(ILReader aReader,
        //                          MethodInformation aMethodInfo)
        //             : base(aReader,
        //                    aMethodInfo) {
        //         }
        // 
        //         public override void DoAssemble() {
        //             Ldelem_Ref.Assemble(Assembler,
        //                                 2);
        //         }
        //     }
        // }

    }
}
