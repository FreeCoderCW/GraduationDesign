using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D rb;
    public Collider2D coll;
    public LayerMask ground;
    public float speed;
    public float lifeTime;
    [SerializeField] float damage=1f;

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

        if (coll.IsTouchingLayers(ground))
            rb.bodyType = RigidbodyType2D.Static;
        

    }

    void DestroyProjectile()
    {
        //Instantiate(destroyEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.GetComponent<PiranhaController>().health -= damage;
            Destroy(gameObject);
        }
        
    }
}
