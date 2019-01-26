using UnityEngine;
using System.Collections;

public class WeaponCanHitAnimationEvent : MonoBehaviour
{

    private Player player;

    public void SetWeaponCanHit(int value)
    {
        if(player == null)
        {
            player = transform.root.GetComponentInChildren<Player>();
        }

        player.SetWeaponCanHit(value != 0);
    }
}
