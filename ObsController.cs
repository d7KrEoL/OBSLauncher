using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace OBSLauncher
{
    public class ObsController : LaunchProgramControllerBase
    {
        public override bool RunProcess(string shortcutPath)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = shortcutPath,
                    UseShellExecute = true,
                    WorkingDirectory = Path.GetDirectoryName(shortcutPath)
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при запуске OBS: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
    }
}
