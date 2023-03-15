using System;
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
    /// 资源加载 区分编辑器模式  还是打包模式
    /// </summary>
    public static class ResourcesManager
    {
#if ASSETBUNDLE
    private static ABManager aBManager = ABManager.GetInstance();
#endif



        public static T GetAssets<T>(string _path) where T : UnityEngine.Object
        {
#if ASSETBUNDLE
        string pathExtension = Path.GetExtension(_path);
        string path = _path.Replace(pathExtension, "");
        return aBManager.GetAssetFromAB<T>(path);
#else

            return GetAssetsFromEditor<T>(_path);
#endif
        }

        /// <summary>
        /// 同步加载
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static GameObject GetPrefab(string path)
        {

#if !ASSETBUNDLE
            return GetGameObjectFromEditorPath(path);
#else
        return  aBManager.GetGameObject(path);
#endif
        }

        /// <summary>
        /// 协程加载
        /// </summary>
        /// <param name="path"></param>
        /// <param name="gameObjLoadAsyncCallBack"></param>
        /// <param name="callback"></param>
        public static void GetGameObjectAsync(string path, ABManager.GameObjLoadAsyncCallBack gameObjLoadAsyncCallBack, Action<GameObject> callback)
        {
#if !ASSETBUNDLE
            GetGameObjectAsyncFromEditor(path, callback);
#else
        aBManager.GetGameObjectAsync(path, gameObjLoadAsyncCallBack, callback);
#endif
        }


        public static Sprite GetSprite(string spritePath)
        {

#if !ASSETBUNDLE
            return GetSpriteFromEditor(spritePath);
#else
        return aBManager.GetSprite(spritePath);
#endif
        }

        public static Sprite GetSpriteFromAtlas(string atlasPath, string spriteName)
        {

#if !ASSETBUNDLE
            return GetSpriteFromEditorPath(atlasPath, spriteName);
#else
        return aBManager.GetSpriterFromAtlas(atlasPath, spriteName);

#endif
        }


        public static string GetTextAsset(string path)
        {
#if !ASSETBUNDLE
            return GetTxtDataFromEditorPath(path);
#else
        return  aBManager.GetTextAssets(path);
#endif
        }

        /// <summary>
        /// 这个是需要传拓展名的
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static AudioClip GetAudioClip(string path)
        {

#if !ASSETBUNDLE

            return GetAudioClipFromEditorPath(path);
#else
        string pathExtension = Path.GetExtension(path);
        string _path = path.Replace(pathExtension, "");
        return  aBManager.GetAudioClip(_path);
#endif
        }


#if !ASSETBUNDLE

        private static T GetAssetsFromEditor<T>(string path) where T : UnityEngine.Object
        {
            string GoPath = Config.EditorPath + path;
            if (File.Exists(Application.dataPath.Replace("Assets", "") + GoPath))
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(GoPath) as T;
            }
            else
            {
                Debug.Log("未找到资源：" + path);
            }
            return null;
        }

        private static void GetGameObjectAsyncFromEditor(string path, Action<GameObject> callback)
        {
            string GoPath = Config.EditorPath + path + ".prefab";
            if (File.Exists(Application.dataPath.Replace("Assets", "") + GoPath))
            {
                callback.Invoke(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(GoPath));
            }
            else
            {
                Debug.Log("未找到资源：" + path);
            }
            //callback.Invoke(null);
        }


        private static GameObject GetGameObjectFromEditorPath(string path)
        {
            string GoPath = Config.EditorPath + path + ".prefab";
            if (File.Exists(Application.dataPath.Replace("Assets", "") + GoPath))
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(GoPath);
            }
            else
            {
                Debug.Log("未找到资源：" + path);
            }
            return null;
        }


        private static Sprite GetSpriteFromEditor(string spritePath)
        {
            string GoPath = Config.EditorPath + spritePath + ".png";
            if (File.Exists(Application.dataPath.Replace("Assets", "") + GoPath))
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            }
            else
            {
                Debug.Log("未找到资源：" + spritePath);
            }
            return null;
        }

        private static Sprite GetSpriteFromEditorPath(string atlasPath, string spriteName)
        {
            string GoPath = Config.EditorPath + atlasPath + ".spriteatlas";
            if (File.Exists(Application.dataPath.Replace("Assets", "") + GoPath))
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<SpriteAtlas>(GoPath).GetSprite(spriteName);
            }
            else
            {
                Debug.Log("未找到资源：" + atlasPath);
            }
            return null;
        }

        public static string GetTxtDataFromEditorPath(string path)
        {
            string Txtpath = Config.EditorPath + path + ".txt";
            if (File.Exists(Application.dataPath.Replace("Assets", "") + Txtpath))
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(Txtpath).text;
            }
            else
            {
                Debug.Log("未找到资源：" + Txtpath);
            }
            return "";
        }


        public static AudioClip GetAudioClipFromEditorPath(string path)
        {
            string audiopath = Config.EditorPath + path;
            if (File.Exists(Application.dataPath.Replace("Assets", "") + audiopath))
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(audiopath);
            }
            else
            {
                Debug.Log("未找到资源：" + audiopath);
            }
            return null;
        }


#endif
    }

}
