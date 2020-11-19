using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class ObjectJsonConverter 
{
    public void ExportToFile(Object objectum, string path)
    {
        string JsonText = CreateJson(objectum);
        CreateAndWriteFile(JsonText, path);
    }

    private string CreateJson(Object objectum)
    {
        return JsonConvert.SerializeObject(objectum);
    }

    private void CreateAndWriteFile(string JsonText, string path)
    {
        TextWriter tw = new StreamWriter(path);
    }

}
