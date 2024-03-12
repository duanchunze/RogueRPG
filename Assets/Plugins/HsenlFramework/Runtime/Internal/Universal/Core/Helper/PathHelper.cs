using System;

namespace Hsenl {
    public static class PathHelper {
        public static string GetFolderPath(string path, char split = '/') {
            return path[..path.LastIndexOf(split)];
        }

        public static string GetLocalPath(string path, string relativePath) {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            return path.Replace(relativePath, "");
        }
    }
}