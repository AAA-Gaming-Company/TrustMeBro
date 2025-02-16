using UnityEngine;

public class Parallax : MonoBehaviour {
    [Header("Normal")]
    public Camera mainCamera;
    public GameObject parallaxPrefab;

    [Header("Auto scroll")]
    public bool autoScroll;
    public ParallaxSO selectedParallax;

    private void Start() {
        this.Init(this.selectedParallax);
    }

    private void Update() {
        if (this.autoScroll) {
            Vector3 pos = this.mainCamera.transform.position;
            pos.x += Time.deltaTime;
            this.mainCamera.transform.position = pos;
        }
    }

    public void Init(ParallaxSO data) {
        //Spawn in the background layers of the parallax
        this.SpawnParallaxChild(data.foreground, 0f, -100);

        float step = 1f / (data.intermediate.Length + 1);
        float current = step;
        for (int i = 0; i < data.intermediate.Length; i++) {
            this.SpawnParallaxChild(data.intermediate[i], current, i + 100);
            current += step;
        }

        this.SpawnParallaxChild(data.background, 1f, data.intermediate.Length + 100, true);
    }

    private void SpawnParallaxChild(Sprite sprite, float parallaxEffect, int layer, bool background = false) {
        GameObject child = Instantiate(this.parallaxPrefab, this.transform);
        child.transform.position = new Vector3(0, 0, layer);
        child.transform.rotation = Quaternion.identity;
        if (background) {
            Vector3 scale = child.transform.localScale;
            scale.x = 1f;
            child.transform.localScale = scale;
        }

        ParallaxChild parallax = child.GetComponent<ParallaxChild>();
        parallax.spriteRenderer.sprite = sprite;
        parallax.parallaxEffect = parallaxEffect;
        parallax.mainCamera = this.mainCamera;
        parallax.isBackground = background;
        parallax.Init();
    }
}
