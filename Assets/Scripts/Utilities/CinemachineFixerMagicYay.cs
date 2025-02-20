using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CinemachineFixerMagicYay : MonoBehaviour {
    CinemachinePositionComposer positionComposer;

    private void Start() {
        this.positionComposer = this.GetComponent<CinemachinePositionComposer>();
        if (this.positionComposer == null) {
            throw new System.Exception("CinemachinePositionComposer is null! How on earth did you manage that?");
        }

        this.positionComposer.Composition.DeadZone.Enabled = false;
        StartCoroutine(CameraCallback());
    }

    private IEnumerator CameraCallback() {
        yield return new WaitForSeconds(.1f);
        this.positionComposer.Composition.DeadZone.Enabled = true;

        Destroy(this);
    }
}
