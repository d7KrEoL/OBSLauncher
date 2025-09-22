using System;
using System.Windows.Forms;

namespace OBSLauncher
{
    public class ProcessRunner : IProcessRunner
    {
        public delegate bool RunProcessDelegate(string processPath);
        private event RunProcessDelegate RunProcessEvent;
        private readonly string _processPath;

        public ProcessRunner(string processPath)
        {
            if (processPath is null || processPath.Length < 1)
                throw new ArgumentException("proccessPath cannot be null or empty");
            _processPath = processPath;
        }
        public void RegisterEvent(RunProcessDelegate runProcessEvent)
            => RunProcessEvent += runProcessEvent;

        public bool RunProcess()
        {
            if (RunProcessEvent == null)
                return false;

            foreach (var handler in RunProcessEvent.GetInvocationList())
            {
                if (!RunSingleProcess(handler))
                    return false;
            }
            return true;
        }

        private bool RunSingleProcess(Delegate process)
        {
            var runHandler = process as RunProcessDelegate;
            if (runHandler is null)
                return false;
            try
            {
                bool success = runHandler.Invoke(_processPath);
                if (!success) return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в обработчике: {ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
    }
}
