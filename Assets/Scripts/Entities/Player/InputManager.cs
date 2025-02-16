using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : Singleton<InputManager> {
    public bool designerMode;
    public AbstractController[] inputControllers;

    private bool isSuperPlayer = false;
    private AbstractController activeController = null;
    private PlayerInput playerInput;

    private float horizontalInput = 0;
    private float verticalInput = 0;

    private bool jumpDown = false;
    private bool jumpUp = false;

    private bool interactDown = false;
    private bool interactUp = false;

    private bool attackWasButton = false;
    private bool attackDown = false;

    private void Start() {
        this.playerInput = GetComponent<PlayerInput>();

    }

    public void Init() {
        //Options to hide the touch input
        this.playerInput = GetComponent<PlayerInput>();
        bool hideInput = PlayerPrefs.GetInt("UIHideInput", 0) == 1 ? true : false;
        if (this.designerMode || hideInput) {
            this.HideAllControllers();
            this.EnableInputSystemEvents();

            if (this.designerMode) {
                Debug.LogWarning("Designer mode is enabled! The input controls will never be shown!");
            }
            return;
        }

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
        this.activeController.SuperPlayer(this.isSuperPlayer); //If the player is a super player, we set that up for them

        this.EnableInputSystemEvents();
    }

    private void LateUpdate() {
        //We only want to set these values during their frame
        this.jumpDown = false;
        this.jumpUp = false;
        this.attackDown = false;
        this.interactDown = false;
        this.interactUp = false;
    }

    private void EnableInputSystemEvents() {
        //These are the non-touch input events
        
        Debug.Log(this.playerInput);

        InputActionMap actionMap = this.playerInput.actions.FindActionMap("Player");

        Debug.Log("Carried on");

        InputAction move = actionMap.FindAction("Move");
        move.started += OnMove;
        move.canceled += OnMoveStop;

        InputAction jump = actionMap.FindAction("Jump");
        jump.started += OnJump;
        jump.canceled += OnJumpStop;

        InputAction attack = actionMap.FindAction("Attack");
        attack.started += OnAttack;

        InputAction interact = actionMap.FindAction("Interact");
        interact.started += OnInteract;
        interact.canceled += OnInteractStop;
    }

    public void HideAllControllers() {
        foreach (AbstractController controller in this.inputControllers) {
            controller.gameObject.SetActive(false);
        }
    }

    public void SuperPlayer(bool enable) {
        //Toggle if a player is a super player
        if (this.activeController != null) {
            this.activeController.SuperPlayer(enable);
        }
        this.isSuperPlayer = enable;
    }

    private void OnMove(InputAction.CallbackContext context) {
        Vector2 value = context.ReadValue<Vector2>();
        this.MoveH(value.x);
        this.MoveV(value.y);
    }

    private void OnMoveStop(InputAction.CallbackContext context) {
        this.MoveH(0f);
        this.MoveV(0f);
    }

    public void MoveH(float input) {
        this.horizontalInput = input;
    }

    public void MoveV(float input) {
        this.verticalInput = input;
    }

    private void OnJump(InputAction.CallbackContext context) {
        this.Jump(true);
    }

    private void OnJumpStop(InputAction.CallbackContext context) {
        this.Jump(false);
    }

    public void Jump(bool down) {
        if (down) {
            this.jumpDown = true;
        } else {
            this.jumpUp = true;
        }
    }

    private void OnAttack(InputAction.CallbackContext context) {
        this.Attack(false);
    }

    public void Attack(bool wasButton) {
        this.attackWasButton = wasButton;
        this.attackDown = true;
    }

    private void OnInteract(InputAction.CallbackContext context) {
        this.Interact(true);
    }

    private void OnInteractStop(InputAction.CallbackContext context) {
        this.Interact(false);
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

    public bool GetInteractDown() {
        return this.interactDown;
    }

    public bool GetInteractUp() {
        return this.interactUp;
    }
}