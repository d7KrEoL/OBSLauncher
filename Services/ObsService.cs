using OBSLauncher.Abstractions;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace OBSLauncher.Services
{
    public class ObsService : IRunnableProgramService
    {
        public event EventHandler? ProcessExited;
        private const string GitHubUrl = "https://github.com/KromSystems/OBSLauncher";
        private Process? _process;
        public ObsService(EventHandler exitProcessHandler)
        {
            ProcessExited += exitProcessHandler;
        }
        public bool RunProcess(string shortcutPath)
        {
            if (_process is not null)
                throw new Exception("Another obs program proccess is already running!\n" +
                    "Please report a bug issue into github repository:\n" +
                    GitHubUrl);
            try
            {
                _process = Process.Start(new ProcessStartInfo
                {
                    FileName = shortcutPath,
                    UseShellExecute = true,
                    WorkingDirectory = Path.GetDirectoryName(shortcutPath)
                });
                if (_process is null)
                {
                    Debug.WriteLine("не получилось запустить процесс OBS");
                    return false;
                }
                _process.EnableRaisingEvents = true;
                _process.Exited += OnProcessExited;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при запуске OBS: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        public bool StopProcess()
        {
            if (_process is null)
                return false;
            if (!_process.CloseMainWindow())
                _process.Kill();
            _process.Exited -= OnProcessExited;
            _process.Dispose();
            _process = null;
            return true;
        }
        private void OnProcessExited(object? sender, EventArgs e)
        {
            ProcessExited?.Invoke(this, EventArgs.Empty);
            _process.Exited -= OnProcessExited;
            _process.Dispose();
            _process = null;
        }
    }
}
