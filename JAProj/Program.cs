using JaProj;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JAProj
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

/*
        // Deklaracja funkcji z biblioteki DLL
        [DllImport("C:\\Users\\wojci\\source\\repos\\JAProj\\x64\\Debug\\JADLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void ProcessArray(
            float** data,
            float* coefficients,
            float** output,
            int dataLength,
            int coeffLength,
            int channels
        );
        static unsafe void Main()
        {
            // Ścieżki do plików
            string inputFilePath = "input.wav";
            string outputFilePath = "output32.wav";

            // Ładowanie danych audio
            var (audioSamples, sampleRate, channels, totalSamples) = AudioProcessor.LoadAudioToFloat(inputFilePath);







            // Generowanie współczynników filtra
            AlignedMemoryFloat coefficients = FIRDesigner.GetCoefficientsFromPython(81, 4000f, sampleRate);



            int filterLength = coefficients.Length;
            int overlap = filterLength - 1;

            // Inicjalizacja tablic wyjściowych
            AlignedMemoryFloat[] outputSamples = new AlignedMemoryFloat[channels];
            for (int ch = 0; ch < channels; ch++)
            {
                outputSamples[ch] = new AlignedMemoryFloat(totalSamples);
            }

            // Ustalanie liczby wątków
            int threadCount = 1;

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
                    for (int ch = 0; ch < channels; ch++)
                    {
                        Console.WriteLine($"Kanał {ch}: pierwsze 10 próbek wejściowych:");
                        for (int n = 4000; n < Math.Min(4010, chunks[i].data[ch].Length); n++)
                        {
                            Console.WriteLine($"data[{ch}][{n}] = {chunks[i].data[ch].AlignedPointer[n]:F15}");
                        }
                        Console.WriteLine();

                    }

                    Console.WriteLine(chunks[i].data[0].Length % 8);
                    ProcessArray(ptrData, ptrCoefficients, ptrOutput, chunks[i].data[0].Length, coefficients.Length, channels);

                    for (int ch = 0; ch < channels; ch++)
                    {
                        Console.WriteLine($"Kanał {ch}: pierwsze 10 próbek wyjściowych:");
                        for (int n = 4000; n < Math.Min(4010, chunks[i].output[ch].Length); n++)
                        {
                            Console.WriteLine($"output[{ch}][{n}] = {chunks[i].output[ch].AlignedPointer[n]:F15}");
                        }
                        Console.WriteLine();
                    }

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
            AudioProcessor.SaveFloatArrayToAudio(outputSamples, outputFilePath, sampleRate, channels);

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

            // Wyświetlenie informacji o czasie przetwarzania
            Console.WriteLine($"Przetwarzanie zakończone. Wynik zapisano w {outputFilePath}");
            Console.WriteLine($"Czas wykonania przetwarzania: {stopwatch.Elapsed.TotalMilliseconds} ms");
            Console.ReadLine();
        }
    }

 }

*/