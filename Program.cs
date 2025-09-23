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
        private const string GtaDefaultProcessName = "gta_sa.exe";
        private const string ObsShortcutDefaultPath = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\OBS Studio\OBS Studio (64bit).lnk";
        private const string RegistryUserKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private TextBox _textBox1;
        private Label _label1;
        private Button _button1;
        private Panel _panel1;
        private readonly string _obsPath;
        private readonly ObsController _obsController;
        private readonly ProcessRunner _runner;
        private ProcessMonitor _monitor;

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
            (var error, _obsPath) = FileController.FileInfo(ObsShortcutDefaultPath);
            if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(_obsPath))
            {
                MessageBox.Show(error,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            _obsController = new ObsController();
            InitializeTrayIcon();
            InitializeComponent();
            AddToStartup();
            _runner = new ProcessRunner(_obsPath);
            _runner.RegisterEvent(_obsController.RunProcess);
            _monitor = new ProcessMonitor(GtaDefaultProcessName, _runner);
            _monitor.StartMonitoring();

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
            _textBox1 = new TextBox();
            _label1 = new Label();
            _button1 = new Button();
            _panel1 = new Panel();
            _panel1.SuspendLayout();
            SuspendLayout();
            // 
            // _textBox1
            // 
            _textBox1.BackColor = Color.CadetBlue;
            _textBox1.ForeColor = Color.Honeydew;
            _textBox1.Location = new Point(100, 41);
            _textBox1.Name = "_textBox1";
            _textBox1.Size = new Size(100, 23);
            _textBox1.TabIndex = 0;
            _textBox1.Text = "gta_sa.exe";
            _textBox1.TextAlign = HorizontalAlignment.Center;
            _textBox1.TextChanged += textBox1_TextChanged;
            // 
            // _label1
            // 
            _label1.AutoSize = true;
            _label1.BackColor = Color.SlateGray;
            _label1.ForeColor = Color.Honeydew;
            _label1.Location = new Point(56, 11);
            _label1.Name = "_label1";
            _label1.Size = new Size(193, 15);
            _label1.TabIndex = 1;
            _label1.Text = "Запускать когда активен процесс:";
            // 
            // _button1
            // 
            _button1.Anchor = AnchorStyles.None;
            _button1.BackColor = Color.CadetBlue;
            _button1.ForeColor = Color.Honeydew;
            _button1.Location = new Point(72, 79);
            _button1.Margin = new Padding(1);
            _button1.Name = "_button1";
            _button1.Size = new Size(141, 23);
            _button1.TabIndex = 2;
            _button1.Text = "Изменить путь к OBS";
            _button1.UseVisualStyleBackColor = false;
            // 
            // _panel1
            // 
            _panel1.BackColor = Color.SlateGray;
            _panel1.Controls.Add(_textBox1);
            _panel1.Controls.Add(_label1);
            _panel1.Location = new Point(-3, -2);
            _panel1.Name = "_panel1";
            _panel1.Size = new Size(289, 73);
            _panel1.TabIndex = 3;
            // 
            // Program
            // 
            BackColor = Color.DarkSlateGray;
            ClientSize = new Size(284, 106);
            Controls.Add(_button1);
            Controls.Add(_panel1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Program";
            Opacity = 0.8D;
            ShowIcon = false;
            SizeGripStyle = SizeGripStyle.Hide;
            _panel1.ResumeLayout(false);
            _panel1.PerformLayout();
            ResumeLayout(false);

        }

        private void AddToStartup()
        {
            try
            {
                string appPath = Application.ExecutablePath;
                RegistryKey rk = Registry.CurrentUser.OpenSubKey(RegistryUserKey, true);
                rk.SetValue("OBSLauncher", appPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении в автозагрузку: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!ProcessNameValidator.Validate(_textBox1.Text))
            {
                _textBox1.BackColor = Color.Red;
                return;
            }
            else
                _textBox1.BackColor = Color.CadetBlue;
            _monitor.StopMonitoring();
            _monitor = new ProcessMonitor(_textBox1.Text, _runner);
            _monitor.StartMonitoring();
        }
    }
}
