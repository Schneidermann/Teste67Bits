using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    int currBodies = 0;
    bool npcInRange;
    NPC currentNpc;
    InertiaSim _inertiaSim;

    private Animator _anim;
    private Rigidbody _char;

    private Vector3 moveDirection = Vector3.zero;


    public GameObject PlayerModel;
    public Transform Hips;
    public PlayerInput inputs;

    public float speed;
    public float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        _char = GetComponent<Rigidbody>();
        _inertiaSim = FindAnyObjectByType<InertiaSim>();
    }

    // Update is called once per frame
    void Update()
    {
        
        Move();
    }


    public void Punch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            print("Punch " + context.phase);
            _anim.SetTrigger("Punch");
            if (npcInRange && currentNpc != null)
            {
                if (!currentNpc.isDown)
                    currentNpc.ActivateRagdoll(PlayerModel.transform);
                else
                {
                    //pick body up logic (change to hips transform as param)
                    currentNpc.PickUpBody(Hips, currBodies);
                    _inertiaSim.BodiesHolder.Add(currentNpc.transform);
                    _inertiaSim.UpdateLineRendererPoints();
                    currBodies++;
                }
            }
        }
           
    }

    void Move()
    {
        InputAction inputValue = inputs.actions.FindAction("Move");
        Vector2 moveValue = inputValue.ReadValue<Vector2>();

        moveDirection = new Vector3(moveValue.x, 0.0f, moveValue.y);

        if (moveDirection != Vector3.zero)
        {
            // Rotate character to face movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            PlayerModel.transform.rotation = Quaternion.Slerp(PlayerModel.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= speed * Time.deltaTime;
        transform.Translate(moveDirection, Space.World);
    }

    public void Movement(InputAction.CallbackContext context)
    {
      

        if (context.performed)
        {
            print("Moving " + context.phase);
            _anim.SetBool("isRunning", true);

        }
        else
        {
            print("Moving " + context.phase);
            _anim.SetBool("isRunning", false);
        }

       
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            npcInRange = true;
            currentNpc = other.gameObject.GetComponentInParent<NPC>();
            print($"trigger entered. current npc name = {currentNpc.gameObject.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            print($"trigger exited!");
            npcInRange = false;
            currentNpc = null;
        }
    }


}
