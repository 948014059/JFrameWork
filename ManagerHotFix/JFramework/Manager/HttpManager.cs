using Assets.ManagerHotFix.JFramework.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.ManagerHotFix.JFramework.Manager
{
    class HttpManager : BaseSingleTon<HttpManager>
    {
        public void StartHttpRequest(string url, string method, Action<string> successCallBack = null, Action<string> errorCallBack = null)
        {
            StartCoroutine(HttpRequest(url, method, successCallBack, errorCallBack));
        }

        IEnumerator HttpRequest(string url, string method, Action<string> successCallBack = null, Action<string> fileCallBack = null)
        {
            UnityWebRequest request = new UnityWebRequest(url, method);
            request.timeout = Config.HttpTimeOut;
            DownloadHandlerBuffer Download = new DownloadHandlerBuffer();
            request.downloadHandler = Download;

            yield return request.SendWebRequest();

            if (request.isDone)
            {
                Debug.Log("httpRequest: " + url +"\n "+
                    request.downloadHandler.text);
                if (request.result !=  UnityWebRequest.Result.Success)
                {
                    fileCallBack?.Invoke(request.error.ToString());
                }
                else
                {                   
                    successCallBack?.Invoke(request.downloadHandler.text);
                }
            }


        }
    }
}
