using UnityEngine;
using System.Collections.Generic;
using ImpactFrameSystem;

[System.Serializable]
public class BossImpactColors
{
    public string bossName;
    public Color edgeColor = Color.white;
    public Color backgroundColor = Color.black;
}

public class BossImpactFrameManager : MonoBehaviour
{
    [Header("Boss Color Settings")]
    [SerializeField] private List<BossImpactColors> _bossColors = new List<BossImpactColors>();

    [Header("Default Colors")]
    [SerializeField] private Color _defaultEdgeColor = Color.white;
    [SerializeField] private Color _defaultBackgroundColor = Color.black;

    public void SetColorsForBoss(string bossName)
    {
        if (ImpactFrameManager.Instance == null)
        {
            Debug.LogWarning("ImpactFrameManager not found!");
            return;
        }

        // Find colors for this boss
        BossImpactColors colors = _bossColors.Find(b => b.bossName == bossName);

        if (colors != null)
        {
            // Apply boss-specific colors
            ImpactFrameManager.Instance.edgeColor = colors.edgeColor;
            ImpactFrameManager.Instance.backgroundColor = colors.backgroundColor;

            Debug.Log($"Applied Impact Frame colors for boss: {bossName} (Edge: {colors.edgeColor}, BG: {colors.backgroundColor})");
        }
        else
        {
            // Apply default colors
            ImpactFrameManager.Instance.edgeColor = _defaultEdgeColor;
            ImpactFrameManager.Instance.backgroundColor = _defaultBackgroundColor;

            Debug.Log($"No custom colors found for '{bossName}', using defaults");
        }
    }
}