using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static InputMaster;

//using OVR;

[RequireComponent(typeof(CharacterController))]
public class JoystickMovement : MonoBehaviour
{
    private Transform cameraMainTransform;
    private CharacterController controller;
    [SerializeField] private float movementSpeed = 3;
    //[SerializeField] private float rotationSpeed = 10;
    [SerializeField] private InputActionReference movementControl;
    [SerializeField] private InputActionAsset im;


    private void Start()
    {
        //inputMaster = gameObject.GetComponent<InputMaster>();
        controller = gameObject.GetComponent<CharacterController>();
        controller.detectCollisions = false;
        
        cameraMainTransform = Camera.main.transform;
    }

    //public void OnMovement(InputAction.CallbackContext context)
    //{
    //    var direction = context.ReadValue<Vector2>();

    //}

    private void Awake()
    {
        //im.Player.SetCallbacks(this);
        //im.FindActionMap("Player").SetCall
    }

    private void OnEnable()
    {
        //inputMaster.Player.Movement.Enable();
        movementControl.action.Enable();
    }
    private void OnDisable()
    {
        //inputMaster.Player.Movement.Disable();
        movementControl.action.Disable();

    }



    private void FixedUpdate()
    {
        Vector2 movement = movementControl.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        move = cameraMainTransform.forward * move.z + cameraMainTransform.right * movement.x;
        move.y = 0f;
        controller.Move(move * Time.deltaTime * movementSpeed);

    }

    //private InputMaster playerControls;
    //private void Awake()
    //{
    //    playerControls = new InputMaster();
    //}
    //private void OnEnable()
    //{
    //    playerControls.Enable();
    //}
    //private void OnDisable()
    //{
    //    playerControls.Disable();
    //}
    //void Start()
    //{

    //}
    //private void Update()
    //{
    //    Vector2 movement = playerControls.Player.Movement.ReadValue<Vector2>();
    //    //Debug.Log(movement);

    //}

}
