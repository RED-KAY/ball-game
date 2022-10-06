using System;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    Text fuelT;
    Jetpack jetpack;
    BallMovement ballMovement;

    private void Start()
    {
        try
        {
            jetpack = GameObject.FindGameObjectWithTag("Player").GetComponent<Jetpack>();
            ballMovement = jetpack.GetComponent<BallMovement>();
        }
        catch (NullReferenceException ex)
        {
            Debug.LogError("No Jetpack or BallMovement component found on player!");
            return;
        }

        fuelT = transform.Find("Fuel").GetComponent<Text>();
    }

    void LateUpdate()
    {
        if (jetpack && fuelT)
        {
            if (!ballMovement.onGround || jetpack.Fuel < jetpack.maxFuelLimit)
            {
                fuelT.gameObject.SetActive(true);
                fuelT.text = "Fuel: " + ((int)jetpack.Fuel).ToString();
            }
            else if (ballMovement.onGround && fuelT.gameObject.activeInHierarchy)
            {
                fuelT.gameObject.SetActive(false);
            }
        }        
    }
}
