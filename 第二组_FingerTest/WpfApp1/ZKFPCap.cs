using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WpfApp1
{
    class ZKFPCap
    {       
        public const string ZKFPRI_NAME = @"ZKFPCap.dll";

        [DllImport(ZKFPRI_NAME, EntryPoint = "sensorInit")]
        public static extern int sensorInit();

        [DllImport(ZKFPRI_NAME, EntryPoint = "sensorFree")]
        public static extern int sensorFree();

        [DllImport(ZKFPRI_NAME, EntryPoint = "sensorOpen")]
        public static extern IntPtr sensorOpen(int index);

        [DllImport(ZKFPRI_NAME, EntryPoint = "sensorClose")]
        public static extern int sensorClose(IntPtr handle);

        [DllImport(ZKFPRI_NAME, EntryPoint = "sensorGetCount")]
        public static extern int sensorGetCount();

        [DllImport(ZKFPRI_NAME, EntryPoint = "sensorGetVersion")]
        public static extern int sensorGetVersion(byte[] version, int len);       

        [DllImport(ZKFPRI_NAME, EntryPoint = "sensorGetParameter")]
        public static extern int sensorGetParameter(IntPtr handle, int paramCode);

        [DllImport(ZKFPRI_NAME, EntryPoint = "sensorSetParameter")]
        public static extern int sensorSetParameter(IntPtr handle, int paramCode, int paramValue);

        [DllImport(ZKFPRI_NAME, EntryPoint = "sensorGetParameterEx")]
        public static extern int sensorGetParameterEx(IntPtr handle, int paramCode, byte[] paramValue, ref int paramLen);        

        [DllImport(ZKFPRI_NAME, EntryPoint = "sensorSetParameterEx")]
        public static extern int sensorSetParameterEx(IntPtr handle, int paramCode, byte[] paramValue, int paramLen);        

        [DllImport(ZKFPRI_NAME, EntryPoint = "sensorCapture")]
        public static extern int sensorCapture(IntPtr handle, byte[] imageBuffer, int imageBufferSize);    
    }
}
