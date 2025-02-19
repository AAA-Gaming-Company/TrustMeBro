using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxChild : MonoBehaviour {
    [Header("Information")]
    public Camera parallaxedCamera;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer[] children;

    [Header("Properties")]
    public float parallaxEffect;
    public bool isBackground;

    private float startPos;
    private float initialWidth;
    private float initialHeight;

    public void Init() {
        this.startPos = this.transform.position.x;
        this.initialWidth = this.spriteRenderer.bounds.size.x;
        this.initialHeight = this.transform.localScale.y;

        if (this.isBackground) {
            foreach (SpriteRenderer child in this.children) {
                Destroy(child.gameObject);
            }

            this.transform.localPosition = new Vector3(0, 0, this.transform.localPosition.z);
        } else {
            foreach (SpriteRenderer child in this.children) {
                child.sprite = this.spriteRenderer.sprite;
            }
        }
    }

    private void Update() {
        if (this.isBackground) { //The background just follows the camera
            float cameraHeight = this.parallaxedCamera.orthographicSize * 2f;
            float cameraWidth = cameraHeight * this.parallaxedCamera.aspect;
            this.transform.localScale = new Vector3((cameraWidth / this.initialWidth) + 0.2f, this.initialHeight + 0.2f, this.transform.localScale.z);
            return;
        }

        //Calculate the offsets
        float distance = this.parallaxedCamera.transform.position.x * this.parallaxEffect;
        float temp = this.parallaxedCamera.transform.position.x * (1 - this.parallaxEffect);

        this.transform.position = new Vector3(this.startPos + distance, this.parallaxedCamera.transform.position.y * (this.parallaxEffect * 0.75f), this.transform.position.z);

        float length = this.spriteRenderer.bounds.size.x;

        if (temp > this.startPos + length) {
            this.startPos += length;
        } else if (temp < this.startPos - length) {
            this.startPos -= length;
        }
    }
}
