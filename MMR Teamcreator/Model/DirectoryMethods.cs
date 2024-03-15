using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryChecker
{
    public static class DirectoryMethods
    {
        public enum EnumDirectoryChecker
        {
            Check = 0,
            CheckOrCreate = 1
        }

        /// <summary>
        /// Checks if all directories in array exist
        /// </summary>
        /// <param name="dirArr">Array of directory pathes</param>
        /// <returns>
        /// Returns -1 - At least one directory doesn't exist - Checker set to 0<br></br>
        /// Returns 0 - At least one directory doesn't exist - Checker set to 1<br></br>
        /// Returns 1 - All directories exist</returns>
        public static int DirectoryChecker(string dirPath, EnumDirectoryChecker checker) => DirectoryChecker(new string[] { dirPath }, checker);

        /// <summary>
        /// Checks if all directories in array exist
        /// </summary>
        /// <param name="dirArr">Array of directory pathes</param>
        /// <returns>
        /// Returns -1 - At least one directory doesn't exist - Checker set to 0<br></br>
        /// Returns 0 - At least one directory doesn't exist - Checker set to 1<br></br>
        /// Returns 1 - All directories exist</returns>
        public static int DirectoryChecker(string[] dirArr, EnumDirectoryChecker checker)
        {
            int retCode = 1;
            try
            {
                foreach (string dir in dirArr)
                {
                    if (retCode == 1)
                        retCode = PathChecker(dir, checker);
                    else
                        PathChecker(dir, checker);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return retCode;
        }

        /// <summary>
        /// Checks if directory exists
        /// </summary>
        /// <param name="dirArr">Path</param>
        /// <returns>
        /// Returns -1 - At least one directory doesn't exist - Checker set to 0<br></br>
        /// Returns 0 - At least one directory doesn't exist - Checker set to 1<br></br>
        /// Returns 1 - All directories exist</returns>
        private static int PathChecker(string path, EnumDirectoryChecker checker)
        {
            int retCode = 1;

            try
            {
                for (int i = path.Count(x => x == '\\'); i > 0; i--)
                {
                    string newPath = RemovePart(path, i);
                    if (!Directory.Exists(newPath) && checker == EnumDirectoryChecker.CheckOrCreate)
                        Directory.CreateDirectory(newPath);
                    else if (!Directory.Exists(newPath) && checker == EnumDirectoryChecker.Check)
                        return -1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return retCode;
        }

        /// <summary>
        /// Removes Part or parts of path
        /// </summary>
        /// <param name="path">File or Folder path</param>
        /// <param name="count">Count of removal</param>
        /// <returns>New path without X parts or given path when count is higher than Path parts</returns>
        public static string RemovePathPart(string path, int count) => RemovePart(path, count);

        private static string RemovePart(string path, int count)
        {
            if (IsRootDir(path))
                return path;

            count += 1;

            path = path.EndsWith("\\") ? path.TrimEnd('\\') : path;
            string[] splitPath = path.Split('\\');

            if (count > splitPath.Count())
                return path;

            string _out = string.Empty;

            //C:\Ahoj\Jak\Je
            //C: 1. Ahoj 2. Jak 3. Je 4.
            // '\' count : 3

            count = (splitPath.Count() + 1) - count;

            for (int i = 0; i < count; i++)
            {
                _out += path.Split('\\')[i];
                _out += i != (count - 1) ? "\\" : string.Empty;
            }

            return _out;
        }

        private static string GetRootDir(string path)
        {
            if (!path.Contains('\\'))
                return "ERR";

            return path.Split('\\')[0];
        }

        private static bool IsRootDir(string path) => (path.TrimEnd('\\').Count(x => x == '\\') == 0);
    }
}