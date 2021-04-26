using UnityEngine;

[AddComponentMenu("JsonUI/LanguageLabel")]
[RequireComponent(typeof(UnityEngine.UI.Text))]
public class LanguageLabel : MonoBehaviour
{
    [Tooltip("对应languge.json中的id")]
    public string id;

    private void Awake() {
        LanguageManager._Instance.switchLanguage += UpdateUI;
        UpdateUI();
    }


    public void UpdateUI(){
        if(!string.IsNullOrEmpty(id)){
           this.GetComponent<UnityEngine.UI.Text>().text = LanguageManager._Instance.GetLanguageLabel(id);
        }
    }
}
