namespace OBSLauncher
{
    public interface IProcessRunner
    {
        delegate bool RunProcessDelegate(string processPath);
        void RegisterEvent(RunProcessDelegate runProcessEvent);
        bool RunProcess();
    }
}