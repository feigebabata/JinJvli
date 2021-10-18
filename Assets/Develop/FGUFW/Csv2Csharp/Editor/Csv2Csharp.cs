using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FGUFW.Core
{
    static public class Csv2Csharp
    {

        const string Extension = ".csv";

        [MenuItem("Assets/Csv2Csharp")]
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
                createScript(path);
            }
            
            AssetDatabase.Refresh();
        }

        private static void createScript(string path)
        {
            var lines = File.ReadAllLines(Application.dataPath+path);
            var fullclassname = lines[0].Split(',')[0].Split('.');
            var memberNames = lines[1].Split(',');
            var memberTypes = lines[2].Split(',');
            var memberInfos = lines[3].Split(',');
            var frist_type = memberTypes[0];
            var frist_member = memberNames[0];

            string class_name = fullclassname[fullclassname.Length-1];
            string namespace_name = String.Join(".",fullclassname,0,fullclassname.Length-1);
            string scriptText = SCRIPT_TEXT;

            StringBuilder text_members = new StringBuilder();
            StringBuilder text_memberSets = new StringBuilder();
            for (int i = 0; i < memberNames.Length; i++)
            {
                string memberType = memberTypes[i];
                string memberName = memberNames[i];
                string info = memberInfos[i];
                text_members.AppendLine(@$"
        /// <summary>
        /// {info}
        /// </summary>
        public {memberType} {memberName};");

                if(memberType=="int")
                {
                    text_memberSets.AppendLine($"            if(!string.IsNullOrEmpty(members[{i}])) {memberName} = int.Parse(members[{i}]);");
                }
                else if(memberType=="float")
                {
                    text_memberSets.AppendLine($"            if(!string.IsNullOrEmpty(members[{i}])) {memberName} = float.Parse(members[{i}]);");
                }
                else if(memberType=="string")
                {
                    text_memberSets.AppendLine($"            {memberName} = members[{i}];");
                }
                else if(memberType=="Vector2")
                {
                    string tempSetText =
@$"
            if(!string.IsNullOrEmpty(members[{i}])) 
            {{
                string[] temp_vec_{i} = members[{i}].Split('\\');
                {memberName} = new Vector2(float.Parse(temp_vec_{i}[0]),float.Parse(temp_vec_{i}[1]));
            }}
";
                    text_memberSets.AppendLine(tempSetText);
                }
                else if(memberType=="Vector3")
                {
                    string tempSetText =
@$"
            if(!string.IsNullOrEmpty(members[{i}])) 
            {{
                string[] temp_vec_{i} = members[{i}].Split('\\');
                {memberName} = new Vector3(float.Parse(temp_vec_{i}[0]),float.Parse(temp_vec_{i}[1]),float.Parse(temp_vec_{i}[2]));
            }}
";
                    text_memberSets.AppendLine(tempSetText);
                }
                else if(memberType.IndexOf("[]")!=-1)
                {
                    string array_type = memberType.Replace("[]","");
                    string tempSetText = null;
                    if(array_type=="int")
                    {
                        tempSetText = $"{memberName}[j] = int.Parse(temp_arr_{i}[j]);";
                    }
                    else if(array_type=="float")
                    {
                        tempSetText = $"{memberName}[j] = float.Parse(temp_arr_{i}[j]);";
                    }
                    else if(array_type=="string")
                    {
                        tempSetText = $"{memberName}[j] = temp_arr_{i}[j];";
                    }
                    else if(array_type=="Vector2")
                    {
                        tempSetText = $"{{string[] temp_vec_arr=temp_arr_{i}[j].Split('\\\\'); {memberName}[j] = new Vector2(float.Parse(temp_vec_arr[0]),float.Parse(temp_vec_arr[1]));}}";
                    }
                    else if(array_type=="Vector3")
                    {
                        tempSetText = $"{{string[] temp_vec_arr=temp_arr_{i}[j].Split('\\\\'); {memberName}[j] = new Vector3(float.Parse(temp_vec_arr[0]),float.Parse(temp_vec_arr[1]),float.Parse(temp_vec_arr[2]));}}";
                    }
                    else
                    {
                        Debug.LogError($"未标注类型 {memberType} 索引={i}");
                    }
                    string array_text=
@$"
            if(!string.IsNullOrEmpty(members[{i}])) 
            {{
                string[] temp_arr_{i} = members[{i}].Split('\'');
                {memberName} = new {array_type}[temp_arr_{i}.Length];
                for (int j = 0; j < temp_arr_{i}.Length; j++)
                    {tempSetText}
            }}
";
                    text_memberSets.AppendLine(array_text);
                }
                else
                {
                    Debug.LogError($"未标注类型 {memberType} 索引={i}");
                }
            }

            scriptText = scriptText.Replace("#NAMESPACE#",namespace_name);
            scriptText = scriptText.Replace("#CLASSNAME#",class_name);
            scriptText = scriptText.Replace("#MEMBERS#",text_members.ToString());
            scriptText = scriptText.Replace("#MEMBER_SETS#",text_memberSets.ToString());
            scriptText = scriptText.Replace("#FRIST_TYPE#",frist_type);
            scriptText = scriptText.Replace("#FRIST_MEMBER#",frist_member);

            string scriptFilePath = $"{Path.GetDirectoryName(Application.dataPath+path)}/{class_name}.cs";
            File.WriteAllText(scriptFilePath,scriptText,Encoding.UTF8);
            
        }

        const string SCRIPT_TEXT = 
@"
using System.Collections.Generic;
using UnityEngine;

namespace #NAMESPACE#
{
    public class #CLASSNAME#
    {
#MEMBERS#

        public #CLASSNAME#(string text)
        {
            text = text.Trim();
            string[] members = text.Split(',');
#MEMBER_SETS#
        }

        static public #CLASSNAME#[] ToArray(string csvText)
        {
            csvText = csvText.Trim();
            string[] lines = csvText.Split('\n');
            #CLASSNAME#[] list = new #CLASSNAME#[lines.Length-4];
            for (int i = 0; i < list.Length; i++)
            {
                list[i] = new #CLASSNAME#(lines[i+4]);
            }
            return list;
        }

        static public Dictionary<#FRIST_TYPE#,#CLASSNAME#> ToDict(string csvText)
        {
            csvText = csvText.Trim();
            string[] lines = csvText.Split('\n');
            Dictionary<#FRIST_TYPE#,#CLASSNAME#> dict = new Dictionary<#FRIST_TYPE#,#CLASSNAME#>();
            for (int i = 4; i < lines.Length; i++)
            {
                var data = new #CLASSNAME#(lines[i]);
                dict.Add(data.#FRIST_MEMBER#,data);
            }
            return dict;
        }

    }
}
";

    }
}