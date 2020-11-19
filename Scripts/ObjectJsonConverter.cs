using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;

public static class ObjectJsonConverter 
{
    public static void ExportToFile(object objectum, string path)
    {
        string JsonText = CreateJson(objectum);
        CreateAndWriteFile(JsonText, path);
    }

    private static string CreateJson(object objectum)
    {
        return JsonConvert.SerializeObject(objectum);
    }

    private static void CreateAndWriteFile(string JsonText, string path)
    {
        try
        {
            TextWriter tw = new StreamWriter(path);
            tw.Write(JsonText);
            tw.Close();
        } catch (Exception e)
        {
            Debug.Log("Writing file failed " + e);
        }
    }

    public static string ReadFile(string path)
    {
        string content = string.Empty;
        try
        {
            using (var sr = new StreamReader("TestFile.txt"))
            {
                content = sr.ReadToEnd();
            }
        }
        catch (IOException e)
        {
            Debug.Log("Reading file failed " + e);
        }
        return content;
    }

}
