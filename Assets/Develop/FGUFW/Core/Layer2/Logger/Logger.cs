using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using UnityEngine;

namespace FGUFW.Core
{
    static public class Logger
    {
        [RuntimeInitializeOnLoadMethod]
        static void runtimeInit()
        {
            init();
        }

        static string _filePath;
        static StringBuilder logcache = new StringBuilder();
        static StreamWriter _writer;


        [Conditional("LOGGER_ON")]
        private static void init()
        {
            createLogFile();
            Application.logMessageReceivedThreaded += logCallback;
            Application.quitting += quitting;
        }

        private static void createLogFile()
        {
            string fileName = $"{DateTime.Now.Year.ToString("D4")}{DateTime.Now.Month.ToString("D2")}{DateTime.Now.Day.ToString("D2")}{DateTime.Now.Hour.ToString("D2")}{DateTime.Now.Minute.ToString("D2")}{DateTime.Now.Second.ToString("D2")}.txt";
            string fileDicr = Path.Combine(Application.persistentDataPath,"Logger");
            // #if UNITY_EDITOR
            //     fileDicr = Application.streamingAssetsPath;
            // #endif
            _filePath = Path.Combine(fileDicr,fileName);

            if (!Directory.Exists(fileDicr))  
            {  
                Directory.CreateDirectory(fileDicr);
            }
            UnityEngine.Debug.Log($"***Logger filePath:{_filePath}");
            _writer = new StreamWriter(_filePath,true,Encoding.UTF8);
        }

        private static void quitting()
        {
            _writer.WriteLine("\n------------------Logger End--------------------");
            _writer.Close();
            _writer.Dispose();
        }

        static void logCallback(string condition, string stackTrace, LogType type)
        {
            _writer.WriteLine($"{condition}\n{stackTrace}\n\n");
        }

        [Conditional("LOGGER_ON")]
        static public void d(object text)
        {
            UnityEngine.Debug.Log(text);
        }

        [Conditional("LOGGER_ON")]
        static public void w(object text)
        {
            UnityEngine.Debug.LogWarning(text);
        }

        [Conditional("LOGGER_ON")]
        static public void e(object text)
        {
            UnityEngine.Debug.LogError(text);
        }
    }
}