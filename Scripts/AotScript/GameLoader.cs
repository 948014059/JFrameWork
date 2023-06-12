using Assets.AotScript.Script;
using HybridCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.AotScript
{

    public class GameLoader : MonoBehaviour
    {

        public static GameLoader Instance;
        public static Dictionary<string, byte[]> s_assetDatas = new Dictionary<string, byte[]>();
        public static List<string> AOTMetaAssemblyNames { get; } = new List<string>()
        {
            "mscorlib.dll",
            "System.dll",
            "System.Core.dll",
        };

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        void Start()
        {
            StartCoroutine(DownLoadAssets(this.StartGame));
        }

        public byte[] GetAssetData(string dllName)
        {
            return s_assetDatas[dllName];
        }


        public string GetWebRequestPath(string asset, string abPath = "", string platFrom = "")
        {
            var path = $"{Application.streamingAssetsPath}/{asset}";

            if (asset == "Assembly-CSharp.dll")
            {
                var hotfixabPath = abPath + platFrom + "/Assembly-CSharp.dll.bytes";
                if (File.Exists(hotfixabPath))
                {
                    return "file://" + hotfixabPath;
                }
            }

            if (asset == "ManagerHotFix.dll")
            {
                
#if UNITY_EDITOR
                return  Application.streamingAssetsPath+ "/ManagerHotFix.dll.bytes";
#else
                return BaseUtils.HotFixServerUrl + BaseUtils.ManagerHotFixFilePath;
#endif
            }

            if (!path.Contains("://"))
            {
                path = "file://" + path;
            }
            if (path.EndsWith(".dll"))
            {

                path += ".bytes";
            }
            return path;
        }

        public void LoadMetadataForAOTAssemblies()
        {
            /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
            /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
            /// 
            HomologousImageMode mode = HomologousImageMode.SuperSet;
            foreach (var aotDllName in AOTMetaAssemblyNames)
            {
                byte[] dllBytes = GetAssetData(aotDllName);
                // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
                LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
                Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
            }
        }

        void StartGame()
        {
            LoadMetadataForAOTAssemblies();


#if ASSETBUNDLE
        System.Reflection.Assembly.Load(GetAssetData("ManagerHotFix.dll"));
        Action openUpdateCallBack = () => {
            Debug.Log("从Aot程序 -----> 资源管理程序");
        };

        // 加载程序集
        Assembly ass = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "ManagerHotFix");
        Type ModuleManagerType = ass.GetType("Assets.ManagerHotFix.JFramework.Manager.ModuleManager"); // 获得ModuleManager类
        MethodInfo moduleManagerIns = ModuleManagerType.BaseType.GetMethod("GetInstance"); //获得基类单例
        object instance = moduleManagerIns.Invoke(null,null); // 实例化单例

        MethodInfo moduleManagerOpenModule = ModuleManagerType.GetMethod("OpenUpdateModule"); // 获得开开UI方法

        Type startType = ass.GetType("Assets.ManagerHotFix.JFramework.Update.UpdateModule");  // 获得要打开UI的module

        object[] parameters = { startType, openUpdateCallBack,null }; // 方法的参数
        moduleManagerOpenModule.Invoke(instance, parameters); // 使用方法。
#else
            Assembly ass = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "Assembly-CSharp");
            Type ModuleManagerType = ass.GetType("Assets.HotFix.Game.GameManager"); // 获得ModuleManager类
            MethodInfo moduleManagerIns = ModuleManagerType.BaseType.GetMethod("GetInstance"); //获得基类单例
            object instance = moduleManagerIns.Invoke(null, null); // 实例化单例

            MethodInfo moduleManagerOpenModule = ModuleManagerType.GetMethod("HotFixGameStart"); // 获得开开UI方法

            moduleManagerOpenModule.Invoke(instance, null); // 使用方法。

#endif


        }


        public IEnumerator DownloadHotFixAssets(string name, Action callback, string abPath = "", string platFrom = "")
        {
            string dllPath = GetWebRequestPath(name, abPath, platFrom);
            Debug.Log($"start download asset:{dllPath}");
            UnityWebRequest www = UnityWebRequest.Get(dllPath);
            yield return www.SendWebRequest();
#if UNITY_2020_1_OR_NEWER
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
#else
            if (www.isHttpError || www.isNetworkError)
            {
                Debug.Log(www.error);
            }
#endif
            else
            {
                // Or retrieve results as binary data
                byte[] assetData = www.downloadHandler.data;
                Debug.Log($"dll:{name}  size:{assetData.Length}");
                s_assetDatas[name] = assetData;
            }
            callback?.Invoke();
        }

        private IEnumerator DownLoadAssets(Action onDownloadComplete)
        {
            var assets = new List<string>
        {
            "ManagerHotFix.dll",
            //"Assembly-CSharp.dll",
        }.Concat(AOTMetaAssemblyNames);

            foreach (var asset in assets)
            {
                string dllPath = GetWebRequestPath(asset);
                Debug.Log($"start download asset:{dllPath}");
                UnityWebRequest www = UnityWebRequest.Get(dllPath);
                yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
#else
            if (www.isHttpError || www.isNetworkError)
            {
                Debug.Log(www.error);
            }
#endif
                else
                {
                    // Or retrieve results as binary data
                    byte[] assetData = www.downloadHandler.data;
                    Debug.Log($"dll:{asset}  size:{assetData.Length}");
                    s_assetDatas[asset] = assetData;
                }
            }

            onDownloadComplete();
        }



    }

}

