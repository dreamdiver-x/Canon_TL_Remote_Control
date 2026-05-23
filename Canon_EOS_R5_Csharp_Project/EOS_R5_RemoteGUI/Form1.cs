using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
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

        public Form1()
        {
            InitializeComponent();

            labelModus.Text = "Kamera Modus (Read-Only):";
            btnShoot.Enabled = false;
            cmbAEMode.Enabled = false;
            cmbIso.Enabled = false;
            cmbTv.Enabled = false;
            cmbDriveMode.Enabled = false;

            try { api = new CanonAPI(); }
            catch (Exception ex) { lblStatus.Text = "Fehler Treiber: " + ex.Message; }

            PopulateStaticDropdowns();
            LoadSettingsFromXml();
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
            cmbIso.Items.Add(new SettingItem("100", 0x48));
            cmbIso.Items.Add(new SettingItem("200", 0x50));
            cmbIso.Items.Add(new SettingItem("400", 0x58));
            cmbIso.Items.Add(new SettingItem("800", 0x60));
            cmbIso.Items.Add(new SettingItem("1600", 0x68));
            cmbIso.Items.Add(new SettingItem("3200", 0x70));
            cmbIso.Items.Add(new SettingItem("6400", 0x78));
            cmbIso.Items.Add(new SettingItem("12800", 0x80));
            cmbIso.Items.Add(new SettingItem("25600", 0x88));

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
                cmbTv.Items.Add(new SettingItem("1\"", 0x38));
                cmbTv.Items.Add(new SettingItem("1/30", 0x60));
                cmbTv.Items.Add(new SettingItem("1/60", 0x68));
                cmbTv.Items.Add(new SettingItem("1/125", 0x70));
                cmbTv.Items.Add(new SettingItem("1/250", 0x78));
                cmbTv.Items.Add(new SettingItem("1/500", 0x80));
                cmbTv.Items.Add(new SettingItem("1/1000", 0x88));
                cmbTv.Items.Add(new SettingItem("1/2000", 0x90));
                cmbTv.Items.Add(new SettingItem("1/4000", 0x98));
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
                    XDocument doc = XDocument.Load(settingsPath);
                    uint modeHex = uint.Parse(doc.Root.Element("AEModeHex")?.Value ?? "3");
                    uint isoHex = uint.Parse(doc.Root.Element("IsoHex")?.Value ?? "0");
                    uint tvHex = uint.Parse(doc.Root.Element("TvHex")?.Value ?? "16");
                    uint driveHex = uint.Parse(doc.Root.Element("DriveModeHex")?.Value ?? "0");

                    SelectDropdownItemByHex(cmbAEMode, modeHex);
                    SelectDropdownItemByHex(cmbIso, isoHex);
                    SelectDropdownItemByHex(cmbDriveMode, driveHex);

                    UpdateTvDropdown(modeHex);
                    SelectDropdownItemByHex(cmbTv, tvHex);
                }
            }
            catch { }
        }

        private void SelectDropdownItemByHex(ComboBox cmb, uint hexValue)
        {
            for (int i = 0; i < cmb.Items.Count; i++)
            {
                if (((SettingItem)cmb.Items[i]).HexValue == hexValue)
                {
                    cmb.SelectedIndex = i; return;
                }
            }
            if (cmb.Items.Count > 0) cmb.SelectedIndex = 0;
        }

        private void SaveSettingsToXml()
        {
            try
            {
                uint modeHex = cmbAEMode.SelectedItem != null ? ((SettingItem)cmbAEMode.SelectedItem).HexValue : 0x03;
                uint isoHex = cmbIso.SelectedItem != null ? ((SettingItem)cmbIso.SelectedItem).HexValue : 0x00;
                uint tvHex = cmbTv.SelectedItem != null ? ((SettingItem)cmbTv.SelectedItem).HexValue : 0x10;
                uint driveHex = cmbDriveMode.SelectedItem != null ? ((SettingItem)cmbDriveMode.SelectedItem).HexValue : 0x00;

                XDocument doc = new XDocument(new XElement("KameraEinstellungen",
                    new XElement("AEModeHex", modeHex), new XElement("IsoHex", isoHex),
                    new XElement("TvHex", tvHex), new XElement("DriveModeHex", driveHex)));
                doc.Save(settingsPath);
            }
            catch { }
        }

        private void cmbAEMode_SelectedIndexChanged(object sender, EventArgs e) { } // Leer lassen

        // --- 2. DIE VERBINDUNG (SD-Karten Modus ohne Zuhörer!) ---
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (isConnected || api != null)
            {
                lblStatus.Text = "Status: Trenne alte Verbindung / Bereinige Treiber...";
                btnShoot.Enabled = false;
                try
                {
                    if (camera != null) camera.CloseSession();
                    if (api != null) api.Dispose();
                }
                catch { }

                api = null; camera = null; isConnected = false;
                btnConnect.Text = "Kamera Verbinden";
                lblStatus.Text = "Status: Getrennt. Bereit für Neustart.";
                return;
            }

            try
            {
                lblStatus.Text = "Status: Starte Canon-Treiber neu...";
                api = new CanonAPI();
                List<Camera> camList = api.GetCameraList();

                if (camList.Count > 0)
                {
                    camera = camList[0];
                    camera.OpenSession();

                    // WICHTIG: 1 = Speichern NUR auf SD-Karte!
                    uint saveToPc = 1;
                    IntPtr saveToPtr = System.Runtime.InteropServices.Marshal.AllocHGlobal(sizeof(uint));
                    try
                    {
                        System.Runtime.InteropServices.Marshal.WriteInt32(saveToPtr, (int)saveToPc);
                        uint err = (uint)CanonSDK.EdsSetPropertyData(camera.Reference, (PropertyID)0x0000000B, 0, sizeof(uint), saveToPtr);
                        if (err == 0x8D)
                        {
                            lblStatus.Text = "Fehler: Kamera ist beschäftigt.";
                            camera.CloseSession();
                            return;
                        }
                    }
                    finally { System.Runtime.InteropServices.Marshal.FreeHGlobal(saveToPtr); }

                    CanonSDK.EdsSendStatusCommand(camera.Reference, (CameraStatusCommand)1, 0);

                    // Aktuellen Modus EINMALIG auslesen
                    try
                    {
                        uint currentMode = (uint)camera.GetInt32Setting((PropertyID)0x00000400);
                        SelectDropdownItemByHex(cmbAEMode, currentMode);
                        UpdateTvDropdown(currentMode);
                    }
                    catch { }

                    // HIER HABEN WIR ALLE "ZUHÖRER" GELÖSCHT! KEINE EVENTS = KEIN ABSTURZ!

                    isConnected = true;
                    btnConnect.Text = "Kamera Trennen / Reconnect";
                    lblStatus.Text = $"Status: {camera.DeviceName} bereit!";

                    btnShoot.Enabled = true;
                    cmbIso.Enabled = true;
                    cmbTv.Enabled = true;
                    cmbDriveMode.Enabled = true;
                }
                else
                {
                    lblStatus.Text = "Status: Keine Kamera gefunden.";
                    if (api != null) api.Dispose(); api = null;
                }
            }
            catch (Exception ex) { lblStatus.Text = "Fehler: " + ex.Message; }
        }

        // --- 4. KAMERAEINSTELLUNGEN ÄNDERN (Direkt, ohne Task.Run) ---
        private void cmbIso_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (camera != null && cmbIso.SelectedItem != null && cmbIso.Enabled)
            {
                try
                {
                    var item = (SettingItem)cmbIso.SelectedItem;
                    camera.SetSetting((PropertyID)0x00000402, item.HexValue);
                    lblStatus.Text = $"Status: ISO auf {item.Name} gesetzt.";
                }
                catch (Exception ex) { lblStatus.Text = $"Fehler ISO: {ex.Message}"; }
            }
        }

        private void cmbTv_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (camera != null && cmbTv.SelectedItem != null && cmbTv.Enabled)
            {
                try
                {
                    var item = (SettingItem)cmbTv.SelectedItem;
                    camera.SetSetting((PropertyID)0x00000406, item.HexValue);
                    lblStatus.Text = $"Status: Belichtungszeit auf {item.Name} gesetzt.";
                }
                catch (Exception ex) { lblStatus.Text = $"Fehler Tv: {ex.Message}"; }
            }
        }

        private void cmbDriveMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (camera != null && cmbDriveMode.SelectedItem != null && cmbDriveMode.Enabled)
            {
                try
                {
                    var item = (SettingItem)cmbDriveMode.SelectedItem;
                    camera.SetSetting((PropertyID)0x00000401, item.HexValue);
                    lblStatus.Text = $"Status: Auslösemodus auf {item.Name} gesetzt.";
                }
                catch (Exception ex) { lblStatus.Text = $"Fehler DriveMode: {ex.Message}"; }
            }
        }

        // --- 5. FOTO AUSLÖSEN ---
        private void btnShoot_Click(object sender, EventArgs e)
        {
            if (camera != null)
            {
                try
                {
                    lblStatus.Text = "Status: Kamera löst aus...";
                    CanonSDK.EdsSendCommand(camera.Reference, (CameraCommand)0, 0);
                    lblStatus.Text = "Status: 📸 Foto sicher auf SD-Karte gespeichert!";
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("TAKE_PICTURE_AF_NG"))
                        lblStatus.Text = "Fehler: Autofokus fehlgeschlagen! (Zu dunkel/nah).";
                    else
                        lblStatus.Text = $"Fehler beim Auslösen: {ex.Message}";
                }
            }
        }

        // --- 6. AUFRÄUMEN BEIM BEENDEN ---
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
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

    // --- HILFSKLASSE ---
    public class SettingItem
    {
        public string Name { get; set; }
        public uint HexValue { get; set; }
        public SettingItem(string name, uint hexValue) { Name = name; HexValue = hexValue; }
    }
}