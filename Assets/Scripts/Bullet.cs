using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking;

using System;

public class Bullet : BulletBehavior
{
    public float moveSpeed = 10;
    public Vector3 spawnPos;
    Rigidbody myRigi;

    NetworkedPlayerController owner;

    public Vector3 pos;
    bool initialized = false;
    protected override void NetworkStart()
    {
        base.NetworkStart();
        networkObject.UpdateInterval = 50;
        myRigi = GetComponent<Rigidbody>();

        if (!networkObject.IsOwner)
        {
            transform.position = networkObject.Position;
            return;
        }
        else
        {
            myRigi.AddForce(transform.forward * moveSpeed, ForceMode.Impulse);
            networkObject.Position = myRigi.position;
        }
    }

    public void SetOwner(NetworkedPlayerController owner, Vector3 pos, Quaternion rot)
    {
        this.owner = owner;

        Physics.IgnoreCollision(owner.GetComponent<Collider>(), GetComponent<Collider>(), true);

        if (networkObject.IsOwner)
        {
            networkObject.Position = pos;
            networkObject.Rotation = rot;
        }

        networkObject.SendRpc(RPC_INIT, Receivers.All, pos, rot);
        initialized = true;
    }
    void Update ()
    {
        if (myRigi == null || initialized == false)
            return;

        if (networkObject.IsOwner)
        {
            networkObject.Position = myRigi.position;
            networkObject.Rotation = transform.rotation;

            Destroy(gameObject, 5);
        }

        if (!networkObject.IsOwner)
        {
            transform.position = networkObject.Position; 
            transform.rotation = (networkObject.Rotation);
            return;
        }
    }
    private void FixedUpdate()
    {
        if (!networkObject.IsOwner)
            return;

        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        if(Physics.Raycast(ray, out hit, 1))
        {
            if(hit.transform.tag == "Player")
            {
                Debug.Log("We Hit A Player");
                networkObject.SendRpc(RPC_HIT_PLAYER, Receivers.AllBuffered, hit.transform.GetComponent<NetworkedPlayerController>().networkObject.MyPlayerId);
            }
        }
    }

    public override void HitPlayer(RpcArgs args)
    {
        uint playerHitID = args.GetNext<uint>();
        NetworkingPlayer player = NetworkManager.Instance.Networker.GetPlayerById(playerHitID);
    }

    public override void Init(RpcArgs args)
    {
        Vector3 pos = args.GetNext<Vector3>();
        Quaternion rot = args.GetNext<Quaternion>();

        Debug.Log("NETWORK POS SET TO: " + pos);

        transform.rotation = rot;
        transform.position = pos;
        initialized = true;
    }
}
