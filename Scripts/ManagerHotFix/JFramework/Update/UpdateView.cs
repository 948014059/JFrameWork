using Assets.AotScript.Script;
using Assets.ManagerHotFix.JFramework.Base;
using Assets.ManagerHotFix.JFramework.Item;
using Assets.ManagerHotFix.JFramework.Manager;
using Assets.Scripts.AotScript;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.ManagerHotFix.JFramework.Update
{
    class UpdateView : BaseView
    {
        private Text updatText;
        private Slider updataSlider;
        private HttpDownLoad httpDownLoad;
        private UpdateModel updateModel;
        private Transform UpdatePrompt;

        private ABManager aBManager;

        public override void ShowView(object obj)
        {
            updateModel = ModelManager.GetInstance().GetModelInstence<UpdateModel>();
            System.Net.ServicePointManager.DefaultConnectionLimit = 50;

            updatText = transform.Find("Update/UpdateInfo/Text_UpdateTips").GetComponent<Text>();
            updataSlider = transform.Find("Update/Slider").GetComponent<Slider>();
            UpdatePrompt = transform.Find("PromptBoxPanel");
            UpdatePrompt.gameObject.SetActive(false);
        }


        private void Start()
        {
            // CheckLocalVersionUpdate();
            LoadABWithWebGL();
        }

        private void Update()
        {
            if (updateModel.IsStartDown)
            {
                if (updateModel.downLen == 0)
                {
                    SetUpdateText("获取资源中......");
                }
                else
                {
                    updateModel.downLoadTime += Time.deltaTime;
                    SetUpdateText("正在更新中...[" + updateModel.currDownNum + "/" + updateModel.needHotUpdateList.Count + "]   " +
                        (Utils.Utils.ByteLength2KB(updateModel.downLen) / updateModel.downLoadTime).ToString("0.0") + "KB/S");
                    updataSlider.value = (float)(updateModel.downLen + updateModel.downTempLen) / updateModel.needUpdateFileLength;
                    if (updateModel.currDownNum >= updateModel.needHotUpdateList.Count)
                    {
                        updataSlider.value = 1;
                        updateModel.IsStartDown = false;
                        Utils.Utils.DelFile(Config.ABPath + Config.VersionTempName);
                        Utils.Utils.SaveStringToPath(updateModel.serverData, Config.ABPath + Config.VersionName);
                        SetUpdateText("更新完成...");
                        if (updateModel.IsDownApk)
                        {
                            if (File.Exists(Config.ABPath + Config.ApkName))
                            {
                                //拉起安装程序
                                bool success = Utils.Utils.InstallNewApk(Config.ABPath + Config.ApkName);
                                Debug.Log("success:" + success);
                            }
                        }
                        else
                        {
                            LoadHotFix();
                        }
                    }
                }
            }
            if (updateModel.IsStartCopy)
            {
                if (updateModel.copyLen == 0)
                {
                    SetUpdateText("资源准备中......");
                }
                else
                {
                    updataSlider.value = (float)updateModel.currCopyNum / updateModel.copyFileLen;
                    if (updateModel.currCopyNum >= updateModel.copyFileLen)
                    {
                        updataSlider.value = 1;
                        updateModel.IsStartCopy = false;
                        Utils.Utils.DelFile(Config.ABPath + Config.CopyResourceTempName);
                        Utils.Utils.SaveStringToPath(updateModel.sAversionData, Config.ABPath + Config.VersionName);
                        SetUpdateText("资源准备完成......");
                        Debug.Log("资源准备完成......");
                        CheckVersionData();
                    }
                }
            }
        }



        private void LoadABWithWebGL()
        {
            aBManager = ABManager.GetInstance();
            StartCoroutine(aBManager.WebGLLoadMainAB(LoadAllBundles));
        }

        private void LoadAllBundles()
        {
            SetUpdateText("资源加载中...");
            StartCoroutine(aBManager.LoadAllABAssets((num,len)=> {
                updataSlider.value = num / len;

            },
            ()=> {
                //LoadHotFix();
                StartCoroutine(GameLoader.Instance.DownloadHotFixAssets("Assembly-CSharp.dll", () =>
                {
                    /// 补充元数据
                    GameLoader.Instance.LoadMetadataForAOTAssemblies();
#if !UNITY_EDITOR
        System.Reflection.Assembly.Load(GameLoader.Instance.GetAssetData("Assembly-CSharp.dll"));
#endif
                    StartG();

                }, Config.UpdateUrl, Config.PlatFrom));
            }));
        }

        /// <summary>
        /// 准备资源(第一次启动，可读写路径没有资源，
        /// 将StreamingAssets中的资源复制到可读写路径中，方便后续更新)
        /// </summary>
        public void CheckLocalVersionUpdate()
        {
            SetUpdateText("资源准备中......");
            if (Directory.Exists(Config.ABPath) &&
            !File.Exists(Config.ABPath + Config.CopyResourceTempName)) // 检查是否有资源，并且已经复制完成
            {
                Debug.Log("可读写路径下存在资源，开始更新");
                CheckVersionData(); //对比版本，开启跟新
            }
            else
            {
                // streamingAssets 目录无法使用C# io读取，只能使用www/unitywebrequest下载
                Debug.Log("可读写路径不存在资源，开始复制StreamingAsset下的资源");
                updateModel.GetVersionFromHttp(Config.streamAssetsDataPath, (data)=> {
                    if (string.IsNullOrEmpty(data))
                    {
                        Debug.Log("资源准备完成......");
                        SetUpdateText("资源准备完成......");
                        Utils.Utils.DelFile(Config.ABPath + Config.CopyResourceTempName);
                        updataSlider.value = 1;
                        CheckVersionData();//对比版本，开启跟新               
                        return;
                    }

                    updateModel.IsStartCopy = true;
                    updateModel.sAversionData = data;
                    updateModel.CopyResourceFileTemp("start");
                    UpdateModel.VersionData streamingData = updateModel.GetVersionJsonData(data);
                    updateModel.copyFileLen = streamingData.filedatas.Count;
                    foreach (var item in streamingData.filedatas)
                    {
                        string fileUrl = Config.streamAssetsDataPath + Config.PlatFrom + "/" + item.filename;
                        HttpDownLoad httpDownLoad = new HttpDownLoad(fileUrl, Config.ABPath + Config.PlatFrom);
                        httpDownLoad.StartDownLoad((currLength) => {
                            updateModel.copyLen += currLength;
                        }, 
                        (fileLength, fileName) => {
                            updateModel.currCopyNum += 1;
                            updateModel.currDownLength += fileLength;
                            updateModel.CopyResourceFileTemp(fileName.Substring(1, fileName.Length - 1));
                        });
                    }

                }); ;
            }
        }

        /// <summary>
        /// 检查资源更新
        /// </summary>
        public void CheckVersionData()
        {
            Action<string> callBack = (_serverData) => {

                if (String.IsNullOrEmpty(_serverData))
                {

                    UpdatePrompt.gameObject.SetActive(true);
                    UpdatePrompt.transform.Find("Text_Title").GetComponent<Text>().text = "网络错误";
                    UpdatePrompt.transform.Find("Text_Info").GetComponent<Text>().text = "无法链接服务器,是否重新链接";
                    AddBtnClickEvent(UpdatePrompt.Find("Button_Yes"), () => { CheckVersionData(); });
                    AddBtnClickEvent(UpdatePrompt.Find("Button_No"), () => { BaseUtils.QuitApp(); });

                    return;
                }

                updateModel.serverData = _serverData;
                SetUpdateText("正在对比本地文件...");
                updateModel.needHotUpdateList.Clear();
                updateModel.serverVersionData = updateModel.GetVersionJsonData(updateModel.serverData);

                string localVersion = Utils.Utils.GetLocalFileData(Config.ABPath + Config.VersionName);
                if (string.IsNullOrEmpty(localVersion)) //本地无版本配置文件
                {
                    updateModel.needHotUpdateList = updateModel.serverVersionData.filedatas;
                }
                else // 对比配置文件
                {
                    updateModel.localVersionData = updateModel.GetVersionJsonData(localVersion);
                    updateModel.ContrastVersion(updateModel.serverVersionData.filedatas, updateModel.localVersionData.filedatas);
                }

                updateModel.CheckDownLoadTempFile();

                if (updateModel.needHotUpdateList.Count > 0)
                {
                    foreach (var item in updateModel.needHotUpdateList)
                    {
                        updateModel.needUpdateFileLength += item.length;
                    }

                    UpdatePrompt.gameObject.SetActive(true);
                    UpdatePrompt.transform.Find("Text_Title").GetComponent<Text>().text = "检测到新版本";
                    UpdatePrompt.transform.Find("Text_Info").GetComponent<Text>().text = string.Format("version:{0} \n检测到版本更新,大小：{1}MB \n是否跟新",
                        updateModel.serverVersionData.version, Utils.Utils.ByteLength2MB(updateModel.needUpdateFileLength).ToString("0.0"));
                    AddBtnClickEvent(UpdatePrompt.Find("Button_Yes"), () => { StartUpdate(); });
                    AddBtnClickEvent(UpdatePrompt.Find("Button_No"), () => { BaseUtils.QuitApp(); });

                }
                else
                {
                    SetUpdateText("比对完成，无需更新...");
                    Debug.Log("比对完成，无需更新...");
                    LoadHotFix();
                }
            };

            SetUpdateText("获取跟新文件中...");
            updateModel.GetVersionFromHttp(Config.UpdateUrl, callBack);
        }

        private void SetUpdateText(string text)
        {
            updatText.text = text;
        }


        public override void AddEventListener()
        {

        }
        public override void RemoveEventListener()
        {

        }

        public override void OnBtnClickEvent()
        {

        }

        /// <summary>
        /// 开始更新
        /// </summary>
        private void StartUpdate()
        {
            if (updateModel.IsStartDown)
            {
                return;
            }
            UpdatePrompt.gameObject.SetActive(false);
            updateModel.IsStartDown = true;
            Debug.Log("需要热更数量：" + updateModel.needHotUpdateList.Count);

            for (int i = 0; i < updateModel.needHotUpdateList.Count; i++)
            {
                string fileUrl = Config.UpdateUrl + Config.PlatFrom + "/" + updateModel.needHotUpdateList[i].filename;
                httpDownLoad = new HttpDownLoad(fileUrl, Config.ABPath + Config.PlatFrom);
                httpDownLoad.StartDownLoad((currLength) =>
                {
                    //long templength = currDownLength + currLength;
                    updateModel.downLen += currLength;
                },
                (fileLength, fileName) =>
                {
                    updateModel.currDownNum += 1;
                    updateModel.currDownLength += fileLength;
                    if (!updateModel.IsDownApk)
                    {
                        updateModel.SaveDownLoadFileTemp(fileName.Substring(1, fileName.Length - 1));
                    }
                },(tempLen)=> {
                    updateModel.downTempLen += tempLen;
                });
            }
        }


        /// <summary>
        /// 检测大版本更新
        /// </summary>
        public void CheckBigUpdate(Action callBack)
        {
            Debug.Log(Application.version + "/" + updateModel.serverVersionData.version);

            Action GotoDownLoadApk = () =>{
                updateModel.ResetModel();
                updateModel.needUpdateFileLength = updateModel.serverVersionData.apkLength;

                updateModel.needHotUpdateList.Add(new UpdateModel.FileData()
                {
                    filename = updateModel.serverVersionData.apkPath,
                    length = updateModel.serverVersionData.apkLength,
                    md5 = updateModel.serverVersionData.apkMd5
                });
                updateModel.IsDownApk = true;
                UpdatePrompt.gameObject.SetActive(true);
                UpdatePrompt.transform.Find("Text_Title").GetComponent<Text>().text = "检测到新版本";
                UpdatePrompt.transform.Find("Text_Info").GetComponent<Text>().text = string.Format("version:{0} \n检测到版本更新,大小：{1}MB \n是否更新",
                    updateModel.serverVersionData.version, Utils.Utils.ByteLength2MB(updateModel.serverVersionData.apkLength).ToString("0.0"));
                AddBtnClickEvent(UpdatePrompt.Find("Button_Yes"), () => { StartUpdate(); });
                AddBtnClickEvent(UpdatePrompt.Find("Button_No"), () => { BaseUtils.QuitApp(); });
            };

            // 当前apk大版本号小于服务器大版本号
            if (int.Parse(Application.version[0].ToString()) < int.Parse( updateModel.serverVersionData.version[0].ToString()))
            {
                string apkPath = Config.ABPath + Config.PlatFrom + "/" + updateModel.serverVersionData.apkPath;
                // 本地是否有apk包
                if (File.Exists(apkPath))
                {
                    Debug.Log(
                        "本地md5："+Utils.Utils.GetMD5HashFromFile(apkPath)+ "最新md5："+updateModel.serverVersionData.apkMd5);
                    if (Utils.Utils.GetMD5HashFromFile(apkPath) == updateModel.serverVersionData.apkMd5)
                    {
                        //拉起安装程序
                        bool success = Utils.Utils.InstallNewApk(Config.ABPath + Config.ApkName);
                        Debug.Log("success:" + success);
                    }
                    else
                    {
                        GotoDownLoadApk();
                    }
                }
                else
                {
                    GotoDownLoadApk();
                }

            }
            else
            {
                callBack?.Invoke();
            }
        }


        /// <summary>
        /// 资源更新完成，可以开始加载游戏逻辑了
        /// </summary>
        private void LoadHotFix()
        {

            CheckBigUpdate(()=> {
                ABManager.GetInstance().ReLoadAssetBundle();  // 重新加载AB包
                /// 加载热更程序
                StartCoroutine(GameLoader.Instance.DownloadHotFixAssets("Assembly-CSharp.dll", () =>
                {
                    /// 补充元数据
                    GameLoader.Instance.LoadMetadataForAOTAssemblies();
#if !UNITY_EDITOR
        System.Reflection.Assembly.Load(GameLoader.Instance.GetAssetData("Assembly-CSharp.dll"));
#endif
                    StartG();

                }, Config.ABPath, Config.PlatFrom));
            });

        }

        /// <summary>
        /// 可以进入到游戏逻辑中了。
        /// </summary>
        private void StartG()
        {

            // 加载程序集
            Assembly ass = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "Assembly-CSharp");
            Type ModuleManagerType = ass.GetType("Assets.HotFix.Game.GameManager"); // 获得ModuleManager类
            MethodInfo moduleManagerIns = ModuleManagerType.BaseType.GetMethod("GetInstance"); //获得基类单例
            object instance = moduleManagerIns.Invoke(null, null); // 实例化单例
            MethodInfo moduleManagerOpenModule = ModuleManagerType.GetMethod("HotFixGameStart"); // 进入游戏逻辑

            moduleManagerOpenModule.Invoke(instance, null); // 使用方法。
        }


    }
}
