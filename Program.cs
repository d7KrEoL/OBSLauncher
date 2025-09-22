using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Security.Principal;
using Microsoft.Win32;

namespace OBSLauncher
{
    public class Program : Form
    {
        private const string GTA_PROCESS_NAME = "gta_sa.exe";
        private const string OBS_SHORTCUT_DEFAULT_PATH = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\OBS Studio\OBS Studio (64bit).lnk";
        private TextBox textBox1;
        private Label label1;
        private Button button1;
        private Panel panel1;
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
            (var error, _obsPath) = FileController.FileInfo(OBS_SHORTCUT_DEFAULT_PATH);
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Close();
            }
            ObsController controller = new ObsController();
            ProcessMonitor processMonitor;
            ProcessRunner runner;
            InitializeTrayIcon();
            InitializeComponent();
            AddToStartup();
            runner = new ProcessRunner(_obsPath);
            runner.RegisterEvent(controller.RunProcess);
            processMonitor = new ProcessMonitor(GTA_PROCESS_NAME, runner);
            processMonitor.StartMonitoring();

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

        private void InitializeComponent()
        {
            textBox1 = new TextBox();
            label1 = new Label();
            button1 = new Button();
            panel1 = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.CadetBlue;
            textBox1.ForeColor = Color.Honeydew;
            textBox1.Location = new Point(100, 41);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(100, 23);
            textBox1.TabIndex = 0;
            textBox1.Text = "gta_sa.exe";
            textBox1.TextAlign = HorizontalAlignment.Center;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.SlateGray;
            label1.ForeColor = Color.Honeydew;
            label1.Location = new Point(56, 11);
            label1.Name = "label1";
            label1.Size = new Size(193, 15);
            label1.TabIndex = 1;
            label1.Text = "Запускать когда активен процесс:";
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.None;
            button1.BackColor = Color.CadetBlue;
            button1.ForeColor = Color.Honeydew;
            button1.Location = new Point(72, 79);
            button1.Margin = new Padding(1);
            button1.Name = "button1";
            button1.Size = new Size(141, 23);
            button1.TabIndex = 2;
            button1.Text = "Изменить путь к OBS";
            button1.UseVisualStyleBackColor = false;
            // 
            // panel1
            // 
            panel1.BackColor = Color.SlateGray;
            panel1.Controls.Add(textBox1);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(-3, -2);
            panel1.Name = "panel1";
            panel1.Size = new Size(289, 73);
            panel1.TabIndex = 3;
            // 
            // Program
            // 
            BackColor = Color.DarkSlateGray;
            ClientSize = new Size(284, 106);
            Controls.Add(button1);
            Controls.Add(panel1);
            MaximizeBox = false;
            Name = "OBSLauncher";
            Opacity = 0.3D;
            ShowIcon = false;
            SizeGripStyle = SizeGripStyle.Hide;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);

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
