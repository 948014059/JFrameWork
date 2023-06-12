using Assets.ManagerHotFix.JFramework.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ManagerHotFix.JFramework.Manager
{
    public  class DownLoadManager : BaseSingleTon<DownLoadManager>
    {
        public static  string NEXT_DOWNLOAD_COROUTINE = "next_download_coroutine";
        public static  string DOWNLOAD_ERROR = "download_error";

        private int maxDownCoroutine;
        private int currDownCoroutine = 0;
        private Queue<IEnumerator> downConQueue = new Queue<IEnumerator>();

        private void Awake()
        {
            maxDownCoroutine = Config.MaxDownCoroutine;
            EventCenter.GetInstance().AddEventListener(NEXT_DOWNLOAD_COROUTINE, StartNextIE);
        }


        private void OnDestroy()
        {
            StopAllCoroutines();
            EventCenter.GetInstance().RemoveEventListener(NEXT_DOWNLOAD_COROUTINE, StartNextIE);

        }


        public void StartConHasMaxNum(IEnumerator con)
        {
            downConQueue.Enqueue(con);
            if (currDownCoroutine < maxDownCoroutine)
            {
                StartIE();
            }
        }

        public void StartNextIE(System.Object obj)
        {
            currDownCoroutine--;
            if (currDownCoroutine < maxDownCoroutine)
            {
                StartIE();
            }
            //Debug.Log("当前下载协程数量：" + currDownCoroutine);

        }


        public void StartIE()
        {
            if (downConQueue.Count > 0)
            {
                IEnumerator getfromQueue = downConQueue.Dequeue();
                StartCoroutine(getfromQueue);
                currDownCoroutine++;
            }

        }


    }
}
