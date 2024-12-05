using JAProj;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JaProj
{
    public class  ProcessAudio
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr hModule);

        private unsafe delegate void ProcessArrayDelegate(
            float** data,
            float* coefficients,
            float** output,
            int dataLength,
            int coeffLength,
            int channels
        );
        private ProcessArrayDelegate processArray;
        private IntPtr dllHandle;

        private void LoadDll(string dllPath)
        {
            dllHandle = LoadLibrary(dllPath);
            if (dllHandle == IntPtr.Zero)
            {
                throw new Exception("Failed to load DLL");
            }
            IntPtr procAddress = GetProcAddress(dllHandle, "ProcessArray");
            if (procAddress == IntPtr.Zero)
            {
                throw new Exception("Failed to get ProcessArray function");
            }
            processArray = Marshal.GetDelegateForFunctionPointer<ProcessArrayDelegate>(procAddress);
        }

        public void UnloadDll()
        {
            if (dllHandle != IntPtr.Zero)
            {
                FreeLibrary(dllHandle);
                dllHandle = IntPtr.Zero;
            }
        }

        public unsafe long Process(ProcessAudioConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            LoadDll(config.DllPath);


            // Wczytaj dane audio
            var (audioSamples, sampleRate, channels, totalSamples) = AudioProcessor.LoadAudioToFloat(config.InputFilePath);

            // Generuj współczynniki filtra
            AlignedMemoryFloat coefficients = FIRDesigner.GetCoefficientsFromPython(config.FilterLength, config.CutOffFrequency, sampleRate);


            int filterLength = coefficients.Length;
            int overlap = filterLength - 1;

            // Inicjalizacja tablic wyjściowych
            AlignedMemoryFloat[] outputSamples = new AlignedMemoryFloat[channels];
            for (int ch = 0; ch < channels; ch++)
            {
                outputSamples[ch] = new AlignedMemoryFloat(totalSamples);
            }

            // Ustalanie liczby wątków
            int threadCount = config.ThreadCount;

            // Przygotowanie fragmentów danych do przetwarzania
            var chunks = new (AlignedMemoryFloat[] data, AlignedMemoryFloat[] output, int startIndex)[threadCount];

            int samplesPerThread = totalSamples / threadCount;
            int remainingSamples = totalSamples % threadCount;

            int currentSampleIndex = 0;

            for (int i = 0; i < threadCount; i++)
            {
                int chunkSize = samplesPerThread + (i < remainingSamples ? 1 : 0);

                int chunkStart = currentSampleIndex - overlap;
                int chunkLength = chunkSize + overlap * 2;

                if (chunkStart < 0)
                {
                    chunkStart = 0;
                    chunkLength = chunkSize + overlap;
                }
                if (chunkStart + chunkLength > totalSamples)
                {
                    chunkLength = totalSamples - chunkStart;
                }

                // Przygotowanie danych dla wszystkich kanałów
                AlignedMemoryFloat[] chunkData = new AlignedMemoryFloat[channels];
                AlignedMemoryFloat[] chunkOutput = new AlignedMemoryFloat[channels];
                for (int ch = 0; ch < channels; ch++)
                {
                    chunkData[ch] = new AlignedMemoryFloat(chunkLength);

                    // Kopiowanie danych do fragmentu
                    Buffer.MemoryCopy(
                        audioSamples[ch].AlignedPointer + chunkStart,
                        chunkData[ch].AlignedPointer,
                        chunkLength * sizeof(float),
                        chunkLength * sizeof(float)
                    );

                    chunkOutput[ch] = new AlignedMemoryFloat(chunkLength);
                }

                chunks[i] = (chunkData, chunkOutput, currentSampleIndex);
                currentSampleIndex += chunkSize;
            }


            // Uruchomienie stopera do pomiaru czasu przetwarzania
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Przetwarzanie fragmentów równolegle
            Parallel.For(0, threadCount, i =>
            {
                unsafe
                {
                    float* ptrCoefficients = coefficients.AlignedPointer;


                    float** ptrData = stackalloc float*[channels];
                    float** ptrOutput = stackalloc float*[channels];

                    for (int ch = 0; ch < channels; ch++)
                    {
                        ptrData[ch] = chunks[i].data[ch].AlignedPointer;
                        ptrOutput[ch] = chunks[i].output[ch].AlignedPointer;
                    }
                    processArray(ptrData, ptrCoefficients, ptrOutput, chunks[i].data[0].Length, coefficients.Length, channels);

                }
            });

            stopwatch.Stop();

            // Scalanie wyników
            foreach (var chunk in chunks)
            {
                int outputStartIndex = (chunk.startIndex == 0) ? 0 : overlap;
                int outputLength = chunk.output[0].Length - ((chunk.startIndex == 0 || chunk.startIndex + chunk.output[0].Length >= totalSamples) ? overlap : overlap * 2);
                int destinationIndex = chunk.startIndex;

                for (int ch = 0; ch < channels; ch++)
                {
                    if (destinationIndex + outputLength > outputSamples[ch].Length)
                    {
                        outputLength = outputSamples[ch].Length - destinationIndex;
                    }

                    // Kopiowanie wyników do głównych tablic wyjściowych
                    Buffer.MemoryCopy(
                        chunk.output[ch].AlignedPointer + outputStartIndex,
                        outputSamples[ch].AlignedPointer + destinationIndex,
                        (outputSamples[ch].Length - destinationIndex) * sizeof(float),
                        outputLength * sizeof(float)
                    );
                }
            }

            // Ograniczanie wartości do zakresu [-1, 1]
            for (int ch = 0; ch < channels; ch++)
            {
                for (int n = 0; n < totalSamples; n++)
                {
                    if (outputSamples[ch].AlignedPointer[n] > 1f)
                        outputSamples[ch].AlignedPointer[n] = 1f;
                    else if (outputSamples[ch].AlignedPointer[n] < -1f)
                        outputSamples[ch].AlignedPointer[n] = -1f;
                }
            }

            //Zapisanie przetworzonych danych do pliku
            AudioProcessor.SaveFloatArrayToAudio(outputSamples, config.OutputFilePath, sampleRate, channels);

            // Zwolnienie pamięci
            foreach (var sample in audioSamples)
            {
                sample.Dispose();
            }

            foreach (var sample in outputSamples)
            {
                sample.Dispose();
            }

            foreach (var chunk in chunks)
            {
                foreach (var data in chunk.data)
                {
                    data.Dispose();
                }
                foreach (var output in chunk.output)
                {
                    output.Dispose();
                }
            }

            return stopwatch.ElapsedMilliseconds;
        }
    }
}
