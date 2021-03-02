using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketController : MonoBehaviour
{
    public static BasketController instance;
    public int index;

    private void Awake()
    {
        instance = this;
    }

 
    //Stop the ball from bouncing when it enters the collider on a basket
    //Hold the ball and let it be launched to other
    private void OnTriggerStay2D(Collider2D collision)
    {
        BallController.instance.isBallLaunched = false;

        Rigidbody2D BallRigidbody2D = collision.GetComponent<Rigidbody2D>();
        BallRigidbody2D.velocity = Vector2.zero;
        BallRigidbody2D.simulated = false;
        collision.transform.position = new Vector3(transform.position.x, transform.position.y - 0.2f, -9);
        collision.transform.SetParent(transform);

        //Check if the ball has not landed on the same basket again
        //If Not then Spawn a New Basket at Random Position
        if(BallController.instance.CurrentBasketIndex != index)
        {
            GameManager.instance.DecreasedSizeIndex = BallController.instance.CurrentBasketIndex;
            GameManager.instance.SizeDecreased = true;
            BallController.instance.CurrentBasketIndex = index;

            GameManager.instance.SpawnBasketAtRandomPosition();
        }
    }
}
