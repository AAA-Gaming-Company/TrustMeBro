using UnityEngine;

public class ProgressBar : MonoBehaviour {
    public float min;
    public float max;
    public Transform filler;

    private float current;

    private void Start() {
        this.current = this.min;
        this.UpdateValue(this.current);
    }

    public void UpdateValue(float value) {
        float clamped = Mathf.Clamp(value, min, max);
        this.current = clamped;

        Vector3 scale = this.filler.transform.localScale;
        scale.x = clamped / this.max;
        this.filler.transform.localScale = scale;
    }
}
