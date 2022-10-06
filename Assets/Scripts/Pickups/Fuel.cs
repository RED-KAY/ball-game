using System;
using UnityEngine;

//Could have totally done this without this base class but I wanted to give a demonstration of how I approach a problem.
//Here, I wanted to make sure that the system is modular (and independent) and extensible for future use cases.

public class Fuel : Pickup
{
    [SerializeField] float amount;

    public override void PickedUp(Transform player)
    {
        base.PickedUp(player);
        try
        {
            player.GetComponent<Jetpack>().Fuel += amount;
            gameObject.SetActive(false);
        }catch (NullReferenceException ex)
        {
            Debug.LogError("Needs Jetpack component on player!");
            return;
        }
    }
}
