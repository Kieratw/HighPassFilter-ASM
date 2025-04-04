﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using JaProj;
using NAudio.Wave;
using NAudio.Gui;

namespace JAProj
{
    public partial class Form1 : Form
    {
        private WaveOutEvent waveOut;

        private AudioFileReader audioFileReaderInput;
        private AudioFileReader audioFileReaderOutput;

        private bool isInputSource= true ; // Domyślnie odtwarzamy wejściowy
      //  private bool isPlaying = false;
     

        private string inputFilePath;  // Ścieżka do pliku wejściowego
        private string outputFilePath; // Ścieżka do pliku wyjściowego


        public Form1()
        {
            InitializeComponent();

            this.Load += new System.EventHandler(this.Form1_Load);
           
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxDll.Items.Clear();
            // Inicjalizacja ComboBox
            comboBoxDll.Items.Add("ASM");
            comboBoxDll.Items.Add("C++");
            comboBoxDll.SelectedIndex = 0;

            // Ustawienia TrackBarHz
            trackBarHz.Minimum = 20;
            trackBarHz.Maximum = 20000;
            trackBarHz.TickFrequency = 1000;
            trackBarHz.Value = 4000;
            labelCutoffFreqValue.Text = $"{trackBarHz.Value} Hz";

            // Ustawienia TrackBarCoeff
            trackBarCoeff.Minimum = 1;   // Minimalna liczba współczynników
            trackBarCoeff.Maximum = 151; // Maksymalna liczba współczynników
            trackBarCoeff.TickFrequency = 2; // Skok co 2
            trackBarCoeff.Value = 81;    // Domyślna liczba współczynników
            labelCoeffValue.Text = $"{trackBarCoeff.Value}"; // Wyświetlenie domyślnej wartości
        }
        private void buttonInput_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Pliki WAV (*.wav)|*.wav";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    inputFilePath = openFileDialog.FileName;
                    buttonInput.BackColor = Color.LightGreen;
                    buttonInput.FlatAppearance.BorderSize = 2;
                    buttonInput.FlatAppearance.BorderColor = Color.Green;

                    // Aktywuj przycisk wyboru miejsca zapisu
                    buttonOutput.Enabled = true;
                }
            }
        }
        private void buttonOutput_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Pliki WAV (*.wav)|*.wav";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    outputFilePath = saveFileDialog.FileName;
                    buttonOutput.BackColor = Color.LightGreen;
                    buttonOutput.FlatAppearance.BorderSize = 2;
                    buttonOutput.FlatAppearance.BorderColor = Color.Green;
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            // Walidacja wejściowych danych
            if (string.IsNullOrEmpty(buttonInput.Text) || string.IsNullOrEmpty(buttonOutput.Text))
            {
                MessageBox.Show("Wybierz pliki wejściowe i wyjściowe!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Pobieranie wartości z kontrolek
                int cutoffFrequency = trackBarHz.Value;  // Częstotliwość odcięcia
                int filterLength = trackBarCoeff.Value; // Liczba współczynników
                string selectedDll = comboBoxDll.SelectedItem.ToString(); // Wybrana DLL

                string dllPath = selectedDll == "ASM"
                 ? ProcessAudioConfig.AsmDllPath
                 : ProcessAudioConfig.CDllPath;
                int threadCount = (int)numericUpDown1.Value;

                // Tworzenie konfiguracji dla przetwarzania
                ProcessAudioConfig config = new ProcessAudioConfig
                {
                    DllPath = dllPath,
                    InputFilePath = inputFilePath,
                    OutputFilePath = outputFilePath,
                    CutOffFrequency = cutoffFrequency,
                    FilterLength = filterLength,
                    ThreadCount = threadCount // Możesz dodać kontrolkę do wyboru liczby wątków
                };

                // Wywołanie procesu przetwarzania
                ProcessAudio processor = new ProcessAudio();
                long elapsedMilliseconds = processor.Process(config);

                // Informacja o sukcesie
                MessageBox.Show("Przetwarzanie zakończone!", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
                label5.Text = $"{elapsedMilliseconds} ms";
            }
            catch (Exception ex)
            {
                // Obsługa błędów
                MessageBox.Show($"Wystąpił błąd: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSource_Click(object sender, EventArgs e)
        {

            isInputSource = !isInputSource;
            buttonSource.Text = isInputSource ? "Wejściowe" : "Wyjściowe";

        }

        private void trackHz_Scroll(object sender, EventArgs e)
        {
            labelCutoffFreqValue.Text = $"{trackBarHz.Value} Hz";
        }

        private void trackCoeff_Scroll(object sender, EventArgs e)
        {
            // Ustaw nieparzystą wartość
            if (trackBarCoeff.Value % 2 == 0) // Jeśli parzysta
            {
                trackBarCoeff.Value += 1; // Przesuń na najbliższą nieparzystą wartość
            }
            labelCoeffValue.Text = $"{trackBarCoeff.Value}";
        }

        private void trackVolume_Scroll(object sender, EventArgs e)
        {

        }

        private void comboBoxDll_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void volumeSlider2_Load(object sender, EventArgs e)
        {

        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            try
            {
                if (waveOut != null && waveOut.PlaybackState == PlaybackState.Playing)
                {
                    StopPlayback();
                    buttonPlay.Text = "Play";
                }
                else
                {
                    StopPlayback();

                    if (isInputSource && !string.IsNullOrEmpty(inputFilePath))
                    {
                        audioFileReaderInput = new AudioFileReader(inputFilePath);
                        waveOut = new WaveOutEvent();
                        waveOut.Init(audioFileReaderInput);
                    }
                    else if (!isInputSource && !string.IsNullOrEmpty(outputFilePath))
                    {
                        audioFileReaderOutput = new AudioFileReader(outputFilePath);
                        waveOut = new WaveOutEvent();
                        waveOut.Init(audioFileReaderOutput);
                    }
                    else
                    {
                        MessageBox.Show("Brak wybranego źródła dźwięku!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    waveOut.Volume = volumeSlider1.Volume; 
                    waveOut.Play();
                    buttonPlay.Text = "Stop";

                    waveOut.PlaybackStopped += (s, ev) => buttonPlay.Text = "Play";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas odtwarzania: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void volumeSlider1_Load(object sender, EventArgs e)
        {
            if (waveOut != null && waveOut.PlaybackState == PlaybackState.Playing)
            {
                waveOut.Volume = volumeSlider1.Volume;
            }
        }


        private void StopPlayback()
        {
            waveOut?.Stop();
            waveOut?.Dispose();
            waveOut = null;

            audioFileReaderInput?.Dispose();
            audioFileReaderInput = null;

            audioFileReaderOutput?.Dispose();
            audioFileReaderOutput = null;
        }

        private void volumeSlider1_VolumeChanged(object sender, EventArgs e)
        {
            if (waveOut != null)
            {
                waveOut.Volume = volumeSlider1.Volume;
            }
        }
    }
}
