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
            alignmentScore = value;
            Alignment = alignmentScore >= 0 ? Alignment.Good : Alignment.Bad;
        }
    }

    public Alignment Alignment { 
        get
        {
            return alignment;
        }
        private set
        {
            alignment = value;
        }
    }

    public void AddAlignmentBonus(int score) => AlignmentScore += score;
}
