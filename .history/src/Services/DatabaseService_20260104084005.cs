using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Keypass.Models;

namespace Keypass.Services
{
    public static class DatabaseService
    {
        private static string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Keypass", "credentials.db");
        private static string connectionString = $"Data Source={dbPath};Version=3;";

        public static void Initialize()
        {
            try
            {
                // Create directory if not exists
                Directory.CreateDirectory(Path.GetDirectoryName(dbPath));

                // Create connection
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Create table if not exists
                    string createTableQuery = @"
                        CREATE TABLE IF NOT EXISTS Credentials (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Website TEXT NOT NULL,
                            Username TEXT NOT NULL,
                            Password TEXT NOT NULL,
                            Notes TEXT,
                            CreatedAt TEXT NOT NULL,
                            UpdatedAt TEXT,
                            UNIQUE(Website, Username)
                        )";

                    using (var command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Database initialization error: {ex.Message}");
            }
        }

        public static void SaveCredential(Credential credential)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    // Use INSERT OR REPLACE to update if duplicate exists
                    string query = @"INSERT OR REPLACE INTO Credentials (Website, Username, Password, Notes, CreatedAt, UpdatedAt) 
                                     VALUES (@Website, @Username, @Password, @Notes, 
                                             COALESCE((SELECT CreatedAt FROM Credentials WHERE Website = @Website AND Username = @Username), @CreatedAt),
                                             @UpdatedAt)";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Website", credential.Website);
                        command.Parameters.AddWithValue("@Username", credential.Username);
                        command.Parameters.AddWithValue("@Password", credential.Password);
                        command.Parameters.AddWithValue("@Notes", credential.Notes ?? "");
                        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error saving credential: {ex.Message}");
            }
        }

        public static void UpdateCredential(Credential credential)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = @"UPDATE Credentials SET Username = @Username, Password = @Password, 
                                     Notes = @Notes, UpdatedAt = @UpdatedAt 
                                     WHERE Website = @Website";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", credential.Username);
                        command.Parameters.AddWithValue("@Password", credential.Password);
                        command.Parameters.AddWithValue("@Notes", credential.Notes ?? "");
                        command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        command.Parameters.AddWithValue("@Website", credential.Website);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error updating credential: {ex.Message}");
            }
        }

        public static void DeleteCredential(string website, string username)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Credentials WHERE Website = @Website AND Username = @Username";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Website", website);
                        command.Parameters.AddWithValue("@Username", username);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error deleting credential: {ex.Message}");
            }
        }

        public static Credential GetCredential(string website, string username)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Credentials WHERE Website = @Website AND Username = @Username LIMIT 1";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Website", website);
                        command.Parameters.AddWithValue("@Username", username);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Credential
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Website = reader["Website"].ToString(),
                                    Username = reader["Username"].ToString(),
                                    Password = reader["Password"].ToString(),
                                    Notes = reader["Notes"].ToString(),
                                    CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString())
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error retrieving credential: {ex.Message}");
            }
            return null;
        }

        public static List<Credential> GetAllCredentials()
        {
            var credentials = new List<Credential>();
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Credentials ORDER BY Website";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                credentials.Add(new Credential
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Website = reader["Website"].ToString(),
                                    Username = reader["Username"].ToString(),
                                    Password = reader["Password"].ToString(),
                                    Notes = reader["Notes"].ToString(),
                                    CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error retrieving credentials: {ex.Message}");
            }
            return credentials;
        }

        public static List<Credential> FindCredentials(string websiteKeyword)
        {
            var credentials = new List<Credential>();
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    
                    // Extract key terms from window title for better matching
                    // e.g., "Đang nhập tiếp tục tới youtube" should match "youtube"
                    string[] commonKeywords = { "google", "youtube", "gmail", "facebook", "fb", "riot", "valorant" };
                    string extractedKeyword = websiteKeyword?.ToLower() ?? "";
                    foreach (var keyword in commonKeywords)
                    {
                        if (extractedKeyword.Contains(keyword))
                        {
                            extractedKeyword = keyword;
                            break;
                        }
                    }
                    
                    string query = @"SELECT * FROM Credentials 
                                       WHERE LOWER(Website) LIKE '%' || LOWER(@Keyword) || '%' 
                                          OR LOWER(@Keyword) LIKE '%' || LOWER(Website) || '%'
                                          OR LOWER(Website) LIKE '%' || LOWER(@ExtractedKeyword) || '%'
                                          OR LOWER(@ExtractedKeyword) LIKE '%' || LOWER(Website) || '%'
                                       ORDER BY CreatedAt DESC";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Keyword", websiteKeyword ?? string.Empty);
                        command.Parameters.AddWithValue("@ExtractedKeyword", extractedKeyword);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                credentials.Add(new Credential
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Website = reader["Website"].ToString(),
                                    Username = reader["Username"].ToString(),
                                    Password = reader["Password"].ToString(),
                                    Notes = reader["Notes"].ToString(),
                                    CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error finding credentials: {ex.Message}");
            }
            return credentials;
        }

        public static bool CredentialExists(string website, string username, string password)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT COUNT(*) FROM Credentials WHERE LOWER(Website) LIKE LOWER(@Website) AND Username = @Username AND Password = @Password";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Website", $"%{website}%");
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);

                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error checking credential: {ex.Message}");
            }
            return false;
        }

        public static List<Credential> SearchCredentials(string website)
        {
            var credentials = new List<Credential>();
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Credentials WHERE Website LIKE @Website ORDER BY Website";

                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Website", $"%{website}%");

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                credentials.Add(new Credential
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Website = reader["Website"].ToString(),
                                    Username = reader["Username"].ToString(),
                                    Password = reader["Password"].ToString(),
                                    Notes = reader["Notes"].ToString(),
                                    CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString())
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error searching credentials: {ex.Message}");
            }
            return credentials;
        }
    }
}
