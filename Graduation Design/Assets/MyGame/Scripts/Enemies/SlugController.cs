using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlugController : MonoBehaviour
{
    Rigidbody2D rb;
    Transform position;
    float speed = 3f;
    float groundDistance = 0.7f;
    public LayerMask groundLayer;
    public LayerMask player;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        position = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        PhysicsCheck();
    }

    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length, LayerMask layer)
    {
        Vector2 pos = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDirection, length, layer);

        Color color = hit ? Color.red : Color.green;

        Debug.DrawRay(pos + offset, rayDirection * length, color);

        return hit;
    }

    void PhysicsCheck()
    {
        float direction = transform.localScale.x;
        Vector2 faceDir = new Vector2(-direction, 0f);
        RaycastHit2D faceCheck = Raycast(new Vector2(-0.4f*direction, 0f), faceDir, 0.5f, groundLayer);
        RaycastHit2D playerCheck = Raycast(new Vector2(-0.4f * direction, 0f), faceDir, 0.5f, player);
        RaycastHit2D groundCheck = Raycast(new Vector2(-0.5f*direction, 0f), Vector2.down, groundDistance, groundLayer);

        if (faceCheck || !groundCheck || playerCheck)
            position.localScale = new Vector2(position.localScale.x * -1, 1);

        if (direction < 0)
            rb.velocity = new Vector2(speed, 0f);
        else rb.velocity = new Vector2(-speed, 0f);
        
        
    }
}
