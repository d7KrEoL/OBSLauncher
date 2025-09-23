using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OBSLauncher
{
    public static class ProcessNameValidator
    {
        public static bool Validate(string processName)
        {
            if (string.IsNullOrEmpty(processName))
                return false;
            var allowedChars = new HashSet<char> { '-', '_', '.', '+', ',', '&', '[', ']' };
            if (processName.Any(ch => !char.IsLetterOrDigit(ch) &&
                          !char.IsWhiteSpace(ch) &&
                          !allowedChars.Contains(ch)))
                return false;
            return true;
        }
    }
}
