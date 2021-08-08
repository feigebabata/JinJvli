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
    const string PROTOC_LOCAL_PATH = "/Develop/FGUFW/Core/Layer2/Protobuf/Editor/protoc-3.7.1-win64/protoc.exe";

    [MenuItem("Assets/Build Proto")]
    public static void Build()
    {
        Object[] objects = Selection.objects;
        if(objects==null || objects.Length==0)
        {
            UnityEngine.Debug.LogWarning("[ProtoBufBuild.Build]没有选择文件");
            return;
        }
        string protocPath = Application.dataPath+PROTOC_LOCAL_PATH;
        if(!File.Exists(protocPath))
        {
            UnityEngine.Debug.LogError("[ProtoBufBuild.Build] ProtocPath路径错误");
            return;
        }
        List<string> protoFilePaths = new List<string>();
        string fileLocalPath;
        for (int i = 0; i < objects.Length; i++)
        {
            fileLocalPath = AssetDatabase.GetAssetPath(objects[i]);
            if(!string.IsNullOrEmpty(fileLocalPath))
            {
                if(Path.GetExtension(fileLocalPath)==Extension)
                {
                    protoFilePaths.Add(fileLocalPath.Replace("Assets/",""));
                }
            }
        }
        // UnityEngine.Debug.LogWarning($"[ProtoBufBuild.Build]选择proto文件数{protoFilePaths.Count}");
        string cmd,protoName,csharpDir;
        List<string> cmds;
        foreach (var path in protoFilePaths)
        {
            // UnityEngine.Debug.LogWarning($"[ProtoBufBuild.Build]等待转换脚本结束 {path}");
            protoName = Path.GetFileName(path);
            csharpDir = path.Replace(protoName,"CSharp");
            string csharpDirFull = $"{Application.dataPath}/{csharpDir}";
            if(!Directory.Exists(csharpDirFull))
            {
                Directory.CreateDirectory(csharpDirFull);
            }
            cmds = new List<string>();
            cmd = $"set ASSETS={Application.dataPath}";
            cmds.Add(cmd);
            cmd = $"set PROTOC={PROTOC_LOCAL_PATH}";
            cmds.Add(cmd);
            cmd = $"set FILE={path}";
            cmds.Add(cmd);
            cmd = $"set CSHARP={csharpDir}";
            cmds.Add(cmd);
            cmd = "%ASSETS%%PROTOC% %FILE% --proto_path=%ASSETS% --csharp_out %ASSETS%/%CSHARP%";
            cmds.Add(cmd);
            var outText = RunCmd(cmds);
           
            UnityEngine.Debug.LogWarning($"[ProtoBufBuild.Build] {outText} 转换脚本结束 {path}");
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
            // UnityEngine.Debug.Log(cmd);
            proc.StandardInput.WriteLine(cmd);
        }
        proc.StandardInput.WriteLine("exit");
        string outStr = proc.StandardOutput.ReadToEnd();
        proc.Close();
        return outStr;
    }
}
