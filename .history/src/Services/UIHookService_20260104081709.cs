using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using Keypass.Models;

namespace Keypass.Services
{
    public static class UIHookService
    {
        private static IntPtr keyboardHookHandle;
        private static LowLevelKeyboardProc keyboardProc;
        private static IntPtr mouseHookHandle;
        private static LowLevelMouseProc mouseProc;
        private static string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Keypass", "debug.log");
        
        // Store credentials being typed
        private static string capturedUsername = "";
        private static string capturedPassword = "";
        private static string lastWebsite = "";
        private static int fieldCounter = 0; // Count which field is being entered (1=username, 2=password)
        private static string lastLoggedWindow = ""; // Track last logged window to reduce spam
        private static bool suggestionShownForWindow = false;
        private static IntPtr lastLoginWindowHandle = IntPtr.Zero;
        private static bool suppressCapture = false; // Skip capturing while we inject keys
        private static bool isSaveDialogOpen = false; // Track if save dialog is open to prevent mouse hook triggering
        private static SuggestionPopup activePopup = null; // Track currently open popup to prevent duplicates
        private static bool usernameFieldActive = false; // Track if cursor is in username field (detected by keyboard input)
        private static bool inMultiPageLogin = false; // Track if we're in multi-page login flow (e.g., Google)
        
        private static List<string> loginKeywords = new List<string> 
        { 
            "login", "sign in", "password", "user", "email", "username", 
            "riot", "valorant", "lol", "league", "sign up", "register",
            "account", "auth", "authenticate", "credentials", "play", "client",
            "signin", "logon", "connect", "facebook", "fb", "meta",
            "google", "youtube", "gmail"
        };

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        private static extern short GetKeyState(int vKey);

        [DllImport("user32.dll")]
        private static extern int ToUnicode(uint wVirtKey, uint wScanCode, byte[] lpKeyState, 
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)] StringBuilder pwszBuff, int cchBuff, uint wFlags);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        // Virtual Key Codes
        private const int VK_RETURN = 0x0D;      // Enter key
        private const int VK_BACK = 0x08;        // Backspace
        private const int VK_SPACE = 0x20;       // Space
        private const int VK_TAB = 0x09;         // Tab
        private const int WH_KEYBOARD_LL = 13;  // Low level keyboard hook
        private const int WH_MOUSE_LL = 14;     // Low level mouse hook
        private const int WM_KEYDOWN = 0x0100;  // Key down event
        private const int WM_LBUTTONDOWN = 0x0201; // Mouse left button down
        private const int SW_SHOWNOACTIVATE = 4;
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_NOZORDER = 0x0004;
        private static readonly IntPtr HWND_TOP = new IntPtr(0);

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static void Log(string message)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                string logMessage = $"[{timestamp}] {message}";
                Debug.WriteLine(logMessage);
                File.AppendAllText(logPath, logMessage + Environment.NewLine);
            }
            catch { }
        }

        public static void Initialize()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                File.WriteAllText(logPath, $"=== Keypass Debug Log Started at {DateTime.Now} ==={Environment.NewLine}");
                Log("UIHookService Initialize() called");
                
                keyboardProc = KeyboardHookCallback;
                IntPtr ptrModule = GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
                Log($"Module handle: {ptrModule}");
                
                keyboardHookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, keyboardProc, ptrModule, 0);
                Log($"Hook handle: {keyboardHookHandle}");
                
                if (keyboardHookHandle == IntPtr.Zero)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    Log($"✗ Failed to install keyboard hook. Error code: {errorCode}");
                }
                else
                {
                    Log("✓ UIHookService initialized - Keyboard hook active");
                }

                mouseProc = MouseHookCallback;
                mouseHookHandle = SetWindowsHookEx(WH_MOUSE_LL, mouseProc, ptrModule, 0);
                Log($"Mouse hook handle: {mouseHookHandle}");

                if (mouseHookHandle == IntPtr.Zero)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    Log($"✗ Failed to install mouse hook. Error code: {errorCode}");
                }
                else
                {
                    Log("✓ Mouse hook active");
                }
            }
            catch (Exception ex)
            {
                Log($"✗ Error initializing UI hook: {ex.Message}");
                Log($"   Stack trace: {ex.StackTrace}");
            }
        }

        private static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            Log($"[HOOK CALLBACK] nCode={nCode}, wParam={wParam}");
            
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                try
                {
                    int vkCode = Marshal.ReadInt32(lParam);
                    IntPtr windowHandle = GetForegroundWindow();
                    string windowTitle = GetWindowTitleSafe(windowHandle);

                    // Check if we're in a login form OR continuing multi-page login
                    bool isLoginForm = IsLoginForm(windowTitle) || inMultiPageLogin;

                    // Log window changes for debugging
                    if (windowTitle != lastLoggedWindow)
                    {
                        Log($"[WINDOW] {windowTitle} (isLoginForm: {isLoginForm})");
                        lastLoggedWindow = windowTitle;
                        
                        // Reset suggestion flag when switching away from login window
                        // BUT preserve username if we're moving between login pages (e.g., Google email → password)
                        if (!isLoginForm && suggestionShownForWindow)
                        {
                            Log("[RESET] User left login window - resetting suggestion flag");
                            suggestionShownForWindow = false;
                            usernameFieldActive = false;
                            // Only reset multi-page if we don't have captured credentials waiting
                            if (string.IsNullOrEmpty(capturedUsername) || !string.IsNullOrEmpty(capturedPassword))
                            {
                                inMultiPageLogin = false;
                            }
                        }
                        
                        // If moving to another login page and we have username but no password,
                        // keep the username (Google-style multi-page login)
                        if (isLoginForm && !string.IsNullOrEmpty(capturedUsername) && string.IsNullOrEmpty(capturedPassword))
                        {
                            Log("[MULTI-PAGE] Keeping captured username for next login page");
                            fieldCounter = 2; // Next input will be password
                            inMultiPageLogin = true; // Enable multi-page login mode
                        }
                    }

                    if (isLoginForm)
                    {
                        lastWebsite = windowTitle;
                        lastLoginWindowHandle = windowHandle;

                        // Skip capturing our own injected keys; only allow Enter to submit
                        if (suppressCapture && vkCode != VK_RETURN)
                        {
                            return (IntPtr)0;
                        }

                        // Show suggestions ONLY when user starts typing in username field
                        // First character input triggers the popup (not just click)
                        if (usernameFieldActive && !suggestionShownForWindow && (activePopup == null || activePopup.IsDisposed))
                        {
                            // Only show on first character, not on special keys like Tab/Enter
                            char keyChar = GetCharFromVKey(vkCode);
                            if (!char.IsControl(keyChar) && keyChar != '\0')
                            {
                                suggestionShownForWindow = true;
                                Log("[SUGGEST] First keyboard input detected in login form - showing suggestions");
                                ShowSuggestions(windowTitle);
                                // Allow first character to be captured normally - don't skip it
                            }
                        }

                        // Detect Enter key - user submitted form
                        if (vkCode == VK_RETURN)
                        {
                            Log($"✓ [ENTER] detected on: {windowTitle}");
                            Log($"  Username: '{capturedUsername}', Password: {'*' * capturedPassword.Length}");
                            
                            // For multi-page login (Google), only save when we have BOTH username and password
                            if (!string.IsNullOrEmpty(capturedUsername) && !string.IsNullOrEmpty(capturedPassword))
                            {
                                // Save credentials before resetting
                                string savedUsername = capturedUsername;
                                string savedPassword = capturedPassword;
                                string savedWebsite = windowTitle;
                                
                                ResetInput();
                                
                                // Call with saved values
                                OnFormSubmitted(savedWebsite, savedUsername, savedPassword);
                            }
                            else
                            {
                                Log("[SKIP SAVE] Incomplete credentials - waiting for full login");
                            }
                            return (IntPtr)1; // Prevent further processing
                        }

                        // Detect Tab key - moving to next field (username -> password)
                        if (vkCode == VK_TAB)
                        {
                            if (fieldCounter == 1 && capturedUsername.Length > 0)
                            {
                                Log($"✓ [TAB] Username field completed: '{capturedUsername}'");
                                fieldCounter = 2;
                            }
                            return (IntPtr)0;
                        }

                        // Detect Backspace - remove last character
                        if (vkCode == VK_BACK)
                        {
                            if (fieldCounter == 1 && capturedUsername.Length > 0)
                                capturedUsername = capturedUsername.Substring(0, capturedUsername.Length - 1);
                            else if (fieldCounter == 2 && capturedPassword.Length > 0)
                                capturedPassword = capturedPassword.Substring(0, capturedPassword.Length - 1);
                            return (IntPtr)0;
                        }

                        // Capture printable characters
                        char keyChar2 = GetCharFromVKey(vkCode);
                        if (!char.IsControl(keyChar2) && keyChar2 != '\0')
                        {
                            // If first input and no field counter set, assume username
                            if (fieldCounter == 0)
                                fieldCounter = 1;

                            // Append to appropriate field
                            if (fieldCounter == 1)
                            {
                                capturedUsername += keyChar2;
                                Log($"  → Username: {capturedUsername}");
                            }
                            else if (fieldCounter == 2)
                            {
                                capturedPassword += keyChar2;
                                Log($"  → Password: {'*' * capturedPassword.Length}");
                            }
                        }
                    }
                    else
                    {
                        // Not in login form, but check for Enter key to trigger save on any form
                        if (vkCode == VK_RETURN && fieldCounter > 0 && (capturedUsername.Length > 0 || capturedPassword.Length > 0))
                        {
                            Log($"✓ [ENTER on non-login window] Window: {windowTitle}");
                            Log($"  Username: '{capturedUsername}', Password: {'*' * capturedPassword.Length}");
                            
                            // Save credentials before resetting
                            string savedUsername = capturedUsername;
                            string savedPassword = capturedPassword;
                            string savedWebsite = windowTitle;
                            
                            ResetInput();
                            OnFormSubmitted(savedWebsite, savedUsername, savedPassword);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log($"✗ Keyboard hook error: {ex.Message}");
                }
            }

            return CallNextHookEx(keyboardHookHandle, nCode, wParam, lParam);
        }

        private static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONDOWN)
            {
                try
                {
                    // Skip mouse hook if save dialog is open to prevent re-triggering suggestions
                    if (isSaveDialogOpen)
                    {
                        return CallNextHookEx(mouseHookHandle, nCode, wParam, lParam);
                    }

                    IntPtr windowHandle = GetForegroundWindow();
                    string windowTitle = GetWindowTitleSafe(windowHandle);
                    bool isLoginForm = IsLoginForm(windowTitle) || inMultiPageLogin;

                    if (windowTitle != lastLoggedWindow)
                    {
                        Log($"[MOUSE CLICK] {windowTitle} (isLoginForm: {isLoginForm})");
                        lastLoggedWindow = windowTitle;
                        
                        // Reset suggestion flag when switching away from login window
                        // BUT preserve username if moving between login pages
                        if (!isLoginForm && suggestionShownForWindow)
                        {
                            Log("[MOUSE RESET] User left login window - resetting suggestion flag");
                            suggestionShownForWindow = false;
                            usernameFieldActive = false;
                            // Only reset multi-page if we don't have captured credentials waiting
                            if (string.IsNullOrEmpty(capturedUsername) || !string.IsNullOrEmpty(capturedPassword))
                            {
                                inMultiPageLogin = false;
                            }
                        }
                        
                        // If moving to another login page and we have username but no password,
                        // keep the username (Google-style multi-page login)
                        if (isLoginForm && !string.IsNullOrEmpty(capturedUsername) && string.IsNullOrEmpty(capturedPassword))
                        {
                            Log("[MOUSE MULTI-PAGE] Keeping captured username for next login page");
                            fieldCounter = 2; // Next input will be password
                            inMultiPageLogin = true; // Enable multi-page login mode
                        }
                    }

                    if (isLoginForm)
                    {
                        lastWebsite = windowTitle;
                        lastLoginWindowHandle = windowHandle;
                        
                        // Mark that user clicked on login form - suggestions will show when they type
                        Log("[MOUSE] Click on login form detected - waiting for keyboard input");
                        usernameFieldActive = true;
                    }
                }
                catch (Exception ex)
                {
                    Log($"[MOUSE HOOK ERROR] {ex.Message}");
                }
            }

            return CallNextHookEx(mouseHookHandle, nCode, wParam, lParam);
        }

        private static string GetWindowTitleSafe(IntPtr hwnd)
        {
            try
            {
                StringBuilder sb = new StringBuilder(256);
                GetWindowText(hwnd, sb, 256);
                return sb.ToString().ToLower();
            }
            catch
            {
                return "";
            }
        }

        private static bool IsLoginForm(string windowTitle)
        {
            if (string.IsNullOrEmpty(windowTitle))
                return false;

            // Ignore our own Keypass dialogs/popups to prevent self-triggered suggestions
            if (windowTitle.Contains("keypass"))
                return false;

            foreach (var keyword in loginKeywords)
            {
                if (windowTitle.Contains(keyword))
                {
                    return true;
                }
            }

            return false;
        }

        private static void OnFormSubmitted(string website, string username, string password)
        {
            try
            {
                // User pressed Enter on login form
                // For multi-step login (Google, etc.), only save when we have both username AND password
                if (username.Length > 0 && password.Length > 0)
                {
                    Log($"✓ [FORM SUBMITTED] Website: {website}");
                    Log($"  → Username: {username}");
                    Log($"  → Password: {password}");

                    // Show save dialog after a short delay
                    System.Windows.Forms.Timer delayTimer = new System.Windows.Forms.Timer();
                    delayTimer.Interval = 300;
                    delayTimer.Tick += (s, e) =>
                    {
                        delayTimer.Stop();
                        ShowSaveCredentialDialog(website, username, password);
                    };
                    delayTimer.Start();
                }
                else
                {
                    Log($"[FORM SKIP] Incomplete credentials - Username: '{username}', Password: {'*' * password.Length}");
                }
            }
            catch (Exception ex)
            {
                Log($"✗ Error in OnFormSubmitted: {ex.Message}");
            }
        }

        private static void ShowSaveCredentialDialog(string website, string username, string password)
        {
            try
            {
                // Check if this credential already exists in database
                if (DatabaseService.CredentialExists(website, username, password))
                {
                    Log($"✓ [SKIP SAVE] Credential already exists: {username} for {website}");
                    isSaveDialogOpen = false;
                    return;
                }

                isSaveDialogOpen = true;
                Log($"✓ [POPUP] Asking user to save credentials for: {website}");
                Log($"  → Username: {username}");
                Log($"  → Password: {password}");

                // Create a TopMost form for the dialog
                Form dialogForm = new Form
                {
                    Text = "Keypass - Save Credentials",
                    Width = 450,
                    Height = 200,
                    StartPosition = FormStartPosition.CenterScreen,
                    TopMost = true,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    ShowIcon = true,
                    Icon = System.Drawing.SystemIcons.Question
                };

                // Add label with message
                Label messageLabel = new Label
                {
                    Text = $"Save credentials for:\n{website}\n\nUsername: {username}",
                    Location = new System.Drawing.Point(20, 20),
                    Width = 410,
                    Height = 90,
                    AutoSize = false
                };
                dialogForm.Controls.Add(messageLabel);

                // Add Yes button
                Button yesButton = new Button
                {
                    Text = "Yes",
                    Location = new System.Drawing.Point(150, 120),
                    Width = 80,
                    DialogResult = DialogResult.Yes
                };
                dialogForm.Controls.Add(yesButton);

                // Add No button
                Button noButton = new Button
                {
                    Text = "No",
                    Location = new System.Drawing.Point(240, 120),
                    Width = 80,
                    DialogResult = DialogResult.No
                };
                dialogForm.Controls.Add(noButton);

                dialogForm.AcceptButton = yesButton;
                dialogForm.CancelButton = noButton;

                DialogResult result = dialogForm.ShowDialog();

                if (result == DialogResult.Yes)
                {
                    Log($"✓ User chose YES - Saving credentials");
                    
                    try
                    {
                        // Extract domain from website title
                        string domain = ExtractDomain(website);
                        
                        // Create new credential
                        var credential = new Credential
                        {
                            Website = domain,
                            Username = username,
                            Password = password,
                            Notes = "Auto-saved from login",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };

                        // Save to database
                        DatabaseService.SaveCredential(credential);
                        Log($"✓ Credential saved successfully to database");

                        // Show success dialog
                        Form successForm = new Form
                        {
                            Text = "Keypass",
                            Width = 350,
                            Height = 150,
                            StartPosition = FormStartPosition.CenterScreen,
                            TopMost = true,
                            FormBorderStyle = FormBorderStyle.FixedDialog,
                            MaximizeBox = false,
                            MinimizeBox = false,
                            ShowIcon = true,
                            Icon = System.Drawing.SystemIcons.Information
                        };

                        Label successLabel = new Label
                        {
                            Text = $"✓ Credentials for {domain} saved!",
                            Location = new System.Drawing.Point(20, 20),
                            Width = 310,
                            Height = 50,
                            AutoSize = false
                        };
                        successForm.Controls.Add(successLabel);

                        Button okButton = new Button
                        {
                            Text = "OK",
                            Location = new System.Drawing.Point(140, 80),
                            Width = 70,
                            DialogResult = DialogResult.OK
                        };
                        successForm.Controls.Add(okButton);
                        successForm.AcceptButton = okButton;
                        successForm.ShowDialog();
                        successForm.Dispose();
                    }
                    catch (Exception saveEx)
                    {
                        Log($"✗ Error saving credential: {saveEx.Message}");
                        
                        Form errorForm = new Form
                        {
                            Text = "Keypass - Error",
                            Width = 400,
                            Height = 150,
                            StartPosition = FormStartPosition.CenterScreen,
                            TopMost = true,
                            FormBorderStyle = FormBorderStyle.FixedDialog,
                            MaximizeBox = false,
                            MinimizeBox = false,
                            ShowIcon = true,
                            Icon = System.Drawing.SystemIcons.Error
                        };

                        Label errorLabel = new Label
                        {
                            Text = $"Error saving credentials:\n{saveEx.Message}",
                            Location = new System.Drawing.Point(20, 20),
                            Width = 360,
                            Height = 60,
                            AutoSize = false
                        };
                        errorForm.Controls.Add(errorLabel);

                        Button okErrorButton = new Button
                        {
                            Text = "OK",
                            Location = new System.Drawing.Point(165, 90),
                            Width = 70,
                            DialogResult = DialogResult.OK
                        };
                        errorForm.Controls.Add(okErrorButton);
                        errorForm.AcceptButton = okErrorButton;
                        errorForm.ShowDialog();
                        errorForm.Dispose();
                    }
                }
                else
                {
                    Log($"✓ User chose NO - credentials not saved");
                }

                dialogForm.Dispose();
            }
            catch (Exception ex)
            {
                Log($"✗ Error showing save dialog: {ex.Message}");
            }
            finally
            {
                isSaveDialogOpen = false;
            }
        }

        private static string ExtractDomain(string windowTitle)
        {
            // Extract meaningful domain name from window title
            // Remove common prefixes and just get the main part
            string domain = windowTitle
                .Replace("login", "")
                .Replace("sign in", "")
                .Replace("riot", "riot")
                .Replace("valorant", "valorant")
                .Replace("lol", "lol")
                .Replace("league", "league")
                .Trim();
            
            return string.IsNullOrEmpty(domain) ? windowTitle : domain;
        }

        private static char GetCharFromVKey(int vkCode)
        {
            try
            {
                // Ignore modifier keys
                if (vkCode == 0x10 || vkCode == 0x11 || vkCode == 0x12) // Shift, Ctrl, Alt
                    return '\0';

                // Get keyboard state
                byte[] keyboardState = new byte[256];
                for (int i = 0; i < 256; i++)
                {
                    keyboardState[i] = (byte)((GetKeyState(i) & 0x8000) != 0 ? 0x80 : 0);
                }

                // Get scan code
                uint scanCode = MapVirtualKey((uint)vkCode, 0);

                // Convert to Unicode
                StringBuilder sb = new StringBuilder(64);
                int result = ToUnicode((uint)vkCode, scanCode, keyboardState, sb, 64, 0);

                if (result > 0 && sb.Length > 0)
                {
                    char ch = sb[0];
                    if (!char.IsControl(ch))
                    {
                        Log($"[KEY] vkCode={vkCode:X2} → '{ch}'");
                        return ch;
                    }
                }

                // Fallback to simple mapping
                return GetCharFromVKeyFallback(vkCode);
            }
            catch (Exception ex)
            {
                Log($"[GetCharFromVKey Error] {ex.Message}");
                return GetCharFromVKeyFallback(vkCode);
            }
        }

        private static char GetCharFromVKeyFallback(int vkCode)
        {
            // Fallback character mapping
            bool shiftPressed = (GetKeyState(0x10) & 0x8000) != 0; // VK_SHIFT

            if (vkCode >= 0x41 && vkCode <= 0x5A) // A-Z
            {
                // Already uppercase, convert to lowercase if shift not pressed
                return shiftPressed ? (char)vkCode : char.ToLower((char)vkCode);
            }

            if (vkCode >= 0x30 && vkCode <= 0x39) // 0-9
            {
                if (shiftPressed)
                {
                    // Shift+number gives special characters
                    switch (vkCode)
                    {
                        case 0x30: return ')';
                        case 0x31: return '!';
                        case 0x32: return '@';
                        case 0x33: return '#';
                        case 0x34: return '$';
                        case 0x35: return '%';
                        case 0x36: return '^';
                        case 0x37: return '&';
                        case 0x38: return '*';
                        case 0x39: return '(';
                    }
                }
                return (char)vkCode;
            }

            switch (vkCode)
            {
                case 0x20: return ' ';  // Space
                
                // Number pad
                case 0x60: return '0';
                case 0x61: return '1';
                case 0x62: return '2';
                case 0x63: return '3';
                case 0x64: return '4';
                case 0x65: return '5';
                case 0x66: return '6';
                case 0x67: return '7';
                case 0x68: return '8';
                case 0x69: return '9';
                case 0x6A: return '*';
                case 0x6B: return '+';
                case 0x6D: return '-';
                case 0x6E: return '.';
                case 0x6F: return '/';

                // Punctuation
                case 0xBA: return shiftPressed ? ':' : ';';      // ; :
                case 0xBB: return shiftPressed ? '+' : '=';      // = +
                case 0xBC: return shiftPressed ? '<' : ',';      // , <
                case 0xBD: return shiftPressed ? '_' : '-';      // - _
                case 0xBE: return shiftPressed ? '>' : '.';      // . >
                case 0xBF: return shiftPressed ? '?' : '/';      // / ?
                case 0xC0: return shiftPressed ? '~' : '`';      // ` ~
                case 0xDB: return shiftPressed ? '{' : '[';      // [ {
                case 0xDC: return shiftPressed ? '|' : '\\';     // \ |
                case 0xDD: return shiftPressed ? '}' : ']';      // ] }
                case 0xDE: return shiftPressed ? '"' : '\'';     // ' "
                
                default: return '\0';
            }
        }

        private static void ResetInput()
        {
            capturedUsername = "";
            capturedPassword = "";
            fieldCounter = 0;
            lastWebsite = "";
            suggestionShownForWindow = false;
            lastLoginWindowHandle = IntPtr.Zero;
            suppressCapture = false;
            usernameFieldActive = false;
            inMultiPageLogin = false;
        }

        private static void ShowSuggestions(string windowTitle)
        {
            try
            {
                // Close existing popup if any
                if (activePopup != null && !activePopup.IsDisposed)
                {
                    Log("[SUGGEST] Closing old popup before showing new one");
                    activePopup.Close();
                    activePopup = null;
                }

                var creds = DatabaseService.FindCredentials(windowTitle);
                if (creds == null || creds.Count == 0)
                {
                    Log($"[SUGGEST] No saved credentials for {windowTitle}");
                    return;
                }

                Log($"[SUGGEST] Showing {creds.Count} saved credentials for {windowTitle}");

                var targetHandle = lastLoginWindowHandle;
                System.Windows.Forms.Timer delay = new System.Windows.Forms.Timer();
                delay.Interval = 50;
                delay.Tick += (s, e) =>
                {
                    delay.Stop();

                    var popup = new SuggestionPopup(creds, selected =>
                    {
                        try
                        {
                            capturedUsername = selected.Username;
                            capturedPassword = selected.Password;
                            fieldCounter = 2; // Assume username+password ready
                            Log($"[SUGGEST] Autofill selected {selected.Username} for {selected.Website}");

                            // Refocus login window then send keys
                            if (targetHandle != IntPtr.Zero)
                                SetForegroundWindow(targetHandle);

                            suppressCapture = true;
                            System.Windows.Forms.Timer sendTimer = new System.Windows.Forms.Timer();
                            sendTimer.Interval = 100;
                            sendTimer.Tick += (s2, e2) =>
                            {
                                sendTimer.Stop();
                                SetForegroundWindow(targetHandle);
                                System.Threading.Thread.Sleep(100);
                                
                                // Assume cursor is in username field when popup appears
                                // Clear any existing content and fill username
                                Log("[AUTOFILL] Filling username field...");
                                SendKeys.SendWait("^a");  // Ctrl+A to select all existing text
                                System.Threading.Thread.Sleep(50);
                                SendKeys.SendWait("{DELETE}");  // Delete selected text
                                System.Threading.Thread.Sleep(50);
                                SendKeys.SendWait(selected.Username);
                                System.Threading.Thread.Sleep(100);
                                
                                // Move to password field
                                Log("[AUTOFILL] Moving to password field...");
                                SendKeys.SendWait("{TAB}");
                                System.Threading.Thread.Sleep(100);
                                
                                // Clear existing content and fill password
                                Log("[AUTOFILL] Filling password field...");
                                SendKeys.SendWait("^a");  // Ctrl+A to select all
                                System.Threading.Thread.Sleep(50);
                                SendKeys.SendWait("{DELETE}");  // Delete selected text
                                System.Threading.Thread.Sleep(50);
                                SendKeys.SendWait(selected.Password);
                                Log("[AUTOFILL] Complete!");

                                // Re-enable capture shortly after we finish injecting
                                System.Windows.Forms.Timer restoreTimer = new System.Windows.Forms.Timer();
                                restoreTimer.Interval = 300;
                                restoreTimer.Tick += (s3, e3) =>
                                {
                                    restoreTimer.Stop();
                                    suppressCapture = false;
                                    
                                    // Reset captured credentials after autofill to prevent using old data
                                    // when user switches to different login form or account
                                    capturedUsername = "";
                                    capturedPassword = "";
                                    fieldCounter = 0;
                                };
                                restoreTimer.Start();
                            };
                            sendTimer.Start();
                        }
                        catch (Exception ex)
                        {
                            Log($"[SUGGEST ERROR] {ex.Message}");
                        }
                    }, targetHandle);
                    
                    // Store reference and handle close to reset flag
                    activePopup = popup;
                    popup.FormClosed += (s, args) =>
                    {
                        Log("[SUGGEST] Popup closed - resetting suggestion flag");
                        suggestionShownForWindow = false;
                        activePopup = null;
                    };
                    
                    ShowPopupNoActivate(popup);

                    // Keep focus on target login control (popup is no-activate window)
                    if (targetHandle != IntPtr.Zero)
                    {
                        SetForegroundWindow(targetHandle);

                        // Extra guard: refocus again shortly after show to handle cases
                        // where the popup steals focus on subsequent openings
                        System.Windows.Forms.Timer refocusTimer = new System.Windows.Forms.Timer();
                        refocusTimer.Interval = 75;
                        refocusTimer.Tick += (s2, e2) =>
                        {
                            refocusTimer.Stop();
                            SetForegroundWindow(targetHandle);
                        };
                        refocusTimer.Start();
                    }
                };
                delay.Start();
            }
            catch (Exception ex)
            {
                Log($"[SUGGEST ERROR] {ex.Message}");
            }
        }

        private static string FormatDisplay(string username, string website)
        {
            string cleanUser = SanitizeForDisplay(username, 30);
            string cleanSite = SanitizeForDisplay(website, 40);
            return $"{cleanUser} — {cleanSite}";
        }

        private static void ShowPopupNoActivate(Form popup)
        {
            // Ensure handle is created
            var handle = popup.Handle;
            ShowWindow(handle, SW_SHOWNOACTIVATE);
            // Keep Z-order unchanged and avoid activation to preserve target focus (helps on Facebook)
            SetWindowPos(handle, HWND_TOP, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_NOZORDER);
        }

        private static string SanitizeForDisplay(string input, int maxLen)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            string clean = input.Replace("\r", " ").Replace("\n", " ");
            clean = Regex.Replace(clean, "\\s+", " ").Trim();

            if (clean.Length > maxLen)
            {
                clean = clean.Substring(0, maxLen - 1) + "…";
            }

            return clean;
        }

        private class SuggestionPopup : Form
        {
            private readonly ListBox listBox;
            private readonly List<Credential> creds;
            private readonly Action<Credential> onSelect;
            private readonly IntPtr targetHandle;

            public SuggestionPopup(List<Credential> creds, Action<Credential> onSelect, IntPtr targetHandle)
            {
                this.creds = creds;
                this.onSelect = onSelect;
                this.targetHandle = targetHandle;

                Text = "Keypass - Select Account";
                Width = 320;
                Height = 220;
                StartPosition = FormStartPosition.CenterScreen;
                TopMost = true;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                ShowInTaskbar = false;

                listBox = new ListBox
                {
                    Dock = DockStyle.Fill
                };
                foreach (var c in creds)
                {
                    listBox.Items.Add(FormatDisplay(c.Username, c.Website));
                }
                listBox.DoubleClick += (_, __) => SelectCurrent();
                listBox.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        e.Handled = true;
                        SelectCurrent();
                    }
                    else if (e.KeyCode == Keys.Escape)
                    {
                        Close();
                    }
                };

                Controls.Add(listBox);

                Shown += (s, e) =>
                {
                    if (listBox.Items.Count > 0)
                    {
                        listBox.SelectedIndex = 0;
                        // Keep target login control focused to avoid stealing focus
                        if (targetHandle != IntPtr.Zero)
                            SetForegroundWindow(targetHandle);
                    }
                };
            }

            // Prevent popup from stealing focus
            protected override bool ShowWithoutActivation => true;

            protected override CreateParams CreateParams
            {
                get
                {
                    const int WS_EX_NOACTIVATE = 0x08000000;
                    var cp = base.CreateParams;
                    cp.ExStyle |= WS_EX_NOACTIVATE;
                    return cp;
                }
            }

            private void SelectCurrent()
            {
                if (listBox.SelectedIndex >= 0 && listBox.SelectedIndex < creds.Count)
                {
                    var selected = creds[listBox.SelectedIndex];
                    onSelect?.Invoke(selected);
                }
                Close();
            }
        }

        public static void Shutdown()
        {
            try
            {
                if (keyboardHookHandle != IntPtr.Zero)
                {
                    UnhookWindowsHookEx(keyboardHookHandle);
                    keyboardHookHandle = IntPtr.Zero;
                }

                if (mouseHookHandle != IntPtr.Zero)
                {
                    UnhookWindowsHookEx(mouseHookHandle);
                    mouseHookHandle = IntPtr.Zero;
                }

                Log("✓ UIHookService shut down");
            }
            catch (Exception ex)
            {
                Log($"✗ Error shutting down hooks: {ex.Message}");
            }
        }
    }
}
