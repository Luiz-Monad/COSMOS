format PE console
entry ___ENTRYPOINT___


section '.code' code readable executable

	___ENTRYPOINT___:
		call System_Void___IL2CPU_Tests_Tests_TestEmptyMethodApp_Main____
		pushd 0
		call [ExitProcess]

	System_Void___IL2CPU_Tests_Tests_TestEmptyMethodApp_Main____:
		mov ebp,esp
		pushd ebp
		; IL: Nop 
		nop
		; IL: Ldc_I4_5 
		pushd 5
		; IL: Stloc_0 
		pop eax
		mov [ebp - 12],eax
		; IL: Ret 
		pop ebp
		ret 

section '.idata' import data readable writeable

	dd 0,0,0,rva kernel_name,rva kernel_table
	dd 0,0,0,0,0

	kernel_table:
		ExitProcess dd rva _ExitProcess
		dd 0

	kernel_name db 'KERNEL32.DLL',0

	_ExitProcess dw 0
	db 'ExitProcess',0
