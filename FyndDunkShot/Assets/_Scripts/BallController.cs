using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    #region PUBLIC Variables
    public bool isBallLaunched;
    public int CurrentBasketIndex;

    #endregion

    #region PRIVATE Variables
    Vector2 BallLaunchVelocity;
    Vector2 Gravity;
    Vector2 InitialPosition;

    float Power, TotalForce, distance, ForcePercentage;
    Rigidbody2D BallRigidbody2D;
    Vector3 normal;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        BallRigidbody2D = GetComponent<Rigidbody2D>();
        Power = 5f;
        TotalForce = 7f;
    }

    // Update is called once per frame
    //Check for user Touch and Drag
    //Implement the Slingshot mechanism
    //If user drags the ball more than ForcePercentage (50%), Launch the Ball int the angle of Touch-Drag
    void Update()
    {
        if (Input.touchCount > 0 && !isBallLaunched)
        {

            switch (Input.GetTouch(0).phase)
            {
                case TouchPhase.Began:
                    InitialPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    break;

                case TouchPhase.Moved:
                    Vector2 toPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                    BallLaunchVelocity = CalculateForceBetween(InitialPosition, toPosition);
                    transform.parent.transform.rotation = Quaternion.Euler(0, 0, CalculateAngleBetween(InitialPosition, toPosition));
                    
                    break;

                case TouchPhase.Ended:
                    if (ForcePercentage > 0.5)
                    {
                        isBallLaunched = true;
                        BallRigidbody2D.simulated = true;
                        BallRigidbody2D.velocity = -1 * BallLaunchVelocity;
                        
                    }
                    break;
            }


        }
    }


    //Calculate Force to move from PositionA to PositionB
    //Clamp the Force value and Calculate ForcePercentage
    private Vector2 CalculateForceBetween(Vector2 fromPos, Vector2 toPos)
    {
        Vector2 force = (toPos - fromPos) * Power;

        distance = Vector2.Distance(fromPos, toPos);

        if (distance > 1.3f)
        {
            distance = 1.3f;
        }
         
        Vector2 clampedForce = Vector2.ClampMagnitude(force, TotalForce) * distance;

        ForcePercentage = clampedForce.magnitude / TotalForce;

        return clampedForce;
    }

    //Calculate the angle formed at the dragging points
    //Launch the ball int the opposite direction of that angle
    private float CalculateAngleBetween(Vector2 fromPos, Vector2 toPos)
    {
        Vector2 v2 = fromPos - toPos;
        float Angle = Mathf.Atan2(v2.x, v2.y) * Mathf.Rad2Deg;
        if (Angle < 0)
            Angle += 360;
        return -1 * Angle;
    }
}
