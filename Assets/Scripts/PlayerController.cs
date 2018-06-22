using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using Cinemachine;
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3, turnSpeed = 2;

    Rigidbody myRigi;
    public Rigidbody MyRigi { get { return myRigi; } }

    Transform myTransform;

    public CinemachineFreeLook myCam;

    float inputX, inputZ;

    bool jumpTrigger = false;

    [SerializeField]
    NetworkedPlayerController networkedPlayer;

    PlayerCombatHandler combatHandler;
    private void Start()
    {
        myRigi = GetComponent<Rigidbody>();

        myTransform = transform;

        networkedPlayer = GetComponent<NetworkedPlayerController>();
        combatHandler = GetComponent<PlayerCombatHandler>();
        Debug.Log(" PLAYER CONTROLLER : " + networkedPlayer.networkObject.IsOwner);

        if (networkedPlayer.isLocalOwner)
        {
            myCam = FindObjectOfType<CinemachineFreeLook>();
            myCam.Follow = transform.Find("CamFollow");
            myCam.LookAt = transform;
        }
    }
    public void Shoot()
    {
        ///networkedPlayer.PlayerShot();
    }
    public void SpawnBullet(Vector3 pos)
    {
        combatHandler.SpawnBullet(pos);
    }
    private void FixedUpdate()
    {
        if (!networkedPlayer.isLocalOwner)
            return;

        if(myCam == null)
        {
            myCam = FindObjectOfType<CinemachineFreeLook>();
            myCam.Follow = transform.Find("CamFollow");
            myCam.LookAt = transform;
        }

        GetInput();

        if(Input.GetButtonDown("Jump"))
        {
            jumpTrigger = true;
        }

        Vector3 WantedVel = GetDesiredVel();

        if (WantedVel == Vector3.zero && !jumpTrigger)
            return;

        if (jumpTrigger)
        {
            WantedVel.y = 5;
            jumpTrigger = false;
        }
        else
            WantedVel.y = myRigi.velocity.y;

        Vector3 velocityChange = WantedVel - myRigi.velocity;

        myRigi.AddForce(velocityChange, ForceMode.VelocityChange);

        ApplyRotation(myRigi.velocity);
    }

    Vector3 GetDesiredVel()
    {
        Vector3 velocity = Vector3.zero;

        if(Mathf.Abs(inputX) > 0 || Mathf.Abs(inputZ) > 0)
        {
            velocity = Camera.main.transform.TransformDirection( new Vector3(inputX, 0, inputZ));
            velocity *= moveSpeed;
        }

        return velocity;
    }

    void GetInput()
    {
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");
    }

    void ApplyRotation(Vector3 velocity)
    {
        Vector2 vel = new Vector2(velocity.x, velocity.z);

        if (vel.magnitude < 0.1f)
            return;

        Quaternion lookDir = Quaternion.LookRotation(velocity.normalized, Vector3.up);
        lookDir.z = 0;
        lookDir.x = 0;
        myRigi.rotation = Quaternion.Slerp(myRigi.rotation, lookDir, turnSpeed * Time.fixedDeltaTime);
    }
}
