﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmos.System {
    public class Console {
        protected int mX = 0;
        public int X {
            get { return mX; }
            set { 
                mX = value;
                UpdateCursor();
            }
        }

        protected int mY = 0;
        public int Y {
            get { return mY; }
            set { 
                mY = value;
                UpdateCursor();
            }
        }

        public int Cols {
            get { return mText.Cols; }
        }

        public int Rows {
            get { return mText.Rows; }
        }
        
        protected Hardware.TextScreen mText = Hardware.Global.TextScreen;

        public void Clear() {
            mText.Clear();
            mX = 0;
            mY = 0;
            UpdateCursor();
        }

        //TODO: This is slow, batch it and only do it at end of updates
        protected void UpdateCursor() {
            mText.SetCursorPos(mX, mY);
        }

        public void NewLine() {
            mY++;
            mX = 0;
            if (mY == mText.Rows) {
                mText.ScrollUp();
                mY = mText.Rows - 1;
                mX = 0;
            }
            UpdateCursor();
        }

        public void WriteChar(char aChar) {
            mText[mX, mY] = aChar;
            mX++;
            if (mX == mText.Cols) {
                NewLine();
            }
            UpdateCursor();
        }

        public void WriteLine(string aText) {
            Write(aText);
            NewLine();
        }

        //TODO: Optimize this
        public void Write(string aText) {
            for (int i = 0; i < aText.Length; i++) {
                if (aText[i] == '\n') {
                    NewLine();
                } else if (aText[i] == '\r') {
                } else if (aText[i] == '\t') {
                    Write("    ");
                } else {
                    WriteChar(aText[i]);
                }
            }
        }

    }
}
