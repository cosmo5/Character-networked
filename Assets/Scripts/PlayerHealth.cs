using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using System;

public class PlayerHealth : PlayerHealthBehavior {

    public int currentHealth;
    public int maxHelath = 10;

    bool isDead = false;

    public delegate void OnDeath();
    public event OnDeath PlayerDied;
 

    public override void TakeDamage(RpcArgs args)
    {
        if(!networkObject.IsOwner)
        {
            currentHealth = networkObject.CurrentHealth;
            return;
        }

        currentHealth -= args.GetNext<int>() ;

        if (currentHealth <= 0)
        {
            isDead = true;

            if (PlayerDied != null)
                PlayerDied();
        }

        networkObject.CurrentHealth = currentHealth;
    }
}
