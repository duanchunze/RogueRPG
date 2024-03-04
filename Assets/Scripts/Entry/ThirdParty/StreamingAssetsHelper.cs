using System.IO;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

/// <summary>
/// 资源文件查询服务类
/// </summary>
public class GameQueryServices : IBuildinQueryServices {
    /// <summary>
    /// 查询内置文件的时候，是否比对文件哈希值
    /// </summary>
    public static bool CompareFileCRC = false;

    public bool Query(string packageName, string fileName, string fileCRC) {
        // 注意：fileName包含文件格式
        return StreamingAssetsHelper.FileExists(packageName, fileName, fileCRC);
    }
}

#if UNITY_EDITOR
public sealed class StreamingAssetsHelper {
    public static void Init() { }

    public static bool FileExists(string packageName, string fileName, string fileCRC) {
        string filePath = Path.Combine(Application.streamingAssetsPath, StreamingAssetsDefine.RootFolderName, packageName, fileName);
        if (File.Exists(filePath)) {
            if (GameQueryServices.CompareFileCRC) {
                string crc32 = YooAsset.Editor.EditorTools.GetFileCRC32(filePath);
                return crc32 == fileCRC;
            }
            else {
                return true;
            }
        }
        else {
            return false;
        }
    }
}
#else
public sealed class StreamingAssetsHelper
{
    private class PackageQuery
    {
        public readonly Dictionary<string, BuildinFileManifest.Element> Elements = new Dictionary<string, BuildinFileManifest.Element>(1000);
    }

    private static bool _isInit = false;
    private static readonly Dictionary<string, PackageQuery> _packages = new Dictionary<string, PackageQuery>(10);

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init()
    {
        if (_isInit == false)
        {
            _isInit = true;

            var manifest = Resources.Load<BuildinFileManifest>("BuildinFileManifest");
            if (manifest != null)
            {
                foreach (var element in manifest.BuildinFiles)
                {
                    if (_packages.TryGetValue(element.PackageName, out PackageQuery package) == false)
                    {
                        package = new PackageQuery();
                        _packages.Add(element.PackageName, package);
                    }
                    package.Elements.Add(element.FileName, element);
                }
            }
        }
    }

    /// <summary>
    /// 内置文件查询方法
    /// </summary>
    public static bool FileExists(string packageName, string fileName, string fileCRC32)
    {
        if (_isInit == false)
            Init();

        if (_packages.TryGetValue(packageName, out PackageQuery package) == false)
            return false;

        if (package.Elements.TryGetValue(fileName, out var element) == false)
            return false;

        if (GameQueryServices.CompareFileCRC)
        {
            return element.FileCRC32 == fileCRC32;
        }
        else
        {
            return true;
        }
    }
}
#endif
