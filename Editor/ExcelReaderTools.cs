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
    class ExcelReaderTools : EditorWindow
    {
        static ExcelReaderTools window;
        private string pythonScriptPath = "/ConfigData/Excel2CsvAndCsharp.py";
        private string exchelPath = "/ConfigData/";
        private string txtSavePath = "/ProjectResources/ConfigDataText/";
        private string csSavePath = "/Hotfix/ConfigData/";
        private string excelExtension = "xls";

        [MenuItem("Tools/ExcelTool")]
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
                "python " + Application.dataPath+ pythonScriptPath +
                " --excelpath=" + Application.dataPath + exchelPath +
                " --cspath=" + Application.dataPath + csSavePath +
                " --txtpath=" + Application.dataPath + txtSavePath +
                " --extension=" + excelExtension;

            //UnityEngine.Debug.Log(cmdStr);

            Process.Start("CMD.exe", "/k " + cmdStr);

        }
    }
}
