namespace OBSLauncher
{
    public interface IRunnableProgramService
    {
        IProcessRunner.RunProcessDelegate RunProcess(string shortcutPath);
        bool StopProcess();
    }
}
