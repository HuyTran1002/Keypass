# Keypass - Password Manager

Keypass is a C# Windows application that runs in the system tray and helps you manage your login credentials securely. It automatically detects login forms and suggests saved credentials for auto-fill.

## Features

- 🔒 **Secure Credential Storage**: SQLite database to store usernames and passwords
- 📌 **System Tray Integration**: Runs in background with system tray icon
- 🔍 **Login Form Detection**: Monitors when you access login pages/forms
- 💾 **Auto-Save**: Asks to save new credentials when detected
- 📋 **Auto-Fill**: Suggests saved credentials when login forms are detected
- ⚙️ **Settings**: Customize auto-fill and auto-save behavior
- 🔐 **Credential Manager**: Add, edit, and delete saved credentials

## Requirements

- Windows 7 or later
- .NET 6.0 Runtime or later
- Administrator privileges (recommended for optimal form detection)

## Installation

1. Download the latest release of Keypass
2. Extract the files to a desired location
3. Run `Keypass.exe`
4. The application will appear in your system tray

## Building from Source

### Prerequisites
- Visual Studio 2022 or later (with .NET 6.0 SDK)
- Or .NET 6.0 SDK command-line tools

### Build Steps

```bash
dotnet build
dotnet publish -c Release -o ./publish
```

## Usage

1. **Launch the Application**: Double-click `Keypass.exe` or find it in your system tray
2. **Add Credentials**: Click "Manage Passwords" → Click "Add" button → Enter website, username, and password
3. **Auto-Fill**: When you visit a login page:
   - Click on the username/password field
   - Keypass will suggest saved credentials for that website
   - Select the credential you want to use
   - Credentials will auto-fill, just press Enter to login
4. **Manage Credentials**: In the Password Manager:
   - Search for credentials by website name
   - Edit existing credentials
   - Delete credentials you no longer need

## Settings

Access Settings through the system tray icon to:
- **Enable/Disable Auto-Fill**: Turn credential suggestions on or off
- **Ask to Save**: Control whether to prompt when new credentials are entered
- **Run on Startup**: Auto-launch Keypass when Windows starts

## Database Location

Credentials are stored in: `%APPDATA%\Keypass\credentials.db`
Settings are stored in: `%APPDATA%\Keypass\settings.json`

## Security Notes

- Credentials are stored locally on your computer
- Consider using Windows DPAPI for additional encryption in future versions
- Keep your computer secure and run antivirus software regularly
- Do not share your credentials database with others

## Project Structure

```
Keypass/
├── src/
│   ├── Program.cs                 # Application entry point
│   ├── Models/
│   │   └── Credential.cs         # Credential data model
│   ├── Services/
│   │   ├── DatabaseService.cs    # SQLite database operations
│   │   ├── UIHookService.cs      # Form/UI monitoring
│   │   └── SettingsService.cs    # Settings management
│   └── UI/
│       ├── TrayApplicationContext.cs  # System tray integration
│       ├── PasswordManagerForm.cs     # Main credential manager UI
│       ├── AddEditPasswordForm.cs     # Add/edit credential form
│       └── SettingsForm.cs            # Settings UI
├── Keypass.csproj                 # Project configuration
└── README.md                      # This file
```

## Future Enhancements

- Support for browser extensions
- Password strength indicator
- Automatic password generator
- Master password protection
- Sync across devices
- Support for other applications (email, FTP, etc.)
- Import/Export credentials
- Biometric authentication

## Troubleshooting

**App doesn't detect login forms:**
- Run as Administrator
- Check that "Auto-Fill" is enabled in Settings
- Some applications may not be compatible with the form detection

**Database errors:**
- Ensure you have write permissions to `%APPDATA%\Keypass\`
- Check your antivirus isn't blocking database operations
- Try deleting the database file and restarting the app

**Credentials won't auto-fill:**
- Check the website name matches exactly in your saved credentials
- Some websites with custom input methods may not support auto-fill
- Try manually selecting the credential from the suggestion popup

## License

This project is provided as-is for personal use.

## Support

For issues or feature requests, please create an issue in the repository.

---

**Note**: This is a demonstration project. For production use, implement additional security measures like:
- DPAPI encryption for stored credentials
- Master password authentication
- Secure memory handling
- Regular security audits
