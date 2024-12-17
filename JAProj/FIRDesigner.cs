using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

public class FIRDesigner
{
    public static unsafe AlignedMemoryFloat GetCoefficientsFromPython(int taps, float cutoffFrequency, float samplingRate)
    {
        // Ścieżka do interpretera Pythona
        string pythonExe = "python"; // Upewnij się, że Python jest w PATH lub podaj pełną ścieżkę

        // Nazwa pliku skryptu Pythona
        // Ścieżka do katalogu głównego projektu
        string projectRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..");

        // Ścieżka do skryptu Python
        string scriptPath = Path.Combine(projectRoot, "fir_coefficients.py");

        // Sprawdź, czy plik skryptu istnieje
        if (!File.Exists(scriptPath))
        {
            throw new FileNotFoundException($"Nie znaleziono pliku skryptu Pythona: {scriptPath}");
        }

        // Argumenty dla skryptu
        string args = $"\"{scriptPath}\" {taps} {cutoffFrequency} {samplingRate}";

        // Konfiguracja procesu
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = pythonExe;
        start.Arguments = args;
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;
        start.CreateNoWindow = true;

        using (Process process = Process.Start(start))
        {
            // Odczyt standardowego wyjścia i błędów
            string result = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                // Obsłuż błędy, jeśli wystąpiły
                throw new Exception($"Błąd podczas wykonywania skryptu Pythona: {error}");
            }

            // Parsowanie wyniku JSON
            List<float> coeffList = JsonConvert.DeserializeObject<List<float>>(result);

            if (coeffList == null)
            {
                throw new Exception("Nie udało się zdeserializować współczynników z wyniku skryptu Pythona.");
            }

            int originalLength = coeffList.Count;

            // Wylicz nową długość jako najbliższą wielokrotność 8
            int paddedLength = (originalLength + 7) & ~7;

            // Alokacja pamięci dla wyrównanych współczynników
            AlignedMemoryFloat alignedcoeff = new AlignedMemoryFloat(paddedLength);

            // Przekopiowanie oryginalnych współczynników
            for (int i = 0; i < originalLength; i++)
            {
                alignedcoeff.AlignedPointer[i] = coeffList[i];
            }

            // Uzupełnienie zerami
            for (int i = originalLength; i < paddedLength; i++)
            {
                alignedcoeff.AlignedPointer[i] = 0.0f;
            }

            return alignedcoeff;
        }
    }
}