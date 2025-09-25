namespace OBSLauncher
{
    public interface IRunnableProgramService
    {
        bool RunProcess(string shortcutPath);
        bool StopProcess();
    }
}
