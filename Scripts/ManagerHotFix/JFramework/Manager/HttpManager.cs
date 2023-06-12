using Assets.ManagerHotFix.JFramework.Base;
using LitJson;
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
    public class HttpManager : BaseSingleTon<HttpManager>
    {
        public void StartHttpGET(string url, Action<string> successCallBack = null, Action<string> errorCallBack = null)
        {
            StartCoroutine(HttpGET(url, "GET", successCallBack, errorCallBack));
        }

        IEnumerator HttpGET(string url, string method, Action<string> successCallBack = null, Action<string> errorCallBack = null)
        {
            UnityWebRequest request = new UnityWebRequest(url, method);
            request.timeout = Config.HttpTimeOut;
            DownloadHandlerBuffer Download = new DownloadHandlerBuffer();
            request.downloadHandler = Download;
            Debug.Log("Start Http GET: " + url );
            yield return request.SendWebRequest();

            if (request.isDone)
            {
                Debug.Log("Http GET Request: " + url +"\n "+
                    request.downloadHandler.text);
                if (request.isHttpError || request.isNetworkError)
                {
                    errorCallBack?.Invoke(request.error.ToString());
                }
                else
                {                   
                    successCallBack?.Invoke(request.downloadHandler.text);
                }
            }


        }
    
    
       public void StartHttpPOST(string url , string jsonData , Action<string> successCallBack = null, Action<string> errorCallBack = null)
       {

            StartCoroutine(HttpPOST(url, jsonData, successCallBack, errorCallBack));
       }

        IEnumerator HttpPOST(string url, string jsonData, Action<string> successCallBack = null, Action<string> errorCallBack = null)
        {
            UnityWebRequest request = UnityWebRequest.Post(url, UnityWebRequest.kHttpVerbPOST);
            request.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
            request.timeout = Config.HttpTimeOut;

            DownloadHandlerBuffer download = new DownloadHandlerBuffer();
            request.downloadHandler = download;

            byte[] uploadbyte = Encoding.UTF8.GetBytes(jsonData);
            UploadHandlerRaw uploadbody = new UploadHandlerRaw(uploadbyte);
            request.uploadHandler = uploadbody;
            Debug.Log("Start Http POST: " + url + "\n" 
                + jsonData);
            yield return request.SendWebRequest();

            if (request.isDone)
            {
                Debug.Log("Http POST Request: " + url + "\n " +
                    request.downloadHandler.text);
                if (request.isHttpError || request.isNetworkError)
                {
                    errorCallBack?.Invoke(request.error.ToString());
                }
                else
                {
                    successCallBack?.Invoke(request.downloadHandler.text);
                }
            }
        }
    
    }
}
