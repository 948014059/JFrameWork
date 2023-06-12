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
    public class AssetBundleTools : EditorWindow
    {
        private static AssetBundleTools AssetBundelToolsWin;
        public static string ProjectResourcesPath;
        public static string ABSavePath;
        public static string VersionName;

        public static string Version = "1.0.0";
#if UNITY_STANDALONE_WIN
        public static string PlatFrom = "StandaloneWindows64";
#elif UNITY_ANDROID
        public static string PlatFrom = "Android";
#endif
        public static bool IsGenHotFix = true;
        public bool IsCopyAB2StreamingAssets = false;
        public bool IsReBuild = false;
        public int TargetPlatformId = 0;

        public BuildTarget[] buildTargets = new BuildTarget[]
        {
            BuildTarget.StandaloneWindows64,
            BuildTarget.Android,
            BuildTarget.iOS,
        };


        private void Awake()
        {
            Reset();

        }

        void Reset()
        {
            ProjectResourcesPath = Application.dataPath + "/ProjectResources/";
            ABSavePath = Application.dataPath.Replace("Assets", "ProjectResources/");
            VersionName = PlatFrom + "/VersionConfig.txt";
        }


        [MenuItem("打包工具/AssetBundelTools")]
        static void ShowWindow()
        {
            Caching.ClearCache();
            AssetBundelToolsWin = GetWindow<AssetBundleTools>(true);
            AssetBundelToolsWin.Show();
        }




        [MenuItem("打包工具/Build/BuildWithOutAB")]
        void BuildHotFixAndVersion()
        {
            Reset();
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
        }

        private void OnGUI()
        {
            ShowSwitchMacro();
            GUILayout.Label("请输入版本号：");
            Version = GUILayout.TextArea(Version);
            IsGenHotFix = GUILayout.Toggle(IsGenHotFix, "是否重新生成hotfix");
            IsReBuild = GUILayout.Toggle(IsReBuild, "是否重新生成AB包");
            IsCopyAB2StreamingAssets = GUILayout.Toggle(IsCopyAB2StreamingAssets, "是否将AB包复制到StreamingAssets(打完整包使用)");

            TargetPlatformId = GUILayout.Toolbar(TargetPlatformId, new[] { "Window", "Android", "Ios" });
            if (GUILayout.Button("开始打AB包"))
            {
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

            CreateHotfixDllAndCopy(target);
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
            //Debug.Log(versionPath);
            if (File.Exists(versionPath)) File.Delete(versionPath);
            JsonData versonJson = new JsonData();
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
                    allLength += fileLen;
                    md5Dict.Add(fileName, fileMd5 + "+" + fileLen);
                }

            }

            JsonData filesdata = new JsonData();
            foreach (var Dictitem in md5Dict)
            {
                JsonData jd = new JsonData();
                jd["file"] = Dictitem.Key;
                string[] nAndL = Dictitem.Value.Split('+');
                jd["md5"] = nAndL[0];
                jd["length"] = nAndL[1];
                filesdata.Add(jd);
            }

            versonJson["version"] = Version;
            versonJson["length"] = allLength;
            versonJson["files"] = filesdata;
            File.WriteAllText(versionPath, versonJson.ToJson());
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
