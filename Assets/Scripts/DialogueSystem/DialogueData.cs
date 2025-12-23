using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue_", menuName = "Data/Dialogue System/New Dialogue data")]
public class DialogueData : ScriptableObject
{
    public CharacterData characterData;
    public DialogueText dialogueText;
    public ChoiceData[] choices;
    public DialogueData nextDialogue;
}
