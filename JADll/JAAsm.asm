; Alias dla rejestrów
.code
rData        EQU rcx  ; float** data
rCoeff       EQU rdx  ; float* coefficients
rOutput      EQU r8   ; float** output
rDataLength  EQU r9   ; int dataLength
rCoeffLength EQU r10  ; int coeffLength
rChannels    EQU r11  ; int channels

ProcessArray PROC
    ; Ustawienie ramki stosu
    push rbp
    mov rbp, rsp

    ; Zachowanie rejestrów nieulotnych
    push rbx
    push rsi
    push rdi
    push r12
    push r13
    push r14
    push r15

    ; Pobranie dodatkowych parametrów ze stosu
    mov rCoeffLength, QWORD PTR [rbp + 48] ; r10 = coeffLength
    mov rChannels,    QWORD PTR [rbp + 56] ; r11 = channels

    ; Inicjalizacja indeksu kana³u
    xor r12, r12      ; r12 = 0 (channel index)
ChannelLoop:
    cmp r12, rChannels    ; Porównaj channel index z channels
    jge EndProcess        ; Jeœli channel index >= channels, zakoñcz pêtlê

    ; Za³aduj wskaŸniki do danych dla kana³u
    mov rax, [rData + r12*8]   ; rax = data[r12]
    mov rbx, [rOutput + r12*8] ; rbx = output[r12]

    ; Inicjalizacja indeksu danych
    xor r13, r13          ; r13 = 0 (data index)

DataLoop:
    mov r14, rDataLength
    sub r14, rCoeffLength
    add r14, 1   
    cmp r13, r14 
    jge NextChannel       ; Jeœli przekroczono limit, przejdŸ do nastêpnego kana³u

    ; Oblicz wskaŸniki do danych wejœciowych i wyjœciowych
    lea rsi, [rax + r13*4] ; rsi = &data[r12][r13]
    lea rdi, [rbx + r13*4] ; rdi = &output[r12][r13]

    ; Wyzeruj akumulator SIMD
    vxorps ymm2, ymm2, ymm2

    ; Inicjalizacja indeksu wspó³czynnika
    xor r14, r14          ; r14 = 0 (coefficient index)

CoeffLoop_SIMD:
    ; Za³aduj 8 wspó³czynników do ymm1
    vmovups ymm1, YMMWORD PTR [rCoeff + r14*4]
    ; Za³aduj 8 próbek danych wejœciowych do ymm0
    vmovups ymm0, YMMWORD PTR [rsi + r14*4]

    ; Mno¿enie i akumulacja: ymm2 += ymm0 * ymm1
    vfmadd231ps ymm2, ymm0, ymm1

    ; Zwiêksz indeks wspó³czynnika
    add r14, 8
    cmp r14, rCoeffLength ; SprawdŸ, czy przetworzono wszystkie wspó³czynniki
    jl CoeffLoop_SIMD

ReduceAccumulator:
    ; Redukcja pozioma rejestru ymm2
    vperm2f128 ymm3, ymm2, ymm2, 1   ; Przenieœ górne 128 bitów na dó³
    vaddps ymm2, ymm2, ymm3          ; Dodaj doln¹ i górn¹ czêœæ

    vhaddps xmm2, xmm2, xmm2         ; Horyzontalna redukcja
    vhaddps xmm2, xmm2, xmm2

    ; Zapis wyniku do pamiêci wyjœciowej
    mov r15, rCoeffLength
    add r15, r13
    sub r15, 1
    shl r15, 2
    lea rdi, [rbx + r15]      ; rdi = &output[channel][dataIndex + coeffLength - 1]
    vmovss DWORD PTR [rdi], xmm2

    inc r13
    jmp DataLoop

NextChannel:
    inc r12
    jmp ChannelLoop

EndProcess:
    ; Przywrócenie rejestrów
    pop r15
    pop r14
    pop r13
    pop r12
    pop rdi
    pop rsi
    pop rbx
    pop rbp

    ret
ProcessArray ENDP
END
