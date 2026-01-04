using System;
using System.Windows.Forms;
using System.Threading;
using Keypass.UI;
using Keypass.Services;

namespace Keypass
{
    static class Program
    {
        private static Mutex mutex = null;
        private const string MutexName = "Global\\Keypass_SingleInstance";

        [STAThread]
        static void Main()
        {
            // Check if another instance is already running
            bool isNewInstance = false;
            try
            {
                mutex = new Mutex(true, MutexName, out isNewInstance);
            }
            catch
            {
                isNewInstance = false;
            }

            // If another instance is running, exit
            if (!isNewInstance)
            {
                MessageBox.Show("Keypass is already running.", "Keypass", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Initialize database
                DatabaseService.Initialize();

                // Initialize UI hooks
                UIHookService.Initialize();

                // Run the main tray application
                Application.Run(new TrayApplicationContext());
            }
            finally
            {
                if (mutex != null)
                {
                    mutex.ReleaseMutex();
                    mutex.Dispose();
                }
            }
        }
    }
}
