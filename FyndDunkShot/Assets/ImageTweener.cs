using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ImageTweener : MonoBehaviour
{
    public Vector2 EndPosition;
    // Start is called before the first frame update
    void Start()
    {
        
        transform.DOMove(EndPosition, 2f).SetLoops(-1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
