﻿using System;

namespace Cosmos.Kernel {

    public static class Heap
    {
        private static uint mStart;
        private static uint mStartAddress;        
        private static uint mLength;
        private static uint mEndOfRam;

        private static void Initialize(uint aStartAddress, uint aEndOfRam)
        {
            mStart = mStartAddress = aStartAddress + (4 - (aStartAddress % 4));
            mLength = aEndOfRam - aStartAddress;
            mLength = (mLength / 4) * 4;
            mStartAddress += 1024;
            mEndOfRam = aEndOfRam;
            mStartAddress = (mStartAddress / 4) * 4;
            mLength -= 1024;
            ClearMemory(aStartAddress, mLength);
            DebugUtil.SendMM_Init(mStartAddress, mLength);
            UpdateDebugDisplay();
        }

        private static void WriteNumber(uint aNumber, byte aBits)
        {
            uint xValue = aNumber;
            byte xCurrentBits = aBits;
            Console.Write("0x");
            while (xCurrentBits >= 4)
            {
                xCurrentBits -= 4;
                byte xCurrentDigit = (byte)((xValue >> xCurrentBits) & 0xF);
                string xDigitString = null;
                switch (xCurrentDigit)
                {
                    case 0:
                        Console.Write('0');
                        break;
                    case 1:
                        Console.Write('1');
                        break;
                    case 2:
                        Console.Write('2');
                        break;
                    case 3:
                        Console.Write('3');
                        break;
                    case 4:
                        Console.Write('4');
                        break;
                    case 5:
                        Console.Write('5');
                        break;
                    case 6:
                        Console.Write('6');
                        break;
                    case 7:
                        Console.Write('7');
                        break;
                    case 8:
                        Console.Write('8');
                        break;
                    case 9:
                        Console.Write('9');
                        break;
                    case 10:
                        Console.Write('A');
                        break;
                    case 11:
                        Console.Write('B');
                        break;
                    case 12:
                        Console.Write('C');
                        break;
                    case 13:
                        Console.Write('D');
                        break;
                    case 14:
                        Console.Write('E');
                        break;
                    case 15:
                        Console.Write('F');
                        break;
                }
            }
        }
        
        private static bool mDebugDisplayInitialized = false;

        // this method displays the used/total memory of the heap on the first line of the text screen
        private static void UpdateDebugDisplay() {
            if (!mDebugDisplayInitialized) {
                mDebugDisplayInitialized = true;
                int xOldPositionLeft = Console.CursorLeft;
                int xOldPositionTop = Console.CursorTop;
                Console.CursorLeft = 0;
                Console.CursorTop = 0;
                Console.Write("[Heap Usage: ");
                WriteNumber(mStartAddress,
                           32);
                Console.Write("/");
                WriteNumber(mEndOfRam,
                          32);
                Console.Write("] bytes");
                while(Console.CursorLeft < (Console.WindowWidth-1)) {
                   Console.Write(" ");
             }
                Console.CursorLeft = xOldPositionLeft;
                Console.CursorTop = xOldPositionTop;
            }else {
                int xOldPositionLeft = Console.CursorLeft;
                int xOldPositionTop = Console.CursorTop;
                Console.CursorLeft = 13;
                Console.CursorTop = 0;
                WriteNumber(mStartAddress,
                            32);
                Console.CursorLeft = xOldPositionLeft;
                Console.CursorTop = xOldPositionTop;
            }
        }

        private static bool mInited;
        public static void Init()
        {
            if (!mInited)
            {
                mInited = true;
                Initialize(CPU.EndOfKernel, (CPU.AmountOfMemory * 1024 * 1024) - 1024);
            }
        }
        private static void ClearMemory(uint aStartAddress, uint aLength)
        {
            CPU.ZeroFill(aStartAddress, aLength);
        }

        public static uint MemAlloc(uint aLength)
        {
            Init();
            uint xTemp = mStartAddress;
            if ((xTemp + aLength) > (mStart + mLength))
            {
                Console.WriteLine("Too large memory block allocated!");
                System.Diagnostics.Debugger.Break();
            }
            mStartAddress += aLength;
            DebugUtil.SendMM_Alloc(xTemp, aLength);
            UpdateDebugDisplay();
            return xTemp;
        }

        public static void MemFree(uint aPointer)
        {
        }

    }

    #region old
    //	public unsafe static class Heap {
//		private enum MemoryBlockState: byte {
//			Free,
//			Used,
//			EndOfMemory
//		}
//		private unsafe struct MemoryBlock {
//			public MemoryBlockState State;
//			public MemoryBlock* Next;
//			public byte FirstByte;
//		}
//		private static uint mStart;
//		private static uint mStartAddress;
//		//		private static uint mCurrentAddress = mStartAddress;
//		private static uint mLength;
//		private static MemoryBlock* mFirstBlock;
//		//private const uint DefaultStartAddress = 4 * 1024 * 1024;
//		//private const uint DefaultMaxMemory = 32 * 1024 * 1024;
//
//		private static void ClearMemory(uint aStartAddress, uint aLength) {
//			//int xStart = (RTC.GetMinutes() * 60) + RTC.GetSeconds();
//			CPU.ZeroFill(aStartAddress, aLength);
//			//int xEnd = (RTC.GetMinutes() * 60) + RTC.GetSeconds();
//			//int xDiff = xEnd - xStart;
//			//Console.Write("Time to clear ");
//			//Hardware.Storage.ATAOld.WriteNumber((uint)xDiff, 32);
//			//Console.WriteLine("");
//		}
//
//		private static void Initialize(uint aStartAddress, uint aEndOfRam) {
//			mStart = mStartAddress = aStartAddress + (4 - (aStartAddress % 4));
//			mLength = aEndOfRam - aStartAddress;
//			mLength = (mLength / 4) * 4;
//			mStartAddress += 1024;
//			mStartAddress = (mStartAddress / 4) * 4;
//			mLength -= 1024;
//			//Console.Write("Clearing Memory at ");
//			int xCursorLeft = Console.CursorLeft;
//			// hack: try to get this working with the full chunk or chunks of 1MB
//			//const int xBlockSize = 1024 * 1024;
//			//for (uint i = 0; i < (mLength / xBlockSize); i++) {
//			//    Console.CursorLeft = xCursorLeft;
//			//    Hardware.Storage.ATAOld.WriteNumber(mStartAddress + (i * xBlockSize), 32);
//			//    ClearMemory(mStartAddress + (i * xBlockSize), xBlockSize);
//			//}
//			//Console.Write("Clearing Memory....");
//			ClearMemory(aStartAddress, mLength);
//			//Console.WriteLine("Done");
//			//mFirstBlock = (MemoryBlock*)aStartAddress;
//			//mFirstBlock->State = MemoryBlockState.Free;
//			//mFirstBlock->Next = (MemoryBlock*)(aStartAddress + mLength);
//			//mFirstBlock->Next->State = MemoryBlockState.EndOfMemory;
//			DebugUtil.SendMM_Init(mStartAddress, mLength);
//		}
//		private static bool mInited;
//		public static void Init() {
//			if (!mInited) {
//				mInited = true;
//				Initialize(CPU.EndOfKernel, (CPU.AmountOfMemory * 1024 * 1024) - 1024);
//			}
//		}
//
//		public static uint MemAlloc(uint aLength) {
//			Init();
//			uint xTemp = mStartAddress;
//			if ((xTemp + aLength) > (mStart + mLength)) {
//				Console.WriteLine("Too large memory block allocated!");
//				Console.Write("   BlockSize = ");
//                // dont use .ToString here, as it uses heap...
//                WriteNumber(aLength, 32);
//				Console.WriteLine("");
//				System.Diagnostics.Debugger.Break();
//			}
//			mStartAddress += aLength;
//			DebugUtil.SendMM_Alloc(xTemp, aLength);
//			return xTemp;
//			//CheckInit();
//			//MemoryBlock* xCurrentBlock = mFirstBlock;
//			//bool xFound = false;
//			//while (!xFound) {
//			//    if (xCurrentBlock->State == MemoryBlockState.EndOfMemory) {
//			//        DebugUtil.SendError("MM", "Reached maximum memory");
//			//        return 0;
//			//    }
//			//    if (xCurrentBlock->Next == null) {
//			//        DebugUtil.SendError("MM", "No next block found, but not yet at EOM", (uint)xCurrentBlock, 32);
//			//        return 0;
//			//    }
//			//    if (((((uint)xCurrentBlock->Next) - ((uint)xCurrentBlock)) >= (aLength + 5)) && (xCurrentBlock->State == MemoryBlockState.Free)) {
//			//        xFound = true;
//			//        break;
//			//    }
//			//    xCurrentBlock = xCurrentBlock->Next;
//			//}
//			//uint xFoundBlockSize = (((uint)xCurrentBlock->Next) - ((uint)xCurrentBlock));
//			//if (xFoundBlockSize > (aLength + 37)) {
//			//    MemoryBlock* xOldNextBlock = xCurrentBlock->Next;
//			//    xCurrentBlock->Next = (MemoryBlock*)(((uint)xCurrentBlock) + aLength + 5);
//			//    xCurrentBlock->Next->Next = xOldNextBlock;
//			//    xCurrentBlock->Next->State = MemoryBlockState.Free;
//			//}
//			//xCurrentBlock->State = MemoryBlockState.Used;
//			//DebugUtil.SendMM_Alloc((uint)xCurrentBlock, aLength);
//			//return ((uint)xCurrentBlock) + 5;
//		}
//
//		public static void MemFree(uint aPointer) {
//			//MemoryBlock* xBlock = (MemoryBlock*)(aPointer - 5);
//			//DebugUtil.SendMM_Free(aPointer - 5, (((uint)xBlock->Next) - ((uint)xBlock)));
//			//xBlock->State = MemoryBlockState.Free;
//			//uint xLength = ((uint)xBlock->Next) - aPointer;
//			//ClearMemory(aPointer, xLength);
//		}
//
//        public static void WriteNumber(uint aNumber, byte aBits)
//        {
//            uint xValue = aNumber;
//            byte xCurrentBits = aBits;
//            Console.Write("0x");
//            while (xCurrentBits >= 4)
//            {
//                xCurrentBits -= 4;
//                byte xCurrentDigit = (byte)((xValue >> xCurrentBits) & 0xF);
//                string xDigitString = null;
//                switch (xCurrentDigit)
//                {
//                    case 0:
//                        xDigitString = "0";
//                        
//                    case 1:
//                        xDigitString = "1";
//                        
//                    case 2:
//                        xDigitString = "2";
//                        
//                    case 3:
//                        xDigitString = "3";
//                        
//                    case 4:
//                        xDigitString = "4";
//                        
//                    case 5:
//                        xDigitString = "5";
//                        
//                    case 6:
//                        xDigitString = "6";
//                        
//                    case 7:
//                        xDigitString = "7";
//                        
//                    case 8:
//                        xDigitString = "8";
//                        
//                    case 9:
//                        xDigitString = "9";
//                        
//                    case 10:
//                        xDigitString = "A";
//                        
//                    case 11:
//                        xDigitString = "B";
//                        
//                    case 12:
//                        xDigitString = "C";
//                        
//                    case 13:
//                        xDigitString = "D";
//                        
//                    case 14:
//                        xDigitString = "E";
//                        
//                    case 15:
//                        xDigitString = "F";
//                        
//                    default:
//                        Console.Write(xDigitString);
//                        break;
//                }
//            }
//        }
    //	}
    #endregion old
}
