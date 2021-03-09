using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

static public class CreatePlayScript
{
    const string TempScriptFolder = "Assets/Develop/FGUFW/Play/CreatePlayScript/Editor/";

    [MenuItem("Assets/Create/PlayManager Script",false,80)]
    static void CreatePlayManager()
    {
        string tempScriptPath = TempScriptFolder + "PlayManager.txt";
        string createPath = GetSelectPathOrFallback()+"/New PlayManager.cs";
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,ScriptableObject.CreateInstance<CreatePlayManagerScript>(),createPath,null,tempScriptPath);
    }

    [MenuItem("Assets/Create/PlayModule Script",false,80)]
    static void CreatePlayModule()
    {
        string tempScriptPath = TempScriptFolder + "PlayModule.txt";
        string createPath = GetSelectPathOrFallback()+"/New PlayModule.cs";
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,ScriptableObject.CreateInstance<CreatePlayModuleScript>(),createPath,null,tempScriptPath);
    }

    [MenuItem("Assets/Create/PlayView Script",false,80)]
    static void CreatePlayView()
    {
        string tempScriptPath = TempScriptFolder + "PlayView.txt";
        string createPath = GetSelectPathOrFallback()+"/New PlayView.cs";
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,ScriptableObject.CreateInstance<CreatePlayModuleScript>(),createPath,null,tempScriptPath);
    }
    
    //取得要创建文件的路径
    public static string GetSelectPathOrFallback()
    {
        string path = "Assets";
        //遍历选中的资源以获得路径
        //Selection.GetFiltered是过滤选择文件或文件夹下的物体，assets表示只返回选择对象本身
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }

    class CreatePlayManagerScript : EndNameEditAction
    {

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            //创建资源
            UnityEngine.Object obj = CreateScriptAssetFromTemplate(pathName, resourceFile);
            ProjectWindowUtil.ShowCreatedAsset(obj);//高亮显示资源
        }
 
        internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
        {
            //获取要创建资源的绝对路径
            string fullPath = Path.GetFullPath(pathName);
            //读取本地的模板文件
            StreamReader streamReader = new StreamReader(resourceFile);
            string text = streamReader.ReadToEnd();
            streamReader.Close();
            //获取文件名，不含扩展名
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
            Debug.Log("text==="+text);
            string nameSpace = Regex.Replace(fileNameWithoutExtension,"PlayManager","");
            //将模板类中的类名替换成你创建的文件名
            text = Regex.Replace(text, "#CLASSNAME#", fileNameWithoutExtension);
            text = Regex.Replace(text, "#NAMESPACE#", "GamePlay."+nameSpace);


            bool encoderShouldEmitUTF8Identifier = true; //参数指定是否提供 Unicode 字节顺序标记
            bool throwOnInvalidBytes = false;//是否在检测到无效的编码时引发异常
            UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
            bool append = false;
            //写入文件
            StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
            streamWriter.Write(text);
            streamWriter.Close();
            //刷新资源管理器
            AssetDatabase.ImportAsset(pathName);
            AssetDatabase.Refresh();
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
        }
    }

    

    class CreatePlayModuleScript : EndNameEditAction
    {

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            //创建资源
            UnityEngine.Object obj = CreateScriptAssetFromTemplate(pathName, resourceFile);
            ProjectWindowUtil.ShowCreatedAsset(obj);//高亮显示资源
        }
 
        internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
        {
            //获取要创建资源的绝对路径
            string fullPath = Path.GetFullPath(pathName);
            //读取本地的模板文件
            StreamReader streamReader = new StreamReader(resourceFile);
            string text = streamReader.ReadToEnd();
            streamReader.Close();
            //获取文件名，不含扩展名
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
            Debug.Log("pathName : "+pathName);

            string[] folderNodes = pathName.Split('/');
            string nameSpace = "";
            for (int i = 0; i < folderNodes.Length; i++)
            {
                Debug.LogWarning(folderNodes[i]);
                if(folderNodes[i]=="GamePlay")
                {
                    nameSpace = nameSpace+folderNodes[i+1];
                    break;
                }
            }
            
            //将模板类中的类名替换成你创建的文件名
            text = Regex.Replace(text, "#CLASSNAME#", fileNameWithoutExtension);
            text = Regex.Replace(text, "#NAMESPACE#", nameSpace);


            bool encoderShouldEmitUTF8Identifier = true; //参数指定是否提供 Unicode 字节顺序标记
            bool throwOnInvalidBytes = false;//是否在检测到无效的编码时引发异常
            UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
            bool append = false;
            //写入文件
            StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
            streamWriter.Write(text);
            streamWriter.Close();
            //刷新资源管理器
            AssetDatabase.ImportAsset(pathName);
            AssetDatabase.Refresh();
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
        }
    }

    

    class CreatePlayViewScript : EndNameEditAction
    {

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            //创建资源
            UnityEngine.Object obj = CreateScriptAssetFromTemplate(pathName, resourceFile);
            ProjectWindowUtil.ShowCreatedAsset(obj);//高亮显示资源
        }
 
        internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
        {
            //获取要创建资源的绝对路径
            string fullPath = Path.GetFullPath(pathName);
            //读取本地的模板文件
            StreamReader streamReader = new StreamReader(resourceFile);
            string text = streamReader.ReadToEnd();
            streamReader.Close();
            //获取文件名，不含扩展名
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
            Debug.Log("pathName : "+pathName);

            string[] folderNodes = pathName.Split('/');
            string nameSpace = "";
            for (int i = 0; i < folderNodes.Length; i++)
            {
                Debug.LogWarning(folderNodes[i]);
                if(folderNodes[i]=="GamePlay")
                {
                    nameSpace = nameSpace+folderNodes[i+1];
                    break;
                }
            }
            
            //将模板类中的类名替换成你创建的文件名
            text = Regex.Replace(text, "#CLASSNAME#", fileNameWithoutExtension);
            text = Regex.Replace(text, "#NAMESPACE#", "GamePlay."+nameSpace);


            bool encoderShouldEmitUTF8Identifier = true; //参数指定是否提供 Unicode 字节顺序标记
            bool throwOnInvalidBytes = false;//是否在检测到无效的编码时引发异常
            UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
            bool append = false;
            //写入文件
            StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
            streamWriter.Write(text);
            streamWriter.Close();
            //刷新资源管理器
            AssetDatabase.ImportAsset(pathName);
            AssetDatabase.Refresh();
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
        }
    }


}
