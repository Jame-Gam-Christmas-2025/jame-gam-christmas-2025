using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData_", menuName = "Data/New character data")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public Sprite avatar;
}
