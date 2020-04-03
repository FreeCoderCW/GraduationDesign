using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private Transform target;
    [SerializeField] private float moveSpeed;

    private float lifeTimer;
    [SerializeField] private float maxLife = 2.0f;

    public GameObject attackEffect;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        lifeTimer += Time.deltaTime;
        if(lifeTimer >= maxLife)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Instantiate(attackEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
