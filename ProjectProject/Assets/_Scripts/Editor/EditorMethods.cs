using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class EditorMethods : Editor
{
    const string extension = ".cs";
    public static void WriteToEnum<T>(string path, string name, ICollection<T> data)
    {
        using (StreamWriter file = File.CreateText(path + name + extension))
        {
            file.WriteLine("public enum " + name + "{");

            int i = 0;
            foreach (var line in data)
            {
                string lineRep = line.ToString().Replace(" ", string.Empty);
                if (!string.IsNullOrEmpty(lineRep))
                {
                    file.WriteLine(string.Format("{0},", lineRep));
                    i++;
                }
            }

            file.WriteLine("}");

            AssetDatabase.ImportAsset(path + name + extension);
        }
    }

    public static List<string> ReadToEnum(string path, string name)
    {
        List<string> enumList = new List<string>();

        using (StreamReader reader = new StreamReader(path + name + extension))
        {
           string enumObject = reader.ReadToEnd();
           enumObject = enumObject.Split("public enum " + name + "{")[1];
           enumObject = enumObject.Split("}")[0];
           string[] levelIds = enumObject.Split("\r\n");
           for(int i = 1; i < levelIds.Length-1; i++)
           {
                levelIds[i] = levelIds[i].Split(",")[0];
                enumList.Add(levelIds[i].Trim());
            }
           
        }

        return enumList;
    }

}
