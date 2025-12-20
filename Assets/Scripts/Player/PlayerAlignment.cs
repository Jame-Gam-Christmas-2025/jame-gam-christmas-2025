using UnityEngine;

public class PlayerAlignment : MonoBehaviour
{
    [SerializeField] private int alignmentScore;

    public static Alignment s_Alignment { get; private set; }

    public void AddAlignmentScore(int score) => alignmentScore += score;
    public void SubtractAlignmentScore(int score) => s_Alignment -= score;
}
