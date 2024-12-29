using System;
using System.Collections.Generic;
using System.IO;
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

        private static string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;
        private static string ParentDirectory => Path.GetFullPath(Path.Combine(BaseDirectory, "..", "..","..",".."));
        private static string BuildConfiguration =>
        #if DEBUG
            "Debug";
        #else
            "Release";
        #endif

        public static string AsmDllPath => Path.Combine(ParentDirectory, "x64", BuildConfiguration, "JADll.dll");
        public static string CDllPath => Path.Combine(ParentDirectory, "x64", BuildConfiguration, "cDll.dll");

        public ProcessAudioConfig()
        {
            DllPath = AsmDllPath; // Domyślnie ustaw na ASM DLL
        }
    }


}

