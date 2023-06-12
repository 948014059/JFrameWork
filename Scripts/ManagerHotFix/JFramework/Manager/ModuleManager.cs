using Assets.ManagerHotFix.JFramework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ManagerHotFix.JFramework.Manager
{
    /// <summary>
    /// Module 管理
    /// </summary>
    public class ModuleManager : BaseSingleTon<ModuleManager>
    {
        private Transform UIFrom;
        private Transform TipsFrom;
        private Transform LogFrom;
        private Transform PromptBox;
        private Dictionary<string, GameObject> ModuleDict = new Dictionary<string, GameObject>();
        public Queue<System.Object> openModelObj = new Queue<System.Object>(); // 打开UI传递消息队列


        private void Awake()
        {
            Transform Canvas = GameObject.FindGameObjectWithTag("Canvas").transform;
            UIFrom = Canvas.Find("UI");
            LogFrom = Canvas.Find("Log");
            TipsFrom = Canvas.Find("Tips");
            PromptBox = Canvas.Find("PromptBox");
        }



        public void OpenUpdateModule(Type type, Action callBack = null, System.Object obj = null)
        {
            if (type == null)
            {
                Debug.Log("Type 为Null ，请确认");
                return;
            }

            if (ModuleDict.ContainsKey(type.Name))
            {
                ModuleDict[type.Name].gameObject.SetActive(true);
                return;
            }

            BaseModule module = (BaseModule)Activator.CreateInstance(type);
            MethodInfo moduleInfo = type.GetMethod("GetView");
            Type viewType = (Type)moduleInfo.Invoke(module, null);
            Debug.Log("正在打开Module: " + type.Name + "        获取资源PreFabs:" + module.PreFabs);
            openModelObj.Enqueue(obj);
            GameObject newGo = null;
            if (Utils.Utils.HasABFileInFroject())
            {
                Destroy(UIFrom.Find("UpdatePanel").gameObject);
                newGo = CreateGameObject(module.PreFabs, (BaseModule.LayerType)module.layer);
            }
            else
            {
                newGo = UIFrom.Find("UpdatePanel").gameObject;
            }
            newGo.SetActive(true);
            newGo.AddComponent(viewType);
            ModuleDict.Add(type.Name, newGo);
            Debug.Log("Module: " + type.Name + "已打开");
            callBack?.Invoke();

        }

        /// <summary>
        /// 打开module  使用反射
        /// </summary>
        /// <param name="type"></param>
        /// <param name="callBack"></param>
        public void OpenModule(Type type, Action callBack = null, System.Object obj = null)
        {
            if (type == null)
            {
                Debug.Log("Type 为Null ，请确认");
                return;
            }

            if (ModuleDict.ContainsKey(type.Name))
            {
                ModuleDict[type.Name].gameObject.SetActive(true);
                return;
            }

            BaseModule module = (BaseModule)System.Activator.CreateInstance(type);
            MethodInfo moduleInfo = type.GetMethod("GetView");
            Type viewType = (Type)moduleInfo.Invoke(module, null);
            Debug.Log("正在打开Module: " + type.Name + "        获取资源PreFabs:" + module.PreFabs);
            openModelObj.Enqueue(obj);
            GameObject newGo = CreateGameObject(module.PreFabs, (BaseModule.LayerType)module.layer);
            newGo.AddComponent(viewType);
            ModuleDict.Add(type.Name, newGo);
            Debug.Log("Module: " + type.Name + "已打开");
            callBack?.Invoke();
        }

        /// <summary>
        /// 使用泛型打开 并且可以传递Obj(只有打开时触发传值,打开后隐藏不会传递)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callBack"></param>
        /// <param name="obj"></param>
        public void OpenModule<T>(Action callBack = null, System.Object obj = null) where T : BaseModule
        {
            if (ModuleDict.ContainsKey(typeof(T).Name))
            {
                ModuleDict[typeof(T).Name].gameObject.SetActive(true);
                return;
            }
            BaseModule module = (T)System.Activator.CreateInstance(typeof(T));
            Type viewType = module.GetView();
            openModelObj.Enqueue(obj);
            GameObject newGo = CreateGameObject(module.PreFabs, (BaseModule.LayerType)module.layer);
            newGo.AddComponent(viewType);
            ModuleDict.Add(typeof(T).Name, newGo);
            Debug.Log("Module: " + typeof(T).Name + "已打开");
            callBack?.Invoke();
        }


        /// <summary>
        /// 关闭module
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isDes"></param>
        public void CloseModule(Type type, bool isDes = true)
        {
            Debug.Log("正在关闭Module: " + type.Name);
            if (ModuleDict.ContainsKey(type.Name))
            {
                if (!isDes)
                {
                    ModuleDict[type.Name].SetActive(false);
                }
                else
                {
                    Destroy(ModuleDict[type.Name]);
                    ModuleDict.Remove(type.Name);
                }

            }
        }

        /// <summary>
        /// 泛型关闭
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isDes"></param>
        public void CloseModule<T>(bool isDes = true) where T : BaseModule
        {
            Debug.Log("正在关闭Module: " + typeof(T).Name);
            if (ModuleDict.ContainsKey(typeof(T).Name))
            {
                if (!isDes)
                {
                    ModuleDict[typeof(T).Name].SetActive(false);
                }
                else
                {
                    Destroy(ModuleDict[typeof(T).Name]);
                    ModuleDict.Remove(typeof(T).Name);
                }

            }
        }

        /// <summary>
        /// 判断某个module是否打开
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsModuleOpen(Type type)
        {
            if (ModuleDict.ContainsKey(type.Name) && ModuleDict[type.Name].activeInHierarchy)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 泛型判断某个UI是否打开
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool IsModuleOpen<T>()
        {
            if (ModuleDict.ContainsKey(typeof(T).Name) && ModuleDict[typeof(T).Name].activeInHierarchy)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// 删除所有的module
        /// </summary>
        public void DesAllModule()
        {
            foreach (var item in ModuleDict)
            {
                Destroy(item.Value);
            }
        }

        /// <summary>
        /// 创建GameObject
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private GameObject CreateGameObject(string path, BaseModule.LayerType type)
        {
            GameObject go = ResourcesManager.GetPrefab(path);

            if (go != null)
            {
                GameObject newGo = Instantiate(go, GetFromByType(type));
                //newGo.transform.SetParent(GetFromByType(type));
                //Debug.Log("设置父物体：" + GetFromByType(type) +
                //    "type:" + type + "  UIFrom:" + UIFrom);
                //newGo.transform.localPosition = Vector3.zero;
                //newGo.transform.localScale = Vector3.one;
                newGo.gameObject.SetActive(true);
                return newGo;
            }
            Debug.Log("未找到：" + path);
            return null;

        }


        /// <summary>
        /// 获取根路径
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private Transform GetFromByType(BaseModule.LayerType type)
        {
            Transform tf = null;
            switch (type)
            {
                case BaseModule.LayerType.UI:
                    tf = UIFrom;
                    break;
                case BaseModule.LayerType.Tips:
                    tf = TipsFrom;
                    break;
                case BaseModule.LayerType.Log:
                    tf = LogFrom;
                    break;
                case BaseModule.LayerType.PromptBox:
                    tf = PromptBox;
                    break;
                default:
                    break;
            }
            return tf;
        }
    }
}
