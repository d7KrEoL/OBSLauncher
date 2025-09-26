namespace OBSLauncher.Abstractions
{
    public interface IRunnableProgramService
    {
        bool RunProcess(string shortcutPath);
        bool StopProcess();
    }
}
