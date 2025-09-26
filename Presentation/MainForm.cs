using System;
using System.Drawing;
using System.Windows.Forms;

namespace OBSLauncher.Presentation
{
    public class MainForm : Form
    {
        private TextBox _textBoxProcessName;
        private Label _label1;
        private Button _button1;
        private Panel _panel1;
        private Label label2;
        private TextBox _textBoxDelay;
        private Button _monitoringActivityButton;

        private bool _isMonitoringActive = true;

        // События для контроллера
        public event EventHandler<string>? ProcessNameChanged;
        public event EventHandler<string>? DelayChanged;
        public event EventHandler? ChangeObsPathClicked;
        public event EventHandler<bool>? MonitoringActivityToggled;

        public MainForm()
        {
            InitializeTrayIcon();
            InitializeComponent();

            // Привязка событий к UI
            _textBoxProcessName.TextChanged += (s, e) =>
                ProcessNameChanged?.Invoke(this, _textBoxProcessName.Text);

            _textBoxDelay.TextChanged += (s, e) =>
                DelayChanged?.Invoke(this, _textBoxDelay.Text);

            _button1.Click += (s, e) =>
                ChangeObsPathClicked?.Invoke(this, EventArgs.Empty);

            _monitoringActivityButton.Click += (s, e) =>
            {
                _isMonitoringActive = !_isMonitoringActive;
                MonitoringActivityToggled?.Invoke(this, _isMonitoringActive);
                UpdateMonitoringButtonUI();
            };

            UpdateMonitoringButtonUI();
        }

        private void InitializeTrayIcon()
        {
            NotifyIcon _trayIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Text = "OBS Launcher",
                Visible = true
            };
            _trayIcon.DoubleClick += (s, e) => ShowForm();

            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Показать", null, (s, e) => ShowForm());
            contextMenu.Items.Add("Закрыть", null, (s, e) => Application.Exit());
            _trayIcon.ContextMenuStrip = contextMenu;
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
            _textBoxProcessName = new TextBox();
            _label1 = new Label();
            _button1 = new Button();
            _panel1 = new Panel();
            _textBoxDelay = new TextBox();
            label2 = new Label();
            _monitoringActivityButton = new Button();
            _panel1.SuspendLayout();
            SuspendLayout();
            // 
            // _textBoxProcessName
            // 
            _textBoxProcessName.BackColor = Color.CadetBlue;
            _textBoxProcessName.ForeColor = Color.Honeydew;
            _textBoxProcessName.Location = new Point(99, 28);
            _textBoxProcessName.Name = "_textBoxProcessName";
            _textBoxProcessName.Size = new Size(100, 23);
            _textBoxProcessName.TabIndex = 0;
            _textBoxProcessName.Text = "gta_sa.exe";
            _textBoxProcessName.TextAlign = HorizontalAlignment.Center;
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
            _button1.FlatStyle = FlatStyle.Flat;
            _button1.ForeColor = Color.Honeydew;
            _button1.Location = new Point(74, 106);
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
            _panel1.Controls.Add(_button1);
            _panel1.Controls.Add(_textBoxDelay);
            _panel1.Controls.Add(label2);
            _panel1.Controls.Add(_textBoxProcessName);
            _panel1.Controls.Add(_label1);
            _panel1.Location = new Point(-3, -2);
            _panel1.Name = "_panel1";
            _panel1.Size = new Size(296, 136);
            _panel1.TabIndex = 3;
            // 
            // _textBoxDelay
            // 
            _textBoxDelay.BackColor = Color.CadetBlue;
            _textBoxDelay.ForeColor = Color.Honeydew;
            _textBoxDelay.Location = new Point(99, 72);
            _textBoxDelay.Name = "_textBoxDelay";
            _textBoxDelay.Size = new Size(100, 23);
            _textBoxDelay.TabIndex = 3;
            _textBoxDelay.Text = "50";
            _textBoxDelay.TextAlign = HorizontalAlignment.Center;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.SlateGray;
            label2.ForeColor = Color.Honeydew;
            label2.Location = new Point(87, 54);
            label2.Name = "label2";
            label2.Size = new Size(128, 15);
            label2.TabIndex = 2;
            label2.Text = "С задержкой (секунд):";
            // 
            // _monitoringActivityButton
            // 
            _monitoringActivityButton.BackColor = Color.DarkCyan;
            _monitoringActivityButton.FlatAppearance.BorderSize = 0;
            _monitoringActivityButton.FlatStyle = FlatStyle.Popup;
            _monitoringActivityButton.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
            _monitoringActivityButton.ForeColor = Color.Honeydew;
            _monitoringActivityButton.Location = new Point(71, 140);
            _monitoringActivityButton.Margin = new Padding(0);
            _monitoringActivityButton.Name = "_monitoringActivityButton";
            _monitoringActivityButton.Size = new Size(141, 33);
            _monitoringActivityButton.TabIndex = 4;
            _monitoringActivityButton.Text = "Мониторинг активен";
            _monitoringActivityButton.UseVisualStyleBackColor = false;
            // 
            // MainForm
            // 
            BackColor = Color.DarkSlateGray;
            ClientSize = new Size(286, 178);
            Controls.Add(_monitoringActivityButton);
            Controls.Add(_panel1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            Opacity = 0.9D;
            ShowIcon = false;
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "OBSLauncher";
            _panel1.ResumeLayout(false);
            _panel1.PerformLayout();
            ResumeLayout(false);
        }

        private void UpdateMonitoringButtonUI()
        {
            if (_isMonitoringActive)
            {
                _monitoringActivityButton.BackColor = Color.DarkSlateGray;
                _monitoringActivityButton.Text = "Мониторинг активен";
            }
            else
            {
                _monitoringActivityButton.BackColor = Color.DarkRed;
                _monitoringActivityButton.Text = "Мониторинг приостановлен";
            }
        }

        // Методы для обновления UI
        public void SetProcessNameValid(bool isValid)
            => _textBoxProcessName.BackColor = isValid ? Color.CadetBlue : Color.Red;

        public void SetDelayValid(bool isValid)
            => _textBoxDelay.BackColor = isValid ? Color.CadetBlue : Color.Red;

        public void ShowError(string message)
            => MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

        public void ShowInfo(string message)
            => MessageBox.Show(message, "Инфо", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
