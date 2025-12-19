using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneBtn : MonoBehaviour
{
    private TMP_Text _textComponent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(LoadScene);
    }

    private void LoadScene()
    {
        _textComponent =  gameObject.GetComponentInChildren<TMP_Text>();

        if(_textComponent != null)
        {
            GameSceneManager.Instance.LoadSceneByName(_textComponent.text);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
