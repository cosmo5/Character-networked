using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using Cinemachine;
public class NetworkedPlayerController : PlayerControllerBehavior
{
    public bool isLocalOwner = false;

    PlayerController myController;

    public uint myId;
    public uint playerID;

    protected override void NetworkStart()
    {
        base.NetworkStart();
        if(networkObject.IsOwner)
        { 
            networkObject.SendRpc(RPC_UPDATE_POS, Receivers.Server, transform.position, transform.rotation);
        }
        myId = networkObject.MyID;
    }
    private void Start()
    {
        isLocalOwner = networkObject.MyPlayerId == networkObject.MyID;

        Debug.Log(" NETWORKED PLAYER CONTROLLER : " + isLocalOwner);
        myId = networkObject.MyID;

        myController = GetComponent<PlayerController>();
        FindObjectOfType<GameController>().RegisterPlayer(networkObject.MyID, this);
    }
    // Update is called once per frame
    void FixedUpdate ()
    {
        isLocalOwner = networkObject.MyID == networkObject.MyPlayerId;

        playerID = networkObject.MyPlayerId;

        myId = networkObject.MyID;

        if (isLocalOwner)
        {
            networkObject.SendRpc(RPC_UPDATE_POS, Receivers.Server, transform.position, transform.rotation);
        }
        else
        {
            myController.MyRigi.position = networkObject.Position;

            if(networkObject.Rotation.eulerAngles.magnitude > 0.1f)
                myController.MyRigi.rotation = networkObject.Rotation;
        }
	}
    public override void UpdatePos(RpcArgs args)
    {
        networkObject.Position = args.GetNext<Vector3>();
        networkObject.Rotation = args.GetNext<Quaternion>();
    }

    public override void Shoot(RpcArgs args)
    {
    }
}
