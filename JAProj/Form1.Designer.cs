namespace JAProj
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonInput = new System.Windows.Forms.Button();
            this.comboBoxDll = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.trackBarHz = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.trackBarCoeff = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.buttonOutput = new System.Windows.Forms.Button();
            this.labelCutoffFreqValue = new System.Windows.Forms.Label();
            this.labelCoeffValue = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.volumeSlider1 = new NAudio.Gui.VolumeSlider();
            this.buttonSource = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarHz)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarCoeff)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonInput
            // 
            this.buttonInput.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonInput.Location = new System.Drawing.Point(55, 35);
            this.buttonInput.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonInput.Name = "buttonInput";
            this.buttonInput.Size = new System.Drawing.Size(149, 32);
            this.buttonInput.TabIndex = 0;
            this.buttonInput.Text = "Wybierz plik .wav";
            this.buttonInput.UseVisualStyleBackColor = true;
            this.buttonInput.Click += new System.EventHandler(this.buttonInput_Click);
            // 
            // comboBoxDll
            // 
            this.comboBoxDll.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDll.Location = new System.Drawing.Point(55, 268);
            this.comboBoxDll.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxDll.Name = "comboBoxDll";
            this.comboBoxDll.Size = new System.Drawing.Size(148, 24);
            this.comboBoxDll.TabIndex = 1;
            this.comboBoxDll.SelectedIndexChanged += new System.EventHandler(this.comboBoxDll_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(52, 109);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Częstotliwość odcięcia";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(365, 350);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Gotowe";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // trackBarHz
            // 
            this.trackBarHz.Location = new System.Drawing.Point(65, 141);
            this.trackBarHz.Maximum = 20000;
            this.trackBarHz.Name = "trackBarHz";
            this.trackBarHz.Size = new System.Drawing.Size(104, 56);
            this.trackBarHz.TabIndex = 4;
            this.trackBarHz.Scroll += new System.EventHandler(this.trackHz_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(241, 109);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(148, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "Liczba współczynników";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // trackBarCoeff
            // 
            this.trackBarCoeff.Location = new System.Drawing.Point(265, 141);
            this.trackBarCoeff.Maximum = 151;
            this.trackBarCoeff.Minimum = 1;
            this.trackBarCoeff.Name = "trackBarCoeff";
            this.trackBarCoeff.Size = new System.Drawing.Size(104, 56);
            this.trackBarCoeff.TabIndex = 6;
            this.trackBarCoeff.Value = 1;
            this.trackBarCoeff.Scroll += new System.EventHandler(this.trackCoeff_Scroll);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(86, 238);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "Wybierz DLL";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // buttonPlay
            // 
            this.buttonPlay.Location = new System.Drawing.Point(614, 238);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(75, 23);
            this.buttonPlay.TabIndex = 9;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // buttonOutput
            // 
            this.buttonOutput.Enabled = false;
            this.buttonOutput.Location = new System.Drawing.Point(229, 35);
            this.buttonOutput.Name = "buttonOutput";
            this.buttonOutput.Size = new System.Drawing.Size(176, 32);
            this.buttonOutput.TabIndex = 10;
            this.buttonOutput.Text = "Wybierz miejsce zapisu";
            this.buttonOutput.UseVisualStyleBackColor = true;
            this.buttonOutput.Click += new System.EventHandler(this.buttonOutput_Click);
            // 
            // labelCutoffFreqValue
            // 
            this.labelCutoffFreqValue.AutoSize = true;
            this.labelCutoffFreqValue.Location = new System.Drawing.Point(101, 190);
            this.labelCutoffFreqValue.Name = "labelCutoffFreqValue";
            this.labelCutoffFreqValue.Size = new System.Drawing.Size(20, 16);
            this.labelCutoffFreqValue.TabIndex = 11;
            this.labelCutoffFreqValue.Text = "hz";
            // 
            // labelCoeffValue
            // 
            this.labelCoeffValue.AutoSize = true;
            this.labelCoeffValue.Location = new System.Drawing.Point(315, 190);
            this.labelCoeffValue.Name = "labelCoeffValue";
            this.labelCoeffValue.Size = new System.Drawing.Size(0, 16);
            this.labelCoeffValue.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(52, 322);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 16);
            this.label4.TabIndex = 13;
            this.label4.Text = "Czas wykonania:";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(160, 322);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(25, 16);
            this.label5.TabIndex = 14;
            this.label5.Text = "ms";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(248, 268);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(137, 22);
            this.numericUpDown1.TabIndex = 15;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(245, 238);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(144, 16);
            this.label6.TabIndex = 16;
            this.label6.Text = "Wybierz liczbe wątków ";
            // 
            // volumeSlider1
            // 
            this.volumeSlider1.Location = new System.Drawing.Point(602, 141);
            this.volumeSlider1.Name = "volumeSlider1";
            this.volumeSlider1.Size = new System.Drawing.Size(96, 16);
            this.volumeSlider1.TabIndex = 17;
            this.volumeSlider1.Load += new System.EventHandler(this.volumeSlider1_Load);
            // 
            // buttonSource
            // 
            this.buttonSource.Location = new System.Drawing.Point(602, 190);
            this.buttonSource.Name = "buttonSource";
            this.buttonSource.Size = new System.Drawing.Size(96, 23);
            this.buttonSource.TabIndex = 18;
            this.buttonSource.Text = "Źródło";
            this.buttonSource.UseVisualStyleBackColor = true;
            this.buttonSource.Click += new System.EventHandler(this.buttonSource_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.buttonSource);
            this.Controls.Add(this.volumeSlider1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelCoeffValue);
            this.Controls.Add(this.labelCutoffFreqValue);
            this.Controls.Add(this.buttonOutput);
            this.Controls.Add(this.buttonPlay);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.trackBarCoeff);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.trackBarHz);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxDll);
            this.Controls.Add(this.buttonInput);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Filtr Górnoprzepustowy";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarHz)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarCoeff)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonInput;
        private System.Windows.Forms.ComboBox comboBoxDll;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TrackBar trackBarHz;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar trackBarCoeff;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.Button buttonOutput;
        private System.Windows.Forms.Label labelCutoffFreqValue;
        private System.Windows.Forms.Label labelCoeffValue;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label6;
        private NAudio.Gui.VolumeSlider volumeSlider1;
        private System.Windows.Forms.Button buttonSource;
    }
}

