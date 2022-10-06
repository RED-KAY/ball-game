using System;
using UnityEngine;

//Could have totally done this without this base class but I wanted to give a demonstration of how I approach a problem.
//Here, I wanted to make sure that the system is modular (and independent) and extensible for future use cases.

public class Key : Pickup
{
    Transform door;
    private void Start()
    {
        try
        {
            door = transform.parent.transform;
        }catch (NullReferenceException ex)
        {
            Debug.LogError("Key needs the corresponding Door as the parent!");
        }

    }
    public override void PickedUp(Transform player)
    {
        base.PickedUp(player);
        door.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }   
}
