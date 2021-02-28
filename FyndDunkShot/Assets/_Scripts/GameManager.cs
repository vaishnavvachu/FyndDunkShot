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
    #endregion

    #region PRIVATE Variables
    float ScreenHeight, ScreenWidth;
    int CurrentBasketNumber = 0;
    Transform[] Baskets;
    int TotalBaskets = 3;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        MainCamera = Camera.main;
        Baskets = new Transform[TotalBaskets];
        //Create the First Basket with the Ball in it,
        //Keep a random position within ScreenHeight & ScreenWidth

        ScreenHeight = Screen.height;
        ScreenWidth = Screen.width;

        SpawnBasketsAtXYPos(ScreenHeight * 0.2f, ScreenWidth * 0.75f, 0);

        //Then add another Basket to move the ball to.
        SpawnBasketsAtXYPos(ScreenHeight * 0.4f, ScreenWidth * 0.25f, 1);



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
        Baskets[BasketCount].localScale = new Vector3(1, 1, 1);
        Baskets[BasketCount].position = MainCamera.ScreenToWorldPoint(new Vector3(XPos, YPos, MainCamera.nearClipPlane));
        Baskets[BasketCount].gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
