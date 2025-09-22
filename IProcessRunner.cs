namespace OBSLauncher
{
    public interface IProcessRunner
    {
        void RegisterEvent(ProcessRunner.RunProcessDelegate runProcessEvent);
        bool RunProcess();
    }
}