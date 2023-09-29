namespace Hsenl {
    public static class PathHelper {
        public static string GetFolderPath(string path, char split = '/') {
            return path[..path.LastIndexOf(split)];
        }
    }
}