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
            /// ע�⣬����Ԫ�����Ǹ�AOT dll����Ԫ���ݣ������Ǹ��ȸ���dll����Ԫ���ݡ�
            /// �ȸ���dll��ȱԪ���ݣ�����Ҫ���䣬�������LoadMetadataForAOTAssembly�᷵�ش���
            /// 
            HomologousImageMode mode = HomologousImageMode.SuperSet;
            foreach (var aotDllName in AOTMetaAssemblyNames)
            {
                byte[] dllBytes = GetAssetData(aotDllName);
                // ����assembly��Ӧ��dll�����Զ�Ϊ��hook��һ��aot���ͺ�����native���������ڣ��ý������汾����
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
            Debug.Log("��Aot���� -----> ��Դ�������");
        };

        // ���س���
        Assembly ass = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "ManagerHotFix");
        Type ModuleManagerType = ass.GetType("Assets.ManagerHotFix.JFramework.Manager.ModuleManager"); // ���ModuleManager��
        MethodInfo moduleManagerIns = ModuleManagerType.BaseType.GetMethod("GetInstance"); //��û��൥��
        object instance = moduleManagerIns.Invoke(null,null); // ʵ��������

        MethodInfo moduleManagerOpenModule = ModuleManagerType.GetMethod("OpenUpdateModule"); // ��ÿ���UI����

        Type startType = ass.GetType("Assets.ManagerHotFix.JFramework.Update.UpdateModule");  // ���Ҫ��UI��module

        object[] parameters = { startType, openUpdateCallBack,null }; // �����Ĳ���
        moduleManagerOpenModule.Invoke(instance, parameters); // ʹ�÷�����
#else
            Assembly ass = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "Assembly-CSharp");
            Type ModuleManagerType = ass.GetType("Assets.HotFix.Game.GameManager"); // ���ModuleManager��
            MethodInfo moduleManagerIns = ModuleManagerType.BaseType.GetMethod("GetInstance"); //��û��൥��
            object instance = moduleManagerIns.Invoke(null, null); // ʵ��������

            MethodInfo moduleManagerOpenModule = ModuleManagerType.GetMethod("HotFixGameStart"); // ��ÿ���UI����

            moduleManagerOpenModule.Invoke(instance, null); // ʹ�÷�����

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

