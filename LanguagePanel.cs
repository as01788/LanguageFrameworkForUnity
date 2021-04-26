using UnityEngine;

[AddComponentMenu("JsonUI/LanguagePanel")]
public class LanguagePanel : MonoBehaviour
{
    public string jsonPath;

    private void Start() {
        Invoke(nameof(UpdateUI),1);
    }
    public void UpdateUI(){
        LanguageManager._Instance.InitPanel(this.transform,jsonPath);
    }
}
