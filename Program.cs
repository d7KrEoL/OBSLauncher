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
        private NotifyIcon trayIcon;
        private ProcessMonitor processMonitor;
        private const string GTA_PROCESS_NAME = "gta_sa";
        private const string OBS_SHORTCUT_PATH = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\OBS Studio\OBS Studio (64bit).lnk";

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
            InitializeTrayIcon();
            AddToStartup();
            processMonitor = new ProcessMonitor(GTA_PROCESS_NAME, OBS_SHORTCUT_PATH);
            processMonitor.StartMonitoring();

            // Настройка формы
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
        }

        private void InitializeTrayIcon()
        {
            trayIcon = new NotifyIcon();
            trayIcon.Icon = SystemIcons.Application;
            trayIcon.Text = "OBS Launcher";
            trayIcon.Visible = true;
            trayIcon.DoubleClick += TrayIcon_DoubleClick;

            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Показать", null, (s, e) => ShowForm());
            contextMenu.Items.Add("Закрыть", null, (s, e) => Application.Exit());
            trayIcon.ContextMenuStrip = contextMenu;
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowForm();
        }

        private void ShowForm()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.ShowInTaskbar = false;
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

    public class ProcessMonitor
    {
        private string targetProcessName;
        private string shortcutPath;
        private Timer checkTimer;
        private Timer delayTimer;
        private bool obsRunning = false;
        private bool gtaDetected = false;
        private const int DELAY_SECONDS = 50;

        public ProcessMonitor(string targetProcessName, string shortcutPath)
        {
            this.targetProcessName = targetProcessName;
            this.shortcutPath = shortcutPath;
            checkTimer = new Timer();
            checkTimer.Interval = 1000; // Проверка каждую секунду
            checkTimer.Tick += CheckTimer_Tick;

            delayTimer = new Timer();
            delayTimer.Interval = DELAY_SECONDS * 1000; // 50 секунд в миллисекундах
            delayTimer.Tick += DelayTimer_Tick;
        }

        public void StartMonitoring()
        {
            checkTimer.Start();
        }

        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            bool gtaRunning = Process.GetProcessesByName(targetProcessName).Length > 0;

            if (gtaRunning && !gtaDetected)
            {
                gtaDetected = true;
                delayTimer.Start();
            }
            else if (!gtaRunning && gtaDetected)
            {
                gtaDetected = false;
                delayTimer.Stop();
                obsRunning = false;
            }
        }

        private void DelayTimer_Tick(object sender, EventArgs e)
        {
            if (gtaDetected && !obsRunning)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = shortcutPath,
                        UseShellExecute = true,
                        WorkingDirectory = Path.GetDirectoryName(shortcutPath)
                    });
                    obsRunning = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при запуске OBS: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            delayTimer.Stop();
        }
    }
}
