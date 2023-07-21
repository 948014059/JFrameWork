using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    [InitializeOnLoad]
    public static class RightClickItemTools
    {
        [MenuItem("GameObject/复制相对路径", false, 0)]
        private static void CopyPanelRelativePath()
        {
            GameObject obj = Selection.objects[0] as GameObject;
            string path = obj.name;
            Transform parent = obj.transform.parent;
            while (parent)
            {
                if (PrefabUtility.IsAnyPrefabInstanceRoot(parent.gameObject))
                {
                    break;
                }
                if (parent.name.Contains("Canvas"))
                {
                    break;
                }
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            GUIUtility.systemCopyBuffer = path;
        }


        [MenuItem("GameObject/复制全路径", false, 1)]
        private static void CopyPanelRelativeFullPath()
        {
            GameObject obj = Selection.objects[0] as GameObject;
            string path = obj.name;
            Transform parent = obj.transform.parent;
            while (parent)
            {

                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            GUIUtility.systemCopyBuffer = path;
        }
    }

}
