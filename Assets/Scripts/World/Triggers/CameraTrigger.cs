using Unity.Cinemachine;
using UnityEngine;

public class CameraTriggerArea : TriggerArea {
    [Header("Camera")]
    public CinemachineCamera cinemachineCamera;
    public float targetOffsetY = 0f;
    public float orthographicSize = 0f;

    protected override void TriggerAction() {
        cinemachineCamera.Lens.OrthographicSize = orthographicSize;
        cinemachineCamera.GetComponent<CinemachinePositionComposer>().TargetOffset.y = targetOffsetY;
    }
}
