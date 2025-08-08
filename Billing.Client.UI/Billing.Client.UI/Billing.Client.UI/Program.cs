using Billing.Client.UI.Login;
using Billing.Utility;

namespace Billing.Client.UI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            Billing.Utility.LogManager.Initialize(
                    applicationName: "SmartBill.Desktop",
                    maxFileSizeMB: 40
                );
            LoginForm loginForm = new LoginForm();
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                // User logged in successfully
                Application.Run(new MainForm());
            }
            LogManager.Shutdown();

        }
    }
}