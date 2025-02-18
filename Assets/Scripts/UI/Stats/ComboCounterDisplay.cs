using UnityEngine;
using TMPro;
using MoreMountains.Feedbacks;

/// <summary>
/// To use this script, simply attach it to a GameObject and link a TextMeshProUGUI
/// component to the comboCounterText field. This will be the text that displays the
/// current combo count. If you want to add feedback to the combo counter, making it
/// more noticeable when the combo increases, link a MMF_Player component to the
/// updateFeedback field.
/// No further configuration is needed.
/// </summary>
public class ComboCounterDisplay : MonoBehaviour {
    public TextMeshProUGUI comboCounterText;
    public MMF_Player updateFeedback;

    private void Awake() {
        StatsManager.Instance.AssignComboCounterDisplay(this);
    }

    private void OnDestroy()  {
        StatsManager.Instance.RemoveComboCounterDisplay();
    }

    public void UpdateComboCounter() {
        this.comboCounterText.text = StatsManager.Instance.GetCurrentCombo() + "x";

        if (StatsManager.Instance.GetCurrentCombo() > 1 && this.updateFeedback != null) {
            this.updateFeedback.PlayFeedbacks();
        }
    }
}
