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
    #endregion

    #region PRIVATE Variables
    float ScreenHeight, ScreenWidth;
    int CurrentBasketNumber = 0;
    Transform[] Baskets;
    Transform Ball;
    int TotalBaskets = 3;
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
            
        Ball.SetParent(Baskets[CurrentBasketNumber]);
        Ball.transform.position = new Vector3(Baskets[CurrentBasketNumber].position.x, Baskets[CurrentBasketNumber].position.y + 0.2f, -1);
        Ball.gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
