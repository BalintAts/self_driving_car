using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ObjectJsonConverter 
{
    public void ExportToFile(Object objectum)
    {
        string JsonText = CreateJson(objectum);
        CreateAndWriteFile(JsonText);
        string networkJson = JsonConvert.SerializeObject(objectum);
    }

    private string CreateJson(Object objectum)
    {
        return JsonConvert.SerializeObject(objectum);
    }

    private void CreateAndWriteFile(string JsonText)
    {

    }

}
