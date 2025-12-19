using TMPro;
using UnityEngine;

public class SceneGrid : MonoBehaviour
{
    public GameObject sceneBtn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(sceneBtn != null)
        {
            foreach(string sceneName in GameSceneManager.Instance.GetAllBuiltSceneNames())
            {
                TMP_Text sceneBtnLabel = sceneBtn.GetComponentInChildren<TMP_Text>();
                sceneBtnLabel.text = sceneName;
                Object.Instantiate(sceneBtn, gameObject.transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
