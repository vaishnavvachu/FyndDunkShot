using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketController : MonoBehaviour
{
    public static BasketController instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

        


    }
}
