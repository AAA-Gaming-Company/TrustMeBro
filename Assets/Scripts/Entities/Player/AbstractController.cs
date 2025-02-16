using UnityEngine;

[DisallowMultipleComponent]
public abstract class AbstractController : MonoBehaviour {
    public void EnableController(bool enable) {
        this.gameObject.SetActive(enable);
        this.EnableController();
    }

    protected abstract void EnableController();

    public abstract void SuperPlayer(bool enable);

    public abstract bool IsAttackButton(GameObject gameObject);

    protected void OnMoveH(float input) {
        InputManager.Instance.MoveH(input);
    }

    protected void OnMoveV(float input) {
        InputManager.Instance.MoveV(input);
    }

    protected void OnJump(bool down) {
        InputManager.Instance.Jump(down);
    }

    protected void OnAttack(bool down) {
        if (down) {
            InputManager.Instance.Attack(true);
        }
    }

    protected void OnInteract(bool down) {
        InputManager.Instance.Interact(down);
    }
}
