using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;

namespace OBSLauncher
{
    public class MainFormService
    {
        private const string ObsShortcutDefaultPath = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\OBS Studio\OBS Studio (64bit).lnk";
        private const string RegistryUserKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private readonly ObsService _obsController;
        private ProcessRunner _runner;
        private ProcessMonitor _monitor;
        private string _obsPath;
        private bool _isMonitoringActive = true;
        public MainFormService()
        {
            (var error, _obsPath) = FileDialogService.FileInfo(ObsShortcutDefaultPath);
            if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(_obsPath))
            {
                throw new Exception(error);
            }
            _obsController = new ObsService(OnRunningProcessExited);
            AddToStartup();
            StartRunner(new List<IProcessRunner.RunProcessDelegate> { _obsController.RunProcess });
        }
        public void OnRunningProcessExited(object? sender, EventArgs e)
        {
            _monitor.SetObsUnactive();
        }
        public void RegisterRunningProcessExitedHandler(EventHandler handler)
        {
            _obsController.ProcessExited += handler;
        }

        public (string error, string path) SelectObsPath()
        {
            return FileDialogService.FileInfo();
        }

        public void SetObsPath(string path)
        {
            _obsPath = path;
        }

        public bool StopObsProcess()
        {
            return _obsController.StopProcess();
        }

        public void StartRunner(List<IProcessRunner.RunProcessDelegate> handlers)
        {
            _runner = new ProcessRunner(_obsPath);
            foreach (var handler in handlers)
                _runner.RegisterEvent(handler);
            _monitor = new ProcessMonitor(_obsPath, _runner);
        }

        public void UpdateRunner()
        {
            _runner = new ProcessRunner(_obsPath);
            _runner.RegisterEvent(_obsController.RunProcess);
            UpdateMonitoring();
        }

        public void UpdateMonitoring(string? targetProcessName = null, int? delayValue = null)
        {
            string processName = targetProcessName ?? _monitor.GetTargetProcessName();
            int delay = delayValue ?? _monitor.GetDelaySeconds();

            _monitor.StopMonitoring();
            _monitor = new ProcessMonitor(processName, _runner, delay);
            if (_isMonitoringActive)
                _monitor.StartMonitoring();
            else
                _monitor.PauseMonitoring();
        }

        public void SetMonitoringActive(bool isActive)
        {
            _isMonitoringActive = isActive;
            if (_isMonitoringActive)
                _monitor.StartMonitoring();
            else
                _monitor.PauseMonitoring();
        }

        public bool ValidateProcessName(string processName)
        {
            return ProcessNameValidator.Validate(processName);
        }

        public (bool isValid, int value) ValidateAndParseDelay(string delayText)
        {
            return DelayTimeValidator.ValidateAndParse(delayText);
        }

        private void AddToStartup()
        {
            try
            {
                string appPath = Application.ExecutablePath;
                using (var registry = Registry.CurrentUser.OpenSubKey(RegistryUserKey, true))
                {
                    registry.SetValue("OBSLauncher", appPath);
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine($"Failed to add to startup: {e.Message}");
            }
        }
    }
}
