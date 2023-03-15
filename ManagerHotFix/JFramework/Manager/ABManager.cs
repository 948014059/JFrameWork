using Assets.ManagerHotFix.JFramework.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

namespace Assets.ManagerHotFix.JFramework.Manager
{
    /// <summary>
    /// AB 包管理
    /// </summary>
    public class ABManager : BaseSingleTon<ABManager>
    {

        private AssetBundle AbMain;
        private AssetBundleManifest ABManifest;
        private Dictionary<string, AssetBundle> ABDict = new Dictionary<string, AssetBundle>();
        public delegate void GameObjLoadAsyncCallBack(string str, float pro);


        //private List<string> loadingAssetName = new List<string>();
        private void Awake()
        {
            InitData();
        }

        private void OnDestroy()
        {
            DestoryAllAB();
        }

        /// <summary>
        /// 初始化AB包
        /// </summary>
        public void InitData()
        {
            LoadMainABWithManifest();
        }

        /// <summary>
        /// 卸载所有Ab包
        /// </summary>
        public void DestoryAllAB()
        {
            AbMain.Unload(false);
            foreach (var item in ABDict)
            {
                item.Value.Unload(false);
            }
            ABDict.Clear();
        }

        /// <summary>
        /// 重新加载新的AB包
        /// </summary>
        public void ReLoadAssetBundle()
        {
            DestoryAllAB();
            InitData();
        }


        /// <summary>
        /// 加载GameObject
        /// </summary>
        /// <param name="_name"></param>
        /// <returns></returns>
        public GameObject GetGameObject(string _name)
        {
            //Debug.Log("使用AssetBundle加载资源");
            string name = _name.ToLower();
            GetAssetBundelWithAllDepend(name);
            if (ABDict.ContainsKey(name))
            {
                string[] nameSplit = name.Split("/");
                return ABDict[name].LoadAsset<GameObject>(nameSplit[nameSplit.Length - 1]);
            }
            Debug.Log("未加载ab包");
            return null;
        }

        public T GetAssetFromAB<T>(string _name) where T : UnityEngine.Object
        {
            string name = _name.ToLower();
            GetAssetBundelWithAllDepend(name);
            if (ABDict.ContainsKey(name))
            {
                string[] nameSplit = name.Split("/");
                return ABDict[name].LoadAsset<T>(nameSplit[nameSplit.Length - 1]);
            }
            Debug.Log("未加载ab包");
            return null;
        }




        /// <summary>
        /// 协程在AB包中加载预制体
        /// </summary>
        /// <param name="_name">预制体名称</param>
        /// <param name="gameObjLoadAsyncCallBack">进度的回调</param>
        /// <param name="finishcallBack">完成时的回调</param>
        public void GetGameObjectAsync(string _name,
             GameObjLoadAsyncCallBack gameObjLoadAsyncCallBack,
            Action<GameObject> finishcallBack)
        {
            List<string> loadingAssetName = new List<string>();
            string name = _name.ToLower();
            GetAssetBundelAllDependName(name, loadingAssetName);
            StartCoroutine(LoadGameObjectDependAsycn(loadingAssetName, name,
                gameObjLoadAsyncCallBack,
                (go) => {
                    finishcallBack?.Invoke(go);
                }));
        }

        /// <summary>
        /// 协程加载AB包、并加载预制
        /// </summary>
        /// <param name="paths">依赖名称</param>
        /// <param name="name">预制体名称</param>
        /// <param name="gameObjLoadAsyncCallBack">进度的回调</param>
        /// <param name="callBack">完成时的回调</param>
        /// <returns></returns>
        public IEnumerator LoadGameObjectDependAsycn(List<string> paths, string name,
            GameObjLoadAsyncCallBack gameObjLoadAsyncCallBack,
            Action<GameObject> callBack)
        {
            int currIndex = 0;
            foreach (var item in paths)
            {
                AssetBundleCreateRequest assetBundlereq = AssetBundle.LoadFromFileAsync(Utils.Utils.GetAssetsPath() + item);
                yield return assetBundlereq;
                if (assetBundlereq.isDone)
                {
                    currIndex += 1;
                    ABDict.Add(item, assetBundlereq.assetBundle);
                    gameObjLoadAsyncCallBack("寻找资源中...", currIndex / 1.0f / paths.Count);
                }
            }
            if (ABDict.ContainsKey(name))
            {
                string[] nameSplit = name.Split("/");
                AssetBundleRequest abr = ABDict[name].LoadAssetAsync<GameObject>(nameSplit[nameSplit.Length - 1]);
                while (!abr.isDone)
                {
                    gameObjLoadAsyncCallBack("加载资源中...", (abr.progress * 100.0f) / 100.0f);
                    yield return new WaitForSeconds(0.1f);
                }
                if (abr.isDone)
                {
                    callBack?.Invoke(abr.asset as GameObject);
                }
            }
            else
            {
                callBack?.Invoke(null);
            }

        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="_name"></param>
        /// <returns></returns>
        public Sprite GetSprite(string _name)
        {
            string name = _name.ToLower();
            GetAssetBundelWithAllDepend(name);
            if (ABDict.ContainsKey(name))
            {
                string[] nameSplit = name.Split("/");
                return ABDict[name].LoadAsset<Sprite>(nameSplit[nameSplit.Length - 1]);
            }
            Debug.Log("未加载ab包");
            return null;
        }

        /// <summary>
        /// 获取图集中的图片
        /// </summary>
        /// <param name="_atlasName"></param>
        /// <param name="string_name"></param>
        /// <returns></returns>
        public Sprite GetSpriterFromAtlas(string _atlasName, string string_name)
        {
            string atlasName = _atlasName.ToLower();
            GetAssetBundelWithAllDepend(atlasName);
            if (ABDict.ContainsKey(atlasName))
            {
                string[] nameSplit = atlasName.Split("/");
                SpriteAtlas spa = ABDict[atlasName].LoadAsset<SpriteAtlas>(nameSplit[nameSplit.Length - 1]);
                if (spa != null)
                {
                    return spa.GetSprite(string_name);
                }
            }
            return null;
        }

        /// <summary>
        /// 获取TextAssets
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        public string GetTextAssets(string _path)
        {
            string path = _path.ToLower();
            GetAssetBundelWithAllDepend(path);
            if (ABDict.ContainsKey(path))
            {
                string[] nameSplit = path.Split("/");
                return ABDict[path].LoadAsset<TextAsset>(nameSplit[nameSplit.Length - 1]).text;
            }
            return null;
        }


        public AudioClip GetAudioClip(string _path)
        {
            string path = _path.ToLower();
            GetAssetBundelWithAllDepend(path);
            if (ABDict.ContainsKey(path))
            {
                string[] nameSplit = path.Split("/");
                return ABDict[path].LoadAsset<AudioClip>(nameSplit[nameSplit.Length - 1]);
            }
            return null;
        }


        /// <summary>
        /// 卸载
        /// </summary>
        /// <param name="_name"></param>
        public void UnloadABundle(string _name)
        {
            string name = _name.ToLower();
            UnloadABundleWithAllDepend(name);
        }


        /// <summary>
        /// 卸载AB包
        /// </summary>
        /// <param name="path"></param>
        private void UnloadABundleWithAllDepend(string path)
        {
            string[] dependName = ABManifest.GetAllDependencies(path);
            foreach (var item in dependName)
            {
                UnloadABundleWithAllDepend(item);
            }
            ABDict[path].Unload(false);
            ABDict.Remove(path);
        }

        /// <summary>
        /// 加载AB包 以及其对应的资源
        /// </summary>
        /// <param name="path"></param>
        private void GetAssetBundelWithAllDepend(string path)
        {
            if (ABDict.ContainsKey(path))
            {
                return;
            }
            else
            {
                string[] dependName = ABManifest.GetAllDependencies(path);
                foreach (var item in dependName)
                {
                    //Debug.Log(path + "   " + item);
                    GetAssetBundelWithAllDepend(item);
                }
                AssetBundle newab = LoadABundleWithPath(path);
                ABDict.Add(path, newab);
            }
        }

        /// <summary>
        /// 获取所有的依赖名字
        /// </summary>
        /// <param name="path"></param>
        private void GetAssetBundelAllDependName(string path, List<string> loadingAssetName)
        {
            if (ABDict.ContainsKey(path) || loadingAssetName.Contains(path))
            {
                return;
            }
            else
            {
                loadingAssetName.Add(path);
                string[] dependName = ABManifest.GetAllDependencies(path);
                foreach (var item in dependName)
                {
                    GetAssetBundelAllDependName(item, loadingAssetName);
                }
            }
        }



        /// <summary>
        /// 通过路径加载AB包
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private AssetBundle LoadABundleWithPath(string path)
        {
            AssetBundle assetBundel = null;
            try
            {
                assetBundel = AssetBundle.LoadFromFile(Utils.Utils.GetAssetsPath() + path);
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
            return assetBundel;
        }

        /// <summary>
        /// 加载主包和依赖文件
        /// </summary>
        private void LoadMainABWithManifest()
        {
            try
            {
                string platFrom = Config.PlatFrom;

                AbMain = AssetBundle.LoadFromFile(Utils.Utils.GetAssetsPath() + platFrom);
                ABManifest = AbMain.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            }
            catch (System.Exception e)
            {

                Debug.Log(e);
            }
        }


    }

}
