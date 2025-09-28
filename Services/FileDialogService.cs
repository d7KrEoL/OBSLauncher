using System.IO;
using System.Windows.Forms;

namespace OBSLauncher.Services
{
    public static class FileDialogService
    {
        public static (string error, string filePath) FileInfo(string defaultPath)
        {
            if (File.Exists(defaultPath))
                return (string.Empty, defaultPath);
            MessageBox.Show("По заданному пути отсутствует программа OBS.\n" +
                    "Пожалуйста укажите путь к программе!",
                "Ошибка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return FileInfo();
        }
        public static (string error, string filePath) FileInfo()
        {
            var folder = new OpenFileDialog();
            folder.CheckFileExists = true;
            folder.CheckPathExists = true;
            if (folder.ShowDialog().Equals(DialogResult.OK))
            {
                if (File.Exists(folder.FileName))
                    return (string.Empty, folder.FileName);
                else
                    return ("Неверно указан путь к OBS", string.Empty);
            }
            else
                return ("Невозможно определить путь к OBS", string.Empty);
        }
    }
}
