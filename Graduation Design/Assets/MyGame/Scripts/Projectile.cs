using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D rb;
    public Collider2D coll;
    public LayerMask ground;
    public LayerMask enemy;
    public float speed;
    public float lifeTime;

    public GameObject destroyEffect;


    private void Start()
    {
        Invoke("DestroyProjectile", lifeTime);
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.bodyType != RigidbodyType2D.Static)
            rb.velocity = transform.up * speed;

        if (coll.IsTouchingLayers(ground) || coll.IsTouchingLayers(enemy))
            rb.bodyType = RigidbodyType2D.Static;
        

    }

    void DestroyProjectile()
    {
        //Instantiate(destroyEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
        }
    }

}
