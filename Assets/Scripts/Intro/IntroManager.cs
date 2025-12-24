using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class IntroManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI introTextMesh;
    [SerializeField, Range(1, 100), Tooltip("Letter rate in char per second")] private int letterRate = 70;
    [SerializeField] private List<string> texts = new();

    private string _introText = "";
    private bool _dialogueAnim = false;
    private Coroutine _textAnimCoroutine;
    private int _currentTextIndex;

    private void Start()
    {
        _textAnimCoroutine = StartCoroutine(TextAnimation());
    }

    /// <summary>
    /// Text animation coroutine.
    /// </summary>
    /// <returns></returns>
    private IEnumerator TextAnimation()
    {
        introTextMesh.text = texts[_currentTextIndex];

        _dialogueAnim = true;

        float letterDelay = 1f / letterRate;
        _introText = introTextMesh.text;

        introTextMesh.text = "";

        foreach (char c in _introText)
        {
            introTextMesh.text += c;
            yield return new WaitForSeconds(letterDelay);
        }

        OnTextAnimEnd();

        yield return new WaitForSeconds(3f);

        _currentTextIndex++;

        if (_currentTextIndex >= texts.Count)
        {
            GameSceneManager.Instance.LoadSceneByName("leveldesign");
            
            yield break;
        }

        
        _textAnimCoroutine = StartCoroutine(TextAnimation());
    }

    /// <summary>
    /// Called when text animation ends.
    /// </summary>
    private void OnTextAnimEnd()
    {
        //StopCoroutine(_textAnimCoroutine);

        _dialogueAnim = false;

        introTextMesh.text = _introText;
    }
}
