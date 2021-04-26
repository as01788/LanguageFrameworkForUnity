/*
需要.Net4

使用UseAD不可异步
*/


using System.Collections.Generic;
using UnityEngine;
using Codeplex.Data;

#if UseAsync
using Cysharp.Threading.Tasks;
#endif

#if UseAD
using UnityEngine.AddressableAssets;
#endif

public delegate void SwitchLanguage();

public enum LanguageFloder
{
    //中文简体
    cn,
    //中文繁体
    hk,
    //英文
    en,
    //德文
    de,
    //西班牙文
    es,
    //丹麦文
    da,
    //捷克文
    cs,
    //希腊文
    el,
    //意大利文
    it,
    //日文
    jp,
    //韩文
    kr,
    //泰文
    th,
    //乌克兰文
    uk,
    //土耳其文
    tr,
    //瑞典文
    sv,
    //俄文
    ru,
    //葡萄牙文
    pt,
    //波兰文
    pl,
}


public class LanguageManager
{
    private static LanguageManager instance;
    public static LanguageManager _Instance
    {
        get
        {
            if (instance == null)
                instance = new LanguageManager();
            return instance;
        }
    }

    private readonly string languagePath = "language";
    private readonly string assetBundlePath = Application.streamingAssetsPath + "/";
    private readonly string assetBundleSuffix = ".ab";

    public SwitchLanguage switchLanguage;

    private Dictionary<string, dynamic> languageLabels;
    public LanguageFloder currentLanguage { get; private set; }
    private AssetBundle currentBundle;

    public void Init(LanguageFloder language)
    {
        currentLanguage = language;
        TextAsset temp = GetJson(languagePath);
        if (temp)
        {
            dynamic json = DynamicJson.Parse(temp.text);
            languageLabels = new Dictionary<string, dynamic>();
            foreach (var item in json.languages)
            {
                languageLabels.Add(item.id, item);
            }
        }
        LoadBundle();
        
    }

    private void LoadBundle()
    {
        if (currentBundle)
        {
            currentBundle.Unload(true);
        }
        currentBundle = AssetBundle.LoadFromFile(assetBundlePath + currentLanguage.ToString() + assetBundleSuffix);
    }

    public void SwitchLanguage(LanguageFloder language)
    {
        if (language == currentLanguage)
        {
            Debug.LogWarning("不可切换相同语言");
            return;
        }
        currentLanguage = language;
        LoadBundle();
        switchLanguage?.Invoke();
    }

    /// <summary>
    /// 得到多语言文本
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string GetLanguageLabel(string id)
    {
        if (languageLabels.ContainsKey(id))
        {
            return languageLabels[id][currentLanguage.ToString()];
        }
        return "";
    }
#if UseAsync
    /// <summary>
    /// 得到多语言图片资源
    /// </summary>
    /// <param name="resPaht"></param>
    /// <returns></returns>
    public async UniTask<Sprite> GetLanguageSprite(string resPaht)
    {
        return await GetRes<Sprite>(currentLanguage.ToString(), resPaht);
    }
    public async UniTask<T> GetRes<T>(string folderName,string resPath) where T:UnityEngine.Object{
        if(folderName=="Resources")
            return await Resources.LoadAsync<T>(folderName+"/"+resPath) as T;
        else{
            #if UseAD
                return await Addressables.LoadAssetAsync<T>(folderName+"/" + resPath);
            #else
                return await currentBundle.LoadAssetAsync<T>(resPath) as T;
            #endif
        }
    }
#else
    /// <summary>
    /// 得到多语言图片资源
    /// </summary>
    /// <param name="resPaht"></param>
    /// <returns></returns>
    public Sprite GetLanguageSprite(string resPaht)
    {
        return GetRes<Sprite>(currentLanguage.ToString(), resPaht);
    }
    public T GetRes<T>(string folderName, string resPath) where T : UnityEngine.Object
    {
        if (folderName == "Resources")
            return Resources.Load<T>(folderName + "/" + resPath);
        else{
            return currentBundle.LoadAsset<T>(resPath);
        }
    }
#endif

    public void InitPanel(Transform startParent, string jsonPath)
    {
        dynamic json = DynamicJson.Parse(GetJson(jsonPath).text);

        foreach (var temp in json.UICompontents)
        {
            if (temp.path == "this")
            {
                InitComponent(startParent, temp);
            }
            else
            {
                if (temp.path.Contains("+"))
                {
                    string[] paths = temp.path.Split('+');
                    if (paths != null && paths.Length > 0)
                    {
                        for (int j = 0; j < paths.Length; j++)
                        {
                            InitComponent(startParent.transform.Find(paths[j]), temp);
                        }
                    }
                }
                else
                {
                    InitComponent(startParent.transform.Find(temp.path), temp);
                }
            }
        }
    }
    private async void InitComponent(Transform temp, dynamic comt)
    {
        if (temp != null)
        {
            switch (comt.uiType)
            {
                case "Image":
                    UnityEngine.UI.Image image = temp.GetComponent<UnityEngine.UI.Image>();
                    if (image != null)
                    {
                        Sprite sprite = await GetRes<Sprite>(comt.bundleName, comt.resName);
                        if (sprite != null)
                            image.sprite = sprite;
                    }
                    else
                    {
                        Debug.Log(string.Format("路径：{0}，无法找到Image组件", comt.path));
                    }
                    break;
                case "Font":
                    UnityEngine.UI.Text text = temp.GetComponent<UnityEngine.UI.Text>();
                    if (text != null)
                    {
                        text.font = await GetRes<Font>(comt.bundleName, comt.resName);
                    }
                    else
                    {
                        Debug.Log(string.Format("路径：{0}，无法找到Text组件", comt.path));
                    }
                    break;
            }
        }
        else
        {
            Debug.Log(string.Format("无法找到路径：{0}", comt.path));
        }
    }


    private TextAsset GetJson(string path)
    {

        return Resources.Load<TextAsset>(path);
    }



}