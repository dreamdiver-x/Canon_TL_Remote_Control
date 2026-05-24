using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TL_CSV_Configurator
{
    public partial class Form1 : Form
    {
        private bool isInitializing = true;

        // Alle Belichtungszeiten exakt in 1/3-Blenden-Schritten sortiert (schnell -> langsam)
        private readonly string[] orderedTvs = {
            "1/8000", "1/6400", "1/5000", "1/4000", "1/3200", "1/2500", "1/2000", "1/1600", "1/1250",
            "1/1000", "1/800", "1/640", "1/500", "1/400", "1/320", "1/250", "1/200", "1/160", "1/125",
            "1/100", "1/80", "1/60", "1/50", "1/40", "1/30", "1/25", "1/20", "1/15", "1/13", "1/10",
            "1/8", "1/6", "1/5", "1/4", "0.3\"", "0.4\"", "0.5\"", "0.6\"", "0.8\"", "1\"", "1.3\"",
            "1.6\"", "2\"", "2.5\"", "3.2\"", "4\"", "5\"", "6\"", "8\"", "10\"", "13\"", "15\"",
            "20\"", "25\"", "30\""
        };

        public Form1()
        {
            InitializeComponent();
            SetupDataGridView();
            SetupBracketingUI();
            SetDefaultTimes();
            isInitializing = false;
            UpdatePhaseTimes();
        }

        private void SetupBracketingUI()
        {
            cmbBracketMin.Items.AddRange(orderedTvs);
            cmbBracketMin.SelectedItem = "1/2000";

            cmbBracketMax.Items.AddRange(orderedTvs);
            cmbBracketMax.SelectedItem = "3.2\"";
        }

        private void SetDefaultTimes()
        {
            DateTime now = DateTime.Now;
            DateTime c1 = new DateTime(now.Year, now.Month, now.Day, 12, 0, 0);

            dtpC1.Value = c1;
            dtpC2.Value = c1.AddHours(1);
            dtpC3.Value = c1.AddHours(1).AddMinutes(4);
            dtpC4.Value = c1.AddHours(2).AddMinutes(4);

            numVorlauf.Value = 5;
            numNachlauf.Value = 5;
        }

        private void SetupDataGridView()
        {
            dgvPhases.Columns.Clear();
            dgvPhases.AllowUserToAddRows = false;
            dgvPhases.AllowUserToDeleteRows = false;
            dgvPhases.RowHeadersVisible = false;
            dgvPhases.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvPhases.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPhase", HeaderText = "Phase der Finsternis", ReadOnly = true });
            dgvPhases.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStart", HeaderText = "Startzeit", ReadOnly = true });
            dgvPhases.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEnd", HeaderText = "Endzeit", ReadOnly = true });
            dgvPhases.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDuration", HeaderText = "Dauer", ReadOnly = true });

            dgvPhases.Columns.Add(new DataGridViewTextBoxColumn { Name = "colInterval", HeaderText = "Intervall (ms)" });

            string[] isos = { "Auto", "50", "100", "125", "160", "200", "250", "320", "400", "500", "640", "800", "1000", "1250", "1600", "2000", "2500", "3200", "4000", "5000", "6400", "8000", "10000", "12800", "16000", "20000", "25600", "32000", "40000", "51200", "102400" };
            var colIso = new DataGridViewComboBoxColumn { Name = "colIso", HeaderText = "ISO" };
            colIso.Items.AddRange(isos);
            dgvPhases.Columns.Add(colIso);

            var colTv = new DataGridViewComboBoxColumn { Name = "colTv", HeaderText = "Belichtungszeit" };
            colTv.Items.AddRange(orderedTvs);
            dgvPhases.Columns.Add(colTv);

            dgvPhases.Rows.Add("1. Vorlauf", "", "", "", "10000", "100", "1/1000");
            dgvPhases.Rows.Add("2. Partielle Phase (C1 - C2)", "", "", "", "10000", "100", "1/1000");
            dgvPhases.Rows.Add("3. Totalität (C2 - C3)", "", "", "", "5000", "100", "1/15");
            dgvPhases.Rows.Add("4. Partielle Phase (C3 - C4)", "", "", "", "10000", "100", "1/1000");
            dgvPhases.Rows.Add("5. Nachlauf", "", "", "", "10000", "100", "1/1000");

            dgvPhases.Columns["colPhase"].DefaultCellStyle.BackColor = Color.LightGray;
            dgvPhases.Columns["colStart"].DefaultCellStyle.BackColor = Color.WhiteSmoke;
            dgvPhases.Columns["colEnd"].DefaultCellStyle.BackColor = Color.WhiteSmoke;
            dgvPhases.Columns["colDuration"].DefaultCellStyle.BackColor = Color.WhiteSmoke;

            dgvPhases.CellValueChanged += DgvPhases_CellValueChanged;
            dgvPhases.CurrentCellDirtyStateChanged += DgvPhases_CurrentCellDirtyStateChanged;
        }

        private void DgvPhases_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvPhases.IsCurrentCellDirty) dgvPhases.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void DgvPhases_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvPhases.Columns["colInterval"].Index) picTimeline.Invalidate();
        }

        private void chkEnableBracketing_CheckedChanged(object sender, EventArgs e)
        {
            bool ena = chkEnableBracketing.Checked;
            cmbBracketMin.Enabled = ena;
            cmbBracketMax.Enabled = ena;
            numBracketEV.Enabled = ena;
            numBracketWait.Enabled = ena;
            picTimeline.Invalidate();
        }

        private void BracketingControl_Changed(object sender, EventArgs e)
        {
            if (!isInitializing) picTimeline.Invalidate();
        }

        private void UpdatePhaseTimes()
        {
            if (isInitializing) return;

            DateTime c1 = dtpC1.Value;
            DateTime c2 = dtpC2.Value;
            DateTime c3 = dtpC3.Value;
            DateTime c4 = dtpC4.Value;

            DateTime startVorlauf = c1.AddMinutes(-(double)numVorlauf.Value);
            DateTime endeNachlauf = c4.AddMinutes((double)numNachlauf.Value);

            UpdateRowTimes(0, startVorlauf, c1);
            UpdateRowTimes(1, c1, c2);
            UpdateRowTimes(2, c2, c3);
            UpdateRowTimes(3, c3, c4);
            UpdateRowTimes(4, c4, endeNachlauf);

            picTimeline.Invalidate();
        }

        private void UpdateRowTimes(int rowIndex, DateTime start, DateTime end)
        {
            if (rowIndex >= dgvPhases.Rows.Count) return;

            TimeSpan duration = end - start;
            dgvPhases.Rows[rowIndex].Cells["colStart"].Value = start.ToString("HH:mm:ss");
            dgvPhases.Rows[rowIndex].Cells["colEnd"].Value = end.ToString("HH:mm:ss");
            dgvPhases.Rows[rowIndex].Cells["colDuration"].Value = $"{(int)duration.TotalMinutes}m {duration.Seconds}s";
            dgvPhases.Rows[rowIndex].Tag = duration;
        }

        private void TimePickers_ValueChanged(object sender, EventArgs e)
        {
            UpdatePhaseTimes();
        }

        // Parst die Belichtungszeit intelligent zu Millisekunden (Sicherheit gegen Kamera-Blockade)
        private int GetTvMs(string tv)
        {
            if (tv.EndsWith("\""))
            {
                if (double.TryParse(tv.TrimEnd('"'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double sec))
                    return (int)(sec * 1000);
            }
            else if (tv.Contains("/"))
            {
                string[] parts = tv.Split('/');
                if (parts.Length == 2 && double.TryParse(parts[1], out double div))
                    return (int)(1000 / div);
            }
            return 50; // Fallback
        }

        // Generiert die Liste der Zeiten für einen Bracketing-Zyklus
        private List<string> GetBracketSequence()
        {
            List<string> seq = new List<string>();
            int startIndex = Array.IndexOf(orderedTvs, cmbBracketMin.Text);
            int endIndex = Array.IndexOf(orderedTvs, cmbBracketMax.Text);
            int step = (int)numBracketEV.Value * 3; // 1 EV = 3 Index-Schritte in der 1/3-Blenden-Reihe

            if (startIndex == -1 || endIndex == -1 || startIndex > endIndex) return seq;

            for (int i = startIndex; i <= endIndex; i += step)
            {
                seq.Add(orderedTvs[i]);
            }
            return seq;
        }

        private void picTimeline_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            if (dgvPhases.Rows.Count < 5) return;

            DateTime startVorlauf = dtpC1.Value.AddMinutes(-(double)numVorlauf.Value);
            DateTime endeNachlauf = dtpC4.Value.AddMinutes((double)numNachlauf.Value);
            TimeSpan totalFinsternis = endeNachlauf - startVorlauf;

            if (totalFinsternis.TotalSeconds <= 0) return;

            float width = picTimeline.Width - 20;
            float height = picTimeline.Height;
            float currentX = 10;

            Color[] phaseColors = { Color.LightBlue, Color.Gold, Color.DarkSlateBlue, Color.Gold, Color.LightBlue };
            string[] phaseNames = { "Vorlauf", "Partiell 1", "Totalität", "Partiell 2", "Nachlauf" };
            Color[] textColors = { Color.Black, Color.Black, Color.White, Color.Black, Color.Black };

            StringFormat sf = new StringFormat();
            sf.Trimming = StringTrimming.EllipsisCharacter;
            sf.FormatFlags = StringFormatFlags.NoWrap;

            for (int i = 0; i < 5; i++)
            {
                var row = dgvPhases.Rows[i];
                if (row.Tag == null) continue;
                TimeSpan duration = (TimeSpan)row.Tag;
                if (duration.TotalSeconds <= 0) continue;

                float phaseWidth = (float)(duration.TotalSeconds / totalFinsternis.TotalSeconds) * width;

                RectangleF rect = new RectangleF(currentX, 25, phaseWidth, height - 50);
                using (Brush b = new SolidBrush(phaseColors[i])) g.FillRectangle(b, rect);
                g.DrawRectangle(Pens.Black, currentX, 25, phaseWidth, height - 50);

                RectangleF topTextRect = new RectangleF(currentX + 2, 8, Math.Max(0, phaseWidth - 4), 15);
                using (Font f = new Font("Segoe UI", 8, FontStyle.Bold))
                using (Brush bText = new SolidBrush(textColors[i]))
                {
                    g.DrawString(phaseNames[i], f, bText, topTextRect, sf);
                }

                if (int.TryParse(row.Cells["colInterval"].Value?.ToString(), out int intervalMs) && intervalMs > 0)
                {
                    int shots = 0;

                    // Spezielle Berechnung für Totalitäts-Bracketing
                    if (i == 2 && chkEnableBracketing.Checked)
                    {
                        var seq = GetBracketSequence();
                        if (seq.Count > 0)
                        {
                            int seqTimeMs = 0;
                            foreach (var tv in seq) seqTimeMs += GetTvMs(tv) + (int)numBracketWait.Value;
                            seqTimeMs += intervalMs; // Das Hauptintervall nach dem Zyklus

                            int cycles = (int)(duration.TotalMilliseconds / seqTimeMs);
                            shots = cycles * seq.Count;
                        }
                    }
                    else
                    {
                        shots = (int)(duration.TotalMilliseconds / intervalMs);
                    }

                    RectangleF bottomTextRect = new RectangleF(currentX + 2, height - 22, Math.Max(0, phaseWidth - 4), 15);
                    using (Font f = new Font("Segoe UI", 8, FontStyle.Bold))
                    using (Brush bText = new SolidBrush(textColors[i]))
                    {
                        g.DrawString($"{shots} Bilder", f, bText, bottomTextRect, sf);
                    }

                    // Zeichnet für jedes Bild einen winzigen Strich zur Visualisierung
                    if (shots > 0)
                    {
                        float stepX = phaseWidth / shots;
                        if (stepX >= 2.0f)
                        {
                            using (Pen p = new Pen(Color.FromArgb(100, textColors[i])))
                            {
                                for (int s = 0; s < shots; s++)
                                {
                                    float lineX = currentX + (s * stepX);
                                    g.DrawLine(p, lineX, 25, lineX, 35);
                                    g.DrawLine(p, lineX, height - 35, lineX, height - 25);
                                }
                            }
                        }
                        else
                        {
                            using (Brush b = new SolidBrush(Color.FromArgb(50, 0, 0, 0))) g.FillRectangle(b, currentX, 25, phaseWidth, 10);
                        }
                    }
                }

                currentX += phaseWidth;
            }
            sf.Dispose();
        }

        private void btnGenerateCSV_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "CSV Datei|*.csv",
                Title = "Eclipse Timelapse CSV speichern",
                FileName = $"Eclipse_TL_{dtpC1.Value:yyyyMMdd}.csv"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("# Eclipse HDR Timelapse Konfiguration");

                    DateTime globaleStartzeit = dtpC1.Value.AddMinutes(-(double)numVorlauf.Value);
                    sb.AppendLine($"STARTTIME; {globaleStartzeit:dd.MM.yyyy HH:mm:ss}");
                    sb.AppendLine("#");
                    sb.AppendLine("# Spalten: WartezeitVor(ms) ; WartezeitNach(ms) ; ISO ; Belichtungszeit");

                    int totalShots = 0;

                    for (int i = 0; i < dgvPhases.Rows.Count; i++)
                    {
                        var row = dgvPhases.Rows[i];
                        TimeSpan duration = (TimeSpan)row.Tag;

                        if (duration.TotalMilliseconds <= 0) continue;

                        if (!int.TryParse(row.Cells["colInterval"].Value?.ToString(), out int intervalMs) || intervalMs < 1000)
                        {
                            MessageBox.Show($"Fehler in Zeile {i + 1}: Das Intervall muss mindestens 1000ms betragen.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        string iso = row.Cells["colIso"].Value?.ToString() ?? "100";
                        string phaseName = row.Cells["colPhase"].Value?.ToString();

                        // --- HDR BRACKETING FÜR PHASE 3 (Totalität) ---
                        if (i == 2 && chkEnableBracketing.Checked)
                        {
                            var bracketSeq = GetBracketSequence();
                            if (bracketSeq.Count == 0)
                            {
                                MessageBox.Show("Bracketing Fehler: Schnellste Zeit muss kleiner/gleich langsamste Zeit sein.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            // Berechne wie lange ein kompletter Zyklus dauert
                            int seqTimeMs = 0;
                            foreach (var tv in bracketSeq) seqTimeMs += GetTvMs(tv) + (int)numBracketWait.Value;
                            seqTimeMs += intervalMs; // Plus das generelle Intervall nach einem kompletten Zyklus

                            int cycles = (int)(duration.TotalMilliseconds / seqTimeMs);

                            sb.AppendLine($"# --- {phaseName} (HDR Bracketing: {cycles} Zyklen à {bracketSeq.Count} Bilder) ---");

                            for (int c = 0; c < cycles; c++)
                            {
                                sb.AppendLine($"# Zyklus {c + 1}");
                                for (int s = 0; s < bracketSeq.Count; s++)
                                {
                                    string tv = bracketSeq[s];

                                    // Der smarte Part: Füge die Belichtungszeit als Delay zur Pause hinzu!
                                    int calcWait = GetTvMs(tv) + ((s == bracketSeq.Count - 1) ? intervalMs : (int)numBracketWait.Value);

                                    sb.AppendLine($"0;{calcWait};{iso};{tv}");
                                    totalShots++;
                                }
                            }
                        }
                        // --- NORMALE PHASEN (ohne Bracketing) ---
                        else
                        {
                            string tv = row.Cells["colTv"].Value?.ToString() ?? "1/1000";
                            int shotsInPhase = (int)(duration.TotalMilliseconds / intervalMs);

                            sb.AppendLine($"# --- {phaseName} ({shotsInPhase} Bilder) ---");

                            for (int s = 0; s < shotsInPhase; s++)
                            {
                                // Auch hier kleine Sicherheit: Belichtungszeit zur Pause addieren
                                int safeInterval = intervalMs + GetTvMs(tv);
                                sb.AppendLine($"0;{safeInterval};{iso};{tv}");
                                totalShots++;
                            }
                        }
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString());
                    MessageBox.Show($"CSV erfolgreich generiert!\nGesamte Bilderanzahl: {totalShots}", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Speichern:\n" + ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}