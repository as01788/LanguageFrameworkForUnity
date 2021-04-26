using UnityEngine;

[AddComponentMenu("JsonUI/LanguageImage")]
[RequireComponent(typeof(UnityEngine.UI.Image))]
public class LanguageImage : MonoBehaviour
{
    [Tooltip("资源路径")]
    public string resPath;

    private void Awake()
    {
        LanguageManager._Instance.switchLanguage += UpdateUI;
        Invoke(nameof(UpdateUI), 1);
    }

#if UseAsync
    public async void UpdateUI(){
        Debug.Log("Is Async");
        if(!string.IsNullOrEmpty(resPath))
            GetComponent<UnityEngine.UI.Image>().sprite = await LanguageManager._Instance.GetLanguageSprite(resPath);
    }
#else
    public void UpdateUI()
    {
        Debug.Log("Not Async");
        if (!string.IsNullOrEmpty(resPath))
            GetComponent<UnityEngine.UI.Image>().sprite = LanguageManager._Instance.GetLanguageSprite(resPath);
    }
#endif
}
