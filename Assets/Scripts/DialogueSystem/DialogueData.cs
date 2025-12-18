using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue_", menuName = "Data/Dialogue System/New Dialogue data")]
public class DialogueData : ScriptableObject
{
    public int id;
    public DialogueText dialogueText;
    public ChoiceData[] choices;
}
