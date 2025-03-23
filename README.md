# High-Pass Filter Using SSE and AVX

## 🚀 Project Description
This project implements a **high-pass FIR filter** for audio signal processing. It offers two implementation options:
- ✅ **C++ DLL** - Efficient implementation in C++
- ✅ **ASM DLL** - Efficient assembly implementation with SIMD instructions (SSE and AVX)

The application provides options to select filters, coefficient counts, and thread numbers to enhance performance.

---

## 🧰 Features
- 🎵 **Load WAV files** as input
- 🎚️ **Adjustable filter settings** (cutoff frequency, number of coefficients)
- ⚙️ **DLL selection** for flexibility in choosing C++ or ASM implementations
- 🚀 **Multi-threading support** to boost performance
- 📈 **Console visualization** and **audio playback** of filtered results

---

## 📋 Requirements
- 🔹 Visual Studio (e.g., 2019 or newer)
- 🔹 Python (installed and added to PATH)
- 📦 Libraries:
  - `NAudio` (for WAV file handling)
  - `Newtonsoft.Json` (for JSON deserialization)

---

## 📥 Installation
1. **Clone the repository**
   ```bash
   git clone https://github.com/username/project-name.git
   ```
2. **Open the project** in Visual Studio.
3. **Build DLL versions:**
   - Compile **cDll** for the C++ DLL.
   - Compile **JADll** for the ASM DLL.
4. Ensure the DLL file paths are set correctly in `ProcessAudioConfig.cs`.
5. Run the project from Visual Studio.

---

## 🟢 Using the Application
1. **Launch the application** and select an input WAV file.
2. Choose the **output file's location**.
3. Adjust filter parameters:
   - 🎧 **Cutoff Frequency**
   - 🔢 **Number of Coefficients**
   - 🛠️ **DLL Type** (C++ or ASM)
   - 🔄 **Thread Count**
4. Click **"Ready"** to start filtering.
5. Optionally, **play** the filtered audio directly from the app.

---

## 👨‍💻 Author
Wojciech Kierat

