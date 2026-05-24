using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EOSDigital.API;
using EOSDigital.SDK;

namespace EOS_R5_RemoteGUI
{
    public partial class Form1 : Form
    {
        private CanonAPI api;
        private Camera camera;
        private bool isConnected = false;

        private readonly string settingsPath = "KameraEinstellungen.xml";
        private string logFilePath;

        // Timelapse Steuerung
        private CancellationTokenSource tlCts;
        private bool isUpdatingUI = false;

        public Form1()
        {
            InitializeComponent();

            logFilePath = $"Log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";
            Log("=== Programmstart ===");

            ErstelleBeispielCsv();

            labelModus.Text = "Kamera Modus (Read-Only):";
            btnShoot.Enabled = false;
            cmbAEMode.Enabled = false;
            cmbIso.Enabled = false;
            cmbTv.Enabled = false;
            cmbDriveMode.Enabled = false;

            if (btnStopTL != null) btnStopTL.Enabled = false;

            try
            {
                api = new CanonAPI();
                Log("CanonAPI erfolgreich initialisiert.");
            }
            catch (Exception ex)
            {
                Log("Fehler Treiber: " + ex.Message);
            }

            PopulateStaticDropdowns();
            LoadSettingsFromXml();
        }

        // --- 0. LOGGING ENGINE ---
        private void Log(string message)
        {
            string timeStamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string logEntry = $"[{timeStamp}] {message}";

            try { File.AppendAllText(logFilePath, logEntry + Environment.NewLine); } catch { }

            if (lblStatus != null)
            {
                if (this.InvokeRequired) this.Invoke(new Action(() => lblStatus.Text = message));
                else lblStatus.Text = message;
            }

            if (rtbLog != null)
            {
                Action appendAction = () =>
                {
                    rtbLog.AppendText(logEntry + Environment.NewLine);
                    if (rtbLog.Lines.Length > 2000)
                    {
                        rtbLog.SelectionStart = 0;
                        rtbLog.SelectionLength = rtbLog.GetFirstCharIndexFromLine(rtbLog.Lines.Length - 2000);
                        rtbLog.SelectedText = "";
                    }
                    rtbLog.ScrollToCaret();
                };

                if (this.InvokeRequired) this.Invoke(appendAction);
                else appendAction();
            }
        }

        // --- 1. SETUP & XML LOGIK ---
        private void PopulateStaticDropdowns()
        {
            cmbAEMode.DisplayMember = "Name";
            cmbAEMode.Items.Add(new SettingItem("Tv (Zeitautomatik)", 0x01));
            cmbAEMode.Items.Add(new SettingItem("Av (Blendenautomatik)", 0x02));
            cmbAEMode.Items.Add(new SettingItem("Manuell (M)", 0x03));
            cmbAEMode.Items.Add(new SettingItem("Bulb", 0x04));

            cmbIso.DisplayMember = "Name";
            cmbIso.Items.Add(new SettingItem("Auto", 0x00));
            cmbIso.Items.Add(new SettingItem("50", 0x40)); // L
            cmbIso.Items.Add(new SettingItem("100", 0x48));
            cmbIso.Items.Add(new SettingItem("125", 0x4B));
            cmbIso.Items.Add(new SettingItem("160", 0x4D));
            cmbIso.Items.Add(new SettingItem("200", 0x50));
            cmbIso.Items.Add(new SettingItem("250", 0x53));
            cmbIso.Items.Add(new SettingItem("320", 0x55));
            cmbIso.Items.Add(new SettingItem("400", 0x58));
            cmbIso.Items.Add(new SettingItem("500", 0x5B));
            cmbIso.Items.Add(new SettingItem("640", 0x5D));
            cmbIso.Items.Add(new SettingItem("800", 0x60));
            cmbIso.Items.Add(new SettingItem("1000", 0x63));
            cmbIso.Items.Add(new SettingItem("1250", 0x65));
            cmbIso.Items.Add(new SettingItem("1600", 0x68));
            cmbIso.Items.Add(new SettingItem("2000", 0x6B));
            cmbIso.Items.Add(new SettingItem("2500", 0x6D));
            cmbIso.Items.Add(new SettingItem("3200", 0x70));
            cmbIso.Items.Add(new SettingItem("4000", 0x73));
            cmbIso.Items.Add(new SettingItem("5000", 0x75));
            cmbIso.Items.Add(new SettingItem("6400", 0x78));
            cmbIso.Items.Add(new SettingItem("8000", 0x7B));
            cmbIso.Items.Add(new SettingItem("10000", 0x7D));
            cmbIso.Items.Add(new SettingItem("12800", 0x80));
            cmbIso.Items.Add(new SettingItem("16000", 0x83));
            cmbIso.Items.Add(new SettingItem("20000", 0x85));
            cmbIso.Items.Add(new SettingItem("25600", 0x88));
            cmbIso.Items.Add(new SettingItem("32000", 0x8B));
            cmbIso.Items.Add(new SettingItem("40000", 0x8D));
            cmbIso.Items.Add(new SettingItem("51200", 0x90));
            cmbIso.Items.Add(new SettingItem("102400", 0x98)); // H

            cmbDriveMode.DisplayMember = "Name";
            cmbDriveMode.Items.Add(new SettingItem("Einzelbild", 0x00));
            cmbDriveMode.Items.Add(new SettingItem("Serienbild", 0x01));
            cmbDriveMode.Items.Add(new SettingItem("High-Speed", 0x04));
            cmbDriveMode.Items.Add(new SettingItem("Timer 10s", 0x10));
            cmbDriveMode.Items.Add(new SettingItem("Timer 2s", 0x11));
        }

        private void UpdateTvDropdown(uint aeModeHex)
        {
            uint currentTvHex = 0;
            if (cmbTv.SelectedItem != null) currentTvHex = ((SettingItem)cmbTv.SelectedItem).HexValue;

            cmbTv.Items.Clear();
            cmbTv.DisplayMember = "Name";

            if (aeModeHex == 0x04) cmbTv.Items.Add(new SettingItem("Bulb", 0x0C));
            else
            {
                cmbTv.Items.Add(new SettingItem("30\"", 0x10));
                cmbTv.Items.Add(new SettingItem("25\"", 0x13));
                cmbTv.Items.Add(new SettingItem("20\"", 0x15));
                cmbTv.Items.Add(new SettingItem("15\"", 0x18));
                cmbTv.Items.Add(new SettingItem("13\"", 0x1B));
                cmbTv.Items.Add(new SettingItem("10\"", 0x1D));
                cmbTv.Items.Add(new SettingItem("8\"", 0x20));
                cmbTv.Items.Add(new SettingItem("6\"", 0x23));
                cmbTv.Items.Add(new SettingItem("5\"", 0x25));
                cmbTv.Items.Add(new SettingItem("4\"", 0x28));
                cmbTv.Items.Add(new SettingItem("3.2\"", 0x2B));
                cmbTv.Items.Add(new SettingItem("2.5\"", 0x2D));
                cmbTv.Items.Add(new SettingItem("2\"", 0x30));
                cmbTv.Items.Add(new SettingItem("1.6\"", 0x33));
                cmbTv.Items.Add(new SettingItem("1.3\"", 0x35));
                cmbTv.Items.Add(new SettingItem("1\"", 0x38));
                cmbTv.Items.Add(new SettingItem("0.8\"", 0x3B));
                cmbTv.Items.Add(new SettingItem("0.6\"", 0x3D));
                cmbTv.Items.Add(new SettingItem("0.5\"", 0x40));
                cmbTv.Items.Add(new SettingItem("0.4\"", 0x43));
                cmbTv.Items.Add(new SettingItem("0.3\"", 0x45));
                cmbTv.Items.Add(new SettingItem("1/4", 0x48));
                cmbTv.Items.Add(new SettingItem("1/5", 0x4B));
                cmbTv.Items.Add(new SettingItem("1/6", 0x4D));
                cmbTv.Items.Add(new SettingItem("1/8", 0x50));
                cmbTv.Items.Add(new SettingItem("1/10", 0x53));
                cmbTv.Items.Add(new SettingItem("1/13", 0x55));
                cmbTv.Items.Add(new SettingItem("1/15", 0x58));
                cmbTv.Items.Add(new SettingItem("1/20", 0x5B));
                cmbTv.Items.Add(new SettingItem("1/25", 0x5D));
                cmbTv.Items.Add(new SettingItem("1/30", 0x60));
                cmbTv.Items.Add(new SettingItem("1/40", 0x63));
                cmbTv.Items.Add(new SettingItem("1/50", 0x65));
                cmbTv.Items.Add(new SettingItem("1/60", 0x68));
                cmbTv.Items.Add(new SettingItem("1/80", 0x6B));
                cmbTv.Items.Add(new SettingItem("1/100", 0x6D));
                cmbTv.Items.Add(new SettingItem("1/125", 0x70));
                cmbTv.Items.Add(new SettingItem("1/160", 0x73));
                cmbTv.Items.Add(new SettingItem("1/200", 0x75));
                cmbTv.Items.Add(new SettingItem("1/250", 0x78));
                cmbTv.Items.Add(new SettingItem("1/320", 0x7B));
                cmbTv.Items.Add(new SettingItem("1/400", 0x7D));
                cmbTv.Items.Add(new SettingItem("1/500", 0x80));
                cmbTv.Items.Add(new SettingItem("1/640", 0x83));
                cmbTv.Items.Add(new SettingItem("1/800", 0x85));
                cmbTv.Items.Add(new SettingItem("1/1000", 0x88));
                cmbTv.Items.Add(new SettingItem("1/1250", 0x8B));
                cmbTv.Items.Add(new SettingItem("1/1600", 0x8D));
                cmbTv.Items.Add(new SettingItem("1/2000", 0x90));
                cmbTv.Items.Add(new SettingItem("1/2500", 0x93));
                cmbTv.Items.Add(new SettingItem("1/3200", 0x95));
                cmbTv.Items.Add(new SettingItem("1/4000", 0x98));
                cmbTv.Items.Add(new SettingItem("1/5000", 0x9B));
                cmbTv.Items.Add(new SettingItem("1/6400", 0x9D));
                cmbTv.Items.Add(new SettingItem("1/8000", 0xA0));
            }

            bool found = false;
            for (int i = 0; i < cmbTv.Items.Count; i++)
            {
                if (((SettingItem)cmbTv.Items[i]).HexValue == currentTvHex)
                {
                    cmbTv.SelectedIndex = i; found = true; break;
                }
            }
            if (!found && cmbTv.Items.Count > 0) cmbTv.SelectedIndex = 0;
        }

        private void LoadSettingsFromXml()
        {
            try
            {
                if (File.Exists(settingsPath))
                {
                    System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Load(settingsPath);
                    uint modeHex = uint.Parse(doc.Root.Element("AEModeHex")?.Value ?? "3");
                    uint isoHex = uint.Parse(doc.Root.Element("IsoHex")?.Value ?? "0");
                    uint tvHex = uint.Parse(doc.Root.Element("TvHex")?.Value ?? "16");
                    uint driveHex = uint.Parse(doc.Root.Element("DriveModeHex")?.Value ?? "0");

                    SelectDropdownItemByHex(cmbAEMode, modeHex);
                    SelectDropdownItemByHex(cmbIso, isoHex);
                    SelectDropdownItemByHex(cmbDriveMode, driveHex);

                    UpdateTvDropdown(modeHex);
                    SelectDropdownItemByHex(cmbTv, tvHex);
                    Log("Settings aus XML geladen.");
                }
            }
            catch (Exception ex) { Log($"Fehler beim Laden der XML: {ex.Message}"); }
        }

        private void SelectDropdownItemByHex(ComboBox cmb, uint hexValue)
        {
            for (int i = 0; i < cmb.Items.Count; i++)
            {
                if (((SettingItem)cmb.Items[i]).HexValue == hexValue)
                {
                    if (cmb.SelectedIndex != i) cmb.SelectedIndex = i;
                    return;
                }
            }
            if (cmb.Items.Count > 0 && cmb.SelectedIndex != 0) cmb.SelectedIndex = 0;
        }

        private void SaveSettingsToXml()
        {
            try
            {
                uint modeHex = cmbAEMode.SelectedItem != null ? ((SettingItem)cmbAEMode.SelectedItem).HexValue : 0x03;
                uint isoHex = cmbIso.SelectedItem != null ? ((SettingItem)cmbIso.SelectedItem).HexValue : 0x00;
                uint tvHex = cmbTv.SelectedItem != null ? ((SettingItem)cmbTv.SelectedItem).HexValue : 0x10;
                uint driveHex = cmbDriveMode.SelectedItem != null ? ((SettingItem)cmbDriveMode.SelectedItem).HexValue : 0x00;

                System.Xml.Linq.XDocument doc = new System.Xml.Linq.XDocument(new System.Xml.Linq.XElement("KameraEinstellungen",
                    new System.Xml.Linq.XElement("AEModeHex", modeHex), new System.Xml.Linq.XElement("IsoHex", isoHex),
                    new System.Xml.Linq.XElement("TvHex", tvHex), new System.Xml.Linq.XElement("DriveModeHex", driveHex)));
                doc.Save(settingsPath);
                Log("Settings in XML gespeichert.");
            }
            catch (Exception ex) { Log($"Fehler beim Speichern der XML: {ex.Message}"); }
        }

        private void cmbAEMode_SelectedIndexChanged(object sender, EventArgs e) { }

        // --- 2. DIE VERBINDUNG ---
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (isConnected || api != null)
            {
                Log("Trenne alte Verbindung / Bereinige Treiber...");
                btnShoot.Enabled = false;
                try
                {
                    if (camera != null) camera.CloseSession();
                    if (api != null) api.Dispose();
                }
                catch { }

                api = null; camera = null; isConnected = false;
                btnConnect.Text = "Kamera Verbinden";
                Log("Getrennt. Bereit für Neustart.");
                return;
            }

            try
            {
                Log("Starte Canon-Treiber neu...");
                api = new CanonAPI();
                List<Camera> camList = api.GetCameraList();

                if (camList.Count > 0)
                {
                    camera = camList[0];
                    camera.OpenSession();

                    uint saveToPc = 1; // 1 = Speichern NUR auf SD-Karte!
                    IntPtr saveToPtr = System.Runtime.InteropServices.Marshal.AllocHGlobal(sizeof(uint));
                    try
                    {
                        System.Runtime.InteropServices.Marshal.WriteInt32(saveToPtr, (int)saveToPc);
                        uint err = (uint)CanonSDK.EdsSetPropertyData(camera.Reference, (PropertyID)0x0000000B, 0, sizeof(uint), saveToPtr);
                        if (err == 0x8D)
                        {
                            Log("Fehler: Kamera ist beschäftigt. (0x8D)");
                            camera.CloseSession();
                            return;
                        }
                    }
                    finally { System.Runtime.InteropServices.Marshal.FreeHGlobal(saveToPtr); }

                    CanonSDK.EdsSendStatusCommand(camera.Reference, (CameraStatusCommand)1, 0);

                    try
                    {
                        uint currentMode = (uint)camera.GetInt32Setting((PropertyID)0x00000400);
                        SelectDropdownItemByHex(cmbAEMode, currentMode);
                        UpdateTvDropdown(currentMode);
                    }
                    catch { Log("Modus konnte nicht ausgelesen werden."); }

                    isConnected = true;
                    btnConnect.Text = "Kamera Trennen / Reconnect";
                    Log($"{camera.DeviceName} erfolgreich verbunden und bereit!");

                    btnShoot.Enabled = true;
                    cmbIso.Enabled = true;
                    cmbTv.Enabled = true;
                    cmbDriveMode.Enabled = true;
                }
                else
                {
                    Log("Keine Kamera gefunden.");
                    if (api != null) api.Dispose(); api = null;
                }
            }
            catch (Exception ex) { Log("Verbindungsfehler: " + ex.Message); }
        }

        // --- 3. KAMERAEINSTELLUNGEN ÄNDERN (Manuell per Klick) ---
        private void cmbIso_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isUpdatingUI) return;

            if (camera != null && cmbIso.SelectedItem != null && cmbIso.Enabled)
            {
                try
                {
                    var item = (SettingItem)cmbIso.SelectedItem;
                    camera.SetSetting((PropertyID)0x00000402, item.HexValue);
                    Log($"ISO auf {item.Name} gesetzt.");
                }
                catch (Exception ex) { Log($"Fehler ISO: {ex.Message}"); }
            }
        }

        private void cmbTv_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isUpdatingUI) return;

            if (camera != null && cmbTv.SelectedItem != null && cmbTv.Enabled)
            {
                try
                {
                    var item = (SettingItem)cmbTv.SelectedItem;
                    camera.SetSetting((PropertyID)0x00000406, item.HexValue);
                    Log($"Belichtungszeit auf {item.Name} gesetzt.");
                }
                catch (Exception ex) { Log($"Fehler Tv: {ex.Message}"); }
            }
        }

        private void cmbDriveMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isUpdatingUI) return;

            if (camera != null && cmbDriveMode.SelectedItem != null && cmbDriveMode.Enabled)
            {
                try
                {
                    var item = (SettingItem)cmbDriveMode.SelectedItem;
                    camera.SetSetting((PropertyID)0x00000401, item.HexValue);
                    Log($"Auslösemodus auf {item.Name} gesetzt.");
                }
                catch (Exception ex) { Log($"Fehler DriveMode: {ex.Message}"); }
            }
        }

        // --- 4. FOTO AUSLÖSEN ---
        private void btnShoot_Click(object sender, EventArgs e)
        {
            TriggerShoot();
        }

        private void TriggerShoot()
        {
            if (camera != null)
            {
                try
                {
                    Log("Kamera löst aus...");
                    CanonSDK.EdsSendCommand(camera.Reference, (CameraCommand)0, 0);
                    Log("📸 Foto sicher auf SD-Karte gespeichert!");
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("TAKE_PICTURE_AF_NG"))
                        Log("Fehler: Autofokus fehlgeschlagen! (Zu dunkel/nah). Bitte auf MF schalten.");
                    else
                        Log($"Fehler beim Auslösen: {ex.Message}");
                }
            }
            else
            {
                Log("Auslösen fehlgeschlagen: Keine Kamera verbunden.");
            }
        }

        // --- 5. TIMELAPSE FUNKTION ---
        private void ErstelleBeispielCsv()
        {
            string path = "TL_Beispiel_Klartext.csv";
            try
            {
                if (!File.Exists(path))
                {
                    string inhalt = "# Timelapse Test-Skript (3 Bilder)\n" +
                                    "# Optional: Geplante Startzeit (z.B. STARTTIME; 18:30:00)\n" +
                                    "# Wenn Du sofort starten willst, lass diese Zeile weg oder mach ein # davor.\n" +
                                    "STARTTIME; 18:30:00\n" +
                                    "#\n" +
                                    "# Spalten: WartezeitVor(ms) ; WartezeitNach(ms) ; ISO ; Belichtungszeit\n" +
                                    "# WICHTIG: ISO und Zeit exakt so schreiben, wie sie im Dropdown stehen!\n" +
                                    "0;3000;100;1/125\n" +
                                    "0;3000;200;1/60\n" +
                                    "0;3000;400;1/30\n";
                    File.WriteAllText(path, inhalt);
                    Log("Klartext-Beispiel-CSV (inkl. STARTTIME) wurde erstellt: " + path);
                }
            }
            catch (Exception ex)
            {
                Log("Konnte Klartext-Beispiel-CSV nicht erstellen: " + ex.Message);
            }
        }

        private void ApplyTimelapseSettings(uint isoHex, uint tvHex)
        {
            isUpdatingUI = true;
            try
            {
                if (camera != null)
                {
                    try { camera.SetSetting((PropertyID)0x00000402, isoHex); }
                    catch (Exception ex) { Log($"Kamera-Fehler bei ISO-Update: {ex.Message}"); }

                    try { camera.SetSetting((PropertyID)0x00000406, tvHex); }
                    catch (Exception ex) { Log($"Kamera-Fehler bei Tv-Update: {ex.Message}"); }
                }

                SelectDropdownItemByHex(cmbIso, isoHex);
                SelectDropdownItemByHex(cmbTv, tvHex);
            }
            finally
            {
                isUpdatingUI = false;
            }
        }

        private async void btnStartTL_Click(object sender, EventArgs e)
        {
            if (!isConnected) { Log("TL Start abgebrochen: Keine Kamera verbunden."); return; }
            if (tlCts != null) { Log("TL läuft bereits."); return; }

            OpenFileDialog ofd = new OpenFileDialog { Filter = "CSV Dateien|*.csv|Alle Dateien|*.*" };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            TLScript script = ParseCsvFile(ofd.FileName);
            if (script.Steps.Count == 0)
            {
                Log("Keine gültigen TL-Schritte in der CSV gefunden.");
                return;
            }

            Log($"CSV geladen: {script.Steps.Count} TL-Schritte gefunden.");

            tlCts = new CancellationTokenSource();
            btnStartTL.Enabled = false;
            btnStopTL.Enabled = true;

            try
            {
                await RunTimelapseAsync(script, tlCts.Token);
                Log("Timelapse vollständig abgeschlossen.");
            }
            catch (TaskCanceledException)
            {
                Log("Timelapse wurde manuell gestoppt.");
            }
            catch (Exception ex)
            {
                Log($"Kritischer Fehler in der Timelapse-Schleife: {ex.Message}");
            }
            finally
            {
                tlCts?.Dispose();
                tlCts = null;
                btnStartTL.Enabled = true;
                btnStopTL.Enabled = false;
            }
        }

        private void btnStopTL_Click(object sender, EventArgs e)
        {
            if (tlCts != null)
            {
                Log("Stoppe Timelapse...");
                tlCts.Cancel();
            }
        }

        private async Task RunTimelapseAsync(TLScript script, CancellationToken token)
        {
            // --- NEU: Warten auf geplante Startzeit ---
            if (script.StartTime.HasValue)
            {
                TimeSpan waitTime = script.StartTime.Value - DateTime.Now;
                if (waitTime.TotalMilliseconds > 0)
                {
                    Log($"⏳ Geplanter Start aktiviert. Warte bis {script.StartTime.Value:dd.MM.yyyy HH:mm:ss} Uhr...");
                    await Task.Delay(waitTime, token); // Blockiert nicht das UI, wartet auf die Sekunde genau
                    Log("🚀 Geplante Startzeit erreicht! Beginne mit dem Timelapse...");
                }
                else
                {
                    Log("⚠️ Startzeit liegt in der Vergangenheit. Timelapse startet sofort.");
                }
            }

            // --- Normale Timelapse Schleife ---
            for (int i = 0; i < script.Steps.Count; i++)
            {
                token.ThrowIfCancellationRequested();
                var step = script.Steps[i];

                if (step.WartezeitVorMs > 0)
                {
                    Log($"TL {i + 1}/{script.Steps.Count}: Wartezeit VOR Auslösung ({step.WartezeitVorMs}ms) läuft...");
                    await Task.Delay(step.WartezeitVorMs, token);
                }

                Log($"TL {i + 1}: Passe Parameter an -> ISO: {step.IsoName}, Tv: {step.TvName}");

                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => ApplyTimelapseSettings(step.IsoHex, step.TvHex)));
                }
                else
                {
                    ApplyTimelapseSettings(step.IsoHex, step.TvHex);
                }

                await Task.Delay(400, token);

                Log($"TL {i + 1}: Auslösen!");
                TriggerShoot();

                if (step.IntervallNachMs > 0)
                {
                    Log($"TL {i + 1}: Intervall NACH Auslösung ({step.IntervallNachMs}ms) läuft...");
                    await Task.Delay(step.IntervallNachMs, token);
                }
            }
        }

        private TLScript ParseCsvFile(string filePath)
        {
            var script = new TLScript();
            var lines = File.ReadAllLines(filePath);
            int lineNum = 0;

            foreach (var line in lines)
            {
                lineNum++;
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                var parts = line.Split(';');

                // NEU: Abfangen der Startzeit
                if (parts[0].Trim().Equals("STARTTIME", StringComparison.OrdinalIgnoreCase))
                {
                    if (parts.Length > 1 && DateTime.TryParse(parts[1].Trim(), out DateTime parsedTime))
                    {
                        // Wenn der User nur "15:30:00" eingibt und die Uhrzeit heute schon rum ist, nimm morgen
                        if (parsedTime < DateTime.Now && !parts[1].Contains("."))
                        {
                            parsedTime = parsedTime.AddDays(1);
                        }
                        script.StartTime = parsedTime;
                        Log($"Zeile {lineNum}: Geplante Startzeit erkannt ({script.StartTime.Value:dd.MM.yyyy HH:mm:ss})");
                    }
                    else
                    {
                        Log($"Zeile {lineNum}: Ungültiges Startzeit-Format. Bitte z.B. 18:30:00 nutzen. Wird ignoriert.");
                    }
                    continue; // Diese Zeile war nur für die Zeit, überspringe den restlichen Bild-Code
                }

                if (parts.Length >= 4)
                {
                    try
                    {
                        var step = new TLStep();
                        step.WartezeitVorMs = int.Parse(parts[0].Trim());
                        step.IntervallNachMs = int.Parse(parts[1].Trim());

                        step.IsoName = parts[2].Trim();
                        step.TvName = parts[3].Trim();

                        step.IsoHex = GetHexFromName(cmbIso, step.IsoName);
                        step.TvHex = GetHexFromName(cmbTv, step.TvName);

                        script.Steps.Add(step);
                    }
                    catch (Exception ex)
                    {
                        Log($"Fehler in Zeile {lineNum} ('{line}'): {ex.Message}");
                    }
                }
            }
            return script;
        }

        private uint GetHexFromName(ComboBox cmb, string name)
        {
            foreach (SettingItem item in cmb.Items)
            {
                if (item.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return item.HexValue;
                }
            }
            throw new Exception($"Der Wert '{name}' existiert nicht in der Dropdown-Auswahl.");
        }

        // --- 6. AUFRÄUMEN BEIM BEENDEN ---
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Log("Programm wird geschlossen. Räume auf...");
            tlCts?.Cancel();
            SaveSettingsToXml();
            try
            {
                if (camera != null)
                {
                    try { CanonSDK.EdsSendCommand(camera.Reference, (CameraCommand)4, 0); } catch { }
                    camera.CloseSession();
                }
                if (api != null) api.Dispose();
            }
            catch { }
            base.OnFormClosing(e);
        }
    }

    // --- HILFSKLASSEN ---
    public class SettingItem
    {
        public string Name { get; set; }
        public uint HexValue { get; set; }
        public SettingItem(string name, uint hexValue) { Name = name; HexValue = hexValue; }
    }

    public class TLStep
    {
        public int WartezeitVorMs { get; set; }
        public int IntervallNachMs { get; set; }
        public string IsoName { get; set; }
        public string TvName { get; set; }
        public uint IsoHex { get; set; }
        public uint TvHex { get; set; }
    }

    // NEU: Hält sowohl die geplanten Bilder als auch die Startzeit
    public class TLScript
    {
        public DateTime? StartTime { get; set; }
        public List<TLStep> Steps { get; set; } = new List<TLStep>();
    }
}