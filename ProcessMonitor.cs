using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace OBSLauncher
{
    public class ProcessMonitor
    {
        private readonly string _targetProcessName;
        private readonly ProcessRunner _runner;
        private readonly Timer _checkTimer;
        private readonly Timer _delayTimer;
        
        private bool _obsRunning = false;
        private bool _gtaDetected = false;
        private const int DELAY_SECONDS = 50;
        private const int MILLISECONDS_IN_SECOND = 1000;

        public ProcessMonitor(string targetProcessName, ProcessRunner runner)
        {
            if (targetProcessName is null || targetProcessName.Length < 1)
                throw new ArgumentException("targetProccessName cannot be empty string");
            _targetProcessName = targetProcessName;
            _checkTimer = new Timer();
            _checkTimer.Interval = MILLISECONDS_IN_SECOND; // Проверка каждую секунду
            _checkTimer.Tick += CheckTimer_Tick;

            _delayTimer = new Timer();
            _delayTimer.Interval = DELAY_SECONDS * MILLISECONDS_IN_SECOND; // 50 секунд в миллисекундах
            _delayTimer.Tick += DelayTimer_Tick;
            _runner = runner;
        }

        public void StartMonitoring()
            => _checkTimer.Start();

        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            bool gtaRunning = Process.GetProcessesByName(_targetProcessName).Length > 0;

            if (gtaRunning && !_gtaDetected)
            {
                _gtaDetected = true;
                _delayTimer.Start();
            }
            else if (!gtaRunning && _gtaDetected)
            {
                _gtaDetected = false;
                _delayTimer.Stop();
                _obsRunning = false;
            }
        }

        private void DelayTimer_Tick(object sender, EventArgs e)
        {
            if (_gtaDetected && !_obsRunning)
                _obsRunning = _runner.RunProcess();
            _delayTimer.Stop();
        }
    }
}
