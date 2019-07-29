using System.IO;

namespace AppBase0500
{
    public class Util
    {

        /// <summary>
        /// Create Folder
        /// </summary>
        /// <param name="path">path to be checked</param>
        public static void CreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Create Folder from filename
        /// </summary>
        /// <param name="filename">filename</param>
        public static void CreateFolderFromFile(string filename)
        {
            var fi = new FileInfo(filename);
            if (!Directory.Exists(fi.DirectoryName))
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }
        }
    }
}
