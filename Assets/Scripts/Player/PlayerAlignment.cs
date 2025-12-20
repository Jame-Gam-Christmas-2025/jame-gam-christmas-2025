using UnityEngine;

public class PlayerAlignment : MonoBehaviour
{
    [SerializeField] private int alignmentScore;
    [SerializeField] private Alignment alignment;

    public int AlignmentScore
    {
        get { return alignmentScore; }
        set
        {
            Alignment = alignmentScore >= 0 ? Alignment.Good : Alignment.Bad;

            alignmentScore = value;
        }
    }

    public Alignment Alignment { get; private set; }

    public void AddAlignmentScore(int score) => AlignmentScore += score;
    public void SubtractAlignmentScore(int score) => AlignmentScore -= score;
}
