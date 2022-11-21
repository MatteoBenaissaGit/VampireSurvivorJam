using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


[RequireComponent(typeof(Rigidbody2D))]
public class ObjectMoney : MonoBehaviour
{
    public int Value;

    private Rigidbody2D _rigidbody2D;
    
    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();

        const float force = 10f;
        _rigidbody2D.AddForce(new Vector2(Random.Range(-1,1), Random.Range(-5,5)) * force);
        StartCoroutine(DeactivateGravity(0.5f));
    }

    private IEnumerator DeactivateGravity(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _rigidbody2D.gravityScale = 0;
        _rigidbody2D.velocity = Vector2.zero;
    }
}