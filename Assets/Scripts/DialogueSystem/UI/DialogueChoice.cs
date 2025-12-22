using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class DialogueChoice : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    [SerializeField] private UnityEngine.UI.Image cocheImage;
    [SerializeField] private NavigationSubmitEvent submitEvent;
    [SerializeField] private TextMeshProUGUI textMesh;

    // Data variables
    private DialogueData _nextDialogue;
    private ChoiceData _choiceData;

    private GameObject _player;

    private void Awake()
    {
        cocheImage.enabled = false;

        _player = GameObject.FindGameObjectWithTag("Player");
    }

    public void ShowCoche(bool enabled)
    {
        cocheImage.enabled = enabled;
    }

    public void SetChoice(ChoiceData choiceData)
    {
        _choiceData = choiceData;

        string choiceText = "";

        // Set choice text depending on game language
        switch (LocalizationManager.s_Instance.Language)
        {
            case Language.English:
                choiceText = choiceData.text.english;
                break;
            case Language.French:
                choiceText = choiceData.text.french;
                break;
        }

        SetText(choiceText);
        _nextDialogue = choiceData.nextDialogue;
    }

    public void SetText(string text)
    {
        textMesh.text = text;
    }

    #region Event functions
    public void OnClick()
    {
        DialogueView dialogueView = FindFirstObjectByType<DialogueView>();
        ChoiceCondition choiceCondition = _choiceData.choiceCondition;

        // Happen only if there is a condition in dialogue
        if (choiceCondition)
        {
            choiceCondition.CheckRequirement();

            if (choiceCondition.ConditionIsTrue == true)
                _nextDialogue = choiceCondition.conditionTrueDialogue;
            else
                _nextDialogue = choiceCondition.conditionFalseDialogue;
        }

        if (_player)
        {
            // Add alignment bonus to player
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAlignment>().AddAlignmentBonus(_choiceData.alignmentBonus);
        }

        // Start next dialogue
        dialogueView.StartDialogue(_nextDialogue);
    }
    #endregion

    #region Event system events
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnSelect(BaseEventData eventData)
    {
        ShowCoche(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ShowCoche(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        OnClick();
    }
    #endregion
}
