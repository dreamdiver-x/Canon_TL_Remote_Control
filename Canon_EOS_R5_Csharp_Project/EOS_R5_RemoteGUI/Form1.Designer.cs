namespace EOS_R5_RemoteGUI
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

        #region Vom Windows Form-Designer generierter Code

        private void InitializeComponent()
        {
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();

            this.labelModus = new System.Windows.Forms.Label();
            this.cmbAEMode = new System.Windows.Forms.ComboBox();

            this.label1 = new System.Windows.Forms.Label();
            this.cmbIso = new System.Windows.Forms.ComboBox();

            this.label2 = new System.Windows.Forms.Label();
            this.cmbTv = new System.Windows.Forms.ComboBox();

            this.label3 = new System.Windows.Forms.Label();
            this.cmbDriveMode = new System.Windows.Forms.ComboBox();

            this.btnShoot = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // btnConnect
            this.btnConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnConnect.Location = new System.Drawing.Point(25, 25);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(320, 40);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Kamera Verbinden";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);

            // lblStatus
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblStatus.Location = new System.Drawing.Point(25, 75);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(262, 16);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Status: Warte auf Verbindung...";

            // labelModus
            this.labelModus.AutoSize = true;
            this.labelModus.Location = new System.Drawing.Point(25, 115);
            this.labelModus.Name = "labelModus";
            this.labelModus.Size = new System.Drawing.Size(100, 16);
            this.labelModus.TabIndex = 9;
            this.labelModus.Text = "Kamera Modus:";

            // cmbAEMode
            this.cmbAEMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAEMode.FormattingEnabled = true;
            this.cmbAEMode.Location = new System.Drawing.Point(145, 112);
            this.cmbAEMode.Name = "cmbAEMode";
            this.cmbAEMode.Size = new System.Drawing.Size(200, 24);
            this.cmbAEMode.TabIndex = 10;
            this.cmbAEMode.SelectedIndexChanged += new System.EventHandler(this.cmbAEMode_SelectedIndexChanged);

            // label1
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 155);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "ISO Wert:";

            // cmbIso
            this.cmbIso.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIso.FormattingEnabled = true;
            this.cmbIso.Location = new System.Drawing.Point(145, 152);
            this.cmbIso.Name = "cmbIso";
            this.cmbIso.Size = new System.Drawing.Size(200, 24);
            this.cmbIso.TabIndex = 3;
            this.cmbIso.SelectedIndexChanged += new System.EventHandler(this.cmbIso_SelectedIndexChanged);

            // label2
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 195);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "Belichtungszeit:";

            // cmbTv
            this.cmbTv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTv.FormattingEnabled = true;
            this.cmbTv.Location = new System.Drawing.Point(145, 192);
            this.cmbTv.Name = "cmbTv";
            this.cmbTv.Size = new System.Drawing.Size(200, 24);
            this.cmbTv.TabIndex = 5;
            this.cmbTv.SelectedIndexChanged += new System.EventHandler(this.cmbTv_SelectedIndexChanged);

            // label3
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 235);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "Auslösemodus:";

            // cmbDriveMode
            this.cmbDriveMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDriveMode.FormattingEnabled = true;
            this.cmbDriveMode.Location = new System.Drawing.Point(145, 232);
            this.cmbDriveMode.Name = "cmbDriveMode";
            this.cmbDriveMode.Size = new System.Drawing.Size(200, 24);
            this.cmbDriveMode.TabIndex = 7;
            this.cmbDriveMode.SelectedIndexChanged += new System.EventHandler(this.cmbDriveMode_SelectedIndexChanged);

            // btnShoot
            this.btnShoot.BackColor = System.Drawing.Color.MistyRose;
            this.btnShoot.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnShoot.Location = new System.Drawing.Point(25, 285);
            this.btnShoot.Name = "btnShoot";
            this.btnShoot.Size = new System.Drawing.Size(320, 55);
            this.btnShoot.TabIndex = 8;
            this.btnShoot.Text = "📸 FOTO AUSLÖSEN";
            this.btnShoot.UseVisualStyleBackColor = false;
            this.btnShoot.Click += new System.EventHandler(this.btnShoot_Click);

            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 365);
            this.Controls.Add(this.btnShoot);
            this.Controls.Add(this.cmbDriveMode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbTv);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbIso);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbAEMode);
            this.Controls.Add(this.labelModus);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnConnect);
            this.Name = "Form1";
            this.Text = "EOS R5 Remote Control";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label labelModus;
        private System.Windows.Forms.ComboBox cmbAEMode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbIso;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbTv;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbDriveMode;
        private System.Windows.Forms.Button btnShoot;
    }
}