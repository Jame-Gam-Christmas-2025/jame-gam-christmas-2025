using UnityEngine;

public abstract class ChoiceCondition : ScriptableObject
{
    public DialogueData conditionTrueDialogue;
    public DialogueData conditionFalseDialogue;

    public bool ConditionIsTrue
    {
        get
        {
            return _conditionIsTrue;
        }
        set
        {
            _conditionIsTrue = value;

            if (value == true)
            {
                OnTrueCondition();
            }
            else
            {
                OnFalseCondition();            
            }
        }
    }

    private bool _conditionIsTrue = false;

    public virtual void CheckRequirement() { }

    public virtual void OnTrueCondition() { }

    public virtual void OnFalseCondition() { }
}
