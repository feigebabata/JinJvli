using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace FGUFW.Core
{
    public static class EditorFileHelper
    {
        public const string META = ".meta";
        public static string[] GetAllAssetPath(string dirPath)
        {
            var dir = Application.dataPath.Replace("Assets",dirPath);
            if(!Directory.Exists(dir))return new string[0];
            int length = 0;
            var flies = Directory.GetFiles(dir);
            foreach (var path in flies)
            {
                if(Path.GetExtension(path)==META)continue;
                length++;
            }
            var paths = new string[length];
            int index = 0;
            foreach (var path in flies)
            {
                if(Path.GetExtension(path)==META)continue;
                paths[index] = path.Replace(Application.dataPath,"Assets").Replace("\\","/");
                index++;
            }
            return paths;
        }
    }
}
