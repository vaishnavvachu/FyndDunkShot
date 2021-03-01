using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    #region PUBLIC Variables
    public bool isBallLaunched;
    public int CurrentBasketIndex;
    public GameObject TrajectoryPointsPrefab;

    public static BallController instance;

    #endregion

    #region PRIVATE Variables
    Vector2 BallLaunchVelocity;
    Vector2 Gravity;
    Vector2 InitialPosition;

    float Power, TotalForce, distance, ForcePercentage;
    float MinOffset, MaxOffset;
    Rigidbody2D BallRigidbody2D;
    Vector3 Normal;

    GameObject[] Points;
    int TotalNumberOfPoints = 10;
    Vector3 normal;
    #endregion



    private void Awake()
    {
        isBallLaunched = false;
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        BallRigidbody2D = GetComponent<Rigidbody2D>();
        Power = 5f;
        TotalForce = 7f;
        Gravity = new Vector2(0f, -9.81f);
        CurrentBasketIndex = 0;

        Points = new GameObject[TotalNumberOfPoints];

        for (int n = 0; n < TotalNumberOfPoints; n++)
        {
            GameObject TrajectoryPoint = Instantiate(TrajectoryPointsPrefab);
            Points[n] = TrajectoryPoint;
            Points[n].SetActive(false);
        }

        GameManager gameManagerInstance = GameManager.instance;

        MinOffset = gameManagerInstance.MainCamera.transform.position.x - gameManagerInstance.WorldScreenWidth / 2;
        MaxOffset = gameManagerInstance.MainCamera.transform.position.y - gameManagerInstance.WorldScreenWidth / 2;
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
                    ShowTrajectoryFromBallPosition();
                    break;

                case TouchPhase.Ended:
                    if (ForcePercentage > 0.5)
                    {
                        isBallLaunched = true;
                        BallRigidbody2D.simulated = true;
                        BallRigidbody2D.velocity = -1 * BallLaunchVelocity;
                        HideTrajectoryFromBall();
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

        //Cap distance at 1.3f
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

    //Show the Trajectory of the ball when user Touches the screen
    //With the help of the tiny Points
    private void ShowTrajectoryFromBallPosition()
    {
        float PointSize = 0.05f;
        int index = 0;

        Vector2 curvePoint = Vector2.zero;
        Vector2 curveForce = BallLaunchVelocity;

        int maxIterations = 100;
        for(int i = 0; i< maxIterations; i++)
        {
            float time = 0.01f * i;
            curvePoint = Gravity * time * time * 0.5f + BallLaunchVelocity * time + (Vector2)transform.position;
            if (curvePoint.x < MinOffset + 0.01f && curvePoint.x > MinOffset - 0.01f || curvePoint.x == MaxOffset)
                curveForce = Vector2.Reflect(curvePoint, normal);

            if (i % 10 == 0)
            {
                float elapsedTime = index * 0.08f;
                if (elapsedTime > 0)
                    elapsedTime *= -1;
                Points[index].SetActive(true);
                Points[index].transform.localScale = new Vector3(PointSize, PointSize, PointSize);
                Points[index].transform.position = Gravity * elapsedTime * elapsedTime * 0.5f + curveForce * elapsedTime + (Vector2)transform.position;

                SpriteRenderer spriteRenderer = Points[index].GetComponent<SpriteRenderer>();
                Color alpha = spriteRenderer.color;

                if (ForcePercentage > 0.5)
                    alpha.a = 1 - ((1 - ForcePercentage) * 2);
                else
                    alpha.a = 0;

                spriteRenderer.color = alpha;

                PointSize -= 0.0025f;
                index++;
            }
        }
    }

    //Hide the Trajectory When the Touch is no longer there
    private void HideTrajectoryFromBall()
    {
        for (int n = 0; n < TotalNumberOfPoints; n++)
        {
            Points[n].SetActive(false);
            Points[n].transform.position = transform.position;
        }
    }

    //Do Not change speed and direction after collision
    void OnCollisionEnter2D(Collision2D collision2D)
    {
        Debug.Log(collision2D.gameObject.name);
        normal = collision2D.contacts[0].normal;
    }

}
