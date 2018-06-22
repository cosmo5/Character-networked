using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using System;

public class NetworkedCombatHandler : CombatHandlerBehavior
{
    PlayerHealth myHealth;
    NetworkedPlayerController nPlayerController;
    public GameObject shootPos, bullet;

    void Start ()
    {
        nPlayerController = GetComponent<NetworkedPlayerController>();

        if (!nPlayerController.isLocalOwner)
            return;

        myHealth = GetComponent<PlayerHealth>();
	}

    public override void PlayerShot(RpcArgs args)
    {
        Vector3 position = args.GetNext<Vector3>();
        Quaternion rot = args.GetNext<Quaternion>();

        uint ID = args.GetNext<uint>();

        networkObject.SendRpc(RPC_PLAYER_SHOT, Receivers.All, position, rot, ID);
    }

    void Update ()
    {
        if (!nPlayerController.isLocalOwner)
        {
            return;
        }

        if (Input.GetButtonDown("Shoot"))
        {
            networkObject.SendRpc(RPC_PLAYER_SHOT, Receivers.Server, shootPos.transform.position, Quaternion.LookRotation(transform.forward, transform.up), nPlayerController.networkObject.MyID);
        }
	}

    public override void SetBullet(RpcArgs args)
    {
        
    }

    public override void SpawnBullet(RpcArgs args)
    {

        Vector3 position = args.GetNext<Vector3>();
        Quaternion rot = args.GetNext<Quaternion>();

        uint ID = args.GetNext<uint>();
        MainThreadManager.Run(() =>
        {
            GameObject b = Instantiate(bullet, position, rot);
            (b.GetComponent<Bullet>()).SetOwner(FindObjectOfType<GameController>().players[ID].GetComponent<NetworkedPlayerController>(), position, rot);
        });
   
    }
}
