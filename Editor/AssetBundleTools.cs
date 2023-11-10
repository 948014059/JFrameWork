//using Assets.AotScript.Script;
//using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Commands;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public class VersionData
    {
        public string apkPath;
        public string apkMd5;
        public long apkLength;
        public string version;
        public long length;
        public List<FileData> filedatas = new List<FileData>();
    }

    public class FileData
    {
        public string filename;
        public string md5;
        public long length;
    }

    public class AssetBundleTools : EditorWindow
    {
        private static AssetBundleTools AssetBundelToolsWin;
        public static string ProjectResourcesPath;
        public static string ABSavePath;
        public static string VersionName;
        public static string ApkDir;
        public static string ApkPath;

        public static string Version = "1.0.0";

#if UNITY_STANDALONE_WIN
        public static string PlatFrom = "StandaloneWindows64";
#elif UNITY_ANDROID
        public static string PlatFrom = "Android";
#elif UNITY_WEBGL
        public static string PlatFrom = "WebGL";
#endif
        public static bool IsGenHotFix = true;
        public static bool IsBigUpdate = false;
        public bool IsCopyAB2StreamingAssets = false;
        public bool IsReBuild = false;
        public int TargetPlatformId = 0;

        public BuildTarget[] buildTargets = new BuildTarget[]
        {
            BuildTarget.StandaloneWindows64,
            BuildTarget.Android,
            BuildTarget.iOS,
            BuildTarget.WebGL,
        };


        private void Awake()
        {
            Reset();
            TargetPlatformId = buildTargets.ToList().IndexOf(EditorUserBuildSettings.activeBuildTarget);
        }

        void Reset()
        {
            ProjectResourcesPath = Application.dataPath + "/ProjectResources/";
            ABSavePath = Application.dataPath.Replace("Assets", "ProjectResources/");
            VersionName = PlatFrom + "/VersionConfig.txt";
            ApkDir   = Application.dataPath.Replace("Assets", "BuildApk/"); 
            ApkPath  = ApkDir + Application.productName + ".apk";
        }


        [MenuItem("打包工具/AssetBundelTools")]
        static void ShowWindow()
        {
            Caching.ClearCache();
            AssetBundelToolsWin = GetWindow<AssetBundleTools>(true);
            AssetBundelToolsWin.Show();
        }




        [MenuItem("打包工具/Build/BuildWithOutAB")]
        static void BuildHotFixAndVersion()
        {
            //Reset();
            ProjectResourcesPath = Application.dataPath + "/ProjectResources/";
            ABSavePath = Application.dataPath.Replace("Assets", "ProjectResources/");
            VersionName = PlatFrom + "/VersionConfig.txt";
            ApkDir = Application.dataPath.Replace("Assets", "BuildApk/");
            ApkPath = ApkDir + Application.productName + ".apk";
            IsGenHotFix = true;
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            CreateHotfixDllAndCopy(target);
            SetVersionJson(target);
        }

        [MenuItem("打包工具/Build/CopyAB2StreamingAssetsDir")]
        void CopyAssetBundle2StreamingAssets()
        {
            Reset();
            CopyFilesRecursively(ABSavePath, Application.streamingAssetsPath + "/ProjectResources/");
        }



        private void ShowSwitchMacro()
        {
            if (GUILayout.Button("打包模式"))
            {
                Dictionary<string, bool> ABMacro = new Dictionary<string, bool>() { { "ASSETBUNDLE", true },
                { "DEV" , true} };
                ChangeDefineSymbols(ABMacro);
            }
            if (GUILayout.Button("编辑器模式"))
            {
                Dictionary<string, bool> ABMacro = new Dictionary<string, bool>() { { "ASSETBUNDLE", false }
                ,{ "DEV" , true}};
                ChangeDefineSymbols(ABMacro);
            }
            if (GUILayout.Button("测试环境"))
            {
                Dictionary<string, bool> ABMacro = new Dictionary<string, bool>() { { "ASSETBUNDLE", true },
                { "DEV" , true} };
                ChangeDefineSymbols(ABMacro);
            }
            if (GUILayout.Button("正式环境"))
            {
                Dictionary<string, bool> ABMacro = new Dictionary<string, bool>() { { "ASSETBUNDLE", true },
                { "DEV" , false} };
                ChangeDefineSymbols(ABMacro);
            }
            GUILayout.Space(20);
        }

        private void BuildAPK()
        {           
            if (!Directory.Exists(ApkDir))
            {
                Directory.CreateDirectory(ApkDir);
            }            
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            PlayerSettings.bundleVersion = Version;
            PlayerSettings.Android.bundleVersionCode = int.Parse(Version[0].ToString());
            //EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            BuildOptions buildOption = BuildOptions.None;
#if DEV
            buildOption |= BuildOptions.Development;
#else
            buildOption &= BuildOptions.Development;
#endif
            BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, ApkPath, BuildTarget.Android, buildOption);
            Debug.Log("------------- 结束 BuildAPK -------------");
            Debug.Log("Build目录：" + ApkPath);

        }

        private void OnGUI()
        {
            ShowSwitchMacro();
            GUILayout.Label("请输入版本号：");
            Version = GUILayout.TextArea(Version);
            IsBigUpdate = GUILayout.Toggle(IsBigUpdate, "是否打包apk(大版本更新/第一次打包需要勾选)");

            IsGenHotFix = GUILayout.Toggle(IsGenHotFix, "是否重新生成hotfix");
            IsReBuild = GUILayout.Toggle(IsReBuild, "是否重新生成AB包");
            IsCopyAB2StreamingAssets = GUILayout.Toggle(IsCopyAB2StreamingAssets, "是否将AB包复制到StreamingAssets(打完整包使用)");

            TargetPlatformId = GUILayout.Toolbar(TargetPlatformId, new[] { "Window", "Android", "Ios" ,"WebGL" });



            if (GUILayout.Button("开始打包"))
            {
                if (IsBigUpdate && TargetPlatformId == 1)
                {
                    BuildAPK();
                }           

                //Debug.Log(Version + "" + TargetPlatformId);
                Build(buildTargets[TargetPlatformId], Version);
            }
        }


        /// <summary>
        /// 使用宏切换模式
        /// </summary>
        /// <param name="ABMacro"></param>
        private void ChangeDefineSymbols(Dictionary<string, bool> ABMacro)
        {
            string Macro = string.Empty;
            foreach (var item in ABMacro)
            {
                if (item.Value)
                {
                    Macro += string.Format("{0};", item.Key);
                }
            }
            BuildTargetGroup buildTarget = EditorUserBuildSettings.selectedBuildTargetGroup;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTarget, Macro);

        }


        /// <summary>
        /// 打包
        /// </summary>
        /// <param name="target"></param>
        /// <param name="version"></param>
        private void Build(BuildTarget target, string version)
        {
            Caching.ClearCache();
            string[] filePaths = Directory.GetDirectories(ProjectResourcesPath, "*.*", SearchOption.TopDirectoryOnly);
            string savePath = ABSavePath + target.ToString();

            if (IsReBuild)
            {
                DeleteOldBundelFiles(savePath);
            }


            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            SetAssetBundlesName(filePaths);
            BuildPipeline.BuildAssetBundles(savePath, BuildAssetBundleOptions.ChunkBasedCompression, target);

            // 复制热更dll
            CreateHotfixDllAndCopy(target);
            // 复制apk到AB包内
            if (File.Exists(ApkPath))
            {
                string apkPath = ApkPath;
                string newCopyPath = ApkPath.Replace(ApkDir, ABSavePath + PlatFrom + "/");
                File.Copy(apkPath, newCopyPath, true);
            }
            else
            {
                Debug.LogError("BuildApk 路径下不存在apk，请勾选打包apk选项");
                return;
            }

            SetVersionJson(target);

            if (IsCopyAB2StreamingAssets)
            {
                CopyFilesRecursively(ABSavePath, Application.streamingAssetsPath + "/ProjectResources/");
            }


        }

        private static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }

        private static Dictionary<string, string> md5Dict = new Dictionary<string, string>();

        /// <summary>
        /// 生成版本对比文件
        /// </summary>
        ///  
        private static void SetVersionJson(BuildTarget target)
        {
            string versionPath = ABSavePath + target.ToString() + VersionName.Replace(target.ToString(), "");
            if (File.Exists(versionPath)) File.Delete(versionPath);
            
            //JsonData versonJson = new JsonData();
            VersionData versionData = new VersionData();

            string[] allFiles = Directory.GetFiles(ABSavePath + target.ToString(), "*.*", SearchOption.AllDirectories);
            md5Dict.Clear();

            long allLength = 0;
            foreach (var item in allFiles)
            {
                string path = ABSavePath + target.ToString() + "/";
                string fileName = item.Replace("\\", "/").Replace(path, "");
                string extension = Path.GetExtension(item);
                if (extension == ".manifest" && fileName != target.ToString() + ".manifest")
                {
                    if (File.Exists(fileName))
                    {
                        Debug.Log("---->" + fileName);
                        File.Delete(fileName);
                    }
                    continue;
                }
                if (extension != ".meta")
                {
                    string fileMd5 = GetMD5HashFromFile(item);
                    int fileLen = File.ReadAllBytes(item).Length;
                    if (extension != ".apk")
                    {
                        allLength += fileLen;
                    }                   
                    md5Dict.Add(fileName, fileMd5 + "+" + fileLen);
                }



            }

            //JsonData filesdata = new JsonData();
            List<FileData> filedatas = new List<FileData>();
            foreach (var Dictitem in md5Dict)
            {
                string[] nAndL = Dictitem.Value.Split('+');

                if (Dictitem.Key.Contains(".apk"))
                {
                    versionData.apkPath = Dictitem.Key;
                    versionData.apkMd5 = nAndL[0];
                    versionData.apkLength = long.Parse(nAndL[1]);
                    continue;
                }
                FileData filedata = new FileData();
                //JsonData jd = new JsonData();
                filedata.filename = Dictitem.Key;
                filedata.md5 = nAndL[0];
                filedata.length = long.Parse(nAndL[1]);
                filedatas.Add(filedata);
            }

            versionData.version = Version;
            versionData.length = allLength;
            versionData.filedatas = filedatas;
            File.WriteAllText(versionPath, JsonMapper.ToJson(versionData));
            md5Dict.Clear();


        }


        /// <summary>
        /// 将文件转换为md5字符
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetMD5HashFromFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            try
            {
                if (File.Exists(fileName))
                {
                    FileStream file = File.OpenRead(fileName);
                    MD5 md5 = new MD5CryptoServiceProvider();
                    byte[] retVal = md5.ComputeHash(file);
                    file.Close();
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < retVal.Length; i++)
                    {
                        sb.Append(retVal[i].ToString("x2"));

                    }
                    return sb.ToString();
                }
                return null;
            }
            catch (Exception e)
            {
                return null;

            }
        }


        /// <summary>
        /// 生成最新的HotFix.dll文件
        /// </summary>
        /// <param name="target"></param>
        private static void GenHotFixDll(BuildTarget target)
        {
            BuildAssetsCommand.BuildAssetBundleByTarget(target);
            CompileDllCommand.CompileDll(target);
            BuildAssetsCommand.CopyABAOTHotUpdateDlls(target);
        }

        /// <summary>
        /// 将生成的hotFix dll 复制到AB包保存目录中
        /// </summary>
        /// <param name="target"></param>
        private static void CreateHotfixDllAndCopy(BuildTarget target)
        {
            if (IsGenHotFix)
            {
                GenHotFixDll(target);
            }

            string hotFixPath = Application.streamingAssetsPath + "/Assembly-CSharp.dll.bytes";
            string newPath = ABSavePath + target.ToString() + "/Assembly-CSharp.dll.bytes";
            File.Copy(hotFixPath, newPath, true);

            string managerHotFixPath = Application.streamingAssetsPath + "/ManagerHotfix.dll.bytes";
            string newmanagerPath = ABSavePath + "ManagerHotfix.dll.bytes";
            File.Copy(managerHotFixPath, newmanagerPath, true);


            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 设置AB包名称
        /// </summary>
        /// <param name="projectPaths"></param>
        private void SetAssetBundlesName(string[] projectPaths)
        {
            foreach (var path in projectPaths)
            {
                //Debug.Log(path);
                string[] childPaths = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                string childPathName, extension, directoryName;
                foreach (var childPath in childPaths)
                {
                    extension = Path.GetExtension(childPath);
                    Debug.Log("------->" + extension);
                    if (extension != ".meta" && extension != ".DS_Store")
                    {
                        childPathName = Path.GetFileNameWithoutExtension(childPath);
                        directoryName = Path.GetDirectoryName(childPath).Replace("\\", "/");

                        AssetImporter aImp = AssetImporter.GetAtPath(childPath.Replace(Application.dataPath, "Assets"));
                        string abName = directoryName.Replace(ProjectResourcesPath, "") + "/" + childPathName;
                        aImp.assetBundleName = abName;

                    }
                }
            }
        }


        /// <summary>
        /// 删除旧的AB包
        /// </summary>
        /// <param name="savePath"></param>
        private void DeleteOldBundelFiles(string savePath)
        {
            Debug.Log(savePath + Directory.Exists(savePath));
            if (Directory.Exists(savePath))
            {
                // Directory.Delete(savePath);
                DirectoryInfo di = new DirectoryInfo(savePath);
                di.Delete(true);
            }


        }


    }
}
