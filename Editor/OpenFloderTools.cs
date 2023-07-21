using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    class OpenFloderTools
    {
        [MenuItem("其他工具/OpenFolder/DataPath")]
        static void OpenDataPath()
        {
            Execute(Application.dataPath);
        }

        [MenuItem("其他工具/OpenFolder/Persistent DataPath")]
        static void OpenPersistentDataPath()
        {
            Execute(Application.persistentDataPath);
        }

        [MenuItem("其他工具/OpenFolder/StreamingAssets DataPath")]
        static void OpenStreamingAssetsDataPath()
        {
            Execute(Application.streamingAssetsPath);
        }

        [MenuItem("其他工具/OpenFolder/Excel Path")]
        static void OpenExcelDataPath()
        {
            Execute(Application.dataPath+ "/ConfigExcels");
        }

        [MenuItem("其他工具/PlayerPrefManager/DelAllKey")]
        static void DelAllPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }



        /// <summary>
        /// 打开指定路径的文件夹。
        /// </summary>
        /// <param name="folder">要打开的文件夹的路径。</param>
        public static void Execute(string folder)
        {
            folder = string.Format("\"{0}\"", folder);
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    Process.Start("Explorer.exe", folder.Replace('/', '\\'));
                    break;

                case RuntimePlatform.OSXEditor:
                    Process.Start("open", folder);
                    break;

                default:
                    throw new Exception(string.Format("Not support open folder on '{0}' platform.",
                        Application.platform.ToString()));
            }
        }
    }




}
