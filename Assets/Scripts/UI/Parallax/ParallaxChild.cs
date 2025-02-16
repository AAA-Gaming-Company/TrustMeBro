using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxChild : MonoBehaviour {
    [Header("Information")]
    public Camera mainCamera;
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
        } else {
            foreach (SpriteRenderer child in this.children) {
                child.sprite = this.spriteRenderer.sprite;
            }
        }
    }

    private void Update() {
        if (this.isBackground) { //The background just follows the camera
            float cameraHeight = this.mainCamera.orthographicSize * 2f;
            float cameraWidth = cameraHeight * this.mainCamera.aspect;
            this.transform.localScale = new Vector3((cameraWidth / this.initialWidth) + 0.2f, this.initialHeight + 0.2f, this.transform.localScale.z);
            this.transform.position = new Vector3(this.mainCamera.transform.position.x, this.mainCamera.transform.position.y, this.transform.position.z);
            return;
        }

        //Calculate the offsets
        float distance = this.mainCamera.transform.position.x * this.parallaxEffect;
        float temp = this.mainCamera.transform.position.x * (1 - this.parallaxEffect);

        this.transform.position = new Vector3(this.startPos + distance, this.mainCamera.transform.position.y * (this.parallaxEffect * 0.75f), this.transform.position.z);

        float length = this.spriteRenderer.bounds.size.x;

        if (temp > this.startPos + length) {
            this.startPos += length;
        } else if (temp < this.startPos - length) {
            this.startPos -= length;
        }
    }
}
