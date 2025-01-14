extern "C" __declspec(dllexport) void ProcessArray(
    float** data,
    float* coefficients,
    float** output,
    int dataLength,
    int coeffLength,
    int channels
)
{
    // Dla ka¿dego kana³u
    for (int ch = 0; ch < channels; ch++)
    {
        float* inputData = data[ch];
        float* outputData = output[ch];

        // Dla ka¿dej próbki w danych
        for (int n = 0; n < dataLength - coeffLength + 1; n++) 
        {
            float y = 0.0f;

            // Wykonanie splotu (filtr FIR)
            for (int k = 0; k < coeffLength; k++)
            {
                y += coefficients[k] * inputData[n + k];
            }

            // Zapisanie wyniku do tablicy wyjœciowej
            outputData[n + coeffLength - 1] = y;
        }
    }
}

