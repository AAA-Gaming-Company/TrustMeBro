using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraTriggerArea : TriggerArea {
    [Header("Camera")]
    public CinemachineCamera cinemachineCamera;
    public float targetOffsetY = 0f;
    public float orthographicSize = 0f;

    protected override void TriggerAction() {
        CinemachinePositionComposer positionComposer = cinemachineCamera.GetComponent<CinemachinePositionComposer>();
        if (positionComposer == null) {
            throw new System.Exception("CinemachinePositionComposer is null!");
        }
        
        positionComposer.Composition.DeadZone.Size.y = 0;

        cinemachineCamera.Lens.OrthographicSize = orthographicSize;
        positionComposer.TargetOffset.y = targetOffsetY;
        StartCoroutine(EnableDeadZone());
    }

    private IEnumerator EnableDeadZone() {
        yield return new WaitForSeconds(.01f);
        cinemachineCamera.GetComponent<CinemachinePositionComposer>().Composition.DeadZone.Size.y = 1;
    }
}
