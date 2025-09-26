using System.Windows.Forms;

namespace OBSLauncher.Validators
{
    public static class DelayTimeValidator
    {
        public static (bool result, int value) ValidateAndParse(string delayString)
        {
            if (int.TryParse(delayString, out int delay) && delay > 0)
            {
                return (true, delay);
            }
            return (false, 0);
        }
    }
}
