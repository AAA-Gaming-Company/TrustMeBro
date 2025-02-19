using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerInput))]
public class InputManager : Singleton<InputManager> {
    public bool designerMode;
    public AbstractController[] inputControllers;

    private AbstractController activeController = null;
    private PlayerInput playerInput;

    private float horizontalInput = 0;
    private float verticalInput = 0;

    private bool jumpDown = false;
    private bool jumpUp = false;

    private bool interactDown = false;
    private bool interactUp = false;

    private bool crouchDown = false;

    private bool sprintDown = false;
    private bool sprintUp = false;

    private bool attackWasButton = false;
    private bool attackDown = false;

    public void Init() {
        this.playerInput = GetComponent<PlayerInput>();

        //Options to hide the touch input
        bool hideInput = PlayerPrefs.GetInt("UIHideInput", 0) == 1 ? true : false;

        if (this.designerMode || hideInput) {
            this.HideAllControllers();

            if (this.designerMode) {
                Debug.LogWarning("Designer mode is enabled! The input controls will never be shown!");
            }
        } else {
            //Find the selected controller
            int selectedUIController = PlayerPrefs.GetInt("UILeftHanded", 0);
            //Use joysticks if needed
            selectedUIController += (PlayerPrefs.GetInt("UIUseJoysticks", 0) == 1 ? 2 : 0); //Add 2 so that we can use the second 2 sets of controllers

            if (selectedUIController < 0 || selectedUIController >= this.inputControllers.Length) {
                Debug.LogError("Unable to find required input controller...");
                return;
            }

            //Make sure all the controllers are disabled, except the one we want
            for (int i = 0; i < this.inputControllers.Length; i++) {
                this.inputControllers[i].EnableController(i == selectedUIController);
            }
            this.activeController = this.inputControllers[selectedUIController];
        }

        this.EnableInputSystemEvents();
    }

    private void LateUpdate() {
        //We only want to set these values during their frame
        this.jumpDown = false;
        this.jumpUp = false;
        this.interactDown = false;
        this.interactUp = false;
        this.crouchDown = false;
        this.sprintDown = false;
        this.sprintUp = false;
        this.attackDown = false;
    }

    private void EnableInputSystemEvents() {
        //These are the non-touch input events
        InputActionMap actionMap = this.playerInput.actions.FindActionMap("Player");

        InputAction move = actionMap.FindAction("Move");
        move.started += OnMove;
        move.canceled += OnMoveStop;

        InputAction jump = actionMap.FindAction("Jump");
        jump.started += OnJump;
        jump.canceled += OnJumpStop;

        InputAction attack = actionMap.FindAction("Attack");
        attack.started += OnAttack;

        InputAction crouch = actionMap.FindAction("Crouch");
        crouch.started += OnCrouch;

        InputAction sprint = actionMap.FindAction("Sprint");
        sprint.started += OnSprint;
        sprint.canceled += OnSprintStop;

        InputAction interact = actionMap.FindAction("Interact");
        interact.started += OnInteract;
        interact.canceled += OnInteractStop;
    }

    public void HideAllControllers() {
        foreach (AbstractController controller in this.inputControllers) {
            controller.gameObject.SetActive(false);
        }
    }

    public static void OnMove(InputAction.CallbackContext context) {
        Vector2 value = context.ReadValue<Vector2>();
        InputManager.Instance.MoveH(value.x);
        InputManager.Instance.MoveV(value.y);
    }

    public static void OnMoveStop(InputAction.CallbackContext context) {
        InputManager.Instance.MoveH(0f);
        InputManager.Instance.MoveV(0f);
    }

    public void MoveH(float input) {
        this.horizontalInput = input;
    }

    public void MoveV(float input) {
        this.verticalInput = input;
    }

    public static void OnJump(InputAction.CallbackContext context) {
        InputManager.Instance.Jump(true);
    }

    public static void OnJumpStop(InputAction.CallbackContext context) {
        InputManager.Instance.Jump(false);
    }

    public void Jump(bool down) {
        if (down) {
            this.jumpDown = true;
        } else {
            this.jumpUp = true;
        }
    }

    public static void OnAttack(InputAction.CallbackContext context) {
        InputManager.Instance.Attack(false);
    }

    public void Attack(bool wasButton) {
        this.attackWasButton = wasButton;
        this.attackDown = true;
    }

    public static void OnCrouch(InputAction.CallbackContext context) {
        InputManager.Instance.Crouch();
    }

    public void Crouch() {
        this.crouchDown = true;
    }

    public static void OnSprint(InputAction.CallbackContext context) {
        InputManager.Instance.Sprint(true);
    }

    public static void OnSprintStop(InputAction.CallbackContext context) {
        InputManager.Instance.Sprint(false);
    }

    public void Sprint(bool down) {
        if (down) {
            this.sprintDown = true;
        } else {
            this.sprintUp = true;
        }
    }

    public static void OnInteract(InputAction.CallbackContext context) {
        InputManager.Instance.Interact(true);
    }

    public static void OnInteractStop(InputAction.CallbackContext context) {
        InputManager.Instance.Interact(false);
    }

    public void Interact(bool down) {
        if (down) {
            this.interactDown = true;
        } else {
            this.interactUp = true;
        }
    }

    public float GetHorizontalInput() {
        return this.horizontalInput;
    }

    public float GetVerticalInput() {
        return this.verticalInput;
    }

    public bool GetJumpDown() {
        return this.jumpDown;
    }

    public bool GetJumpUp() {
        return this.jumpUp;
    }

    public bool GetAttackDown(bool canBeButton) {
        if (EventSystem.current.IsPointerOverGameObject()) {
            if (!canBeButton) {
                return false;
            } else if (canBeButton && !this.attackWasButton) {
                return false;
            }
        }
        return this.attackDown;
    }

    public bool GetCrouchDown() {
        return this.crouchDown;
    }

    public bool GetSprintDown() {
        return this.sprintDown;
    }

    public bool GetSprintUp() {
        return this.sprintUp;
    }

    public bool GetInteractDown() {
        return this.interactDown;
    }

    public bool GetInteractUp() {
        return this.interactUp;
    }
}