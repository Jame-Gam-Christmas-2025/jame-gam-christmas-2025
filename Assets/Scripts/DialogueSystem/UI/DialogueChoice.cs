using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DialogueChoice : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    [SerializeField] private UnityEngine.UI.Image cocheImage;
    [SerializeField] private NavigationSubmitEvent submitEvent;
    [SerializeField] private TextMeshProUGUI textMesh;

    public DialogueData nextDialogue { get; set; }

    private ChoiceData _choiceData;

    private void Awake()
    {
        cocheImage.enabled = false;
    }

    public void ShowCoche(bool enabled)
    {
        cocheImage.enabled = enabled;
    }

    public void SetChoice(ChoiceData choiceData)
    {
        _choiceData = choiceData;

        string choiceText = "";

        switch (GameManager.Instance.Language)
        {
            case Language.English:
                choiceText = choiceData.text.english;
                break;
            case Language.French:
                choiceText = choiceData.text.french;
                break;
        }

        SetText(choiceText);
        nextDialogue = choiceData.nextDialogue;
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

        if (choiceCondition != null)
        {
            choiceCondition.CheckRequirement();

            if (choiceCondition.ConditionIsTrue == true)
                nextDialogue = choiceCondition.conditionTrueDialogue;
            else
                nextDialogue = choiceCondition.conditionFalseDialogue;
        }

        dialogueView.StartDialogue(nextDialogue);
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
