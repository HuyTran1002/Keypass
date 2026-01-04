using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Keypass.Services
{
    public static class SettingsService
    {
        private static string settingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Keypass", "settings.json");

        private static Dictionary<string, object> settings = new Dictionary<string, object>();

        static SettingsService()
        {
            LoadSettings();
        }

        private static void LoadSettings()
        {
            try
            {
                if (File.Exists(settingsPath))
                {
                    string json = File.ReadAllText(settingsPath);
                    var jsonDoc = JsonDocument.Parse(json);
                    foreach (var prop in jsonDoc.RootElement.EnumerateObject())
                    {
                        settings[prop.Name] = prop.Value.GetString();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error loading settings: {ex.Message}");
            }
        }

        public static T GetSetting<T>(string key, T defaultValue)
        {
            if (settings.ContainsKey(key))
            {
                try
                {
                    return (T)Convert.ChangeType(settings[key], typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        public static void SaveSetting(string key, object value)
        {
            settings[key] = value;
            SaveSettingsToFile();
        }

        private static void SaveSettingsToFile()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(settingsPath));
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(settingsPath, json);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error saving settings: {ex.Message}");
            }
        }
    }
}
