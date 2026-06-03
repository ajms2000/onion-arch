namespace System
{
    public static class PathUtilities
    {
        public static bool AreEqual(string path1, string path2)
        {
            var urlP1 = new Uri(path1);
            var urlP2 = new Uri(path2);

            var cmprResult = Uri.Compare(urlP1, urlP2, UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase);
            return cmprResult == 0;
        }

        public static string GetDesktopFolder(bool isAllUserDesktop = false)
        {
            Environment.SpecialFolder par = isAllUserDesktop ? Environment.SpecialFolder.CommonDesktopDirectory : Environment.SpecialFolder.DesktopDirectory;
            var ret = Environment.GetFolderPath(par);
            return ret;
        }

        public static string? GetTempFolder(EnvironmentVariableTarget target = EnvironmentVariableTarget.User)
        {
            var ret = Environment.GetEnvironmentVariable("TEMP", target);
            return ret;
        }

        public static string CleanSeparators(string path, bool endWithSeperator = false)
        {
            var ret = path;
            if (!string.IsNullOrWhiteSpace(path))
            {
                ret = path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                if (endWithSeperator)
                {
                    ret = ret + Path.DirectorySeparatorChar.ToString();
                }
            }
            return ret;
        }


        public static bool IsSharedNetworkPath(string path, bool throwOnError = true)
        {
            var ret = false;

            try
            {
                var url = new Uri(path);
                var isFileScheme = url.Scheme == Uri.UriSchemeFile;
                var hasAuthority = !string.IsNullOrWhiteSpace(url.Authority);

                ret = isFileScheme && hasAuthority;
            }
            catch
            {
                if (throwOnError)
                {
                    throw;
                }
            }

            return ret;
        }

        public static bool IsLocalPath(string path)
        {
            var url = new Uri(path);
            var ret = url.IsFile;

            if (ret)
            {
                try
                {
                    var di = new DriveInfo(path);
                }
                catch
                {
                    ret = false;
                }
            }

            return ret;
        }

        public static string GetApplicationBaseDirectory(bool endWithSeperator = false)
        {
            var ret = AppDomain.CurrentDomain.BaseDirectory;

            ret = ret.TrimEnd('\\');

            if (endWithSeperator)
            {
                ret += "\\";
            }

            return ret;
        }

        public static string GetApplicationBinDirectory(bool endWithSeperator = false)
        {
            var ret = AppDomain.CurrentDomain.RelativeSearchPath;

            if (string.IsNullOrWhiteSpace(ret))
            {
                ret = AppDomain.CurrentDomain.BaseDirectory;
            }

            ret = ret.TrimEnd('\\');

            if (endWithSeperator)
            {
                ret += "\\";
            }

            return ret;
        }


        public static void ValidateRelativePath(string path)
        {
            if (path.NullOrWhiteSpace())
            {
                throw new ArgumentNullException("path");
            }

            if (IsSharedNetworkPath(path, throwOnError: false))
            {
                throw new AppException("Path is not a relative path.");
            }

            path = path.Trim(@"\/".ToArray());

            var isRooted = Path.IsPathRooted(path);

            if (isRooted)
            {
                throw new AppException("Path is not a relative path.");
            }
        }

        public static void ValidateAbsolutePath(string path)
        {
            if (path.NullOrWhiteSpace())
            {
                throw new ArgumentNullException("path");
            }

            if (IsSharedNetworkPath(path))
            {
                return;
            }

            path = path.Trim("\\/".ToArray());

            var isRooted = Path.IsPathRooted(path);

            if (!isRooted)
            {
                throw new AppException("Path is not a relative path.");
            }
        }


        public static string TrimPathSlashes(this string path, bool atStart = true, bool atEnd = true)
        {
            return TrimPathSlashes_Core(ref path, CommonUtilities.CHARS_SLASHES, atStart, atEnd);
        }

        public static string TrimPathBackSlashes(this string path, bool atStart = true, bool atEnd = true)
        {
            return TrimPathSlashes_Core(ref path, CommonUtilities.CHARS_SLASH_B, atStart, atEnd);
        }

        public static string TrimPathForwardSlashes(this string path, bool atStart = true, bool atEnd = true)
        {
            return TrimPathSlashes_Core(ref path, CommonUtilities.CHARS_SLASH_F, atStart, atEnd);
        }

        private static string TrimPathSlashes_Core(ref string path, char[] trimChars, bool atStart = true, bool atEnd = true)
        {
            if (atStart && atEnd)
            {
                path = path.Trim(trimChars);
            }
            else if (atStart)
            {
                path = path.TrimStart(trimChars);
            }
            else if (atEnd)
            {
                path = path.TrimEnd(trimChars);
            }
            else
            {
                throw new ArgumentException($"Either one of the parameter '{nameof(atStart)}' or '{nameof(atEnd)}' must be true.");
            }

            return path;
        }


        public static string CleanInvalidFilenameChars(this string filename, char substitute = ' ')
        {
            Path.GetInvalidFileNameChars()
                .ToList()
                .ForEach(c =>
                {
                    filename = filename.Replace(c, substitute);
                });

            return filename;
        }
    }
}
