using OBSLauncher.Presentation;
using OBSLauncher.Services;
using System;
using System.Diagnostics;

namespace OBSLauncher.Controllers
{
    public class MainFormController
    {
        private readonly MainFormService _service;
        private readonly MainForm _view;

        public MainFormController(MainForm view, MainFormService service)
        {
            _view = view;
            _service = service;

            _view.ProcessNameChanged += OnProcessNameChanged;
            _view.DelayChanged += OnDelayChanged;
            _view.ChangeObsPathClicked += OnChangeObsPathClicked;
            _view.MonitoringActivityToggled += OnMonitoringActivityToggled;
            _service.RegisterRunningProcessExitedHandler(OnProcessExited);
        }

        private void OnProcessNameChanged(object? sender, string processName)
        {
            if (!_service.ValidateProcessName(processName))
            {
                _view.SetProcessNameValid(false);
                return;
            }
            _view.SetProcessNameValid(true);
            _service.UpdateMonitoring(processName, null);
        }

        private void OnDelayChanged(object? sender, string delayText)
        {
            var (isValid, value) = _service.ValidateAndParseDelay(delayText);
            if (!isValid)
            {
                _view.SetDelayValid(false);
                return;
            }
            _view.SetDelayValid(true);
            _service.UpdateMonitoring(null, value);
        }

        private void OnChangeObsPathClicked(object? sender, EventArgs e)
        {
            var (error, path) = _service.SelectObsPath();
            if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(path))
            {
                _view.ShowError(error);
                return;
            }
            _service.SetObsPath(path);
            if (_service.StopObsProcess())
                _view.ShowInfo("Текущий процесс OBS будет завершён, так как изменён путь к программе");
            _service.UpdateRunner();
        }

        private void OnMonitoringActivityToggled(object? sender, bool isActive)
        {
            _service.SetMonitoringActive(isActive);
        }
        private void OnProcessExited(object? sender, EventArgs e)
        {
            Debug.WriteLine("MainFormController: Detected running process exit");
            _service.OnRunningProcessExited(sender, e);
        }
    }
}
