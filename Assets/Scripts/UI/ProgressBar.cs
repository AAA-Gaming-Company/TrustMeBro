using UnityEngine;

public class ProgressBar : MonoBehaviour {
    [Min(0)]
    public float min;
    [Min(0)]
    public float max;
    public Transform filler;

    private float current;

    private void Start() {
        this.current = this.min;
        this.UpdateValue(this.current);
    }

    public void UpdateValue(float value) {
        float clamped = Mathf.Clamp(value, this.min, this.max);
        this.current = clamped;

        Vector3 scale = this.filler.transform.localScale;
        if (this.max == 0) {
            scale.x = 0;
        } else {
            scale.x = clamped / this.max;
        }
        this.filler.transform.localScale = scale;
    }
}
