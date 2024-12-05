import sys
import numpy as np
from scipy.signal import firwin
import json

def design_highpass_filter(taps, cutoff_frequency, sampling_rate):
    nyquist_rate = sampling_rate / 2.0
    normalized_cutoff = cutoff_frequency / nyquist_rate
    coefficients = firwin(taps, normalized_cutoff, pass_zero=False)
    return coefficients.tolist()

if __name__ == "__main__":
    taps = int(sys.argv[1])
    cutoff_frequency = float(sys.argv[2])
    sampling_rate = float(sys.argv[3])

    coefficients = design_highpass_filter(taps, cutoff_frequency, sampling_rate)

    # Wypisz współczynniki w formacie JSON
    print(json.dumps(coefficients))
