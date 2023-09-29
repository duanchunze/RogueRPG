using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Hsenl {
    public static class FileHelper {
        public static List<string> GetAllFiles(string dir, bool includeSub = true, string searchPattern = "*") {
            var list = new List<string>();
            GetAllFiles(list, dir, includeSub, searchPattern);
            return list;
        }

        public static void GetAllFiles(List<string> files, string dir, bool includeSub = true, string searchPattern = "*") {
            var fls = Directory.GetFiles(dir, searchPattern);
            foreach (var fl in fls) {
                files.Add(fl);
            }

            if (!includeSub) return;

            var subDirs = Directory.GetDirectories(dir);
            foreach (var subDir in subDirs) {
                GetAllFiles(files, subDir, true, searchPattern);
            }
        }

        // 清理目录, 除了我指定不删的文件, 把能删的文件都删了, 完了把空的文件夹也删了, 但不会删除根目录
        public static void CleanDirectory(string dir, params string[] exclusive) {
            if (!Directory.Exists(dir)) {
                return;
            }

            if (exclusive.Length == 0) {
                foreach (var subDir in Directory.GetDirectories(dir)) {
                    Directory.Delete(subDir, true);
                }

                foreach (var subFile in Directory.GetFiles(dir)) {
                    File.Delete(subFile);
                }

                return;
            }

            // exclusiveFoldPaths: 按文件夹路径去排除 
            // exclusiveFileNames: 按文件名去排除
            // exclusiveFilePaths: 按文件路径去排除
            // 清理的时候, 只会清理文件, 如果清理过后, 该文件夹空了, 则会删除该文件夹
            GetSearchPattern(exclusive, out var exclusiveFoldPaths, out var exclusiveFileNames, out var exclusiveFilePaths);

            RecursiveClean(dir);

            bool Searchs(string src, List<string> searchPattern) {
                foreach (var exclusiveFile in searchPattern) {
                    if (SearchMatching(src, exclusiveFile)) {
                        return true;
                    }
                }

                return false;
            }

            // 递归清理
            void RecursiveClean(string cleanDir) {
                if (Searchs(cleanDir, exclusiveFoldPaths)) {
                    return;
                }

                foreach (var subDir in Directory.GetDirectories(cleanDir)) {
                    RecursiveClean(subDir);
                }

                foreach (var subFile in Directory.GetFiles(cleanDir)) {
                    var fileName = Path.GetFileName(subFile);
                    if (Searchs(fileName, exclusiveFileNames)) {
                        continue;
                    }

                    if (Searchs(subFile, exclusiveFilePaths)) {
                        continue;
                    }

                    File.Delete(subFile);
                    Debug.LogError($"delete file '{subFile}'");
                }

                // 清理过后, 如果该文件夹空了, 则删除该文件夹
                if (cleanDir == dir) return; // 不会删除跟目录
                if (Directory.GetFiles(cleanDir).Length == 0 && Directory.GetDirectories(cleanDir).Length == 0) {
                    Directory.Delete(cleanDir);
                    Debug.LogError($"delete dir '{cleanDir}'");
                }
            }
        }

        // 清理指定目录里，指定匹配的某些文件或文件夹
        public static void CleanTargets(string dir, string[] targets) {
            GetSearchPattern(targets, out var exclusiveFoldPaths, out var exclusiveFileNames, out var exclusiveFilePaths);
            RecursiveCleanReverse(Directory.CreateDirectory(dir).FullName);

            bool Searchs(string src, List<string> searchPattern) {
                foreach (var exclusiveFile in searchPattern) {
                    if (SearchMatching(src, exclusiveFile)) {
                        return true;
                    }
                }

                return false;
            }

            void RecursiveCleanReverse(string cleanDir) {
                if (!Directory.Exists(cleanDir)) {
                    return;
                }

                if (Searchs(cleanDir, exclusiveFoldPaths)) {
                    Directory.Delete(cleanDir, true);
                    return;
                }

                foreach (var subFile in Directory.GetFiles(cleanDir)) {
                    var fileName = Path.GetFileName(subFile);
                    if (Searchs(fileName, exclusiveFileNames)) {
                        File.Delete(subFile);
                    }

                    if (Searchs(subFile, exclusiveFilePaths)) {
                        File.Delete(subFile);
                    }
                }

                foreach (var subDir in Directory.GetDirectories(cleanDir)) {
                    RecursiveCleanReverse(subDir);
                }

                if (Directory.GetFiles(cleanDir).Length == 0 && Directory.GetDirectories(cleanDir).Length == 0) {
                    Directory.Delete(cleanDir);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GetSearchPattern(string[] searchPatterns, out List<string> foldPaths, out List<string> fileNames, out List<string> filePaths) {
            foldPaths = new List<string>();
            fileNames = new List<string>();
            filePaths = new List<string>();
            foreach (var s in searchPatterns) {
                var str = s.Replace("/", "\\");
                var splits = str.Split('\\');

                // 首先排除这种情况 user/*data/json.txt，星号只允许加在第一段或者最后一段里面
                if (splits.Length > 2) {
                    for (var i = 1; i < splits.Length - 1; i++) {
                        if (splits[i].Contains('*')) {
                            throw new ArgumentException($"exclusive argument error: '{s}'");
                        }
                    }
                }

                var hasExtend = splits[^1].Contains('.');
                var hasSearchBack = splits[^1].Contains('*');
                var isName = splits.Length == 1;
                var hasSearchFront = (!isName || !hasSearchBack) && splits[0].Contains('*'); // 只有isName和hasSearchBack都为假的时候，才判断，否则的话，直接为false

                if (hasSearchFront && hasSearchBack) {
                    // 前后只能有一个搜索符
                    throw new ArgumentException($"exclusive argument error: '{s}'");
                }

                int GetCharCount(ref string src, char c) {
                    var num = 0;
                    for (var i = 0; i < src.Length; i++) {
                        if (src[i] == c) {
                            num++;
                        }
                    }

                    return num;
                }

                if (hasSearchFront) {
                    if (splits[0][0] != '*') {
                        // 如果搜索符在前段，那就必须在最开头
                        throw new ArgumentException($"exclusive argument error: '{s}'");
                    }

                    // 每段也只能有一个搜索符
                    if (GetCharCount(ref splits[0], '*') > 1) {
                        throw new ArgumentException($"exclusive argument error: '{s}'");
                    }
                }

                if (hasSearchBack) {
                    // 每段也只能有一个搜索符
                    if (GetCharCount(ref splits[^1], '*') > 1) {
                        throw new ArgumentException($"exclusive argument error: '{s}'");
                    }
                }

                if (isName) {
                    // json         （自动补充.*，匹配所有名叫json，后缀名不限的文件）
                    // json.*       （也就是上面补充后的效果，匹配所有名叫json，后缀名不限的文件）
                    // json.cn.*    （匹配所有名叫json.cn，后缀名不限的文件）
                    // json.txt     （全字符匹配同名文件）
                    // json*        （匹配json开头的任意文件，例如json.txt, json_cn.xlsx, json.cn.meta）
                    // json*.txt    （匹配json开头但限制了后缀名的文件，例如 json.txt, json_cn.txt, json.cn.txt）
                    if (!hasExtend && !hasSearchBack && !hasSearchFront) {
                        str = $"{str}.*";
                    }

                    fileNames.Add(str);
                }
                else {
                    // user/data/json.*       （匹配该路径下，所有名叫json，后缀名不限的文件）
                    // user/data/json.cn.*    （匹配该路径下，所有名叫json.cn，后缀名不限的文件）
                    // user/data/json.txt     （全字符匹配同名文件）
                    // user/data/json*        （匹配该路径下，以json开头的任意文件，例如json.txt, json_cn.xlsx, json.cn.meta）
                    // user/data/json*.txt    （匹配该路径下，以json开头但限制了后缀名的文件，例如json.txt, json_cn.txt, json.cn.txt）
                    if (hasExtend || hasSearchBack) {
                        var splitPath = GetSplitPath(str, "\\");
                        if (Directory.Exists(splitPath[0])) {
                            filePaths.Add(Directory.CreateDirectory(splitPath[0]).FullName + "\\" + splitPath[1]);
                        }
                    }
                    // user/data/json               （没后缀名，被视为文件夹目录，全字符匹配该文件夹目录）
                    // */user/data/json             （匹配该相对路径）
                    else {
                        if (hasSearchFront) {
                            foldPaths.Add(str);
                        }
                        else {
                            // 我们需要拿到譬如 ../../这种的完整路径，后面才能做匹配
                            if (Directory.Exists(str)) {
                                foldPaths.Add(Directory.CreateDirectory(str).FullName);
                            }
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool SearchMatching(string src, string searchPattern) {
            if (string.IsNullOrEmpty(src)) {
                return false;
            }

            if (string.IsNullOrEmpty(searchPattern)) {
                throw new ArgumentException("search pattern is null");
            }

            if (!searchPattern.Contains('*')) {
                return src == searchPattern;
            }

            var splits = searchPattern.Split('*');
            if (splits.Length > 2) {
                throw new ArgumentException("search pattern are allowed to contain only one *");
            }

            if (!string.IsNullOrEmpty(splits[0])) {
                if (!src.StartsWith(splits[0])) {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(splits[1])) {
                if (!src.EndsWith(splits[1])) {
                    return false;
                }
            }

            return true;
        }

        public static void CopyDirectory(string srcDir, string tgtDir) {
            var source = new DirectoryInfo(srcDir);
            var target = new DirectoryInfo(tgtDir);

            if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase)) {
                throw new Exception("父目录不能拷贝到子目录！");
            }

            if (!source.Exists) {
                return;
            }

            if (!target.Exists) {
                target.Create();
            }

            var files = source.GetFiles();

            for (var i = 0; i < files.Length; i++) {
                File.Copy(files[i].FullName, Path.Combine(target.FullName, files[i].Name), true);
            }

            var dirs = source.GetDirectories();

            for (var j = 0; j < dirs.Length; j++) {
                CopyDirectory(dirs[j].FullName, Path.Combine(target.FullName, dirs[j].Name));
            }
        }

        public static void ReplaceExtensionName(string srcDir, string extensionName, string newExtensionName) {
            if (Directory.Exists(srcDir)) {
                var fls = Directory.GetFiles(srcDir);

                foreach (var fl in fls) {
                    if (fl.EndsWith(extensionName)) {
                        File.Move(fl, fl.Substring(0, fl.IndexOf(extensionName)) + newExtensionName);
                        File.Delete(fl);
                    }
                }

                var subDirs = Directory.GetDirectories(srcDir);

                foreach (var subDir in subDirs) {
                    ReplaceExtensionName(subDir, extensionName, newExtensionName);
                }
            }
        }

        /// <summary>
        /// 获得所有目录信息
        /// </summary>
        /// <param name="self"></param>
        /// <param name="includeChildren"></param>
        /// <returns></returns>
        public static List<DirectoryInfo> GetAllDirectoryInfos(this DirectoryInfo self, bool includeChildren = true) {
            var directoryInfos = new List<DirectoryInfo>();
            var subDirInfos = self.GetDirectories();
            directoryInfos.AddRange(subDirInfos);

            if (includeChildren) {
                foreach (var subDirInfo in subDirInfos) {
                    GetAllDirectoryInfos(directoryInfos, subDirInfo, true);
                }
            }

            return directoryInfos;
        }

        /// <summary>
        /// 获得所有目录信息
        /// </summary>
        /// <param name="directoryInfos"></param>
        /// <param name="directoryInfo"></param>
        /// <param name="includeChildren"></param>
        public static void GetAllDirectoryInfos(List<DirectoryInfo> directoryInfos, DirectoryInfo directoryInfo, bool includeChildren = true) {
            var subDirInfos = directoryInfo.GetDirectories();
            directoryInfos.AddRange(subDirInfos);

            if (includeChildren) {
                foreach (var subDirInfo in subDirInfos) {
                    GetAllDirectoryInfos(directoryInfos, subDirInfo, true);
                }
            }
        }

        /// <summary>
        /// 获得所有子目录
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="includeChildren"></param>
        /// <returns></returns>
        public static List<string> GetAllDirectory(string dir, bool includeChildren = true) {
            var directoryList = new List<string>();
            var subDirs = Directory.GetDirectories(dir);
            directoryList.AddRange(subDirs);

            if (includeChildren) {
                foreach (var subDir in subDirs) {
                    GetAllDirectory(directoryList, subDir, true);
                }
            }

            return directoryList;
        }

        /// <summary>
        /// 获得所有子目录
        /// </summary>
        /// <param name="directoryList"></param>
        /// <param name="dir"></param>
        /// <param name="includeChildren"></param>
        public static void GetAllDirectory(List<string> directoryList, string dir, bool includeChildren = true) {
            var subDirs = Directory.GetDirectories(dir);
            directoryList.AddRange(subDirs);

            if (includeChildren) {
                foreach (var subDir in subDirs) {
                    GetAllDirectory(directoryList, subDir, true);
                }
            }
        }

        /// <summary>
        /// 获得所有文件目录
        /// </summary>
        /// <param name="self"></param>
        /// <param name="includeChildren"></param>
        /// <returns></returns>
        public static List<FileInfo> GetAllFileInfos(this DirectoryInfo self, bool includeChildren = true) {
            var fileInfos = new List<FileInfo>();
            GetAllFileInfos(fileInfos, self, includeChildren);

            return fileInfos;
        }

        /// <summary>
        /// 获得所有文件信息
        /// </summary>
        /// <param name="fileInfos"></param>
        /// <param name="directoryInfo"></param>
        /// <param name="includeChildren"></param>
        public static void GetAllFileInfos(List<FileInfo> fileInfos, DirectoryInfo directoryInfo, bool includeChildren = true) {
            var subFileInfos = directoryInfo.GetFiles();
            fileInfos.AddRange(subFileInfos);

            if (includeChildren) {
                foreach (var subDirectoryInfo in directoryInfo.GetDirectories()) {
                    GetAllFileInfos(fileInfos, subDirectoryInfo, true);
                }
            }
        }

        /// <summary>
        /// 移除空文件夹。
        /// </summary>
        /// <param name="directoryName">要处理的文件夹名称。</param>
        /// <returns>是否移除空文件夹成功。</returns>
        public static bool RemoveEmptyDirectory(string directoryName) {
            if (string.IsNullOrEmpty(directoryName)) {
                throw new Exception("Directory name is invalid.");
            }

            try {
                if (!Directory.Exists(directoryName)) {
                    return false;
                }

                // 不使用 SearchOption.AllDirectories，以便于在可能产生异常的环境下删除尽可能多的目录
                var subDirectoryNames = Directory.GetDirectories(directoryName, "*");
                var subDirectoryCount = subDirectoryNames.Length;
                foreach (var subDirectoryName in subDirectoryNames) {
                    if (RemoveEmptyDirectory(subDirectoryName)) {
                        subDirectoryCount--;
                    }
                }

                if (subDirectoryCount > 0) {
                    return false;
                }

                if (Directory.GetFiles(directoryName, "*").Length > 0) {
                    return false;
                }

                Directory.Delete(directoryName);
                return true;
            }
            catch {
                return false;
            }
        }

        /// <summary>
        /// 分割成路径和文件名
        /// </summary>
        /// <param name="path"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static string[] GetSplitPath(string path, string symbol = "/") {
            if (path.Contains(symbol) == false) {
                return new[] { "", path };
            }

            var firstHalf = path.Substring(0, path.LastIndexOf(symbol, StringComparison.Ordinal));
            var secondHalf = path.Substring(path.LastIndexOf(symbol, StringComparison.Ordinal) + 1);

            return new[] { firstHalf, secondHalf };
        }

        public static bool CopyFile(string sourcePath, string targetPath, bool overwrite) {
            string sourceText = null;
            string targetText = null;

            if (File.Exists(sourcePath)) {
                sourceText = File.ReadAllText(sourcePath);
            }

            if (File.Exists(targetPath)) {
                targetText = File.ReadAllText(targetPath);
            }

            if (sourceText != targetText && File.Exists(sourcePath)) {
                File.Copy(sourcePath, targetPath ?? throw new ArgumentNullException(nameof(targetPath)), overwrite);
                return true;
            }

            return false;
        }
    }
}