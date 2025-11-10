# 📦 Instalare și Build - BEBE Task Recorder v3.0 (C#)

## ✅ Cerințe

### Pentru DEZVOLTARE (build-ul aplicației principale):
1. **.NET SDK 6.0 sau mai nou**
   - Download: https://dotnet.microsoft.com/download
   - Alege: ".NET 6.0 SDK" sau ".NET 8.0 SDK" (recomandat)
   - După instalare, verifică: `dotnet --version`

### Pentru UTILIZATORI FINALI (doar rulare exe):
- **NIMIC!** Executabilul include .NET runtime
- Doar Windows 10/11

## 🚀 Pași de Build

### Metoda 1: Automată (RECOMANDAT)

```
cmd
1. Dublu-click pe: RUN_ME_FIRST.bat
2. Acceptă instalarea .NET SDK dacă este necesar
3. Așteaptă build-ul (~30-60 secunde)
4. Gata! Executabilul este în: dist\BebeTaskRecorder.exe
```

### Metoda 2: Manuală

```
bash
# 1. Deschide Command Prompt în acest folder

# 2. Restorează dependențe
dotnet restore

# 3. Build
dotnet build -c Release

# 4. Publish (single-file exe)
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o dist

# 5. Executabilul va fi în:
#    dist\BebeTaskRecorder.exe
```

## 📊 Comparație cu versiunea Python

| Aspect | Python v3.0 | C# v3.0 |
|--------|-------------|---------|
| **Dimensiune exe** | 131 MB | ~15-25 MB |
| **Dependențe runtime** | Python + Tkinter + multe altele | .NET (preinstalat pe Win10/11) |
| **Export task ca EXE** | ❌ Necesită PyInstaller pe sistem | ✅ Funcționează direct! |
| **Viteză execuție** | Medie | Rapidă |
| **Erori Tk/Tcl** | Da | Nu |
| **GUI** | Tkinter (vechi) | WinForms (nativ Windows) |

## ✨ Features

- ✅ Înregistrare globală mouse + keyboard (F9/ESC pentru stop)
- ✅ Redare cu control viteză (0.1x - 10x)
- ✅ Loop și "Run until stop"
- ✅ Export JSON
- ✅ **Export EXE (FĂRĂ Python, FĂRĂ PyInstaller!)**
- ✅ Programare (zile și ore) - în taskurile exportate
- ✅ GUI nativ Windows
- ✅ Admin privileges auto

## 🎯 Cum funcționează "Save as EXE"

**Spre deosebire de Python + PyInstaller**, C# folosește o abordare MULT MAI SIMPLĂ:

1. **Generează cod C#** cu datele task-ului embedded
2. **Compilează direct cu `dotnet publish`**
3. **Rezultat**: Executabil standalone de 2-5 MB

**Avantaje**:
- ✅ Nu necesită PyInstaller
- ✅ Nu necesită Python pe sistem target
- ✅ Compilare rapidă (10-30 secunde vs 2-3 minute)
- ✅ Executabile mici și rapide
- ✅ Fără erori Tk/Tcl

**Dezavantaj**:
- ⚠️ Necesită .NET SDK pe sistemul unde CREEZI executabilele
- ✅ Executabilele GENERATE nu necesită nimic (self-contained)

## 🔧 Troubleshooting

### Build-ul eșuează cu "dotnet command not found"
```bash
# Instalează .NET SDK:
# https://dotnet.microsoft.com/download

# După instalare, închide și redeschide terminalul
dotnet --version  # Trebuie să afișeze versiunea
```

### "Save as EXE" dă eroare
```bash
# Verifică că .NET SDK este instalat:
dotnet --version

# Verifică că 'dotnet' este în PATH:
where dotnet  # Windows
```

### Executabilul este prea mare (>30 MB)
```bash
# Folosește PublishTrimmed pentru reducere:
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -o dist
```

## 📝 Notă pentru distribuție

Pentru a distribui aplicația către utilizatori:

1. **Build** cu: `.\build.bat`
2. **Distribuie**: `dist\BebeTaskRecorder.exe`
3. **Utilizatorii finali**:
   - Doar dublu-click și accept UAC (admin)
   - NU trebuie să instaleze nimic!
   - Pentru "Save as EXE", trebuie .NET SDK

## 🆚 De ce C# în loc de Python?

Versiunea Python avea probleme:
- PyInstaller necesită Python pe sistem pentru export EXE
- Erori complexe cu Tk/Tcl paths
- Executabile mari și lente
- Dependențe grele

C# rezolvă TOATE acestea:
- Native Windows, rapid, simplu
- Export EXE funcționează perfect
- Executabile mici și independente
- GUI nativ și profesional

---

**Succes!** 🚀

