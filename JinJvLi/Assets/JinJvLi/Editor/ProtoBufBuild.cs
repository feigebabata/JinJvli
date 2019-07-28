using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class ProtoBufBuild : Editor
{
    const string Extension = ".proto";

    [MenuItem("Assets/Build Proto")]
    public static void Build()
    {
        Object[] objects = Selection.objects;
        if(objects==null || objects.Length==0)
        {
            UnityEngine.Debug.LogWarning("[ProtoBufBuild.Build]没有选择文件");
            return;
        }
        List<string> protoFilePaths = new List<string>();
        string localPath,filePath;
        for (int i = 0; i < objects.Length; i++)
        {
            localPath = AssetDatabase.GetAssetPath(objects[i]);
            if(!string.IsNullOrEmpty(localPath))
            {
                filePath = Application.dataPath.Replace("Assets",localPath);
                if(Path.GetExtension(filePath)==Extension)
                {
                    protoFilePaths.Add(filePath);
                }
            }
        }
        UnityEngine.Debug.LogWarning($"[ProtoBufBuild.Build]选择proto文件数{protoFilePaths.Count}");
        string cmd,protoName,csharpDir;
        string cmdFilepath=Application.dataPath+"/JinJvLi/Editor/build.cmd";
        string protocPath = Application.dataPath+"/protoc-3.7.1-win64/Output/protoc.exe";
        List<string> cmds;
        foreach (var path in protoFilePaths)
        {
            protoName = Path.GetFileName(path);
            csharpDir = path.Replace(protoName,"CSharp");
            if(!Directory.Exists(csharpDir))
            {
                Directory.CreateDirectory(csharpDir);
            }
            cmds = new List<string>();
            cmd = $"\n{Path.GetPathRoot(path)}";
            cmds.Add(cmd);
            cmd = $"\ncd {path.Replace(protoName,"")}";
            cmds.Add(cmd);
            cmd = $"\n{protocPath} ./{protoName}  --csharp_out ./CSharp";
            cmds.Add(cmd);
            RunCmd(cmds);
        }
        AssetDatabase.Refresh();
    }

    public static string RunCmd(List<string> cmds)
    {
        Process proc = new Process();
        proc.StartInfo.CreateNoWindow = true;
        proc.StartInfo.FileName = "cmd.exe";
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.RedirectStandardError = true;
        proc.StartInfo.RedirectStandardInput = true;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.Start();
        foreach(string cmd in cmds)
        {
            proc.StandardInput.WriteLine(cmd);
        }
        proc.StandardInput.WriteLine("exit");
        string outStr = proc.StandardOutput.ReadToEnd();
        proc.Close();
        return outStr;
    }
}
