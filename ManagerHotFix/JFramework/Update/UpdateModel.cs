using Assets.ManagerHotFix.JFramework.Base;
using Assets.ManagerHotFix.JFramework.Item;
using Assets.ManagerHotFix.JFramework.Manager;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.ManagerHotFix.JFramework.Update
{
    public class UpdateModel : BaseModel
    {
        public class VersionData
        {
            public string version;
            public long length;
            public List<FileData> filedatas = new List<FileData>();
        }

        public class FileData
        {
            public string filename;
            public string md5;
            public string length;
        }

        public List<FileData> needHotUpdateList = new List<FileData>(); // 最终需要下载的文件
        public long needUpdateFileLength = 0;  // 需要下载的文件长度
        public int currDownNum = 0;           //当前下载的数量
        public long currDownLength = 0;       //当前下载的文件长度
        public float downLoadTime = 0;        //下载的时间  
        public bool IsStartDown = false;      //是否开始下载  
        public long downLen = 0;


        public int copyFileLen  = 0;
        public bool IsStartCopy = false;      //是否开始复制 
        public int  currCopyNum = 0;           //当前复制的数量
        public long currCopyLength = 0;       //当前复制的文件长度
        public long copyLen = 0;

        public string serverData;             // 服务器的更新数据 
        public string sAversionData;

        public override void Init()
        {
        }

        public override void ResetModel()
        {
        }

        /// <summary>
        /// 将下载临时文件中存在的文件 从需要更新的目录中去除
        /// </summary>
        /// <param name="name"></param>
        private void RemoveSameNameFromList(string name)
        {
            foreach (var item in needHotUpdateList)
            {
                if (item.filename.Trim() == name.Trim())
                {
                    needHotUpdateList.Remove(item);
                    break;
                }
            }
        }


        public IEnumerator CopyStreamingAssetsDir2PersistentDataPath(string path, Action<long> callback = null,
            Action<long, string> finishCallBack = null)
        {

            UnityWebRequest www = UnityWebRequest.Get(path+Config.VersionName);
            yield return www.SendWebRequest();
            string versiontext = www.downloadHandler.text;
            if (string.IsNullOrEmpty(versiontext))
            {
                Debug.Log("本地无文件");
            }
            else
            {
                VersionData streamingData = GetVersionJsonData(versiontext);
                foreach (var item in streamingData.filedatas)
                {
                    string fileUrl = path + Config.PlatFrom + "/" + item.filename;
                    Debug.Log(fileUrl);
                    HttpDownLoad httpDownLoad = new HttpDownLoad(fileUrl, Config.ABPath + Config.PlatFrom);
                    httpDownLoad.StartDownLoad(callback, finishCallBack);
                }
            }
        }


        public void GetVersionFromHttp(string path,Action<string> callBack)
        {
            HttpManager.GetInstance().StartHttpRequest(path + Config.VersionName, "GET", (serverData) =>
            {
                callBack(serverData);
            }, (error) => {
                Debug.LogError(error);
            });
        }


        /// <summary>
        /// 下载完成保存到临时文件中
        /// </summary>
        /// <param name="Str"></param>
        public void SaveDownLoadFileTemp(string Str)
        {
            Utils.Utils.CreateDirectory(Config.ABPath + Config.PlatFrom);
            using (StreamWriter sw = File.AppendText(Config.ABPath + Config.VersionTempName))
            {
                sw.WriteLineAsync(Str);
            }
        }

        public void CopyResourceFileTemp(string Str)
        {
            Utils.Utils.CreateDirectory(Config.ABPath+Config.PlatFrom+"/");
            Debug.Log(Config.ABPath + Config.PlatFrom + "/");
            using (StreamWriter sw = File.AppendText(Config.ABPath + Config.CopyResourceTempName))
            {
                sw.WriteLineAsync(Str);
            }
        }


       /// <summary>
       /// 检查是否有临时文件(上次更新未完成)
       /// </summary>
        public void CheckDownLoadTempFile()
        {
            if (File.Exists(Config.ABPath + Config.VersionTempName))
            {
                string downFileTempStr = Utils.Utils.GetLocalFileData(Config.ABPath + Config.VersionTempName);
                string[] TempList = downFileTempStr.Split("\n");
                foreach (var item in TempList)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        RemoveSameNameFromList(item);
                    }
                }
            }
        }

        /// <summary>
        /// 解析版本json
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public VersionData GetVersionJsonData(string jsonStr)
        {
            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            VersionData versiondata = new VersionData();
            versiondata.version = (string)jsonData["version"];
            versiondata.length = long.Parse(jsonData["length"].ToString());
            foreach (JsonData item in jsonData["files"])
            {
                FileData fileData = new FileData();
                fileData.filename = (string)item["file"];
                fileData.md5 = (string)item["md5"];
                fileData.length = (string)item["length"];
                versiondata.filedatas.Add(fileData);
            }
            return versiondata;
        }


        /// <summary>
        /// 本地文件和服务器文件对比
        /// </summary>
        /// <param name="serverDataList"></param>
        /// <param name="localDataList"></param>
        public void ContrastVersion(List<FileData> serverDataList, List<FileData> localDataList)
        {
            foreach (FileData item in serverDataList)
            {
                if (NeedUpdateFile(localDataList, item))
                {
                    //Debug.Log("需要跟新:" + item.filename);
                    needHotUpdateList.Add(item);
                }
            }
        }


        /// <summary>
        /// 对比文件
        /// </summary>
        /// <param name="localData">本地文件</param>
        /// <param name="filedata">服务器文件</param>
        /// <returns></returns>
        private bool NeedUpdateFile(List<FileData> localData, FileData filedata)
        {
            foreach (var item in localData)
            {
                if (item.filename == filedata.filename && item.md5 == filedata.md5)
                {
                    return false;
                }
            }
            return true;
        }







    }
}
