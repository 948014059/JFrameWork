using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    class ExcelReaderTools : EditorWindow
    {
        static ExcelReaderTools window;
        private string pythonScriptPath = "/ConfigExcels/Excel2CsvAndCsharp.py";
        private string exchelPath = "/ConfigExcels/";
        private string txtSavePath = "/ProjectResources/Configs/";
        private string csSavePath = "/Scripts/HotFixScript/Config/";
        private string excelExtension = "xls";

        [MenuItem("其他工具/配置表生成工具")]
        static void ShowWindow()
        {
            window = (ExcelReaderTools)EditorWindow.GetWindow(typeof(ExcelReaderTools), false);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Python脚本位置");
            pythonScriptPath = GUILayout.TextField(pythonScriptPath);
            if (GUILayout.Button("浏览"))
            {
                pythonScriptPath = EditorUtility.OpenFilePanel("选择脚本", pythonScriptPath, "py");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Excel路径");
            exchelPath = GUILayout.TextField(exchelPath);
            if (GUILayout.Button("浏览"))
            {
                exchelPath = EditorUtility.OpenFolderPanel("选择Excel路径", exchelPath, "");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("txt保存路径");
            txtSavePath = GUILayout.TextField(txtSavePath);
            if (GUILayout.Button("浏览"))
            {
                txtSavePath = EditorUtility.OpenFolderPanel("选择txt保存路径", txtSavePath, "");
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("csharp保存路径");
            csSavePath = GUILayout.TextField(csSavePath);
            if (GUILayout.Button("浏览"))
            {
                csSavePath = EditorUtility.OpenFolderPanel("选择脚本", csSavePath, "");
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("开始转换"))
            {
                //Debug.Log(pythonScriptPath+ txtSavePath+ csSavePath);
                StartExchange();
            }

        }

        private void StartExchange()
        {
            string cmdStr =
                "python " + Application.dataPath + pythonScriptPath +
                " --excelpath=" + Application.dataPath + exchelPath +
                " --cspath=" + Application.dataPath + csSavePath +
                " --txtpath=" + Application.dataPath + txtSavePath +
                " --extension=" + excelExtension;

            //Process po =  Process.Start("CMD.exe", "/k " + cmdStr);

            Process p = new Process();

            p.StartInfo.FileName = "cmd.exe";
            //是否使用操作系统shell启动
            p.StartInfo.UseShellExecute = false;
            // 接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardInput = true;
            //输出信息
            p.StartInfo.RedirectStandardOutput = true;
            // 输出错误
            p.StartInfo.RedirectStandardError = true;
            //不显示程序窗口
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            //启动程序
            p.Start();
            p.StandardInput.WriteLine(cmdStr + "&exit");

            p.StandardInput.AutoFlush = true;

            //获取输出信息
            string strOuput = p.StandardOutput.ReadToEnd();
            string strErrOuput = p.StandardError.ReadToEnd();
            //等待程序执行完退出进程
            p.WaitForExit();
            p.Close();

            UnityEngine.Debug.Log(strOuput);
            AssetDatabase.Refresh();
            //if (EditorApplication.isPlaying)
            //{
            //    Assembly ass = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "ManagerHotFix");
            //    Type ModuleManagerType = ass.GetType("Assets.ManagerHotFix.JFramework.Manager.ConfigDataManager"); // 获得ModuleManager类
            //    MethodInfo moduleManagerIns = ModuleManagerType.BaseType.GetMethod("GetInstance"); //获得基类单例
            //    object instance = moduleManagerIns.Invoke(null, null); // 实例化单例

            //    MethodInfo moduleManagerOpenModule = ModuleManagerType.GetMethod("UpdateAllConfig"); // 进入游戏逻辑

            //    moduleManagerOpenModule.Invoke(instance, null); // 使用方法。

            //}


        }
    }
}
