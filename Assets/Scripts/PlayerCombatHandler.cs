using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking;
public class PlayerCombatHandler : MonoBehaviour {

    NetworkedPlayerController nPlayerController;

    public GameObject shootPos, bullet;

    PlayerHealth myHealth;
	// Use this for initialization
	void Start ()
    {
        nPlayerController = GetComponent<NetworkedPlayerController>();
        myHealth = GetComponent<PlayerHealth>();

        if (!nPlayerController.isLocalOwner)
            return;

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!nPlayerController.isLocalOwner)
            return;

        if (Input.GetButtonDown("Shoot"))
        {
            
            //BulletBehavior b = NetworkManager.Instance.InstantiateBullet(position: shootPos.transform.position);
            //b.transform.rotation = Quaternion.LookRotation(transform.forward, transform.up);
        }
    }

    public void SpawnBullet(Vector3 pos)
    {
        Bullet b = Instantiate(bullet, pos, Quaternion.LookRotation(transform.forward, transform.up)).GetComponent<Bullet>();
    }
}
