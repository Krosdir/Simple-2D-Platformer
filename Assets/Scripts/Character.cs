using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Unit
{
    [SerializeField] private float speed = 3.0f;
    
    public int Lives
    {
        get { return lives; }
        set
        {
            if (value < 5) lives = value;
            livesBar.Refresh();
            if (value == 0) isDead = true;
        }
    }
    [SerializeField] private float jumpforce = 15.0f;

    public bool isGrounded = false;
    private bool isShootable = false;
    private bool isHit = false;
    public float dropping = 5f;
    float dropTime = 0;

    public float nextShot = 1f;
    private float nextShotTime = 0f;

    public float crouchcollsize;
    public float crouchcolloffset;
    private float origincollsize;
    private float origincolloffset;

    private LivesBar livesBar;
    private Bullet bullet;
    private BoxCollider2D boxcoll;

    protected CharState State
    {
        get { return (CharState)animator.GetInteger("State"); }
        set { animator.SetInteger("State", (int)value); }
    }

    new private Rigidbody2D rigidbody;

    private void Awake()
    {
        livesBar = FindObjectOfType<LivesBar>();
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        bullet = Resources.Load<Bullet>("Bullet");

        boxcoll = GetComponent<BoxCollider2D>();
        origincollsize = boxcoll.size.y;
        origincolloffset = boxcoll.offset.y;
    }

    private void Run()
    {
        if (isGrounded) State = CharState.Run;
        Vector3 direction = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
        //rigidbody.velocity = new Vector2(direction.x * speed ,rigidbody.velocity.y);
        sprite.flipX = direction.x < 0.1F;
        
    }

    private void Jump()
    {
        rigidbody.AddForce(transform.up * jumpforce, ForceMode2D.Impulse);
    }

    private void Crouch()
    {
        boxcoll.size = new Vector2(boxcoll.size.x, crouchcollsize);
        boxcoll.offset = new Vector2(boxcoll.offset.x, crouchcolloffset);
        State = CharState.Crouch;
    }

    private void Shoot()
    {
        Vector3 position = transform.position;
        position.x = (sprite.flipX ? position.x += -1f : position.x += 0.5f);
        position.y = (Input.GetButton("Vertical") ? position.y : position.y + 0.15f);
        Bullet newBullet = Instantiate(bullet, position, bullet.transform.rotation) as Bullet;
        newBullet.Parent = gameObject;
        newBullet.Direction = newBullet.transform.right * (sprite.flipX ? -1.0f : 1.0f);
    }

    private void RateShoot()
    {
        if (nextShotTime == 0 || Time.time > nextShotTime)
        {
            Shoot();
            nextShotTime = Time.time + nextShot;
        }
    }

    private void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.2f);

        isGrounded = colliders.Length > 1;
        if (!isGrounded) State = CharState.Jump;
    }

    public override void ReceiveDamage()
    {
        Lives--;
        StartCoroutine(Hit());
        //rigidbody.AddForce(transform.up * dropping + (sprite.flipX ? transform.right * dropping/4 : -transform.right * dropping/4), ForceMode2D.Impulse);
        Debug.Log(lives);
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    private IEnumerator Hit()
    {
        isHit = true;
        yield return new WaitForSeconds(0.35f);
        isHit = false;
    }

    private IEnumerator Dying()
    {
        //boxcoll.enabled = false;
        if (dropTime == 0) dropTime = Time.time;
        if (Time.time < dropTime + deathTime / 10)
        {
            rigidbody.AddForce(transform.up * dropping + (sprite.flipX ? transform.right * dropping / 4 : -transform.right * dropping / 4), ForceMode2D.Impulse);
        }
        if (!isGrounded)
            State = CharState.JumpHit;
        else
            State = CharState.Dead;
        yield return new WaitForSeconds(deathTime);
        Die();
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        Bullet bullet = collider.GetComponent<Bullet>();
        if (bullet && gameObject != bullet.Parent)
            ReceiveDamage();
    }

    // Update is called once per frame
    void Update()
    {

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (isDead)
            StartCoroutine(Dying());
        else
        if (isHit)
        {
            if (isGrounded && Input.GetButton("Vertical"))
                State = CharState.CrouchHit;
            else if (isGrounded)
                State = CharState.Hit;
            //else if (!isGrounded)
            //    State = CharState.JumpHit;

        }
        else
        {
            if (isShootable)
            {
                if (isGrounded)
                {
                    State = CharState.Idle;
                    if (Input.GetButton("Fire1"))
                    {
                        State = CharState.Shoot;
                        RateShoot();
                    }
                }

                if (isGrounded && Input.GetButton("Vertical"))
                {
                    Crouch();
                    if (Input.GetButton("Fire1"))
                    {
                        State = CharState.CrouchShoot;
                        RateShoot();
                    }
                }
                if (!isGrounded && Input.GetButton("Fire1"))
                {
                    State = CharState.JumpShoot;
                    RateShoot();
                }
            }
            if (!Input.GetButton("Vertical"))
            {
                boxcoll.size = new Vector2(boxcoll.size.x, origincollsize);
                boxcoll.offset = new Vector2(boxcoll.offset.x, origincolloffset);
            }

            if (Input.GetButton("Horizontal"))
            {
                Run();
                isShootable = false;
            }
            else
                isShootable = true;
            if (isGrounded && Input.GetButtonDown("Jump") && !Input.GetButton("Vertical"))
                Jump();
        }
        
        
        
    }
}

public enum CharState
{
    Idle,
    Run,
    Jump,
    Crouch,
    Shoot,
    JumpShoot,
    CrouchShoot,
    Hit,
    CrouchHit,
    JumpHit,
    Dead
}
