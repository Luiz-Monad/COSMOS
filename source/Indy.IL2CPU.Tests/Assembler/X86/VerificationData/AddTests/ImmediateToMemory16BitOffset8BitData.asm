use32
org 0x200000
			add byte [EAX + 0x4B3], 0x41
			add byte [EBX + 0x4B3], 0x41
			add byte [ECX + 0x4B3], 0x41
			add byte [EDX + 0x4B3], 0x41
			add byte [ESI + 0x4B3], 0x41
			add byte [EDI + 0x4B3], 0x41
			add byte [EBP + 0x4B3], 0x41
			add byte [ESP + 0x4B3], 0x41
