using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerMovement : MonoBehaviour, IPunObservable
{
    CharacterController characterController;
    public float speed = 3f;
    public float timerDuration = 5f;
    public float timerCounter;

    float x,z;
    public TMP_Text PlayerNameDisplay;
    Transform cam;
    public GameObject chatBubble;
    public TMP_Text displayMessage;
    bool chatBubbleSync;    
    Vector3 move;
    PhotonView view;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();

        if(view.IsMine)
        {
            PlayerNameDisplay.gameObject.SetActive(false);
            characterController = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
            cam = Camera.main.transform;
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        if(!view.IsMine)
        {
            PlayerNameDisplay.transform.parent.LookAt(Camera.main.transform);
            return;
        }

        if(NetworkScript.CheckChatBox.isFocused)
        {
            return;
        }

        Movement();
        CameraSync();   
        Animations();
        ChatBubbleFunction();
    }

    void Movement()
    {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        move = cam.forward*z + cam.right*x;
        characterController.Move(move*speed*Time.deltaTime);
        characterController.Move(Physics.gravity * Time.deltaTime);
    }

    void CameraSync()
    {
        cam.parent.position = transform.position;
        transform.rotation = Quaternion.AngleAxis(cam.parent.rotation.eulerAngles.y, Vector3.up);
    }

    void Animations()
    {
        if(x !=0 || z != 0)
        {
            animator.SetBool("Motion", true);
        }

        else
        {
            animator.SetBool("Motion", false);
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            animator.SetBool("Hi", true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            animator.SetBool("Dance", true);
        }
    }

    void ChatBubbleFunction()
    {
        displayMessage.transform.localRotation = Quaternion.Euler(0,180,0);  
        
        if(chatBubble.activeInHierarchy)
        {
            displayMessage.text = NetworkScript.message;
        }

        if(NetworkScript.IsChatBubble)
        {
            if(timerCounter <= 0)
            {
                NetworkScript.IsChatBubble = false;
                timerCounter = timerDuration;
            }

            else
            {
                timerCounter = timerCounter - Time.deltaTime;
            }
        }

        chatBubbleSync = NetworkScript.IsChatBubble;
        chatBubble.SetActive(chatBubbleSync);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(NetworkScript.PlayerName);
            stream.SendNext(NetworkScript.message);
            stream.SendNext(chatBubbleSync);
        }

        else if (stream.IsReading)
        {
            PlayerNameDisplay.text = (string)stream.ReceiveNext();
            displayMessage.text = (string)stream.ReceiveNext();
            chatBubble.SetActive((bool)stream.ReceiveNext());
        }
    }

}
