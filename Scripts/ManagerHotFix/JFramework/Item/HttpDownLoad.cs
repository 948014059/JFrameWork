using Assets.ManagerHotFix.JFramework.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.ManagerHotFix.JFramework.Item
{
    public class HttpDownLoad : DownLoadItem
    {

        string tempFileExt = ".temp";
        string tempSaveFilePath;


        public HttpDownLoad(string url, string path) : base(url, path)
        {
            tempSaveFilePath = string.Format("{0}/{1}{2}", savePath, fileNameWithoutExt, tempFileExt);
        }

        public override void StartDownLoad(Action<long> callback = null, Action<long, string> finishCallBack = null)
        {
            base.StartDownLoad(callback, finishCallBack);
            DownLoadManager.GetInstance().StartConHasMaxNum(DownLoad(callback, finishCallBack));
        }

        IEnumerator DownLoad(Action<long> callback = null, 
            Action<long, string> finishCallBack = null,Action<string> errorCallBack = null)
        {
            Debug.Log("开始下载：" + srcUrl);
            UnityWebRequest request = UnityWebRequest.Get(srcUrl);
            request.timeout = Config.DownloadTimeOut;
            FileStream fileStream;
            if (File.Exists(tempSaveFilePath))
            {
                fileStream = File.OpenWrite(tempSaveFilePath);
                currentLength = fileStream.Length;
                fileStream.Seek(currentLength, SeekOrigin.Current); // 移动游标到最后                                                    /
                request.SetRequestHeader("Range", "bytes=" + (int)currentLength + "-");
            }
            else
            {
                fileStream = new FileStream(tempSaveFilePath, FileMode.Create, FileAccess.Write);
                currentLength = 0;
            }
            yield return request.SendWebRequest();
            if (request.isDone)
            {
                if (request.isHttpError || request.isNetworkError)
                {
                    Debug.Log(request.error);
                    errorCallBack?.Invoke(srcUrl);
                    EventCenter.GetInstance().EventTrigger(DownLoadManager.DOWNLOAD_ERROR);
                }
                else
                {
                    //Debug.Log(request.downloadHandler.data.Length);
                    Stream stream = new MemoryStream(request.downloadHandler.data);
                    fileLength = request.downloadHandler.data.Length + currentLength;

                    isStartDownLoad = true;
                    int lengthOnce;
                    int buffMaxLength = Config.BuffDownLength;
                    while (currentLength < fileLength)
                    {
                        byte[] buffer = new byte[buffMaxLength];
                        if (stream.CanRead)
                        {
                            lengthOnce = stream.Read(buffer, 0, buffer.Length);
                            currentLength += lengthOnce;
                            fileStream.Write(buffer, 0, lengthOnce);
                            callback?.Invoke(lengthOnce);
                        }
                        else
                        {
                            break;
                        }
                        yield return null;
                    }

                    isStartDownLoad = false;
                    stream.Close();
                    fileStream.Close();
                    File.Move(tempSaveFilePath, saveFilePath);
                    finishCallBack?.Invoke(GetLength(), fileNameWithoutExt);
                }
                EventCenter.GetInstance().EventTrigger(DownLoadManager.NEXT_DOWNLOAD_COROUTINE);
            }


        }

        public override void Destroy()
        {
        }

        public override long GetCurrentLength()
        {
            return currentLength;
        }

        public override long GetLength()
        {
            return fileLength;
        }

        public override float GetProcess()
        {
            if (fileLength > 0)
            {
                return Mathf.Clamp((float)currentLength / fileLength, 0, 1);
            }
            return 0;
        }
    }

}
