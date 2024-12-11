; Alias dla rejestr�w
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

    ; Zachowanie rejestr�w nieulotnych
    push rbx
    push rsi
    push rdi
    push r12
    push r13
    push r14
    push r15


    ; Pobranie dodatkowych parametr�w ze stosu
    mov rCoeffLength, QWORD PTR [rbp + 48] ; r10 = coeffLength
    mov rChannels,    QWORD PTR [rbp + 56] ; r11 = channels

          


    ; Inicjalizacja indeksu kana�u
    xor r12, r12      ; r12 = 0 (channel index)
    vxorps ymm2, ymm2, ymm2
ChannelLoop:
    cmp r12, rChannels    ; Por�wnaj channel index z channels
    jge EndProcess        ; Je�li channel index >= channels, zako�cz p�tl�

    ; Za�aduj wska�niki do danych dla kana�u
    mov rax, [rData + r12*8]   ; rax = data[r12] 
    mov rbx, [rOutput + r12*8] ; rbx = output[r12]

    ; Inicjalizacja indeksu danych
    xor r13, r13          ; r13 = 0 (data index)

DataLoop:
    mov r14, rDataLength
    sub r14, rCoeffLength
    add r14, 1   
    cmp r13, r14 
    jge NextChannel       ; Je�li przekroczono limit, przejd� do nast�pnego kana�u

    ; Oblicz wska�niki do danych wej�ciowych i wyj�ciowych
    lea rsi, [rax + r13*4] ; rsi = &data[r12][r13]
    lea rdi, [rbx + r13*4] ; rdi = &output[r12][r13]

    ; Wyzeruj akumulator SIMD
    vxorps ymm2, ymm2, ymm2

    ; Inicjalizacja indeksu wsp�czynnika
    xor r14, r14          ; r14 = 0 (coefficient index)

    ; Oblicz liczb� pe�nych blok�w po 8 element�w
    mov r15, rCoeffLength
    shr r15, 3            ; r15 = coeffLength / 8 (liczba pe�nych blok�w)
    jz CoeffLoop_Scalar   ; Je�li brak pe�nych blok�w, przejd� do skalara

CoeffLoop_SIMD:
    ; Za�aduj 8 wsp�czynnik�w do ymm1
    vmovups ymm1, YMMWORD PTR [rCoeff + r14*4]
    ; Za�aduj 8 pr�bek danych wej�ciowych do ymm0
    vmovups ymm0, YMMWORD PTR [rsi + r14*4]

    ; Mno�enie i akumulacja: ymm2 += ymm0 * ymm1
    vfmadd231ps ymm2, ymm0, ymm1

    ; Zwi�ksz indeks wsp�czynnika
    add r14, 8
    ; Dekrementuj licznik blok�w
    dec r15
    jnz CoeffLoop_SIMD

    ; Sprawd�, czy pozosta�y jakie� wsp�czynniki do przetworzenia
    mov r15, rCoeffLength
    and r15, 7            ; r15 = coeffLength % 8 (pozosta�e wsp�czynniki)
    cmp r15, 0
    je ReduceAccumulator  ; Je�li brak pozosta�ych wsp�czynnik�w, przejd� do redukcji

CoeffLoop_Scalar:
	mov r15, rCoeffLength
    and r15, 7    
    jmp CoeffLoop_Scalar_Inner

CoeffLoop_Scalar_Inner:
     
    ; Za�aduj pojedynczy wsp�czynnik do xmm1
    vmovss xmm1, DWORD PTR [rCoeff + r14*4]
    ; Za�aduj pojedyncz� pr�bk� danych wej�ciowych do xmm0
    vmovss xmm0, DWORD PTR [rsi + r14*4]
     vmovaps ymm5, ymm2
    ; Mno�enie i akumulacja: xmm2[0] += xmm0 * xmm1
     ; Mno�enie i akumulacja: xmm2[0] += xmm0 * xmm1
    mulss xmm0, xmm1                 ; xmm0 = xmm0 * xmm1
    addss xmm2, xmm0                 ; xmm2[0] += xmm0
  
    ; vmovaps ymm5, ymm2
    ; Zwi�ksz indeks wsp�czynnika
    inc r14
    ; Dekrementuj licznik pozosta�ych wsp�czynnik�w
    dec r15
    jnz CoeffLoop_Scalar_Inner
   
ReduceAccumulator:
; Wydobycie g�rnej po�owy rejestru ymm2
vmovaps ymm5, ymm2
   vperm2f128 ymm3, ymm2, ymm2, 01h   ; ymm3 = [a4, a5, a6, a7, ..., ...]      ; xmm3 = [a4, a5, a6, a7]
    vaddps xmm2, xmm2, xmm3          ; xmm2 = [a0+a4, a1+a5, a2+a6, a3+a7]

    ; R�czna redukcja w dolnej po�owie xmm2
    movaps xmm3, xmm2                ; Skopiowanie xmm2 do xmm3
    shufps xmm3, xmm3, 4Eh           ; xmm3 = [a2+a6, a3+a7, a0+a4, a1+a5]
    addps xmm2, xmm3                 ; xmm2 = [a0+a4+a2+a6, a1+a5+a3+a7, -, -]

    ; Finalne horyzontalne dodanie za pomoc� haddps
    haddps xmm2, xmm2                ; xmm2 = [a0+a4+a2+a6+a1+a5+a3+a7, -, -, -]

    ; Obliczenie indeksu wyj�ciowego z przesuni�ciem
    mov r15, rCoeffLength             ; r15 = coeffLength
    add r15, r13                      ; r15 = coeffLength + dataIndex
    sub r15, 1                        ; r15 = coeffLength + dataIndex - 1
    shl r15, 2                        ; r15 *= 4 (rozmiar float w bajtach)
    lea rdi, [rbx + r15]              ; rdi = &output[channel][dataIndex + coeffLength - 1]

    ; Zapis wyniku do pami�ci
   
    vmovss DWORD PTR [rdi], xmm2 ; Zapisujemy wynik z xmm2[0] do pami�ci wyj�ciowej
    vmovss xmm5, DWORD PTR [rdi]
    inc r13                ; Zwi�kszenie indeksu danych
    jmp DataLoop           ; Powr�t do pocz�tku p�tli
NextChannel:
    ; Zwi�ksz indeks kana�u
    inc r12
    jmp ChannelLoop       ; Wr�� do przetwarzania nast�pnego kana�u

EndProcess:
    ; Przywr�cenie rejestr�w
   
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
