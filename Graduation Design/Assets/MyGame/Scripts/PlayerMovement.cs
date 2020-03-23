using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sr;

    [Header("CD的UI组件")]
    public Image cdImage;

    [Header("移动参数")]
    public float speed = 8f;
    public float crouchSpeedDivisor = 3f;
    public float horizontalMove;

    [Header("跳跃参数")]
    public float jumpForce = 6.3f;
    public float hangingJumpForce = 15f;
    int jumpCount;

    [Header("Dash参数")]
    public float dashTime;//dash时长
    private float dashTimeLeft;//dash剩余时长
    private float lastDash = -10f;//上一次dash时间
    public float dashCoolDown;
    public float dashSpeed;
    public float dashForce = 50f;

    [Header("状态")]
    public bool isCrouch;
    public bool isOnGround;
    public bool isJump;
    public bool isHeadBlocked;
    public bool isHanging;
    public bool isDashing;
    public bool isAttack;
    public bool isHurt;
    public float hurtTime;

    [Header("环境检测")]
    public float footOffset = 0.47f;
    public float headClearance = 0.5f;
    public float groundDistance = 0.2f;
    float playerHeight;
    public float eyeHeight = 0.12f;
    public float grabDistance = 0.4f;
    public float reachOffset = 0.7f;

    public LayerMask groundLayer;

    public float xVelocity;

    //按键设置
    bool jumpPressed;
    bool crouchHeld;
    bool crouchPressed;

    //碰撞体尺寸
    Vector2 colliderStandSize;
    Vector2 colliderStandOffset;
    Vector2 colliderCrouchSize;
    Vector2 colliderCrouchOffset;

    //碰撞体中心距上下左右边界的距离
    float up = 0.5f;
    float down = 0.76f;
    float left = 0.44f;
    //float right = 0.5f;

    //受伤参数
    Color disappear = new Color(1, 1, 1, 0);
    Color appear = new Color(1, 1, 1, 1);
    float duringTime = 1f;
    float gapTime = 0.1f;
    float temp = 0f;
    bool IsDisplay = true;
    bool NoHurt = false;




    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();


        playerHeight = up;
        colliderStandSize = coll.size;
        colliderStandOffset = coll.offset;
        colliderCrouchSize = new Vector2(coll.size.x, coll.size.y / 2f);
        colliderCrouchOffset = new Vector2(coll.offset.x,coll.offset.y - coll.size.y/4f);     
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetButtonDown("Jump") && jumpCount > 0 )
            jumpPressed = true;

        crouchHeld = Input.GetButton("Crouch");
        crouchPressed = Input.GetButtonDown("Crouch");

        if (Input.GetMouseButtonDown(1))
        {
            if(Time.time >= (lastDash + dashCoolDown))
            {
                //可以执行dash
                ReadyToDash();
            }
        }


        //cdUI
        cdImage.fillAmount -= 1.0f / dashCoolDown * Time.deltaTime;

        //WeaponDirection();

        if (NoHurt)//受伤后无敌时间
        {
            duringTime -= Time.deltaTime;
            if (duringTime <= 0)
            {
                if (!IsDisplay)
                {
                    sr.color = appear;
                    IsDisplay = true;
                    temp = 0;
                }
                NoHurt = false;
                duringTime = 1f;
            } 
                

            temp += Time.deltaTime;
            if (temp >= gapTime)
            {
                if (IsDisplay)
                {
                    sr.color = disappear;
                    IsDisplay = false;
                    temp = 0;
                }
                else
                {
                    sr.color = appear;
                    IsDisplay = true;
                    temp = 0;
                }
            }
        }

        if (Time.time > hurtTime + 0.2f) 
        {
            isHurt = false;
        }
    }

    private void FixedUpdate()
    {

        PhysicsCheck();

        if(!isHurt)
            GroundMovement();

        Jump();

    }

    void PhysicsCheck()
    {
        //脚底射线
        RaycastHit2D leftCheck = Raycast(new Vector2(-left, -down), Vector2.down, groundDistance, groundLayer);
        RaycastHit2D rightCheck = Raycast(new Vector2(left, -down), Vector2.down, groundDistance, groundLayer);

        if (leftCheck || rightCheck)
            isOnGround = true;
        else isOnGround = false;

        //isOnGround = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        //头顶射线
        RaycastHit2D leftheadCheck = Raycast(new Vector2(-left, coll.offset.y + coll.size.y /2f), Vector2.up, headClearance, groundLayer);
        RaycastHit2D rightheadCheck = Raycast(new Vector2(left, coll.offset.y + coll.size.y / 2f), Vector2.up, headClearance, groundLayer);

        if (leftheadCheck||rightheadCheck)
            isHeadBlocked = true;
        else isHeadBlocked = false;


        //悬挂判定射线
        float direction = transform.localScale.x;
        Vector2 grabDir = new Vector2(direction, 0f);
        RaycastHit2D blockedCheck = Raycast(new Vector2(footOffset * direction, playerHeight), grabDir, grabDistance, groundLayer);
        RaycastHit2D wallCheck = Raycast(new Vector2(footOffset * direction, eyeHeight), grabDir, grabDistance, groundLayer);
        RaycastHit2D ledgeCheck = Raycast(new Vector2(reachOffset * direction, playerHeight), Vector2.down, grabDistance, groundLayer);

        if (!isOnGround && rb.velocity.y < 0f && ledgeCheck && wallCheck && !blockedCheck)
        {
            Vector3 pos = transform.position;

            pos.x += (wallCheck.distance -0.05f) * direction;
            pos.y -= ledgeCheck.distance;

            transform.position = pos;

            rb.bodyType = RigidbodyType2D.Static;
            isHanging = true;
        }
    }

    void GroundMovement()
    {
        //判断悬挂
        if (isHanging) 
            return;

        Dash();

        if (isDashing)
            return;


        if (crouchHeld && !isCrouch && isOnGround)
            Crouch();
        else if (!crouchHeld && isCrouch && !isHeadBlocked)
            StandUp();
        else if (!isOnGround && isCrouch)
            StandUp();

        xVelocity = Input.GetAxis("Horizontal");

        if (isCrouch)
            xVelocity /= crouchSpeedDivisor;

        rb.velocity = new Vector2(xVelocity * speed, rb.velocity.y);

        FilpDirection();
        
    }

    void Jump()
    {

        if (isHanging)
        {
            jumpCount=1;
            if (jumpPressed)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.velocity = new Vector2(rb.velocity.x, hangingJumpForce);
                isHanging = false;
            }
        }
        if (crouchPressed)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;

            isHanging = false;
        }

        if (isOnGround)
        {
            jumpCount = 2;
            isJump = false;
        }
        if (jumpPressed && isOnGround && !isHeadBlocked)
        {
            isJump = true;
            //rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            jumpCount--;
            jumpPressed = false;
        }
        else if (jumpPressed && jumpCount > 0 && isJump && !isHeadBlocked)//二段跳
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            //rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            jumpCount--;
            jumpPressed = false;
        }
    }


    //人物朝向
    void FilpDirection()
    {
        //人物朝向跟随鼠标
        //获取鼠标坐标
        Vector3 mousePosition = Input.mousePosition;
        //获取人物的世界坐标并转化成屏幕坐标
        Vector3 ScreenPosition = Camera.main.WorldToScreenPoint(transform.position);


        if(mousePosition.x > ScreenPosition.x)
            transform.localScale = new Vector2(1, 1);
        else
            transform.localScale = new Vector2(-1, 1);

        /*人物朝向跟随键盘按键
        if (xVelocity < 0)
            transform.localScale = new Vector2(-1, 1);
        if (xVelocity > 0)
            transform.localScale = new Vector2(1, 1);
        */

    }

    //下蹲与站起
    void Crouch()
    {
        isCrouch = true;
        coll.size = colliderCrouchSize;
        coll.offset = colliderCrouchOffset;
    }

    void StandUp()
    {
        isCrouch = false;
        coll.size = colliderStandSize;
        coll.offset = colliderStandOffset;
    }

    //射线重载
    RaycastHit2D Raycast(Vector2 offset,Vector2 rayDirection,float length,LayerMask layer)
    {
        Vector2 pos = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDirection, length, layer);

        Color color = hit ? Color.red : Color.green;

        Debug.DrawRay(pos + offset, rayDirection * length,color);

        return hit;
    }

    //冲刺函数
    void ReadyToDash()
    {
        if (!isHanging && !isCrouch)
            isDashing = true;


        dashTimeLeft = dashTime;

        lastDash = Time.time;

        cdImage.fillAmount = 1;

    }

    void Dash()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");//-1，0，1
        Vector2 mouseVector = getMouseVector();
        
        if (isDashing)
        {
            if(dashTimeLeft > 0)
            {
                /*只能左右冲刺
                if (rb.transform.localScale.x >= 0)
                    rb.velocity = new Vector2(dashSpeed , rb.velocity.y);
                else
                    rb.velocity = new Vector2(dashSpeed * -1f, rb.velocity.y);
                */

                //向鼠标位置冲刺
                rb.velocity = new Vector2(dashSpeed * mouseVector.x,dashForce * mouseVector.y);

                dashTimeLeft -= Time.deltaTime;

                ShadowPool.instance.GetFromPool();
            }
            if (dashTimeLeft <= 0)
            {

                rb.velocity = new Vector2(rb.velocity.x, 0f);
                isDashing = false;
                
            }
                
        }
    }


    private Vector2 getMouseVector()
    {
        Vector2 mouse = Input.mousePosition;
        Vector2 obj = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 direction = mouse - obj;
        direction = direction.normalized;
        return direction;

    }

    //受伤函数
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.tag == "Enemy"&&!NoHurt)
        {

            if (transform.position.x < collision.gameObject.transform.position.x)
            {
                rb.velocity = new Vector2(-15, 5);
                hurtTime = Time.time;
                isHurt = true;
                NoHurt = true;
                
            }
            else if (transform.position.x > collision.gameObject.transform.position.x)
            {
                rb.velocity = new Vector2(15, 5);
                hurtTime = Time.time;
                isHurt = true;
                NoHurt = true;
            }
                
        }
    }
}


