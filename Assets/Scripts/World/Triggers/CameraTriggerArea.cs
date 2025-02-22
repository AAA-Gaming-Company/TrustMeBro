using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraTriggerArea : TriggerArea {
    [Header("Camera")]
    public CinemachineCamera cinemachineCamera;
    public float targetOffsetY = 0f;
    public float targetOffsetX = 0f;

    public float orthographicSize = 0f;

    public bool deadZone = true;

    public float duration = 0f;

    private CinemachinePositionComposer positionComposer;

    protected override void TriggerAction(PlayerController player) {
        this.positionComposer = cinemachineCamera.GetComponent<CinemachinePositionComposer>();
        if (this.positionComposer == null) {
            throw new System.Exception("CinemachinePositionComposer is null!");
        }

        if (duration == 0f) {
            this.cinemachineCamera.Lens.OrthographicSize = this.orthographicSize;
            this.positionComposer.Composition.DeadZone.Enabled = this.deadZone;
            this.positionComposer.Composition.ScreenPosition.y = this.targetOffsetY;
            this.positionComposer.Composition.ScreenPosition.x = this.targetOffsetX;
        } else {
            StartCoroutine(this.ChangeCamera());
        }
    }

    private IEnumerator ChangeCamera() {
        this.positionComposer.Composition.DeadZone.Enabled = this.deadZone;

        float elapsedTime = 0f;
        float startOrthographicSize = this.cinemachineCamera.Lens.OrthographicSize;
        float startTargetOffsetY = this.positionComposer.Composition.ScreenPosition.y;
        float startTargetOffsetX = this.positionComposer.Composition.ScreenPosition.x;

        while (elapsedTime < duration) {
            this.cinemachineCamera.Lens.OrthographicSize = Mathf.Lerp(startOrthographicSize, this.orthographicSize, elapsedTime / this.duration);
            this.positionComposer.Composition.ScreenPosition.y = Mathf.Lerp(startTargetOffsetY, this.targetOffsetY, elapsedTime / this.duration);
            this.positionComposer.Composition.ScreenPosition.x = Mathf.Lerp(startTargetOffsetX, this.targetOffsetX, elapsedTime / this.duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        this.cinemachineCamera.Lens.OrthographicSize = this.orthographicSize;
        this.positionComposer.Composition.ScreenPosition.y = this.targetOffsetY;
        this.positionComposer.Composition.ScreenPosition.x = this.targetOffsetX;
    }
}
