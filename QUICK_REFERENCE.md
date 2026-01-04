# 🚀 Keypass - Quick Reference Card

## Start Here! 👇

### Run the App (30 seconds)
```
Navigate to: d:\Program Files\Code\Keypass\publish\
Double-click: Keypass.exe
Done! ✓
```

---

## 📚 Documentation Quick Links

| Document | Purpose | For Whom |
|----------|---------|----------|
| **QUICK_START.md** | 5-minute setup guide | Everyone |
| **README.md** | General overview & features | Users |
| **HUONG_DAN_VIETNAMESE.md** | Complete Vietnamese guide | Vietnamese users |
| **BUILD_GUIDE.md** | Building from source | Developers |
| **PROJECT_SUMMARY.md** | Project overview | Technical leads |

---

## 🎯 First Time Steps

### 1️⃣ Run Application
```
✓ Open publish folder
✓ Double-click Keypass.exe
✓ See tray icon appear
```

### 2️⃣ Add First Credential
```
✓ Visit any login page
✓ Enter username & password
✓ Press Enter or click Login
✓ Click "Yes" when asked to save
```

### 3️⃣ Test Auto-Fill
```
✓ Visit same login page again
✓ Click username field
✓ Select credential from popup
✓ Username & password auto-fill
```

### 4️⃣ Adjust Settings
```
✓ Right-click tray icon
✓ Click "Settings"
✓ Enable auto-fill & auto-save
✓ Click "Save"
```

---

## 🖱️ Main Menu Commands

### Right-Click Tray Icon
```
📌 Manage Passwords    → Open credential manager
⚙️  Settings           → Configure options
════════════════════════════════════════════
❌ Exit               → Close application
```

### Manage Passwords Window
```
🔍 Search Box         → Find credentials
➕ Add               → Add new credential
✏️  Edit             → Modify selected credential
🗑️  Delete           → Remove credential
```

---

## 🔒 Where Data is Stored

### Credentials Database
```
Location: C:\Users\[YourName]\AppData\Roaming\Keypass\
File: credentials.db
Type: SQLite Database
```

### Settings File
```
Location: C:\Users\[YourName]\AppData\Roaming\Keypass\
File: settings.json
Type: JSON Configuration
```

**Note:** Back up these files regularly!

---

## ⚙️ Settings Explained

| Setting | What It Does | Default |
|---------|-------------|---------|
| **Enable Auto-Fill** | Show credential suggestions | ✓ ON |
| **Ask to save** | Prompt when new credentials | ✓ ON |
| **Run on Startup** | Launch with Windows | ☐ OFF |

---

## ⌨️ Keyboard Shortcuts

| Key | Action |
|-----|--------|
| `Enter` | Confirm dialog / Apply settings |
| `Esc` | Close dialog / Cancel |
| `Ctrl+F` | Focus search box (in Manager) |
| `Ctrl+A` | Select all in search |

---

## 🆘 Common Issues & Quick Fixes

### Issue: "App won't start"
**Fix:** 
1. Check .NET 6.0 installed: `dotnet --version`
2. Try running as Administrator
3. Restart computer

### Issue: "No auto-fill popup"
**Fix:**
1. Check "Enable Auto-Fill" in Settings
2. Run as Administrator
3. Some websites may need manual entry

### Issue: "Can't find saved credentials"
**Fix:**
1. Use Search box to find by website name
2. Check settings aren't too restrictive
3. Verify credentials were saved (check popup)

### Issue: "Database errors"
**Fix:**
1. Close all instances of app
2. Delete `credentials.db` file
3. Restart app (creates new database)
4. Re-add credentials

---

## 📱 Mobile Integration

### Currently Not Supported
❌ iPhone/iPad
❌ Android
❌ Cloud Sync

### Workaround
- Manual backup of `credentials.db`
- Copy between computers (advanced users)
- Consider future cloud version

---

## 🛡️ Security Tips

### ✅ DO:
- ✓ Use strong Windows password
- ✓ Keep computer locked when away
- ✓ Run Windows Defender/Antivirus
- ✓ Backup credentials.db regularly
- ✓ Use admin account for setup

### ❌ DON'T:
- ✗ Share credentials database
- ✗ Leave computer unlocked
- ✗ Install from untrusted sources
- ✗ Disable Windows security
- ✗ Ignore security warnings

---

## 🔧 Build Commands (For Developers)

```bash
# Build Debug
dotnet build

# Build Release
dotnet build -c Release

# Run Application
dotnet run

# Publish for Distribution
dotnet publish -c Release -o ./publish

# Clean Build
dotnet clean
dotnet build -c Release
```

---

## 📊 Application Size

| Component | Size |
|-----------|------|
| Keypass.exe | 14 KB |
| Main DLL | 20 KB |
| SQLite | 410 KB |
| Total with deps | ~10-15 MB |
| With Runtime | ~25-30 MB |

---

## 🌐 Supported Platforms

### ✅ Windows Versions
- Windows 7 SP1+
- Windows 8.x
- Windows 10 (all versions)
- Windows 11

### ✅ System Requirements
- .NET 6.0 Runtime
- 100 MB disk space minimum
- 100 MB RAM minimum
- Internet connection (optional)

---

## 📞 Getting Help

### Step 1: Documentation
Read the appropriate guide:
- English: README.md
- Vietnamese: HUONG_DAN_VIETNAMESE.md
- Build: BUILD_GUIDE.md

### Step 2: Troubleshooting
Check "Known Limitations" in docs

### Step 3: Check Logs
Review Event Viewer for errors

### Step 4: Advanced
Examine source code in `src/` folder

---

## 🎯 Version Info

```
App Name: Keypass Password Manager
Version: 1.0.0
Built: January 3, 2026
Framework: .NET 6.0 Windows Forms
Language: C# 10.0
Database: SQLite
```

---

## 🔄 Update Path

### From 1.0.0
When version 2.0 releases:
1. Save backup of `credentials.db`
2. Download new version
3. Run new executable
4. Old data usually compatible

---

## 📋 Checklist for First-Time Users

- [ ] Downloaded and ran Keypass.exe
- [ ] Verified system tray icon appears
- [ ] Added first credential
- [ ] Tested auto-fill feature
- [ ] Adjusted settings
- [ ] Backed up credentials.db
- [ ] Enabled "Run on Startup" (optional)
- [ ] Bookmarked documentation
- [ ] Comfortable with basic operations

---

## 🎓 Tips for Power Users

### Naming Convention for Websites
Use consistent naming:
```
❌ Bad:  "facebook", "FB", "face book"
✓ Good: "facebook.com", "gmail.com", "dropbox.com"
```

### Organizing Credentials
```
Good practice:
- Work credentials in notes
- Personal credentials grouped
- Legacy accounts marked
- Inactive accounts noted
```

### Backup Strategy
```
Monthly: Backup credentials.db
- Keep on external drive
- Keep in secure cloud (encrypted)
- Multiple copies (2-3)
```

---

## 🚀 Advanced Features (Future)

Coming in later versions:
- 🔐 Master password
- 📱 Mobile sync
- 🌐 Browser extension
- 🔑 Password generator
- 📊 Breach notification
- ☁️ Cloud backup

---

## ❤️ Support The Project

### Help Us Improve
- Report bugs with details
- Suggest new features
- Share feedback
- Recommend to friends

### Show Appreciation
- ⭐ Star the project
- 💬 Leave feedback
- 🐛 Report issues
- 📣 Spread the word

---

## 📄 Quick Links

| Link | Description |
|------|-------------|
| See README.md | Full feature list |
| See BUILD_GUIDE.md | Installation details |
| See HUONG_DAN_VIETNAMESE.md | Complete guide (Vietnamese) |
| See DEPLOYMENT_CHECKLIST.md | Project status |

---

**Last Updated:** January 3, 2026  
**Status:** ✅ Ready to Use  
**Version:** 1.0.0

**Questions? Check the documentation files above! 📚**

---

## 🎉 Enjoy Keypass Password Manager!

*Secure, Simple, and Smart Password Management for Windows*

