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

        // Alle Belichtungszeiten exakt in 1/3-Blenden-Schritten sortiert
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
            SetupPerfectLayout(); // <-- Die ultimative Layout-Lösung!
            SetupDataGridView();
            SetupBracketingUI();
            SetDefaultTimes();
            isInitializing = false;
            UpdatePhaseTimes();
        }

        private void SetupPerfectLayout()
        {
            // 1. Flexibles Fenster erlauben und Mindestgröße setzen
            this.MinimumSize = new Size(1100, 750);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;

            // 2. Wir suchen den linken Rand (Margin) des Fensters, meist 12 oder 20 Pixel
            int margin = 20;
            foreach (Control c in this.Controls)
            {
                if (c is GroupBox && c.Left > 0) { margin = c.Left; break; }
            }

            // 3. Jedes Haupt-Control (GroupBoxen) auf die volle Breite zwingen
            foreach (Control c in this.Controls)
            {
                if (c is GroupBox || c == dgvPhases || c == picTimeline)
                {
                    // Erziele Symmetrie: Die Box wird exakt so breit, dass der Rand rechts gleich wie links ist!
                    c.Width = this.ClientSize.Width - c.Left - margin;
                    c.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                    // Tabellen und Timeline INNERHALB der GroupBoxen ebenfalls anpassen
                    if (c is GroupBox)
                    {
                        foreach (Control child in c.Controls)
                        {
                            if (child == dgvPhases || child == picTimeline)
                            {
                                child.Width = c.ClientSize.Width - (child.Left * 2);
                                child.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                            }
                        }
                    }
                }
                else
                {
                    // Bei Buttons: Bottom-Anker deaktivieren, da wir vertikal manuell verschieben
                    c.Anchor = (c.Anchor & ~AnchorStyles.Bottom) | AnchorStyles.Top;
                }
            }

            // 4. Vertikale Logik im Resize Event (Verhindert Überlappen von Boxen!)
            int baseFormHeight = this.ClientSize.Height;

            // Finde heraus, in welcher Box Grid und Timeline liegen
            Control gridContainer = dgvPhases.Parent != null && dgvPhases.Parent != this ? dgvPhases.Parent : dgvPhases;
            Control timelineContainer = picTimeline.Parent != null && picTimeline.Parent != this ? picTimeline.Parent : picTimeline;

            int originalGridHeight = gridContainer.Height;
            int originalTimelineHeight = timelineContainer.Height;

            // Speichere die originalen Y-Positionen aller Elemente
            Dictionary<Control, int> originalTops = new Dictionary<Control, int>();
            foreach (Control c in this.Controls) { originalTops[c] = c.Top; }

            this.Resize += (s, e) =>
            {
                if (this.ClientSize.Height < 400) return;

                int addedHeight = this.ClientSize.Height - baseFormHeight;
                if (addedHeight < 0) return;

                // Wir verteilen die neue gewonnene Höhe zu 50% an die Tabelle und 50% an die Timeline
                int extraForGrid = addedHeight / 2;
                int extraForTimeline = addedHeight - extraForGrid;

                gridContainer.Height = originalGridHeight + extraForGrid;
                timelineContainer.Height = originalTimelineHeight + extraForTimeline;

                // Alle Elemente auf dem Formular werden nun intelligent nach unten geschoben
                foreach (Control c in this.Controls)
                {
                    // Die oberste Grid-Box bleibt natürlich, wo sie ist
                    if (c == gridContainer) continue;

                    // Lag dieses Element ursprünglich UNTERHALB der Tabelle? (z.B. HDR Box, Timeline)
                    if (originalTops[c] >= originalTops[gridContainer] + originalGridHeight)
                    {
                        int shift = extraForGrid;

                        // Lag dieses Element sogar UNTERHALB der Timeline? (z.B. der CSV-Button ganz unten)
                        if (originalTops[c] >= originalTops[timelineContainer] + originalTimelineHeight)
                        {
                            shift += extraForTimeline;
                        }
                        c.Top = originalTops[c] + shift;
                    }
                }
                picTimeline.Invalidate();
            };

            // 5. Fenster zum Start etwas breiter und höher machen.
            // Löst das Resize aus: Die Boxen schießen in die Breite, die Timeline in die Höhe!
            this.Width += 150;
            this.Height += 150;
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
            return 50;
        }

        private List<string> GetBracketSequence()
        {
            List<string> seq = new List<string>();
            int startIndex = Array.IndexOf(orderedTvs, cmbBracketMin.Text);
            int endIndex = Array.IndexOf(orderedTvs, cmbBracketMax.Text);
            int step = (int)numBracketEV.Value * 3;

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

            g.Clear(Color.FromArgb(215, 220, 225));

            if (dgvPhases.Rows.Count < 5) return;

            DateTime startVorlauf = dtpC1.Value.AddMinutes(-(double)numVorlauf.Value);
            DateTime endeNachlauf = dtpC4.Value.AddMinutes((double)numNachlauf.Value);
            TimeSpan totalFinsternis = endeNachlauf - startVorlauf;

            if (totalFinsternis.TotalSeconds <= 0) return;

            // Ränder für das Layout innerhalb der PictureBox
            float leftMargin = 30;
            float rightMargin = 80;
            float width = picTimeline.Width - (leftMargin + rightMargin);
            float height = picTimeline.Height;
            float currentX = leftMargin;

            // Dynamische Balken-Höhe: Oben 55 Pixel reservieren, unten 25 Pixel Puffer
            float barTop = 55;
            float barBottom = height - 25;

            if (barBottom <= barTop + 20) barBottom = barTop + 20;

            float barHeight = barBottom - barTop;

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

                // 1. BALKEN ZEICHNEN
                RectangleF rect = new RectangleF(currentX, barTop, phaseWidth, barHeight);
                using (Brush b = new SolidBrush(phaseColors[i])) g.FillRectangle(b, rect);
                g.DrawRectangle(Pens.Black, currentX, barTop, phaseWidth, barHeight);

                // 2. UHRZEIT AN DER GRENZE (45° gedreht)
                string startTimeStr = row.Cells["colStart"].Value?.ToString();
                if (!string.IsNullOrEmpty(startTimeStr))
                {
                    g.DrawLine(Pens.Black, currentX, barTop, currentX, barTop - 5);
                    using (Font fTime = new Font("Segoe UI", 9, FontStyle.Regular))
                    {
                        g.TranslateTransform(currentX, barTop - 5);
                        g.RotateTransform(-45);
                        g.DrawString(startTimeStr, fTime, Brushes.Black, 2, -14);
                        g.ResetTransform();
                    }
                }

                // 3. PHASEN-NAME
                RectangleF topTextRect = new RectangleF(currentX + 2, barTop + 4, Math.Max(0, phaseWidth - 4), 15);
                using (Font f = new Font("Segoe UI", 8, FontStyle.Bold))
                using (Brush bText = new SolidBrush(textColors[i]))
                {
                    g.DrawString(phaseNames[i], f, bText, topTextRect, sf);
                }

                // 4. BILDERANZAHL & STRICHE
                if (int.TryParse(row.Cells["colInterval"].Value?.ToString(), out int intervalMs) && intervalMs > 0)
                {
                    int shots = 0;
                    if (i == 2 && chkEnableBracketing.Checked)
                    {
                        var seq = GetBracketSequence();
                        if (seq.Count > 0)
                        {
                            int seqTimeMs = 0;
                            foreach (var tv in seq) seqTimeMs += GetTvMs(tv) + (int)numBracketWait.Value;
                            seqTimeMs += intervalMs;
                            int cycles = (int)(duration.TotalMilliseconds / seqTimeMs);
                            shots = cycles * seq.Count;
                        }
                    }
                    else
                    {
                        shots = (int)(duration.TotalMilliseconds / intervalMs);
                    }

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
                                    g.DrawLine(p, lineX, barTop, lineX, barTop + 10);
                                    g.DrawLine(p, lineX, barBottom - 10, lineX, barBottom);
                                }
                            }
                        }
                        else
                        {
                            using (Brush b = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
                                g.FillRectangle(b, currentX, barTop, phaseWidth, 10);
                        }
                    }

                    // --- SMART TEXT LAYOUT ---
                    string shotText = $"{shots} Bilder";
                    using (Font f = new Font("Segoe UI", 8, FontStyle.Bold))
                    using (Brush bText = new SolidBrush(textColors[i]))
                    {
                        SizeF textSize = g.MeasureString(shotText, f);

                        if (textSize.Width <= phaseWidth - 4)
                        {
                            RectangleF bottomTextRect = new RectangleF(currentX + 2, barBottom - 18, Math.Max(0, phaseWidth - 4), 15);
                            g.DrawString(shotText, f, bText, bottomTextRect, sf);
                        }
                        else
                        {
                            float maxAllowedHeight = barHeight - 20;

                            if (textSize.Width > maxAllowedHeight)
                            {
                                shotText = $"{shots}";
                                textSize = g.MeasureString(shotText, f);
                            }

                            if (textSize.Width <= maxAllowedHeight)
                            {
                                float textX = currentX + (phaseWidth / 2f) - (textSize.Height / 2f);
                                float textY = barBottom - 5f;

                                g.TranslateTransform(textX, textY);
                                g.RotateTransform(-90);
                                g.DrawString(shotText, f, bText, new PointF(0, 0));
                                g.ResetTransform();
                            }
                        }
                    }
                }

                currentX += phaseWidth;
            }

            // 5. FINALE ENDZEIT
            string finalTimeStr = dtpC4.Value.AddMinutes((double)numNachlauf.Value).ToString("HH:mm:ss");
            g.DrawLine(Pens.Black, currentX, barTop, currentX, barTop - 5);
            using (Font fTime = new Font("Segoe UI", 9, FontStyle.Regular))
            {
                g.TranslateTransform(currentX, barTop - 5);
                g.RotateTransform(-45);
                g.DrawString(finalTimeStr, fTime, Brushes.Black, 2, -14);
                g.ResetTransform();
            }

            sf.Dispose();
            g.DrawRectangle(Pens.Gray, 0, 0, picTimeline.Width - 1, picTimeline.Height - 1);
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

                        if (i == 2 && chkEnableBracketing.Checked)
                        {
                            var bracketSeq = GetBracketSequence();
                            if (bracketSeq.Count == 0)
                            {
                                MessageBox.Show("Bracketing Fehler: Schnellste Zeit muss kleiner/gleich langsamste Zeit sein.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            int seqTimeMs = 0;
                            foreach (var tv in bracketSeq) seqTimeMs += GetTvMs(tv) + (int)numBracketWait.Value;
                            seqTimeMs += intervalMs;

                            int cycles = (int)(duration.TotalMilliseconds / seqTimeMs);

                            sb.AppendLine($"# --- {phaseName} (HDR Bracketing: {cycles} Zyklen à {bracketSeq.Count} Bilder) ---");

                            for (int c = 0; c < cycles; c++)
                            {
                                sb.AppendLine($"# Zyklus {c + 1}");
                                for (int s = 0; s < bracketSeq.Count; s++)
                                {
                                    string tv = bracketSeq[s];
                                    int calcWait = GetTvMs(tv) + ((s == bracketSeq.Count - 1) ? intervalMs : (int)numBracketWait.Value);
                                    sb.AppendLine($"0;{calcWait};{iso};{tv}");
                                    totalShots++;
                                }
                            }
                        }
                        else
                        {
                            string tv = row.Cells["colTv"].Value?.ToString() ?? "1/1000";
                            int shotsInPhase = (int)(duration.TotalMilliseconds / intervalMs);

                            sb.AppendLine($"# --- {phaseName} ({shotsInPhase} Bilder) ---");

                            for (int s = 0; s < shotsInPhase; s++)
                            {
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