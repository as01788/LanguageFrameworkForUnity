using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Text;
using Codeplex.Data;
using System.IO;
using System;
using Newtonsoft.Json;

public class LanguageFrameworkTool : Editor
{

    private static string outPath;

    private static void Init()
    {
        if (outPath == null)
        {
            outPath = string.Format("{0}/LanguageForamework_Outs", Application.dataPath);
            Directory.CreateDirectory(outPath);
        }
    }

    [MenuItem(@"LanguageTools/ConvertToJson")]
    public static void ConvertToJson()
    {
        if (Selection.transforms.Length <= 0) return;
        Init();

        foreach (var item in Selection.transforms)
        {
            JsonUI datas = new JsonUI();
            datas.UICompontents = new List<UICompontents>();
            // dynamic datas = new DynamicJson();
            //var datas = new {UICompontents=""};
            string parentName = item.name;
            //获得该节点开始的所有需要的组件
            Image[] images = item.GetComponentsInChildren<Image>();
            // List<string> uICompontents = new List<string>();
            if (images != null && images.Length > 0)
            {
                foreach (var image in images)
                {
                    UICompontents ui = new UICompontents();

                    #region path
                    ui.path=GetTransformPath(item.transform,image.transform);
                    #endregion

                    ui.uiType = "Image";

                    #region bundleName
                    string guid;
                    long id;
                    AssetDatabase.TryGetGUIDAndLocalFileIdentifier<Sprite>(image.sprite, out guid, out id);
                    string resPath = AssetDatabase.GUIDToAssetPath(guid);
                    if (resPath.Contains("Assets/Resources/"))
                    {
                        //证明在Resoureces下
                        ui.bundleName = "Resources";
                    }
                    else
                    {
                        string bundleName = AssetDatabase.GetImplicitAssetBundleName(resPath);
                        ui.bundleName = bundleName;
                        // Debug.Log(string.Format("GUID:{0},ID:{1},PATH:{2},BUNDLE:{3}", guid, id, resPath, bundleName));
                    }

                    #endregion

                    #region resName
                    string[] resPaths = resPath.Split('/');
                    string resName = resPaths[resPaths.Length - 1];
                    resName = resName.Replace(".jpg", "");
                    ui.resName = resName.Replace(".png", "");
                    #endregion

                    // uICompontents.Add(JsonUtility.ToJson(ui));
                    datas.UICompontents.Add(ui);
                    //Debug.Log(JsonUtility.ToJson(ui));
                }
            }
            string data = JsonConvert.SerializeObject(datas);

            // string data = DynamicJson.Serialize(new {UICompontents=uICompontents});

            // datas.UICompontents = DynamicJson.Serialize(uICompontents);
            // string data = datas.ToString();
            //把\和多余冒号去掉
            // data = data.Replace("\\", "");
            // data = data.Replace("\"[\"", "[");
            // data = data.Replace("\"]\"", "]");

            // data = data.Replace("\"{\"","{\"");
            // data = data.Replace("\"]","]");

            //data = data.Replace("\\\"","\"");


            //生成JSON文件
            //实际的文件名路径
            string jsonPath = string.Format("{0}/{1}.json", outPath, parentName);
            //判断该文件是否已存在，如果存在，则重命名备份一份
            if (File.Exists(jsonPath))
            {
                string jsonPath2 = string.Format("{0}/{1}_{2}.json", outPath, parentName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                FileInfo fi = new FileInfo(jsonPath);
                fi.MoveTo(jsonPath2);
            }
            File.WriteAllText(jsonPath, data, Encoding.UTF8);
            //匿名对象
            // var t = new{

            // };
        }


        AssetDatabase.Refresh();

        Debug.Log("Json文件已生成");
    }

    [MenuItem(@"LanguageTools/UnloadAllImage")]
    public static void UnloadAllImage()
    {
        if (Selection.transforms.Length <= 0) return;
        Init();

        foreach (var item in Selection.transforms)
        {
            string parentName = item.transform.name;
            string jsonPath = string.Format("{0}/{1}.json", outPath, parentName);
            if (File.Exists(jsonPath))
            {
                Image[] images = item.GetComponentsInChildren<Image>();
                JsonUI json = GetJsonUI(parentName);
                foreach (var image in images)
                {
                    string transformPath = GetTransformPath(item.transform,image.transform);

                    //判断是否存在该路径
                    foreach (var uiPath in json.UICompontents)
                    {
                        if (uiPath.path == transformPath)
                        {
                            image.sprite=null;
                            break;
                        }
                    }
                }
            }
            else
            {
                Debug.Log(string.Format("<color=#FD1C1C>没有找到 <color=#FFA013>{0}</color> 的JSON文件，为了安全起见,请先导出一份JSON文件。</color>", parentName));
            }
        }

        AssetDatabase.Refresh();

        Debug.Log("卸载所有Sprite完成.");
    }

    [MenuItem(@"LanguageTools/LoadRes")]
    public static void LoadRes()
    {
        if (Selection.transforms.Length <= 0) return;
        Init();

        foreach (var item in Selection.transforms)
        {
            string parentName = item.transform.name;
            string jsonPath = string.Format("{0}/{1}.json", outPath, parentName);
            if (File.Exists(jsonPath))
            {
                JsonUI json = GetJsonUI(parentName);
                Image[] images = item.transform.GetComponentsInChildren<Image>();
                foreach (var image in images)
                {
                    string path = GetTransformPath(item.transform,image.transform);
                    foreach (var ui in json.UICompontents)
                    {
                        if(ui.path==path){
                            //读取资源
                            string[] directorys= Directory.GetDirectories(Application.dataPath,ui.bundleName,SearchOption.AllDirectories);
                            foreach (var directory in directorys)
                            {
                                string[] assets = Directory.GetFiles(directory);
                                foreach (var asset in assets)
                                {
                                    if(asset.Contains(ui.resName+".jpg") || asset.Contains(ui.resName+".png")){
                                        string fielPath ="Assets/" +asset.Replace(Application.dataPath+"/","");
                                        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(fielPath);
                                        image.sprite=sprite;
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                Debug.Log(string.Format("<color=#FD1C1C>没有找到 <color=#FFA013>{0}</color> 的JSON文件</color>", parentName));
            }
        }
    }

    /// <summary>
    /// 得到JsonUI对象
    /// </summary>
    /// <param name="jsonName"></param>
    /// <returns></returns>
    private static JsonUI GetJsonUI(string jsonName)
    {
        TextAsset jsonText = AssetDatabase.LoadAssetAtPath<TextAsset>(string.Format("Assets/LanguageForamework_Outs/{0}.json", jsonName));
        return JsonConvert.DeserializeObject<JsonUI>(jsonText.text);
    }

    private static string GetTransformPath(Transform parent, Transform self)
    {
        if (self == parent)
        {
            return "this";
        }
        else
        {
            StringBuilder path = new StringBuilder();
            path.Append(self.name);
            Transform m_parent = self.parent;

            while (m_parent != null && m_parent != parent)
            {
                path.Insert(0, "/");
                path.Insert(0, m_parent.transform.name);
                m_parent = m_parent.parent;
            }
            string temp = path.ToString();
            path.Clear();
            return temp;
        }
    }

}

[System.Serializable]
public class JsonUI
{
    public List<UICompontents> UICompontents;
}
[System.Serializable]
public class UICompontents
{
    public string path;
    public string uiType;
    public string bundleName;
    public string resName;
}