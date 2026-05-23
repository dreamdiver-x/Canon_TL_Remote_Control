# Canon EOS R5 Remote Control App (NuGet Version)

Dieses Projekt ist eine saubere, moderne Windows Forms Anwendung (.NET Framework 4.8), die das fertige NuGet-Paket **EOSDigital.API** verwendet. 

Das bedeutet: Keine manuellen DLL-Verweise mehr in Visual Studio! Alles wird automatisch beim ersten Laden aus dem Internet gezogen und fehlerfrei konfiguriert.

## Der EINZIGE manuelle Schritt, den du tun musst:

Das NuGet-Paket enthält nur den C#-Wrapper, aber NICHT die originalen Kamera-Treiber von Canon (aus Lizenzgründen).

**So startest du das Projekt richtig:**

1. Öffne die `.sln` in Visual Studio 2022.
2. Gehe oben im Menü auf **Erstellen -> Projektmappe erstellen**.
3. Öffne deinen Windows-Explorer und navigiere in den Projektordner zu:
   `EOS_R5_RemoteGUI/bin/Debug/net48/`
4. Kopiere die nativen Canon-Dateien (`EDSDK.dll` und `EdsImage.dll` etc., die du von Canon heruntergeladen hast) genau in diesen `net48`-Ordner.
5. Drücke in Visual Studio auf **Starten**. 

Das Programm wird die DLLs nun finden und die EOS R5 erfolgreich verbinden!
