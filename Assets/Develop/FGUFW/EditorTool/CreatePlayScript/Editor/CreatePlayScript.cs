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
    const string TempScriptFolder = "Assets/Develop/FGUFW/EditorTool/CreatePlayScript/Editor/";

    [MenuItem("Assets/Create/Worlds/World Folder",false,80)]
    static void CreateWorld()
    {
        string tempScriptPath = TempScriptFolder + "World.txt";
        string createPath = GetSelectPathOrFallback()+"/New World";
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,ScriptableObject.CreateInstance<CreateWorldScript>(),createPath,null,tempScriptPath);
    }

    [MenuItem("Assets/Create/Worlds/Part Folder",false,80)]
    static void CreatePartFolder()
    {
        string createPath = GetSelectPathOrFallback()+"/Part";
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,ScriptableObject.CreateInstance<CreatePartFolderAction>(),createPath,null,null);
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

    class CreateWorldScript : EndNameEditAction
    {

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            //创建资源
            UnityEngine.Object obj = CreateScriptAssetFromTemplate(pathName, resourceFile);
            ProjectWindowUtil.ShowCreatedAsset(obj);//高亮显示资源
        }
 
        internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
        {
            
            #region 创建文件夹
            string direPath = Application.dataPath.Replace("Assets",pathName);
            var folders = direPath.Split('/');
            string nameSpace = "";
            for (int i = 0; i < folders.Length; i++)
            {
                if(folders[i]=="Worlds")
                {
                    nameSpace = folders[i+1];
                    break;
                }
            }
            string className = nameSpace+"World";
            // Debug.Log("创建文件夹 "+direPath);
            string moduleName = folders[folders.Length-1];
            if(!Directory.Exists(direPath))
            {
                Directory.CreateDirectory(direPath);
            }
            #endregion

            //获取要创建资源的绝对路径
            string fullPath = Path.GetFullPath($"{direPath}/{className}.cs");
            //读取本地的模板文件
            StreamReader streamReader = new StreamReader(resourceFile);
            string text = streamReader.ReadToEnd();
            streamReader.Close();

            //将模板类中的类名替换成你创建的文件名
            text = Regex.Replace(text, "#CLASSNAME#", className);
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
            string localPath = $"{pathName}/{className}.cs";
            AssetDatabase.ImportAsset(localPath);
            AssetDatabase.Refresh();
            return AssetDatabase.LoadAssetAtPath(localPath, typeof(UnityEngine.Object));
        }
    }

    

    class CreatePartFolderAction : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            #region 创建文件夹
            string direPath = Application.dataPath.Replace("Assets",pathName);
            // Debug.Log("创建文件夹 "+direPath);
            var folders = direPath.Split('/');
            string moduleName = folders[folders.Length-1];
            if(!Directory.Exists(direPath))
            {
                Directory.CreateDirectory(direPath);
            }
            #endregion

            string cloneScriptPath = null,newScriptPath=null,scriptText=null;
            string nameSpace = "";
            for (int i = 0; i < folders.Length; i++)
            {
                if(folders[i]=="Worlds")
                {
                    nameSpace = folders[i+1];
                    break;
                }
            }

            #region 创建PartScript
            cloneScriptPath = TempScriptFolder + "Part.txt";
            newScriptPath = $"{direPath}/{moduleName}.cs";
            scriptText = File.ReadAllText(cloneScriptPath);
            scriptText = Regex.Replace(scriptText, "#CLASSNAME#", moduleName);
            scriptText = Regex.Replace(scriptText, "#NAMESPACE#", nameSpace);
            File.WriteAllText(newScriptPath,scriptText);
            #endregion

            #region 创建PartInputScript
            cloneScriptPath = TempScriptFolder + "PartInput.txt";
            newScriptPath = $"{direPath}/{moduleName}Input.cs";
            scriptText = File.ReadAllText(cloneScriptPath);
            scriptText = Regex.Replace(scriptText, "#CLASSNAME#", moduleName+"Input");
            scriptText = Regex.Replace(scriptText, "#NAMESPACE#", nameSpace);
            File.WriteAllText(newScriptPath,scriptText);
            #endregion

            #region 创建PartOutputScript
            cloneScriptPath = TempScriptFolder + "PartOutput.txt";
            newScriptPath = $"{direPath}/{moduleName}Output.cs";
            scriptText = File.ReadAllText(cloneScriptPath);
            scriptText = Regex.Replace(scriptText, "#CLASSNAME#", moduleName+"Output");
            scriptText = Regex.Replace(scriptText, "#NAMESPACE#", nameSpace);
            File.WriteAllText(newScriptPath,scriptText);
            #endregion


            
            AssetDatabase.Refresh();
        }

    }


}
