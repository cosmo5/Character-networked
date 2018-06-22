using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;

public class GameController : GameControllerBehavior
{
    Vector3 spawnPos = Vector3.zero;
    public int numPlayers = 0;

    public Dictionary<uint, PlayerControllerBehavior> players = new Dictionary<uint, PlayerControllerBehavior>();

    public List<uint> ID = new List<uint>();
    public List<PlayerControllerBehavior> _players = new List<PlayerControllerBehavior>();

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    protected override void NetworkStart()
    {
        base.NetworkStart();
        //if we are the clent we want to get all the other players in the scene
        Debug.Log("NETWORK START");

        if (!networkObject.IsServer)
        {
            return;
        }

        players = new Dictionary<uint, PlayerControllerBehavior>();
        //Adds the server player 
        PlayerControllerBehavior p = NetworkManager.Instance.InstantiatePlayerController();
        //sets their ID
        p.networkObject.MyID = p.networkObject.MyPlayerId;
       
        //instantiates player models and sets thier ids on acceptance
        NetworkManager.Instance.Networker.playerAccepted += NewPlayer;
    }
    private void Start()
    {
        Debug.Log("Unity START");

    }

    private void NewPlayer(NetworkingPlayer player, NetWorker sender)
    {
        MainThreadManager.Run(() =>
        {
            Vector3 pos = spawnPos + Random.onUnitSphere * 3;
            pos.y = 1;

            PlayerControllerBehavior p = NetworkManager.Instance.InstantiatePlayerController(0, position: pos);
            p.networkObject.MyID = player.NetworkId;
            p.name = p.name + networkObject.numPlayers;

            Debug.Log(p.networkObject.MyID);

            networkObject.numPlayers++;
            
        });
                    networkObject.SendRpc(RPC_SET_PLAYERS, Receivers.All, player.NetworkId);

    }
    public void RegisterPlayer(uint id, PlayerControllerBehavior player)
    {
        players.Add(id, player);
        _players.Add(player);
        ID.Add(id);
    }
    void GetPlayers()
    {
        if (networkObject.IsServer)
            return;

        PlayerControllerBehavior[] p  = FindObjectsOfType<PlayerControllerBehavior>();

        Debug.Log("Client Found: " + p.Length + " players in scene");
        if (p != null && p.Length > 0)
        {
            players = new Dictionary<uint, PlayerControllerBehavior>();
            for (int i = 0; i < p.Length; i++)
            {
                Debug.Log(p[i].name + " " + p[i].networkObject.MyID);
                if(players.ContainsKey(p[i].networkObject.MyID))
                    Debug.Log("Player: " + p[i].name + " " + p[i].networkObject.MyID + " already in");

                players.Add(p[i].networkObject.MyID, p[i]);
                _players.Add(p[i]);
                ID.Add(p[i].networkObject.MyID);
            }
        }
    }
    public override void SetPlayers(RpcArgs args)
    {
        if (networkObject.IsServer)
            return;

        uint id = args.GetNext<uint>();
        MainThreadManager.Run(() =>
        {
            Debug.Log(id);
            PlayerControllerBehavior[] p = FindObjectsOfType<PlayerControllerBehavior>();
            for (int i = 0; i < p.Length; i++)
            {
                if (p[i].networkObject.MyID == id)
                {
                    Debug.Log(p[i].name);

                    if(!players.ContainsKey(id))
                        players.Add(id, p[i]);
                }

            }
        });
        
    }
}
