using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using FGUFW.Core;
using System;

namespace FGUFW.ECS
{
    static public class CreateScript
    {
        const string TempScriptFolder = "Assets/Develop/FGUFW/ECS/Editor/";

        
        [UnityEditor.MenuItem("FGUFW.ECS/CheckCompType")]
        static public void CheckCompType()
        {
            ComponentHelper.CheckCompType();
        }

        [UnityEditor.MenuItem("FGUFW.ECS/CreateEnumComps")]
        static public void CreateEnumComps()
        {
            // var enumType = typeof(Worlds.BattleECS.Faction);
            // var direPath = GetSelectPathOrFallback();
            // Debug.Log($"创建枚举组件集合 {direPath}");
            // foreach (Worlds.BattleECS.Faction item in Enum.GetValues(enumType))
            // {
            //     var moduleName = ComponentHelper.GetEnumComponentName(item);
            //     createCompScript(moduleName,direPath);
            // }

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

        [MenuItem("Assets/Create/ECS/Component",false,80)]
        static void CreateComponent()
        {
            string tempScriptPath = GetSelectPathOrFallback();
            string createPath = GetSelectPathOrFallback()+"/New Component";
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,ScriptableObject.CreateInstance<CreateComponentScript>(),createPath,null,tempScriptPath);
        }

        class CreateComponentScript : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                // Debug.Log($"{pathName} {resourceFile}");
                string direPath = resourceFile;
                var folders = pathName.Split('/');
                string moduleName = folders[folders.Length-1];

                createCompScript(moduleName,direPath);

                string localPath = $"{direPath}/{moduleName}.cs";
                
                AssetDatabase.Refresh();
                var obj = AssetDatabase.LoadAssetAtPath(localPath, typeof(UnityEngine.Object));
                ProjectWindowUtil.ShowCreatedAsset(obj);//高亮显示资源
            }
        }

        static void createCompScript(string moduleName,string direPath)
        {
            var configPath = $"{TempScriptFolder}Config.txt";
            var config = File.ReadAllLines(configPath);
            var typeIndex = config[0].ToInt32();
            string nameSpace = config[1]; 

            string cloneScriptPath = null,newScriptPath=null,scriptText=null;
            

            #region 创建Script
            cloneScriptPath = TempScriptFolder + "Component.txt";
            // Debug.Log($"{direPath}\n{moduleName}");
            newScriptPath = $"{direPath}/{moduleName}.cs";
            scriptText = File.ReadAllText(cloneScriptPath);
            scriptText = Regex.Replace(scriptText, "#CLASSNAME#", moduleName);
            scriptText = Regex.Replace(scriptText, "#NAMESPACE#", nameSpace);
            scriptText = Regex.Replace(scriptText, "#TYPE#", typeIndex.ToString());
            File.WriteAllText(newScriptPath,scriptText);
            #endregion

            typeIndex++;
            config[0] = typeIndex.ToString();
            File.WriteAllLines(configPath,config);
            
            string localPath = $"{direPath}/{moduleName}.cs";
            localPath = localPath.Replace(Application.dataPath,"Assets");
            AssetDatabase.ImportAsset(localPath);
        }

        

        [MenuItem("Assets/Create/ECS/System",false,80)]
        static void CreateSystem()
        {
            string tempScriptPath = GetSelectPathOrFallback();
            string createPath = GetSelectPathOrFallback()+"/New System";
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,ScriptableObject.CreateInstance<CreateSystemScript>(),createPath,null,tempScriptPath);
        }

        class CreateSystemScript : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                // Debug.Log($"{pathName} {resourceFile}");
                string direPath = resourceFile;
                var folders = pathName.Split('/');
                string moduleName = folders[folders.Length-1];
                var configPath = $"{TempScriptFolder}Config.txt";
                var config = File.ReadAllLines(configPath);
                var typeIndex = config[2].ToInt32();
                string nameSpace = config[1]; 

                string cloneScriptPath = null,newScriptPath=null,scriptText=null;
                

                #region 创建Script
                cloneScriptPath = TempScriptFolder + "System.txt";
                // Debug.Log($"{direPath}\n{moduleName}");
                newScriptPath = $"{direPath}/{moduleName}.cs";
                scriptText = File.ReadAllText(cloneScriptPath);
                scriptText = Regex.Replace(scriptText, "#CLASSNAME#", moduleName);
                scriptText = Regex.Replace(scriptText, "#NAMESPACE#", nameSpace);
                scriptText = Regex.Replace(scriptText, "#TYPE#", typeIndex.ToString());
                File.WriteAllText(newScriptPath,scriptText);
                #endregion

                typeIndex++;
                config[2] = typeIndex.ToString();
                File.WriteAllLines(configPath,config);
                
                string localPath = $"{direPath}/{moduleName}.cs";
                localPath = localPath.Replace(Application.dataPath,"Assets");
                AssetDatabase.ImportAsset(localPath);
                AssetDatabase.Refresh();
                var obj = AssetDatabase.LoadAssetAtPath(localPath, typeof(UnityEngine.Object));
                ProjectWindowUtil.ShowCreatedAsset(obj);//高亮显示资源
            }
        }


    }
}