using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JaProj
{
    public class ProcessAudioConfig
    {

        public string DllPath { get; set; }
        public string InputFilePath { get; set; }
        public string OutputFilePath { get; set; }
        public int CutOffFrequency { get; set; }
        public int FilterLength { get; set; }
        public int ThreadCount { get; set; }

        public static readonly string AsmDllPath = @"C:\Users\wojci\source\repos\JAProj\x64\Debug\JADll.dll";
        public static readonly string CDllPath = @"C:\Users\wojci\source\repos\JAProj\x64\Debug\cDll.dll";
    }


}

