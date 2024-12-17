; Alias dla rejestrów
.code
rData        EQU rcx  ; float** data
rCoeff       EQU rdx  ; float* coefficients
rOutput      EQU r8   ; float** output
rDataLength  EQU r9   ; int dataLength
rCoeffLength EQU r10  ; int coeffLength
rChannels    EQU r11  ; int channels

ProcessArray PROC
    ; Parametry funkcji:
    ; rcx: float** data
    ; rdx: float* coefficients
    ; r8 : float** output
    ; r9 : int dataLength
    ; Parametry na stosie:
    ; [rsp+40h]: int coeffLength
    ; [rsp+48h]: int channels

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
    vxorps ymm2, ymm2, ymm2
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

    ; Oblicz liczbê pe³nych bloków po 8 elementów
    mov r15, rCoeffLength
    shr r15, 3            ; r15 = coeffLength / 8 (liczba pe³nych bloków)
    jz CoeffLoop_Scalar   ; Jeœli brak pe³nych bloków, przejdŸ do skalara

CoeffLoop_SIMD:
    ; Za³aduj 8 wspó³czynników do ymm1
    vmovups ymm1, YMMWORD PTR [rCoeff + r14*4]
    ; Za³aduj 8 próbek danych wejœciowych do ymm0
    vmovups ymm0, YMMWORD PTR [rsi + r14*4]

    ; Mno¿enie i akumulacja: ymm2 += ymm0 * ymm1
    vfmadd231ps ymm2, ymm0, ymm1

    ; Zwiêksz indeks wspó³czynnika
    add r14, 8
    ; Dekrementuj licznik bloków
    dec r15
    jnz CoeffLoop_SIMD

    ; SprawdŸ, czy pozosta³y jakieœ wspó³czynniki do przetworzenia
    mov r15, rCoeffLength
    and r15, 7            ; r15 = coeffLength % 8 (pozosta³e wspó³czynniki)
    cmp r15, 0
    je ReduceAccumulator  ; Jeœli brak pozosta³ych wspó³czynników, przejdŸ do redukcji

CoeffLoop_Scalar:
	mov r15, rCoeffLength
    and r15, 7     
    

CoeffLoop_Scalar_Inner:
     
    ; Za³aduj pojedynczy wspó³czynnik do xmm1
    vmovss xmm1, DWORD PTR [rCoeff + r14*4]
    ; Za³aduj pojedyncz¹ próbkê danych wejœciowych do xmm0
    vmovss xmm0, DWORD PTR [rsi + r14*4]
   ; Mno¿enie i akumulacja: xmm2[0] += xmm0 * xmm1
    mulss xmm0, xmm1                 ; xmm0 = xmm0 * xmm1
    addss xmm2, xmm0                 ; xmm2[0] += xmm0
  

    ; Zwiêksz indeks wspó³czynnika
    inc r14
    ; Dekrementuj licznik pozosta³ych wspó³czynników
    dec r15
    jnz CoeffLoop_Scalar_Inner
   
ReduceAccumulator:
    ; Wydobycie i dodanie górnej po³owy do dolnej
    vperm2f128 ymm3, ymm2, ymm2, 1      ; Przenieœ górne 128 bitów na dó³
    vaddps ymm2, ymm2, ymm3             ; Dodaj doln¹ i górn¹ czêœæ

    ; Horyzontalne dodanie dolnych 128 bitów
    vhaddps xmm2, xmm2, xmm2            ; Dodaj s¹siaduj¹ce wartoœci w dolnej po³owie

    ; Finalne sumowanie
    vhaddps xmm2, xmm2, xmm2            ; Finalne dodanie wszystkich elementów

    ; Obliczenie indeksu wyjœciowego z przesuniêciem
    mov r15, rCoeffLength             ; r15 = coeffLength
    add r15, r13                      ; r15 = coeffLength + dataIndex
    sub r15, 1                        ; r15 = coeffLength + dataIndex - 1
    shl r15, 2                        ; r15 *= 4 (rozmiar float w bajtach)
    lea rdi, [rbx + r15]              ; rdi = &output[channel][dataIndex + coeffLength - 1]

   
    vmovss DWORD PTR [rdi], xmm2 ; Zapisujemy wynik z xmm2[0] do pamiêci wyjœciowej
  
    inc r13                ; Zwiêkszenie indeksu danych
    jmp DataLoop           ; Powrót do pocz¹tku pêtli
NextChannel:
    ; Zwiêksz indeks kana³u
    inc r12
    jmp ChannelLoop       ; Wróæ do przetwarzania nastêpnego kana³u

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