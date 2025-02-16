using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class SafeArea : MonoBehaviour {
    private static List<SafeArea> helpers = new List<SafeArea>();

    private static bool screenChangeVarsInitialized = false;
    private static Vector2 lastResolution = Vector2.zero;
    private static Rect lastSafeArea = Rect.zero;

    private Canvas canvas;
    private RectTransform rectTransform;
    private RectTransform safeAreaTransform;

    private void Awake() {
        if (!helpers.Contains(this)) {
            helpers.Add(this);
        }

        canvas = GetComponent<Canvas>();
        rectTransform = GetComponent<RectTransform>();

        safeAreaTransform = transform.Find("SafeArea") as RectTransform;

        if (!screenChangeVarsInitialized) {
            lastResolution.x = Screen.width;
            lastResolution.y = Screen.height;
            lastSafeArea = GetSafeArea();

            screenChangeVarsInitialized = true;
        }

        ApplySafeArea(lastSafeArea);
    }

#if PLATFORM_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern string GetWebSafeArea();

    private static Rect GetSafeArea() {
        return JsonUtility.FromJson<Rect>(GetWebSafeArea());
    }

    private class WebSafeArea {
        public string top;
        public string bottom;
        public string left;
        public string right;
    }
#else
    private static Rect GetSafeArea() {
        return Screen.safeArea;
    }
#endif

    private void Update() {
        if (helpers[0] != this) {
            return;
        }

        if (GetSafeArea() != lastSafeArea) {
            SafeAreaChanged();
        }
    }

    private void ApplySafeArea(Rect safeArea) {
        if (safeAreaTransform == null) {
            return;
        }

        var anchorMin = safeArea.position;
        var anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= canvas.pixelRect.width;
        anchorMin.y /= canvas.pixelRect.height;
        anchorMax.x /= canvas.pixelRect.width;
        anchorMax.y /= canvas.pixelRect.height;

        safeAreaTransform.anchorMin = anchorMin;
        safeAreaTransform.anchorMax = anchorMax;
    }

    private void OnDestroy() {
        if (helpers != null && helpers.Contains(this)) {
            helpers.Remove(this);
        }
    }

    private static void SafeAreaChanged() {
        lastSafeArea = GetSafeArea();

        for (int i = 0; i < helpers.Count; i++) {
            helpers[i].ApplySafeArea(lastSafeArea);
        }
    }
}