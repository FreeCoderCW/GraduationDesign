using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiranhaController : MonoBehaviour
{
    public Animator anim;
    public Transform piranha;
    public Transform player;
    public GameObject deathEffect;

    [SerializeField] private float attackRange;

    public BoxCollider2D boxcoll;
    public float health;

    public GameObject projectile;


    Vector2 OldOffset;
    Vector2 OldSize;
    Vector2 NewOffset;
    Vector2 NewSize;
    

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        piranha = GetComponent<Transform>();
        boxcoll = GetComponent<BoxCollider2D>();
        OldOffset = new Vector2(boxcoll.offset.x,boxcoll.offset.y);
        OldSize = new Vector2(boxcoll.size.x, boxcoll.size.y);
        NewOffset = new Vector2(boxcoll.offset.x - 0.75f, boxcoll.offset.y);
        NewSize = new Vector2(2.5f, boxcoll.size.y);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        death();

        if (Vector2.Distance(transform.position,player.position) < attackRange)
            anim.SetBool("attack", true);
        else 
            anim.SetBool("attack", false);

        if (player.position.x < piranha.position.x)
            piranha.localScale = new Vector2(1, 1);
        else
            piranha.localScale = new Vector2(-1, 1);


    }

    public void Attack()
    {
        boxcoll.offset = NewOffset;
        boxcoll.size = NewSize;
        shot();
    }

    public void Back()
    {
        boxcoll.offset = OldOffset;
        boxcoll.size = OldSize;
    }

    public void death()
    {
        if (health <= 0)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
            
    }

    public void shot()
    {
        Instantiate(projectile, transform.position, Quaternion.identity);
    }

}
