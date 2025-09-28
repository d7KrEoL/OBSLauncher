using OBSLauncher.Abstractions;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace OBSLauncher.Services
{
    public class ProcessMonitor : IProcessMonitor
    {
        private readonly string _targetProcessName;
        private readonly IProcessRunner _runner;
        private readonly Timer _checkTimer;
        private readonly Timer _delayTimer;

        private bool _obsRunning = false;
        private bool _gtaDetected = false;
        private const int DelayDefaultSeconds = 50;
        private const int MillisecondsInSecond = 1000;

        public ProcessMonitor(string targetProcessName, IProcessRunner runner, int delaySeconds) 
            : this(targetProcessName, runner)
        {
            if (delaySeconds < 1)
                throw new ArgumentException("delaySeconds must be greater than zero");
            _delayTimer.Interval = delaySeconds * MillisecondsInSecond;
        }

        public ProcessMonitor(string targetProcessName, IProcessRunner runner)
        {
            if (targetProcessName is null || targetProcessName.Length < 1)
                throw new ArgumentException("targetProccessName cannot be empty string");
            _targetProcessName = targetProcessName;
            _checkTimer = new Timer();
            _checkTimer.Interval = MillisecondsInSecond; // Проверка каждую секунду
            _checkTimer.Tick += CheckTimer_Tick;

            _delayTimer = new Timer();
            _delayTimer.Interval = DelayDefaultSeconds * MillisecondsInSecond; // 50 секунд в миллисекундах
            _delayTimer.Tick += DelayTimer_Tick;
            _runner = runner;
        }

        public string GetTargetProcessName()
            => _targetProcessName;

        public int GetDelaySeconds()
            => _delayTimer.Interval / MillisecondsInSecond;
        public void SetObsUnactive()
        {
            _obsRunning = false;
            StopMonitoring();
            StartMonitoring();
        }
        /// <summary>
        /// Запускает мониторинг процесса
        /// </summary>
        /// <exception cref="Exception">Возникает, при попытке
        /// запустить мониторинг у экземпляра, отмеченного для удаления</exception>
        public void StartMonitoring()
        {
            if (_checkTimer is null)
                throw new Exception("This object should've been deleted and timer cannot be started");
            _checkTimer.Start();
        }
        /// <summary>
        /// Приостанавливает мониторинг процесса. Для возобновления нужно вызвать StartMonitoring.
        /// Для полного удаления нужно вызвать StopMonitoring.
        /// </summary>
        public void PauseMonitoring()
        {
            _checkTimer.Stop();
            _delayTimer.Stop();
        }
        /// <summary>
        /// Освобождает ресурсы таймеров, нужно вызывать только при удалении объекта
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void StopMonitoring()
        {
            _checkTimer.Stop();
            _delayTimer.Stop();
            _delayTimer.Tick -= DelayTimer_Tick;
            _checkTimer.Tick -= CheckTimer_Tick;
            _delayTimer.Dispose();
            _checkTimer.Dispose();
        }

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
