using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ProtoBufBuild : Editor
{
    const string Extension = ".proto";

    [MenuItem("ProtoBuf/Build Selection")]
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
        string csharpDir = Application.dataPath+"/JinJvLi/Script/ProtoCsharp";
        if(!Directory.Exists(csharpDir))
        {
            Directory.CreateDirectory(csharpDir);
        }
        string cmd,proto_path,csharp_out;
        csharp_out = csharpDir;
        foreach (var path in protoFilePaths)
        {
            proto_path = path;
            cmd = $"protoc --proto_path={proto_path} test.proto --csharp_out={csharp_out}";
            RunCmd(cmd);
        }
        AssetDatabase.Refresh();
    }

    public static string RunCmd(string cmd)
    {
        Process proc = new Process();
        proc.StartInfo.CreateNoWindow = true;
        proc.StartInfo.FileName = "cmd.exe";
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.RedirectStandardError = true;
        proc.StartInfo.RedirectStandardInput = true;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.Start();
        proc.StandardInput.WriteLine(cmd);
        proc.StandardInput.WriteLine("exit");
        string outStr = proc.StandardOutput.ReadToEnd();
        proc.Close();
        return outStr;
    }
}
