using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform weaponHold;

    public GameObject NormalArrow;
    public GameObject FastArrow;
    public Transform shotPoint;
    public Sprite readyBowSR;
    public Sprite bowSR;

    public float damage;

    private float timeBtwShots;
    public float startTimeBtwShots;

    //蓄力射击
    float holdTime;
    float focusTime = 0.5f;

    private void Start()
    {
        weaponHold = transform.GetComponentInParent<Transform>();
       
        
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouse = Input.mousePosition;
        Vector2 obj = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 direction = mouse - obj;
        direction = direction.normalized;
        weaponHold.up = direction;



        if (timeBtwShots <= 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                holdTime = Time.time;
                gameObject.GetComponent<SpriteRenderer>().sprite = readyBowSR;
            }
            if (Input.GetMouseButtonUp(0))
            {
                holdTime = Time.time - holdTime;
                if (holdTime >= focusTime)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = bowSR;
                    Instantiate(FastArrow, shotPoint.position, transform.rotation);
                    damage = 2;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = bowSR;
                    Instantiate(NormalArrow, shotPoint.position, transform.rotation);
                    damage = 1;
                } 
                
                timeBtwShots = startTimeBtwShots;
            }
        }
        else
        {
            timeBtwShots -= Time.deltaTime;
        }
        
    }

}
