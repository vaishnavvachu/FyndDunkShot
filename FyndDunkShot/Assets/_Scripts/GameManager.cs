using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region PUBLIC Variables
    public static GameManager instance;
    public Camera MainCamera;
    public GameObject BasketPrefab;
    public GameObject BallPrefab;
    public float WorldScreenWidth, WorldScreenHeight;
    public bool SizeDecreased;
    public int DecreasedSizeIndex = 0;
    public float Margin;

    #endregion

    #region PRIVATE Variables
    float ScreenHeight, ScreenWidth;
    int CurrentBasketNumber = 0;
    Transform[] Baskets;
    Transform Ball;
    int TotalBaskets = 3;
    float BasketWidth;
    int IncreasedSizeIndex;
    bool SizeIncreased = false;
    float CameraAndBasketYDifference;
    float Spacing;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
      
        instance = this;
        MainCamera = Camera.main;
        Baskets = new Transform[TotalBaskets];
        Ball = null;

        //Create the First Basket 
        //Keep a random position within ScreenHeight & ScreenWidth
        ScreenHeight = Screen.height;
        ScreenWidth = Screen.width;

        SpawnBasketsAtXYPos(ScreenWidth * 0.30f, ScreenHeight * 0.25f, 0);

        //Then add another Basket to move the ball to.
        SpawnBasketsAtXYPos(ScreenWidth * 0.75f, ScreenHeight * 0.40f, 1);

        //Spawn a Ball in the First Basket
        SpawnABall();

        //Calculate World Screen Height, Width
        WorldScreenHeight = MainCamera.orthographicSize * 2;
        WorldScreenWidth = WorldScreenHeight * (ScreenWidth / ScreenHeight);

        //Set Boundaries on both side of the screen
        //Ball will bounce back when hit
        SetScreenBoundaries();

        //Calculate Width of the Basket Sprite
        SpriteRenderer BasketSpriteRenderer = Baskets[0].GetComponent<SpriteRenderer>();

        float BasketWidthMax = MainCamera.WorldToScreenPoint(new Vector3(BasketSpriteRenderer.bounds.max.x, 0, 0)).x;
        float BasketWidthMin = MainCamera.WorldToScreenPoint(new Vector3(BasketSpriteRenderer.bounds.min.x, 0, 0)).x;
        BasketWidth = BasketWidthMax - BasketWidthMin;


        //Distance between first spawned Basket and Camera
        float CameraAndBasketYDifference = MainCamera.transform.position.y - Baskets[0].position.y;

        //Margins
        Spacing = WorldScreenHeight * Margin;
        
    }

    private void LateUpdate()
    {
        
        //Move the Camera along with the Ball
        
        if (BallController.instance.isBallLaunched)
        {
            //Calculate Camera Bounds
            float BallPosition = Ball.position.y;
            float CameraPosition = MainCamera.transform.position.y - CameraAndBasketYDifference;
            
            float CameraBoundaryTop = CameraPosition + Spacing;
            float CameraBoundaryBottom = CameraPosition - Spacing;

            if (BallPosition > CameraBoundaryTop || BallPosition < CameraBoundaryBottom)
            {
                //If Ball Moves Up, 
                //Move Camera Up
                if (BallPosition > CameraBoundaryTop)
                {
                    MainCamera.transform.position = new Vector3(0, BallPosition + CameraAndBasketYDifference - Spacing, -10);
                }

                //Dont allow the Camera to Fall below the Current Basket Height
                else if (BallPosition < CameraBoundaryBottom && CameraPosition > Baskets[CurrentBasketNumber].position.y)
                {
                    MainCamera.transform.position = new Vector3(0, BallPosition + CameraAndBasketYDifference + Spacing, -10);
                }

                //If Ball Drops Below 
                //Spawn A New Ball
                else if (BallPosition < CameraPosition - CameraAndBasketYDifference)
                {
                    BallController.instance.isBallLaunched = false;
                    Ball.gameObject.SetActive(false);
                    SpawnABall();
                }
            }
        }


        if (SizeDecreased)
        {
            Baskets[DecreasedSizeIndex].localScale -= new Vector3(0.3f, 0.3f, 0);
            if (Baskets[DecreasedSizeIndex].localScale.x <= 0)
            {
                SizeDecreased = false;
                Baskets[DecreasedSizeIndex].localScale = new Vector3(0, 0, 0);
                Baskets[DecreasedSizeIndex].rotation = Quaternion.Euler(0, 0, 0);
                Baskets[DecreasedSizeIndex].gameObject.SetActive(false);
            }
        }

        if (SizeIncreased)
        {
            Baskets[IncreasedSizeIndex].localScale += new Vector3(0.3f, 0.3f, 0);
            if (Baskets[IncreasedSizeIndex].localScale.x >= 1)
            {
                SizeIncreased = false;
                Baskets[IncreasedSizeIndex].localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
        }


    }

    //Spawn Baskets at specified X and Y Position 
    //and mark BasketCount
    private void SpawnBasketsAtXYPos(float XPos, float YPos, int BasketCount)
    {
        //Instantiate BasketsPrefab if baskets[BasketCount] is empty
        if(Baskets[BasketCount] == null)
        {
            for(int n = 0; n < TotalBaskets; n++)
            {
                GameObject NewBasket = Instantiate(BasketPrefab);
                NewBasket.SetActive(false);
                NewBasket.GetComponent<BasketController>().index = n;
                Baskets[n] = NewBasket.transform;
            }
        }
       
        
        //Set BasketPrebs at XPos and YPos
        Baskets[BasketCount].localScale = new Vector3(0.5f, 0.5f, 0.5f);
        Baskets[BasketCount].position = MainCamera.ScreenToWorldPoint(new Vector3(XPos, YPos, MainCamera.nearClipPlane));
        Baskets[BasketCount].gameObject.SetActive(true);
    }

    private void SpawnABall()
    {
        if (Ball == null)
        {
            GameObject BasketBall = Instantiate(BallPrefab);
            Ball = BasketBall.transform;
        }
        
        //Attach the Ball as the Child of CurrentBasket
        Ball.SetParent(Baskets[CurrentBasketNumber]);
        Ball.transform.position = new Vector3(Baskets[CurrentBasketNumber].position.x, Baskets[CurrentBasketNumber].position.y + 0.2f, -1);
        Ball.gameObject.SetActive(true);
    }

    //Take the two edge colliders on the Camera and 
    //Place it on Both sides of the screen
    private void SetScreenBoundaries()
    {
        EdgeCollider2D[] edgeColliders = MainCamera.GetComponents<EdgeCollider2D>();

        List<Vector2> newVerticies = new List<Vector2>();

        newVerticies.Add(new Vector2(WorldScreenWidth / 2, WorldScreenHeight / 2));
        newVerticies.Add(new Vector2(WorldScreenWidth / 2, -WorldScreenHeight / 2));
        edgeColliders[0].points = newVerticies.ToArray();
        newVerticies.Clear();


        newVerticies.Add(new Vector2(-WorldScreenWidth / 2, WorldScreenHeight / 2));
        newVerticies.Add(new Vector2(-WorldScreenWidth / 2, -WorldScreenHeight / 2));
        edgeColliders[1].points = newVerticies.ToArray();

    }
    

    //Calculate Screen EndPoints
    //Spawn Basket at alternate side of the screen At Random X and Y coordinates
    public void SpawnBasketAtRandomPosition()
    {
        CurrentBasketNumber = BallController.instance.CurrentBasketIndex;

        float XEndPoint = 0;
        float XStartingPoint = 0;

        Vector3 CurrentBasketPosition = MainCamera.WorldToScreenPoint(Baskets[CurrentBasketNumber].position);

        if(CurrentBasketPosition.x > ScreenWidth / 2)
        {
            XEndPoint = CurrentBasketPosition.x - BasketWidth;
            XStartingPoint = 0 + BasketWidth;
        }
        else
        {
            XEndPoint = ScreenWidth - BasketWidth / 2;
            XStartingPoint = CurrentBasketPosition.x + BasketWidth;
        }

        float YStartingPoint = CurrentBasketPosition.y + (ScreenHeight * 0.20f);
        float YEndPoint = CurrentBasketPosition.y + (ScreenHeight * 0.40f);

        float RandomX = UnityEngine.Random.Range(XStartingPoint, XEndPoint);
        float RandomY = UnityEngine.Random.Range(YStartingPoint, YEndPoint);


        //Get The Next Basket Index and 
        //Spawn the basket at Random X and Y Position
        Baskets[GetNextBasketIndex(CurrentBasketNumber)].position = MainCamera.ScreenToWorldPoint(new Vector3(RandomX, RandomY, MainCamera.nearClipPlane));
        Baskets[IncreasedSizeIndex].gameObject.SetActive(true);
        SizeIncreased = true;
    }

    // Generate the Index for the Next Basket Based on the Previous Basket Index
    private int GetNextBasketIndex(int index)
    {
        for(int n = 0; n< Baskets.Length; n++)
        {
            if (n != index && index != DecreasedSizeIndex)
                return IncreasedSizeIndex = n;
        }

        return 0;
    }


}
