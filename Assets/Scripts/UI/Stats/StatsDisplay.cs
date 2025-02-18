using UnityEngine;
using TMPro;

/// <summary>
/// This display will require another script to update the stats. It will only update
/// the text fields with the current stats if the UpdateStats method is called. The
/// only extra setup required is to link the TextMeshProUGUI components to the fields.
/// </summary>
public class StatsDisplay : MonoBehaviour {
    public TextMeshProUGUI enemiesKilledText;
    public TextMeshProUGUI shotsFiredText;
    public TextMeshProUGUI biggestComboText;

    public void UpdateStats() {
        this.enemiesKilledText.text = "Enemies killed: " + StatsManager.Instance.GetEnemiesKilled();
        this.shotsFiredText.text = "Shots fired: " + StatsManager.Instance.GetShotsFired();
        this.biggestComboText.text = "Biggest combo: " + StatsManager.Instance.GetBiggestCombo();
    }
}
