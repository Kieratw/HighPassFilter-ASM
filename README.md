Project: High-Pass Filter Using SSE and AVX

Project Description

This project implements a high-pass FIR filter for audio signal processing. It includes two implementation versions:

C++ DLL (efficient implementation in C++)

ASM DLL (efficient implementation in assembly with SIMD instructions - SSE and AVX)

The application allows for selecting filters, the number of coefficients, and the number of threads to optimize performance.

Features

Loading WAV files as input

Audio filtering using the FIR filter

Support for DLL files written in C++ and ASM

Setting filter parameters (cutoff frequency, number of coefficients)

Selecting the number of processing threads

Viewing results in the console and playing filtered audio

Requirements

Visual Studio (e.g., 2019 or later)

Python (installed and added to PATH)

Libraries:

NAudio (for WAV file handling)

Newtonsoft.Json (for deserializing filter coefficients)

Installation

Clone the repository:

git clone https://github.com/username/project-name.git

Open the project in Visual Studio.

Prepare the compiled DLL versions:

Build the cDll project for the C++ DLL.

Build the JADll project for the ASM DLL.

Ensure the DLL file paths are correctly set in ProcessAudioConfig.cs.

Run the project from Visual Studio.

Using the Application

Run the application and select the input WAV file.

Choose the location for the output file.

Adjust filter parameters:

Cutoff Frequency

Number of Coefficients

DLL Selection (C++ or ASM)

Number of Threads

Click "Ready" to start filtering.

Optionally play the filtered audio.

