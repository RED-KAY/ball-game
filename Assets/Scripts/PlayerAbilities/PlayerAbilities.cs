using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Could have totally done this without this base class but I wanted to give a demonstration of how I approach a problem.
//Here, I wanted to make sure that the system is modular (and independent) and extensible for future use cases.
public class PlayerAbilities : MonoBehaviour
{
    public virtual void Use()
    {

    }
}
