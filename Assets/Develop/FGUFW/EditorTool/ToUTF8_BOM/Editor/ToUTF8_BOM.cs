using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace FGUFW.Core
{
    static public class ToUTF8_BOM
    {

        const string Extension = ".csv";

        [MenuItem("Assets/Csv2UTF8_BOM")]
        private static void Build()
        {
            var selects = Selection.objects;
            if(selects==null)
            {
                return;
            }

            //筛选csv文件
            List<string> paths = new List<string>();
            foreach (var obj in selects)
            {
                string path = AssetDatabase.GetAssetPath(obj).Substring(6);
                if(Path.GetExtension(path)==Extension)
                {
                    paths.Add(path);
                }
            }
            if(paths.Count==0)return;
            
            foreach (var path in paths)
            {
                var fliepath = Application.dataPath+path;
                var text = File.ReadAllText(fliepath);
                File.WriteAllText(fliepath,text,new UTF8Encoding(true));
            }
            
            AssetDatabase.Refresh();
        }
    }

}
