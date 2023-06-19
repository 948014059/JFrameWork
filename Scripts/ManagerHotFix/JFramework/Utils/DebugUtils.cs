using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ManagerHotFix.JFramework.Utils
{

    public class DebugUtils
    {
        public static string LOG_UPDATE = "log_update";

        private static bool UseLog = true;
        private static string fullPath;

        /// <summary>
        /// 初始化log  游戏开始时初始化
        /// </summary>
        /// <param name="use">是否打印log</param>
        public static void InitLogger(bool use = true)
        {
            UseLog = use;

#if !DEV
            return;
#endif

            if (!use)
            {
                return;
            }

            fullPath = Config.LogFilePath + GetTimeStamp() + ".txt";
            if (!Directory.Exists(Config.LogFilePath))
            {
                Directory.CreateDirectory(Config.LogFilePath);
            }

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            FileStream fs = File.Create(fullPath);
            fs.Close();
            Application.logMessageReceived += logCallback;

        }


        private static void logCallback(string condition, string stackTrace, LogType type)
        {
            if (File.Exists(fullPath))
            {
                string _logString = "";
                switch (type)
                {
                    case LogType.Error:
                        _logString = "------------------------------------------\n" +
                            GetNowTimeString() + ":   " + condition + "\n"
                            + stackTrace
                            + "------------------------------------------";
                        break;
                    case LogType.Assert:
                        break;
                    case LogType.Warning:
                        break;
                    case LogType.Log:
                        _logString = GetNowTimeString() + ":   " + condition + "";
                        break;
                    case LogType.Exception:
                        if (condition == "DllNotFoundException: libzeewainpose")
                        {
                            return;
                        }

                        _logString = "------------------------------------------\n" +
                            GetNowTimeString() + ":   " + condition + "\n"
                            + stackTrace
                            + "------------------------------------------";
                        break;
                    default:
                        break;
                }


                using (StreamWriter sw = File.AppendText(fullPath))
                {
                    sw.WriteLine(_logString);
                }
                //GameModel.GetInstance().logString.Append(_logString + "\n");
                // EventCenter.GetInstance().EventTrigger(LOG_UPDATE);
            }
        }

        public static string GetNowTimeString()
        {
            return DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        public static void Log(string info)
        {
            if (UseLog)
            {
                Debug.Log(info);
            }
        }


        public static void LogWarning(string info)
        {
            if (UseLog)
            {
                Debug.LogWarning(info);
            }
        }

        public static void LogError(string info)
        {
            if (UseLog)
            {
                Debug.LogError(info);
            }
        }
    }

}
