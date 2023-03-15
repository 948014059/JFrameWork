using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ManagerHotFix.JFramework.Item
{
    /// <summary>
    /// 下载的抽象类
    /// </summary>
    public abstract class DownLoadItem
    {
        protected string srcUrl;  // 下载url
        protected string savePath; // 保存路径
        protected string fileNameWithoutExt; // 无拓展名
        protected string fileExt; //文件拓展名
        protected string saveFilePath; // 最终保存路径
        protected long fileLength; // 文件长度
        protected long currentLength; // 当前下载长度
        protected bool isStartDownLoad; // 是否开始下载

        public bool IsStartDownLoad
        {
            get
            {
                return isStartDownLoad;
            }
        }
        /// <summary>
        /// 处理路径
        /// </summary>
        /// <param name="_url"></param>
        /// <param name="_savePath"></param>
        public DownLoadItem(string _url, string _savePath)
        {
            srcUrl = _url;
            savePath = _savePath;
            isStartDownLoad = false;
            fileNameWithoutExt = _url.Replace(Config.UpdateUrl + Config.PlatFrom, "").
                Replace(Config.streamAssetsDataPath+Config.PlatFrom,"");
            fileExt = Path.GetExtension(srcUrl);
            if (fileExt == ".bytes")
            {
                saveFilePath = string.Format("{0}/{1}", savePath, fileNameWithoutExt);
            }
            else
            {
                saveFilePath = string.Format("{0}/{1}{2}", savePath, fileNameWithoutExt, fileExt);
            }
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="finishCallBack"></param>
        public virtual void StartDownLoad(Action<long> callback = null, Action<long, string> finishCallBack = null)
        {
            if (string.IsNullOrEmpty(srcUrl) || string.IsNullOrEmpty(saveFilePath))
            {
                return;
            }
            CreateDirectory(saveFilePath);

        }

        /// <summary>
        /// 处理路径
        /// </summary>
        /// <param name="filePath"></param>
        public void CreateDirectory(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                string dirName = Path.GetDirectoryName(filePath);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
            }
        }

        /// <summary>
        /// 获取下载进度
        /// </summary>
        /// <returns>进度，0-1</returns>
        public abstract float GetProcess();

        /// <summary>
        /// 获取当前下载了的文件大小
        /// </summary>
        /// <returns>当前文件大小</returns>
        public abstract long GetCurrentLength();

        /// <summary>
        /// 获取要下载的文件大小
        /// </summary>
        /// <returns>文件大小</returns>
        public abstract long GetLength();

        public abstract void Destroy();


    }


}
