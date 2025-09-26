using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

namespace OBSLauncher
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            if (!IsAdministrator())
            {
                RestartAsAdmin();
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var mainForm = new Presentation.MainForm();
            var service = new Services.MainFormService();
            var controller = new Controllers.MainFormController(mainForm, service);
            Application.Run(mainForm);
        }

        private static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static void RestartAsAdmin()
        {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.UseShellExecute = true;
            proc.WorkingDirectory = Environment.CurrentDirectory;
            proc.FileName = Application.ExecutablePath;
            proc.Verb = "runas";

            try
            {
                Process.Start(proc);
            }
            catch
            {
                MessageBox.Show("Программа требует прав администратора для работы!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Environment.Exit(0);
        }
    }
}
