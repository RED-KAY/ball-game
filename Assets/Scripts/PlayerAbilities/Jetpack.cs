using UnityEngine;

public enum JetpackState
{
    None,
    Activate,
    Terminate
}


//Could have totally done this without this base class but I wanted to give a demonstration of how I approach a problem.
//Here, I wanted to make sure that the system is modular (and independent) and extensible for future use cases.

[RequireComponent(typeof(BallMovement))]
[RequireComponent(typeof(Rigidbody))]
public class Jetpack : PlayerAbilities 
{
    [SerializeField, Range(100f, 200f)]
    public float maxFuelLimit = 100f;

    [SerializeField] float fuelConsumptionRate = 25; // Rate at which fuel is consumed
    [SerializeField] float fuelRechargeRate = 25; // Recharge rate.

    [SerializeField] float jetpackThrust = 5;

    float fuel;

    Vector3 velocity;

    [SerializeField]
    public float Fuel {
        get {
            return fuel;
        }
        set {
            fuel = value;
            if(fuel < 0f)
            {
                fuel = 0f;
            }else if(fuel > maxFuelLimit)
            {
                fuel = maxFuelLimit;
            }
        }
    }

    BallMovement ballMovement;

    Rigidbody body;

    JetpackState jetpackState = JetpackState.None;

    void Start()
    {
        Fuel = maxFuelLimit;
        ballMovement = GetComponent<BallMovement>();
        body = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //Cehck whether Space bar was held to use jetpack & if jump was already performed.
        if(ballMovement.jumpPhase == 0 && !ballMovement.onGround)
        {            
            if (Input.GetButtonDown("Jump"))
                jetpackState = JetpackState.Activate;            
        }

        if (jetpackState == JetpackState.Activate)
        {
            if(Input.GetButtonUp("Jump") || Fuel <= 0f)
                jetpackState = JetpackState.Terminate;            
        }

        // Recharge only when on ground and if fuel is already not full.
        if (ballMovement.onGround && Fuel < maxFuelLimit)
            Recharge();        
    }

    private void FixedUpdate()
    {
        velocity = body.velocity;
        if(jetpackState == JetpackState.Activate)
        {
            Use();
        }
        body.velocity = velocity;
    }

    void Recharge()
    {
        Fuel += Time.fixedDeltaTime * fuelRechargeRate;
    }

    public override void Use()
    {
        float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jetpackThrust);

        velocity.y += jumpSpeed * Time.fixedDeltaTime;
        Fuel -= Time.fixedDeltaTime * fuelConsumptionRate;
        if(Fuel <= 0f)
        {
            jetpackState = JetpackState.Terminate;
        }
    }
}
