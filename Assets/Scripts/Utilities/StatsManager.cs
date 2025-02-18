using System.Collections;
using UnityEngine;

/// <summary>
/// This class will keep track of the player's stats, such as enemies killed, shots fired.
/// There is no need to attach this script to any GameObject, it will set itself up.
/// </summary>
[DisallowMultipleComponent]
public class StatsManager : Singleton<StatsManager> {
    private int enemiesKilled = 0;
    private int shotsFired = 0;
    private int biggestCombo = 0;

    private int comboCounter = 0;
    private Coroutine comboTimerCoroutine;
    [Tooltip("How long the player has to kill an enemy to keep the combo going")]
    private int comboTimer = 5;
    private ComboCounterDisplay comboCounterDisplay = null;

    public void AddEnemyKilled() {
        this.enemiesKilled++;
        this.ComputeCombo();
    }

    public int GetEnemiesKilled() {
        return this.enemiesKilled;
    }

    public void AddShotFired() {
        this.shotsFired++;
    }

    public int GetShotsFired() {
        return this.shotsFired;
    }

    public int GetBiggestCombo() {
        return this.biggestCombo;
    }

    public int GetCurrentCombo() {
        return this.comboCounter;
    }

    // --- Combo functions ---

    private void ComputeCombo() {
        if (this.comboTimerCoroutine != null) {
            StopCoroutine(this.comboTimerCoroutine);
        }
        this.comboTimerCoroutine = StartCoroutine(this.ComboTimer());
    }

    private IEnumerator ComboTimer() {
        this.comboCounter++;

        if (this.comboCounter > this.biggestCombo) {
            this.biggestCombo = this.comboCounter;
        }

        yield return new WaitForSeconds(this.comboTimer);
        this.comboCounter = 0;
        this.comboTimerCoroutine = null;
    }

    public void AssignComboCounterDisplay(ComboCounterDisplay comboCounterDisplay) {
        this.comboCounterDisplay = comboCounterDisplay;
    }

    public void RemoveComboCounterDisplay() {
        this.comboCounterDisplay = null;
    }

    public void UpdateComboCounter() {
        if (this.comboCounterDisplay == null) {
            return;
        }

        if (this.comboCounter == 0) {
            this.comboCounterDisplay.gameObject.SetActive(false);
        } else {
            this.comboCounterDisplay.gameObject.SetActive(true);
            this.comboCounterDisplay.UpdateComboCounter();
        }
    }
}
