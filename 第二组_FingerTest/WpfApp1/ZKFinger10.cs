using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WpfApp1
{
    class ZKFinger10
    {       
        public const string ZKFPRI_NAME = @"ZKFinger10.dll";

        public const int THRESHOLD_MIDDLE = 55; // 1:N recommendation threshold
        public const int THRESHOLD_LOW = 35;    // 1:1 recommendation threshold

        [DllImport(ZKFPRI_NAME, EntryPoint = "BIOKEY_INIT")]
        public static extern IntPtr BIOKEY_INIT(int License, short[] isize, byte[] Params, byte[] Buffer, int ImageFlag);

        [DllImport(ZKFPRI_NAME, EntryPoint = "BIOKEY_CLOSE")]
        public static extern int BIOKEY_CLOSE(IntPtr Handle);

        [DllImport(ZKFPRI_NAME, EntryPoint = "BIOKEY_SET_PARAMETER")]
        public static extern int BIOKEY_SET_PARAMETER(IntPtr Handle, int paramCode, int paramValue);

        [DllImport(ZKFPRI_NAME, EntryPoint = "BIOKEY_MATCHINGPARAM")]
        public static extern int BIOKEY_MATCHINGPARAM(IntPtr Handle, int speed, int threshold);

        [DllImport(ZKFPRI_NAME, EntryPoint = "BIOKEY_GETLASTQUALITY")]
        public static extern int BIOKEY_GETLASTQUALITY();

        [DllImport(ZKFPRI_NAME, EntryPoint = "BIOKEY_GETLASTERROR")]
        public static extern int BIOKEY_GETLASTERROR();

        [DllImport(ZKFPRI_NAME, EntryPoint = "BIOKEY_EXTRACT")]
        public static extern int BIOKEY_EXTRACT(IntPtr Handle, byte[] PixelsBuffer, byte[] Template, int PurposeMode);

        [DllImport(ZKFPRI_NAME, EntryPoint = "BIOKEY_GENTEMPLATE")]
        public unsafe static extern int BIOKEY_GENTEMPLATE(IntPtr Handle, byte*[] Templates, int TmpCount, byte[] GTemplate);

        [DllImport(ZKFPRI_NAME, EntryPoint = "BIOKEY_GENTEMPLATE_SP")]
        public unsafe static extern int BIOKEY_GENTEMPLATE_SP(IntPtr Handle, byte[] Template1, byte[] Template2, byte[] Template3, int TmpCount, byte[] GTemplate);

        [DllImport(ZKFPRI_NAME, EntryPoint = "BIOKEY_DB_ADD")]
        public static extern int BIOKEY_DB_ADD(IntPtr Handle, int TID, int TempLength, byte[] Template);

        [DllImport(ZKFPRI_NAME, EntryPoint = "BIOKEY_DB_DEL")]
        public static extern int BIOKEY_DB_DEL(IntPtr Handle, int TID);

        [DllImport(ZKFPRI_NAME, EntryPoint = "BIOKEY_DB_COUNT")]
        public static extern int BIOKEY_DB_COUNT(IntPtr Handle);

        [DllImport(ZKFPRI_NAME, EntryPoint = "BIOKEY_DB_CLEAR")]
        public static extern int BIOKEY_DB_CLEAR(IntPtr Handle);

        [DllImport(ZKFPRI_NAME, EntryPoint = "BIOKEY_IDENTIFYTEMP")]
        public static extern int BIOKEY_IDENTIFYTEMP(IntPtr Handle, byte[] Template, ref int TID, ref int Score);

        [DllImport(ZKFPRI_NAME, EntryPoint = "BIOKEY_VERIFY")]
        public static extern int BIOKEY_VERIFY(IntPtr handle, byte[] Template1, byte[] Template2);        
    }
}
