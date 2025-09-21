using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Security.Principal;
using System.IO;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace OBSLauncher
{
    public class Program : Form
    {
        private const string GTA_PROCESS_NAME = "gta_sa.exe";
        private const string OBS_SHORTCUT_DEFAULT_PATH = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\OBS Studio\OBS Studio (64bit).lnk";
        private readonly string _obsPath;

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
            Application.Run(new Program());
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

        public Program()
        {
            if (!File.Exists(OBS_SHORTCUT_DEFAULT_PATH))
            {
                MessageBox.Show("По заданному пути отсутствует программа OBS." +
                    "\nПожалуйста укажите путь к программе!",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                var folder = new OpenFileDialog();
                folder.CheckFileExists = true;
                folder.CheckPathExists = true;
                if (folder.ShowDialog().Equals(DialogResult.OK))
                {
                    if (File.Exists(folder.FileName))
                        _obsPath = folder.FileName;
                    else
                    {
                        MessageBox.Show("Невозможно определить путь к OBS",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                        Close();
                    }
                }
                else
                {
                    MessageBox.Show("Невозможно определить путь к OBS",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    Close();
                }
            }
            else
                _obsPath = OBS_SHORTCUT_DEFAULT_PATH;
                
            ProcessMonitor _processMonitor;
            ProcessRunner _runner;
            InitializeTrayIcon();
            AddToStartup();
            _runner = new ProcessRunner(_obsPath);
            _runner.RegisterEvent(ObsController.RunProcess);
            _processMonitor = new ProcessMonitor(GTA_PROCESS_NAME, _runner);
            _processMonitor.StartMonitoring();

            // Настройка формы
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
        }

        private void InitializeTrayIcon()
        {
            NotifyIcon _trayIcon;
            _trayIcon = new NotifyIcon();
            _trayIcon.Icon = SystemIcons.Application;
            _trayIcon.Text = "OBS Launcher";
            _trayIcon.Visible = true;
            _trayIcon.DoubleClick += TrayIcon_DoubleClick;

            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Показать", null, (s, e) => ShowForm());
            contextMenu.Items.Add("Закрыть", null, (s, e) => Application.Exit());
            _trayIcon.ContextMenuStrip = contextMenu;
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowForm();
        }

        private void ShowForm()
        {
            Show();
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                ShowInTaskbar = false;
            }
        }

        private void AddToStartup()
        {
            try
            {
                string appPath = Application.ExecutablePath;
                RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                rk.SetValue("OBSLauncher", appPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении в автозагрузку: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
