namespace TL_CSV_Configurator
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.grpKontakte = new System.Windows.Forms.GroupBox();
            this.labelVorlauf = new System.Windows.Forms.Label();
            this.numVorlauf = new System.Windows.Forms.NumericUpDown();
            this.labelC1 = new System.Windows.Forms.Label();
            this.dtpC1 = new System.Windows.Forms.DateTimePicker();
            this.labelC2 = new System.Windows.Forms.Label();
            this.dtpC2 = new System.Windows.Forms.DateTimePicker();
            this.labelC3 = new System.Windows.Forms.Label();
            this.dtpC3 = new System.Windows.Forms.DateTimePicker();
            this.labelC4 = new System.Windows.Forms.Label();
            this.dtpC4 = new System.Windows.Forms.DateTimePicker();
            this.labelNachlauf = new System.Windows.Forms.Label();
            this.numNachlauf = new System.Windows.Forms.NumericUpDown();
            this.grpPhasen = new System.Windows.Forms.GroupBox();
            this.dgvPhases = new System.Windows.Forms.DataGridView();
            this.grpBracketing = new System.Windows.Forms.GroupBox();
            this.chkEnableBracketing = new System.Windows.Forms.CheckBox();
            this.lblBracketMin = new System.Windows.Forms.Label();
            this.cmbBracketMin = new System.Windows.Forms.ComboBox();
            this.lblBracketMax = new System.Windows.Forms.Label();
            this.cmbBracketMax = new System.Windows.Forms.ComboBox();
            this.lblBracketEV = new System.Windows.Forms.Label();
            this.numBracketEV = new System.Windows.Forms.NumericUpDown();
            this.lblBracketWait = new System.Windows.Forms.Label();
            this.numBracketWait = new System.Windows.Forms.NumericUpDown();
            this.grpTimeline = new System.Windows.Forms.GroupBox();
            this.picTimeline = new System.Windows.Forms.PictureBox();
            this.btnGenerateCSV = new System.Windows.Forms.Button();
            this.grpKontakte.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVorlauf)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNachlauf)).BeginInit();
            this.grpPhasen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPhases)).BeginInit();
            this.grpBracketing.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBracketEV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBracketWait)).BeginInit();
            this.grpTimeline.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTimeline)).BeginInit();
            this.SuspendLayout();
            // 
            // grpKontakte
            // 
            this.grpKontakte.Controls.Add(this.labelVorlauf);
            this.grpKontakte.Controls.Add(this.numVorlauf);
            this.grpKontakte.Controls.Add(this.labelC1);
            this.grpKontakte.Controls.Add(this.dtpC1);
            this.grpKontakte.Controls.Add(this.labelC2);
            this.grpKontakte.Controls.Add(this.dtpC2);
            this.grpKontakte.Controls.Add(this.labelC3);
            this.grpKontakte.Controls.Add(this.dtpC3);
            this.grpKontakte.Controls.Add(this.labelC4);
            this.grpKontakte.Controls.Add(this.dtpC4);
            this.grpKontakte.Controls.Add(this.labelNachlauf);
            this.grpKontakte.Controls.Add(this.numNachlauf);
            this.grpKontakte.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpKontakte.Location = new System.Drawing.Point(20, 20);
            this.grpKontakte.Name = "grpKontakte";
            this.grpKontakte.Size = new System.Drawing.Size(1000, 100);
            this.grpKontakte.TabIndex = 0;
            this.grpKontakte.TabStop = false;
            this.grpKontakte.Text = "Astronomische Zeiten & Kontaktpunkte";
            // 
            // labelVorlauf
            // 
            this.labelVorlauf.AutoSize = true;
            this.labelVorlauf.Location = new System.Drawing.Point(20, 35);
            this.labelVorlauf.Name = "labelVorlauf";
            this.labelVorlauf.Size = new System.Drawing.Size(99, 20);
            this.labelVorlauf.TabIndex = 0;
            this.labelVorlauf.Text = "Vorlauf (Min):";
            // 
            // numVorlauf
            // 
            this.numVorlauf.Location = new System.Drawing.Point(20, 58);
            this.numVorlauf.Name = "numVorlauf";
            this.numVorlauf.Size = new System.Drawing.Size(60, 27);
            this.numVorlauf.TabIndex = 1;
            this.numVorlauf.ValueChanged += new System.EventHandler(this.TimePickers_ValueChanged);
            // 
            // labelC1
            // 
            this.labelC1.AutoSize = true;
            this.labelC1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelC1.Location = new System.Drawing.Point(140, 35);
            this.labelC1.Name = "labelC1";
            this.labelC1.Size = new System.Drawing.Size(84, 20);
            this.labelC1.TabIndex = 2;
            this.labelC1.Text = "1. Kontakt";
            // 
            // dtpC1
            // 
            this.dtpC1.CustomFormat = "dd.MM.yyyy HH:mm:ss";
            this.dtpC1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpC1.Location = new System.Drawing.Point(140, 58);
            this.dtpC1.Name = "dtpC1";
            this.dtpC1.Size = new System.Drawing.Size(180, 27);
            this.dtpC1.TabIndex = 3;
            this.dtpC1.ValueChanged += new System.EventHandler(this.TimePickers_ValueChanged);
            // 
            // labelC2
            // 
            this.labelC2.AutoSize = true;
            this.labelC2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelC2.Location = new System.Drawing.Point(330, 35);
            this.labelC2.Name = "labelC2";
            this.labelC2.Size = new System.Drawing.Size(84, 20);
            this.labelC2.TabIndex = 4;
            this.labelC2.Text = "2. Kontakt";
            // 
            // dtpC2
            // 
            this.dtpC2.CustomFormat = "dd.MM.yyyy HH:mm:ss";
            this.dtpC2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpC2.Location = new System.Drawing.Point(330, 58);
            this.dtpC2.Name = "dtpC2";
            this.dtpC2.Size = new System.Drawing.Size(180, 27);
            this.dtpC2.TabIndex = 5;
            this.dtpC2.ValueChanged += new System.EventHandler(this.TimePickers_ValueChanged);
            // 
            // labelC3
            // 
            this.labelC3.AutoSize = true;
            this.labelC3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelC3.Location = new System.Drawing.Point(520, 35);
            this.labelC3.Name = "labelC3";
            this.labelC3.Size = new System.Drawing.Size(84, 20);
            this.labelC3.TabIndex = 6;
            this.labelC3.Text = "3. Kontakt";
            // 
            // dtpC3
            // 
            this.dtpC3.CustomFormat = "dd.MM.yyyy HH:mm:ss";
            this.dtpC3.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpC3.Location = new System.Drawing.Point(520, 58);
            this.dtpC3.Name = "dtpC3";
            this.dtpC3.Size = new System.Drawing.Size(180, 27);
            this.dtpC3.TabIndex = 7;
            this.dtpC3.ValueChanged += new System.EventHandler(this.TimePickers_ValueChanged);
            // 
            // labelC4
            // 
            this.labelC4.AutoSize = true;
            this.labelC4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.labelC4.Location = new System.Drawing.Point(710, 35);
            this.labelC4.Name = "labelC4";
            this.labelC4.Size = new System.Drawing.Size(84, 20);
            this.labelC4.TabIndex = 8;
            this.labelC4.Text = "4. Kontakt";
            // 
            // dtpC4
            // 
            this.dtpC4.CustomFormat = "dd.MM.yyyy HH:mm:ss";
            this.dtpC4.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpC4.Location = new System.Drawing.Point(710, 58);
            this.dtpC4.Name = "dtpC4";
            this.dtpC4.Size = new System.Drawing.Size(180, 27);
            this.dtpC4.TabIndex = 9;
            this.dtpC4.ValueChanged += new System.EventHandler(this.TimePickers_ValueChanged);
            // 
            // labelNachlauf
            // 
            this.labelNachlauf.AutoSize = true;
            this.labelNachlauf.Location = new System.Drawing.Point(900, 35);
            this.labelNachlauf.Name = "labelNachlauf";
            this.labelNachlauf.Size = new System.Drawing.Size(110, 20);
            this.labelNachlauf.TabIndex = 10;
            this.labelNachlauf.Text = "Nachlauf (Min):";
            // 
            // numNachlauf
            // 
            this.numNachlauf.Location = new System.Drawing.Point(900, 58);
            this.numNachlauf.Name = "numNachlauf";
            this.numNachlauf.Size = new System.Drawing.Size(60, 27);
            this.numNachlauf.TabIndex = 11;
            this.numNachlauf.ValueChanged += new System.EventHandler(this.TimePickers_ValueChanged);
            // 
            // grpPhasen
            // 
            this.grpPhasen.Controls.Add(this.dgvPhases);
            this.grpPhasen.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.grpPhasen.Location = new System.Drawing.Point(20, 130);
            this.grpPhasen.Name = "grpPhasen";
            this.grpPhasen.Size = new System.Drawing.Size(1000, 200);
            this.grpPhasen.TabIndex = 1;
            this.grpPhasen.TabStop = false;
            this.grpPhasen.Text = "Belichtungseinstellungen pro Finsternis-Phase";
            // 
            // dgvPhases
            // 
            this.dgvPhases.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPhases.Location = new System.Drawing.Point(20, 30);
            this.dgvPhases.Name = "dgvPhases";
            this.dgvPhases.RowHeadersWidth = 51;
            this.dgvPhases.RowTemplate.Height = 28;
            this.dgvPhases.Size = new System.Drawing.Size(960, 150);
            this.dgvPhases.TabIndex = 0;
            // 
            // grpBracketing
            // 
            this.grpBracketing.Controls.Add(this.chkEnableBracketing);
            this.grpBracketing.Controls.Add(this.lblBracketMin);
            this.grpBracketing.Controls.Add(this.cmbBracketMin);
            this.grpBracketing.Controls.Add(this.lblBracketMax);
            this.grpBracketing.Controls.Add(this.cmbBracketMax);
            this.grpBracketing.Controls.Add(this.lblBracketEV);
            this.grpBracketing.Controls.Add(this.numBracketEV);
            this.grpBracketing.Controls.Add(this.lblBracketWait);
            this.grpBracketing.Controls.Add(this.numBracketWait);
            this.grpBracketing.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBracketing.Location = new System.Drawing.Point(20, 340);
            this.grpBracketing.Name = "grpBracketing";
            this.grpBracketing.Size = new System.Drawing.Size(1000, 80);
            this.grpBracketing.TabIndex = 2;
            this.grpBracketing.TabStop = false;
            this.grpBracketing.Text = "HDR Bracketing (Ersetzt die Zeit für Totalität C2 - C3)";
            // 
            // chkEnableBracketing
            // 
            this.chkEnableBracketing.AutoSize = true;
            this.chkEnableBracketing.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.chkEnableBracketing.Location = new System.Drawing.Point(20, 35);
            this.chkEnableBracketing.Name = "chkEnableBracketing";
            this.chkEnableBracketing.Size = new System.Drawing.Size(110, 24);
            this.chkEnableBracketing.TabIndex = 0;
            this.chkEnableBracketing.Text = "Aktivieren";
            this.chkEnableBracketing.UseVisualStyleBackColor = true;
            this.chkEnableBracketing.CheckedChanged += new System.EventHandler(this.chkEnableBracketing_CheckedChanged);
            // 
            // lblBracketMin
            // 
            this.lblBracketMin.AutoSize = true;
            this.lblBracketMin.Location = new System.Drawing.Point(140, 35);
            this.lblBracketMin.Name = "lblBracketMin";
            this.lblBracketMin.Size = new System.Drawing.Size(112, 20);
            this.lblBracketMin.TabIndex = 1;
            this.lblBracketMin.Text = "Schnellste Zeit:";
            // 
            // cmbBracketMin
            // 
            this.cmbBracketMin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBracketMin.Enabled = false;
            this.cmbBracketMin.FormattingEnabled = true;
            this.cmbBracketMin.Location = new System.Drawing.Point(260, 32);
            this.cmbBracketMin.Name = "cmbBracketMin";
            this.cmbBracketMin.Size = new System.Drawing.Size(100, 28);
            this.cmbBracketMin.TabIndex = 2;
            this.cmbBracketMin.SelectedIndexChanged += new System.EventHandler(this.BracketingControl_Changed);
            // 
            // lblBracketMax
            // 
            this.lblBracketMax.AutoSize = true;
            this.lblBracketMax.Location = new System.Drawing.Point(380, 35);
            this.lblBracketMax.Name = "lblBracketMax";
            this.lblBracketMax.Size = new System.Drawing.Size(119, 20);
            this.lblBracketMax.TabIndex = 3;
            this.lblBracketMax.Text = "Langsamste Zeit:";
            // 
            // cmbBracketMax
            // 
            this.cmbBracketMax.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBracketMax.Enabled = false;
            this.cmbBracketMax.FormattingEnabled = true;
            this.cmbBracketMax.Location = new System.Drawing.Point(500, 32);
            this.cmbBracketMax.Name = "cmbBracketMax";
            this.cmbBracketMax.Size = new System.Drawing.Size(100, 28);
            this.cmbBracketMax.TabIndex = 4;
            this.cmbBracketMax.SelectedIndexChanged += new System.EventHandler(this.BracketingControl_Changed);
            // 
            // lblBracketEV
            // 
            this.lblBracketEV.AutoSize = true;
            this.lblBracketEV.Location = new System.Drawing.Point(620, 35);
            this.lblBracketEV.Name = "lblBracketEV";
            this.lblBracketEV.Size = new System.Drawing.Size(84, 20);
            this.lblBracketEV.TabIndex = 5;
            this.lblBracketEV.Text = "EV-Schritte:";
            // 
            // numBracketEV
            // 
            this.numBracketEV.Enabled = false;
            this.numBracketEV.Location = new System.Drawing.Point(710, 33);
            this.numBracketEV.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            this.numBracketEV.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numBracketEV.Name = "numBracketEV";
            this.numBracketEV.Size = new System.Drawing.Size(50, 27);
            this.numBracketEV.TabIndex = 6;
            this.numBracketEV.Value = new decimal(new int[] { 2, 0, 0, 0 });
            this.numBracketEV.ValueChanged += new System.EventHandler(this.BracketingControl_Changed);
            // 
            // lblBracketWait
            // 
            this.lblBracketWait.AutoSize = true;
            this.lblBracketWait.Location = new System.Drawing.Point(780, 35);
            this.lblBracketWait.Name = "lblBracketWait";
            this.lblBracketWait.Size = new System.Drawing.Size(106, 20);
            this.lblBracketWait.TabIndex = 7;
            this.lblBracketWait.Text = "Pause Bild (ms):";
            // 
            // numBracketWait
            // 
            this.numBracketWait.Enabled = false;
            this.numBracketWait.Increment = new decimal(new int[] { 100, 0, 0, 0 });
            this.numBracketWait.Location = new System.Drawing.Point(890, 33);
            this.numBracketWait.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
            this.numBracketWait.Name = "numBracketWait";
            this.numBracketWait.Size = new System.Drawing.Size(60, 27);
            this.numBracketWait.TabIndex = 8;
            this.numBracketWait.Value = new decimal(new int[] { 500, 0, 0, 0 });
            this.numBracketWait.ValueChanged += new System.EventHandler(this.BracketingControl_Changed);
            // 
            // grpTimeline
            // 
            this.grpTimeline.Controls.Add(this.picTimeline);
            this.grpTimeline.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.grpTimeline.Location = new System.Drawing.Point(20, 430);
            this.grpTimeline.Name = "grpTimeline";
            this.grpTimeline.Size = new System.Drawing.Size(1000, 160);
            this.grpTimeline.TabIndex = 3;
            this.grpTimeline.TabStop = false;
            this.grpTimeline.Text = "Live Vorschau (Zeitstrahl)";
            // 
            // picTimeline
            // 
            this.picTimeline.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picTimeline.Location = new System.Drawing.Point(3, 23);
            this.picTimeline.Name = "picTimeline";
            this.picTimeline.Size = new System.Drawing.Size(994, 134);
            this.picTimeline.TabIndex = 0;
            this.picTimeline.TabStop = false;
            this.picTimeline.Paint += new System.Windows.Forms.PaintEventHandler(this.picTimeline_Paint);
            // 
            // btnGenerateCSV
            // 
            this.btnGenerateCSV.BackColor = System.Drawing.Color.LightGreen;
            this.btnGenerateCSV.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerateCSV.Location = new System.Drawing.Point(720, 600);
            this.btnGenerateCSV.Name = "btnGenerateCSV";
            this.btnGenerateCSV.Size = new System.Drawing.Size(300, 50);
            this.btnGenerateCSV.TabIndex = 4;
            this.btnGenerateCSV.Text = "💾 CSV Datei Generieren";
            this.btnGenerateCSV.UseVisualStyleBackColor = false;
            this.btnGenerateCSV.Click += new System.EventHandler(this.btnGenerateCSV_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1050, 670);
            this.Controls.Add(this.btnGenerateCSV);
            this.Controls.Add(this.grpTimeline);
            this.Controls.Add(this.grpBracketing);
            this.Controls.Add(this.grpPhasen);
            this.Controls.Add(this.grpKontakte);
            this.Name = "Form1";
            this.Text = "Eclipse Timelapse Configurator (EOS R5)";
            this.grpKontakte.ResumeLayout(false);
            this.grpKontakte.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVorlauf)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNachlauf)).EndInit();
            this.grpPhasen.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPhases)).EndInit();
            this.grpBracketing.ResumeLayout(false);
            this.grpBracketing.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBracketEV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numBracketWait)).EndInit();
            this.grpTimeline.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picTimeline)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpKontakte;
        private System.Windows.Forms.Label labelC1;
        private System.Windows.Forms.DateTimePicker dtpC1;
        private System.Windows.Forms.Label labelC2;
        private System.Windows.Forms.DateTimePicker dtpC2;
        private System.Windows.Forms.Label labelC3;
        private System.Windows.Forms.DateTimePicker dtpC3;
        private System.Windows.Forms.Label labelC4;
        private System.Windows.Forms.DateTimePicker dtpC4;
        private System.Windows.Forms.Label labelVorlauf;
        private System.Windows.Forms.NumericUpDown numVorlauf;
        private System.Windows.Forms.Label labelNachlauf;
        private System.Windows.Forms.NumericUpDown numNachlauf;
        private System.Windows.Forms.GroupBox grpPhasen;
        private System.Windows.Forms.DataGridView dgvPhases;
        private System.Windows.Forms.GroupBox grpBracketing;
        private System.Windows.Forms.CheckBox chkEnableBracketing;
        private System.Windows.Forms.Label lblBracketMin;
        private System.Windows.Forms.ComboBox cmbBracketMin;
        private System.Windows.Forms.Label lblBracketMax;
        private System.Windows.Forms.ComboBox cmbBracketMax;
        private System.Windows.Forms.Label lblBracketEV;
        private System.Windows.Forms.NumericUpDown numBracketEV;
        private System.Windows.Forms.Label lblBracketWait;
        private System.Windows.Forms.NumericUpDown numBracketWait;
        private System.Windows.Forms.GroupBox grpTimeline;
        private System.Windows.Forms.PictureBox picTimeline;
        private System.Windows.Forms.Button btnGenerateCSV;
    }
}