using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine.UI;
public class debug : MonoBehaviour {

    public Text numPlayers;

    public Text ids;

    string text = "";

    public Dictionary<uint, PlayerControllerBehavior> players = null;
    public List<uint> id = new List<uint>();
    private void Update()
    {
        if (players == null)
            return;

        text = "";
        numPlayers.text = players.Count.ToString();
        for (int i = 0; i < id.Count; i++)
        {
            text = text + "\n" + id[i].ToString();
        }
    }

}
