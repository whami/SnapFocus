using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SnapFocus.Core.Paths
{
    public static class AppPaths
    {
        // Win32: returns APPMODEL_ERROR_NO_PACKAGE (15700) if unpackaged
        private const int APPMODEL_ERROR_NO_PACKAGE = 15700;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetCurrentPackageFamilyName(ref int packageFamilyNameLength, StringBuilder packageFamilyName);

        public static bool IsPackaged()
        {
            int length = 0;
            int rc = GetCurrentPackageFamilyName(ref length, null);
            return rc != APPMODEL_ERROR_NO_PACKAGE;
        }

        public static string GetPackageFamilyNameOrNull()
        {
            int length = 0;
            int rc = GetCurrentPackageFamilyName(ref length, null);
            if (rc == APPMODEL_ERROR_NO_PACKAGE) return null;
            if (length <= 0) return null;

            var sb = new StringBuilder(length);
            rc = GetCurrentPackageFamilyName(ref length, sb);
            if (rc != 0) return null;

            return sb.ToString();
        }

        /// <summary>
        /// Returns a *real, physical* writable root.
        /// - Unpackaged: %LOCALAPPDATA%
        /// - Packaged (MSIX): %LOCALAPPDATA%\Packages\<PFN>\LocalCache\Local
        /// </summary>
        public static string GetWritableLocalRoot()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            var pfn = GetPackageFamilyNameOrNull();
            if (string.IsNullOrWhiteSpace(pfn))
                return localAppData;

            return Path.Combine(localAppData, "Packages", pfn, "LocalCache", "Local");
        }

        public static string GetLogDirectory(string appName = "SnapFocus")
        {
            // keep SnapFocus\logs shape everywhere
            return Path.Combine(GetWritableLocalRoot(), appName, "logs");
        }
    }
}
