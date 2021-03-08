using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    #region PUBLIC Variables

    [Header("Ball Properties")]
    public bool isBallLaunched;
    public int CurrentBasketIndex;

    [Header("Trajectory Prefabs")]
    public GameObject TrajectoryPointsPrefab;

    [NonSerialized]public static BallController instance;

    #endregion

    #region PRIVATE Variables

    //Ball Properties
    Vector2 _ballLaunchVelocity;
    Vector2 _gravity;
    Vector2 _initialPosition;

    float _power, _totalForce, _distance, _forcePercentage;
    float _minOffset, _maxOffset;

    Rigidbody2D _ballRigidbody2D;

    //Trajectory Points
    GameObject[] _points;
    int _totalNumberOfPoints = 10;
    Vector3 _normal;

    //UI Controller
    UIController _uiController;

    #endregion

    private void Awake()
    {
        isBallLaunched = false;
        instance = this;
        _uiController = FindObjectOfType<UIController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _ballRigidbody2D = GetComponent<Rigidbody2D>();
        _power = 5f;
        _totalForce = 7f;
        _gravity = new Vector2(0f, -9.81f);
        CurrentBasketIndex = 0;

        _points = new GameObject[_totalNumberOfPoints];

        for (int n = 0; n < _totalNumberOfPoints; n++)
        {
            GameObject TrajectoryPoint = Instantiate(TrajectoryPointsPrefab);
            _points[n] = TrajectoryPoint;
            _points[n].SetActive(false);
        }

       
        GameManager gameManagerInstance = GameManager.instance;

        _minOffset = gameManagerInstance.MainCamera.transform.position.x - gameManagerInstance.WorldScreenWidth / 2;
        _maxOffset = gameManagerInstance.MainCamera.transform.position.y - gameManagerInstance.WorldScreenWidth / 2;
        
    }

    // Update is called once per frame
    //Check for user Touch and Drag
    
    void Update()
    {
        if (Input.touchCount > 0 && !isBallLaunched)
        {
            ProcessTouchInput();
        }
    }

    //Implement the Slingshot mechanism
    //If user drags the ball more than ForcePercentage (50%), Launch the Ball int the angle of Touch-Drag
    private void ProcessTouchInput()
    {
        switch (Input.GetTouch(0).phase)
        {
            case TouchPhase.Began:
                _initialPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                break;

                //When Drag Occurs
                //Show Trajectory
            case TouchPhase.Moved:
                _uiController.BeginGame();
                Vector2 toPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                _ballLaunchVelocity = CalculateForceBetween(_initialPosition, toPosition);
                transform.parent.transform.rotation = Quaternion.Euler(0, 0, CalculateAngleBetween(_initialPosition, toPosition));
                ShowTrajectoryFromBallPosition();
                break;

                //When Drag Ends, Launch Ball
                //Hide Trajecotry
            case TouchPhase.Ended:
                if (_forcePercentage > 0.5)
                {
                    isBallLaunched = true;
                    _ballRigidbody2D.simulated = true;
                    _ballRigidbody2D.velocity = -1 * _ballLaunchVelocity;
                    HideTrajectoryFromBall();
                }
                break;
        }

    }


    //Calculate Force to move from PositionA to PositionB
    //Clamp the Force value and Calculate ForcePercentage
    private Vector2 CalculateForceBetween(Vector2 fromPos, Vector2 toPos)
    {
        Vector2 force = (toPos - fromPos) * _power;

        _distance = Vector2.Distance(fromPos, toPos);

        //Cap distance at 1.3f
        if (_distance > 1.3f)
        {
            _distance = 1.3f;
        }
         
        Vector2 clampedForce = Vector2.ClampMagnitude(force, _totalForce) * _distance;

        _forcePercentage = clampedForce.magnitude / _totalForce;

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
        Vector2 curveForce = _ballLaunchVelocity;

        //Calculate the distance for 100 points
        int maxIterations = 100;
        for(int i = 0; i< maxIterations; i++)
        {
            //Trace the Ball's Path through each of the 100 Points
            float time = 0.01f * i;
            curvePoint = _gravity * time * time * 0.5f + _ballLaunchVelocity * time + (Vector2)transform.position;
            if (curvePoint.x < _minOffset + 0.01f && curvePoint.x > _minOffset - 0.01f || curvePoint.x == _maxOffset)
                curveForce = Vector2.Reflect(curvePoint, _normal);

            //Plot the Position of the Path to Only 10 Points
            if (i % 10 == 0)
            {
                float elapsedTime = index * 0.08f;
                if (elapsedTime > 0)
                    elapsedTime *= -1;

                //Enable the Points
                _points[index].SetActive(true);
                _points[index].transform.localScale = new Vector3(PointSize, PointSize, PointSize);
                _points[index].transform.position = _gravity * elapsedTime * elapsedTime * 0.5f + curveForce * elapsedTime + (Vector2)transform.position;

                SpriteRenderer spriteRenderer = _points[index].GetComponent<SpriteRenderer>();
                Color alpha = spriteRenderer.color;

                if (_forcePercentage > 0.5)
                    alpha.a = 1 - ((1 - _forcePercentage) * 2);
                else
                    alpha.a = 0;

                spriteRenderer.color = alpha;

                //Decrease Size of Points Gradually
                PointSize -= 0.0025f;
                index++;
            }
        }
    }

    //Hide the Trajectory When the Touch is no longer there
    private void HideTrajectoryFromBall()
    {
        for (int n = 0; n < _totalNumberOfPoints; n++)
        {
            _points[n].SetActive(false);
            _points[n].transform.position = transform.position;
        }
    }

    //Do Not change speed and direction after collision
    void OnCollisionEnter2D(Collision2D collision2D)
    {
        _normal = collision2D.contacts[0].normal;

        //Play Sound When Ball Hits anything
        GameManager.instance.PlayBounceSound();
    }

}
