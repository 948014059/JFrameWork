﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AotScript.Script
{
    public static class BaseUtils
    {

        public static string HotFixServerUrl = "http://192.168.2.221:88/";
        public static string ManagerHotFixFilePath = "ProjectResources/ManagerHotfix.dll.bytes";


        /// <summary>
        /// 退出APP
        /// </summary>
        public static void QuitApp()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

        /// <summary>
        /// 设置帧率 -1 30 60
        /// </summary>
        /// <param name="frame"></param>
        public static void SetGameFrame(int frame)
        {
            Application.targetFrameRate = frame;
        }


        #region 字节转换
        /// <summary>
        /// 字节转MB
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static float ByteLength2MB(float length)
        {
            return length / 1024 / 1024;
        }

        /// <summary>
        /// 字节转KB
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static float ByteLength2KB(float length)
        {
            return length / 1024;
        }

        #endregion

        #region 文件操作
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="filePath"></param>
        public static void CreateDirectory(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                string dirName = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
            }
        }

        /// <summary>
        /// 读取本地文件
        /// </summary>
        /// <param name="filePath"></param>
        public static string GetLocalFileData(string filePath)
        {
            string readData;
            if (!File.Exists(filePath))
            {
                return null;
            }
            using (StreamReader sr = File.OpenText(filePath))
            {
                readData = sr.ReadToEnd();
                sr.Close();
            }
            return readData;
        }


        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath"></param>
        public static void DelFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// 保存字符到文件
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        public static void SaveStringToPath(string data, string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            try
            {
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.WriteLine(data);
                sw.Close();
                fs.Close();
            }
            catch (Exception e)
            {

                Debug.LogError(e);
            }



        }


        /// <summary>
        /// 将文件转换为md5字符
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetMD5HashFromFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            try
            {
                if (File.Exists(fileName))
                {
                    FileStream file = File.OpenRead(fileName);
                    MD5 md5 = new MD5CryptoServiceProvider();
                    byte[] retVal = md5.ComputeHash(file);
                    file.Close();
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < retVal.Length; i++)
                    {
                        sb.Append(retVal[i].ToString("x2"));

                    }
                    return sb.ToString();
                }
                return null;


            }
            catch (Exception e)
            {
                return null;

            }
        }
        #endregion
    }
}
