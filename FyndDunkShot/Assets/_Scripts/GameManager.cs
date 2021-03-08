using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    #region PUBLIC Variables

    [NonSerialized] public static GameManager instance;

    [Header("Camera & Prefabs")]
    public Camera MainCamera;

    public GameObject BasketPrefab;
    public GameObject BallPrefab;

    [Header("Screen Measurements")]
    public float WorldScreenWidth, WorldScreenHeight;
    public bool SizeDecreased;
    public int DecreasedSizeIndex = 0;
    public float Margin;

    [Header("UI Elements")]
    public TextMeshProUGUI ScoreTextBox;
    public CanvasGroup GameOverPanel;
    public TextMeshProUGUI GameOverScoreTextBox;
    public TextMeshProUGUI HighScoreTextBox;

    [Header("Audio Components")]
    public AudioSource audioSource;
    public AudioClip WooSoundClip;
    public AudioClip BounceSoundClip;

    #endregion

    #region PRIVATE Variables

    //Scree Resolutions
    float _screenHeight, _screenWidth;
    int _currentBasketNumber = 0;
    float _cameraAndBasketYDifference;
    float _spacing;

    //Transform Components
    Transform[] _baskets;
    Transform _ball;

    //Basket Properties
    int _totalBaskets = 3;
    float _basketWidth;
    int _increasedSizeIndex;
    bool _sizeIncreased = false;
    
    //Score Related Values
    int _score = 0;
    int _highScore = 0;

    string AssetBundleURL;
    #endregion

    private void Awake()
    {
        //Download The AssetBundles
        StartCoroutine(DownloadAssetBundles());

    }
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
   
        MainCamera = Camera.main;
        _baskets = new Transform[_totalBaskets];
        _ball = null;

        //Create the First Basket 
        //Keep a random position within ScreenHeight & ScreenWidth
        _screenHeight = Screen.height;
        _screenWidth = Screen.width;

        SpawnBasketsAtXYPos(_screenWidth * 0.30f, _screenHeight * 0.25f, 0);

        //Then add another Basket to move the ball to.
        SpawnBasketsAtXYPos(_screenWidth * 0.75f, _screenHeight * 0.40f, 1);

        //Spawn a Ball in the First Basket
        SpawnABall();

        //Calculate World Screen Height, Width
        WorldScreenHeight = MainCamera.orthographicSize * 2;
        WorldScreenWidth = WorldScreenHeight * (_screenWidth / _screenHeight);

        //Set Boundaries on both side of the screen
        //Ball will bounce back when hit
        SetScreenBoundaries();

        //Calculate Width of the Basket Sprite
        SpriteRenderer BasketSpriteRenderer = _baskets[0].GetComponent<SpriteRenderer>();

        float BasketWidthMax = MainCamera.WorldToScreenPoint(new Vector3(BasketSpriteRenderer.bounds.max.x, 0, 0)).x;
        float BasketWidthMin = MainCamera.WorldToScreenPoint(new Vector3(BasketSpriteRenderer.bounds.min.x, 0, 0)).x;
        _basketWidth = BasketWidthMax - BasketWidthMin;


        //Distance between first spawned Basket and Camera
        float CameraAndBasketYDifference = MainCamera.transform.position.y - _baskets[0].position.y;

        //Calculate Margins
        _spacing = WorldScreenHeight * Margin;

        //Hide GameOver Menu
        GameOverPanel.alpha = 0;

        

    }

    private void LateUpdate()
    {
        
        //Move the Camera along with the Ball
        
        if (BallController.instance.isBallLaunched )
        {
            //Calculate Camera Bounds
            float BallPosition = _ball.position.y;
            float CameraPosition = MainCamera.transform.position.y - _cameraAndBasketYDifference;
            
            float CameraBoundaryTop = CameraPosition + _spacing;
            float CameraBoundaryBottom = CameraPosition - _spacing;

            if (BallPosition > CameraBoundaryTop || BallPosition < CameraBoundaryBottom)
            {
                //If Ball Moves Up, 
                //Move Camera Up
                if (BallPosition > CameraBoundaryTop)
                {
                    MainCamera.transform.position = new Vector3(0, BallPosition + _cameraAndBasketYDifference - _spacing, -10);
                }

                //Dont allow the Camera to Fall below the Current Basket Height
                else if (BallPosition < CameraBoundaryBottom && CameraPosition > _baskets[_currentBasketNumber].position.y)
                {
                    MainCamera.transform.position = new Vector3(0, BallPosition + _cameraAndBasketYDifference + _spacing, -10);
                }

                //If Ball Drops Below 
                //Spawn A New Ball
                else if (BallPosition < CameraPosition - _cameraAndBasketYDifference)
                {
                    BallController.instance.isBallLaunched = false;
                    _ball.gameObject.SetActive(false);
                    GameOver(); 
                }
            }
        }


        if (SizeDecreased)
        {
            _baskets[DecreasedSizeIndex].localScale -= new Vector3(0.3f, 0.3f, 0);
            if (_baskets[DecreasedSizeIndex].localScale.x <= 0)
            {
                SizeDecreased = false;
                _baskets[DecreasedSizeIndex].localScale = new Vector3(0, 0, 0);
                _baskets[DecreasedSizeIndex].rotation = Quaternion.Euler(0, 0, 0);
                _baskets[DecreasedSizeIndex].gameObject.SetActive(false);
            }
        }

        if (_sizeIncreased)
        {
            _baskets[_increasedSizeIndex].localScale += new Vector3(0.3f, 0.3f, 0);
            if (_baskets[_increasedSizeIndex].localScale.x >= 1)
            {
                _sizeIncreased = false;
               _baskets[_increasedSizeIndex].localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
        }


    }

    //Spawn Baskets at specified X and Y Position 
    //and mark BasketCount
    private void SpawnBasketsAtXYPos(float XPos, float YPos, int BasketCount)
    {
        //Instantiate BasketsPrefab if baskets[BasketCount] is empty
        if(_baskets[BasketCount] == null)
        {
            for(int n = 0; n < _totalBaskets; n++)
            {
                GameObject NewBasket = Instantiate(BasketPrefab);
                NewBasket.SetActive(false);
                NewBasket.GetComponent<BasketController>().index = n;
                _baskets[n] = NewBasket.transform;
            }
        }


        //Set BasketPrebs at XPos and YPos
        _baskets[BasketCount].localScale = new Vector3(0.5f, 0.5f, 0.5f);
        _baskets[BasketCount].position = MainCamera.ScreenToWorldPoint(new Vector3(XPos, YPos, MainCamera.nearClipPlane));
        _baskets[BasketCount].gameObject.SetActive(true);
    }

    private void SpawnABall()
    {
        if (_ball == null)
        {
            GameObject BasketBall = Instantiate(BallPrefab);
            _ball = BasketBall.transform;
        }

        //Attach the Ball as the Child of CurrentBasket
        _ball.SetParent(_baskets[_currentBasketNumber]);
        _ball.transform.position = new Vector3(_baskets[_currentBasketNumber].position.x, _baskets[_currentBasketNumber].position.y + 0.2f, -1);
        _ball.gameObject.SetActive(true);

        
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
        _currentBasketNumber = BallController.instance.CurrentBasketIndex;

        float XEndPoint = 0;
        float XStartingPoint = 0;

        Vector3 CurrentBasketPosition = MainCamera.WorldToScreenPoint(_baskets[_currentBasketNumber].position);

        if(CurrentBasketPosition.x > _screenWidth / 2)
        {
            XEndPoint = CurrentBasketPosition.x - _basketWidth;
            XStartingPoint = 0 + _basketWidth;
        }
        else
        {
            XEndPoint = _screenWidth - _basketWidth / 2;
            XStartingPoint = CurrentBasketPosition.x + _basketWidth;
        }

        float YStartingPoint = CurrentBasketPosition.y + (_screenHeight * 0.10f);
        float YEndPoint = CurrentBasketPosition.y + (_screenHeight * 0.30f);

        float RandomX = UnityEngine.Random.Range(XStartingPoint, XEndPoint);
        float RandomY = UnityEngine.Random.Range(YStartingPoint, YEndPoint);


        //Get The Next Basket Index and 
        //Spawn the basket at Random X and Y Position
        _baskets[GetNextBasketIndex(_currentBasketNumber)].position = MainCamera.ScreenToWorldPoint(new Vector3(RandomX, RandomY, MainCamera.nearClipPlane));
        _baskets[_increasedSizeIndex].gameObject.SetActive(true);
        _sizeIncreased = true;
    }

    // Generate the Index for the Next Basket Based on the Previous Basket Index
    private int GetNextBasketIndex(int index)
    {
        for(int n = 0; n< _baskets.Length; n++)
        {
            if (n != index && index != DecreasedSizeIndex)
                return _increasedSizeIndex = n;
        }

        return 0;
    }

    public void IncreaseScoreCount()
    {
        _score++;

        //Display Score
        ScoreTextBox.gameObject.SetActive(true);

        ScoreTextBox.text = _score.ToString();

        //Play Sounds
        PlayWooSound();
    }

    private void GameOver()
    {
        //Show Game Over Screen
        GameOverPanel.alpha = 1;
        GameOverScoreTextBox.text = "" + _score;
        GameOverPanel.interactable = true;
       _baskets[_currentBasketNumber].transform.rotation = new Quaternion(0f,0f,0f,1);
        
        if(_score > _highScore)
        {
            PlayerPrefs.SetInt("HighScore", _score);
            PlayerPrefs.Save();
        }

        _highScore = PlayerPrefs.GetInt("HighScore", 0);

        HighScoreTextBox.text = _highScore.ToString();

        Time.timeScale = 0;
    }

    public void PlayAgain()
    {
        //Reset Score
        _score = 0;
        ScoreTextBox.text = _score.ToString();

        StartCoroutine(RestartGame());
       
    }

    IEnumerator RestartGame()
    {
        Time.timeScale = 1;

        yield return new WaitForSeconds(0.5f);
        
        GameOverPanel.alpha = 0;
        GameOverPanel.interactable = false;
        SpawnABall();
       
    }

    //Change Sound Clip
    //Play BounceSound
    public void PlayBounceSound()
    {
        audioSource.clip = BounceSoundClip;
        audioSource.Play();
    }

    //Change Sound Clip
    //Play Woo Sound
    public void PlayWooSound()
    {
        audioSource.clip = WooSoundClip;
        audioSource.Play();
    }

    //Download The AssetBundles and 
    //Load Basketball Prefab and Basket Prefab
    IEnumerator DownloadAssetBundles()
    {
        
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            AssetBundleURL = "https://firebasestorage.googleapis.com/v0/b/fynddunkshoot.appspot.com/o/basketball?alt=media&token=864ab03d-1a48-47b0-9aea-bd5d99f1aabd";
        }
        else
        {
            AssetBundleURL = "https://firebasestorage.googleapis.com/v0/b/fynddunkshoot.appspot.com/o/Android_AssetBundles%2Fbasketball?alt=media&token=6ec61c35-d78f-48f2-b54a-a14796c95617";
        }
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(AssetBundleURL);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);

            BallPrefab = bundle.LoadAsset("basketball") as GameObject;

            BasketPrefab = bundle.LoadAsset("basket") as GameObject;
        }
    }
}
