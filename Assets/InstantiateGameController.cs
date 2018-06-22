using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Unity;
public class InstantiateGameController : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	    if(NetworkManager.Instance.IsServer)
        {
            NetworkManager.Instance.InstantiateGameController(0);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
