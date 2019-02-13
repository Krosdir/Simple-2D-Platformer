using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : Unit
{
    public Transform bite;
    private float biteDistance;
    public float biteRadius;

    RaycastHit2D groundInfo;
    public Transform groundDetection;
    private float groundDetectionDistance;
    private float distanceGround = 1f;

    [SerializeField] private float speed = 2f;

    private Character character;
    private Vector3 direction;
    private BoxCollider2D boxcoll;

    public float sleepcolloffset;
    public float sleepcollsize;
    private float origincolloffset;
    public float flipcolloffset;

    private bool isAttacking = false;

    private float distance = 0;
    public float vision = 10f;
    private float attackDistance = 1f;

    private RaycastHit2D saw, saw1;
    new private Rigidbody2D rigidbody;

    protected SnakeState State
    {
        get { return (SnakeState)animator.GetInteger("State"); }
        set { animator.SetInteger("State", (int)value); }
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        character = FindObjectOfType<Character>();

        boxcoll = GetComponent<BoxCollider2D>();
        origincolloffset = boxcoll.offset.x;
    }

    private void Move()
    {
        groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distanceGround);

        if (groundInfo.collider == false)
        {
            distanceGround = 1.5f;
            groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distanceGround);
            if (groundInfo.collider == false)
            {
                direction.x *= -1f;
                sprite.flipX = !sprite.flipX;
                if (sprite.flipX)
                {
                    boxcoll.offset = new Vector2(flipcolloffset, boxcoll.offset.y);
                    bite.position = new Vector2(transform.position.x - biteDistance, bite.position.y);
                    groundDetection.position = new Vector2(transform.position.x - groundDetectionDistance, groundDetection.position.y);

                }
                else
                {
                    boxcoll.offset = new Vector2(origincolloffset, boxcoll.offset.y);
                    bite.position = new Vector2(transform.position.x + biteDistance, bite.position.y);
                    groundDetection.position = new Vector2(transform.position.x + groundDetectionDistance, groundDetection.position.y);
                }
            }

        }
        else
        {
            direction.x *= -1f;
            sprite.flipX = !sprite.flipX;
            if (sprite.flipX)
            {
                boxcoll.offset = new Vector2(flipcolloffset, boxcoll.offset.y);
                bite.position = new Vector2(transform.position.x - biteDistance, bite.position.y);
                groundDetection.position = new Vector2(transform.position.x - groundDetectionDistance, groundDetection.position.y);
            }
            else
            {
                boxcoll.offset = new Vector2(origincolloffset, boxcoll.offset.y);
                bite.position = new Vector2(transform.position.x + biteDistance, bite.position.y);
                groundDetection.position = new Vector2(transform.position.x + groundDetectionDistance, groundDetection.position.y);
            }
        }
        distanceGround = 1f;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
    }

    //private void Move(Vector3 character)
    //{
    //    Vector3 pos = Vector2.zero;
    //    if (transform.position.x > character.x)
    //    {
    //        pos = new Vector2(character.x + 1f, transform.position.y);
    //    }
    //    else
    //    {
    //        pos = new Vector2(character.x - 1f, transform.position.y);
    //    }
    //    transform.position = Vector2.MoveTowards(transform.position, pos, speed * Time.deltaTime);
    //    if (transform.position == pos)
    //    {
    //        //direction.x = 0;
    //        State = SnakeState.Attack;

    //    }
    //}

    private void Attack()
    {

        Fight2D.Action(bite.position, biteRadius, 9, false);
    }

    private IEnumerator Dying()
    {
        direction.x = 0;
        State = SnakeState.Dying;
        boxcoll.offset = new Vector2(boxcoll.offset.x, sleepcolloffset);
        boxcoll.size = new Vector2(boxcoll.offset.x, sleepcollsize);
        yield return new WaitForSeconds(deathTime);
        ReceiveDamage();
    }

    // Start is called before the first frame update
    void Start()
    {
        direction = transform.right;
        biteDistance = bite.position.x - transform.position.x;
        groundDetectionDistance = groundDetection.position.x - transform.position.x;
    }



    // Update is called once per frame
    void Update()
    {

        if (!isAttacking)
        {
            State = SnakeState.Walking;
            Move();
        }
        if (!character.isDead)
        {
            saw = Physics2D.Linecast(transform.position + transform.up * 0.5f + transform.right * 0.7f, new Vector2(transform.position.x + vision, transform.position.y + 0.5f));
            saw1 = Physics2D.Linecast(transform.position + transform.up * 0.5f - transform.right * 0.7f, new Vector2(transform.position.x - vision, transform.position.y + 0.5f));
            Debug.DrawLine(transform.position + transform.up * 0.5f + transform.right * 0.7f, new Vector2(transform.position.x + vision, transform.position.y + 0.5f));
            Debug.DrawLine(transform.position + transform.up * 0.5f - transform.right * 0.7f, new Vector2(transform.position.x - vision, transform.position.y + 0.5f));
            distance = Vector2.Distance(transform.position, character.transform.position);
            if (distance < vision && character.transform.position.y < transform.position.y + 2.5f && character.transform.position.y >= transform.position.y - 0.01f)
            {
                isAttacking = true;
                if (distance > attackDistance)
                {

                    State = SnakeState.Walking;
                    
                    
                    if (saw.collider == true && saw.collider.tag == "Player")
                    {
                        sprite.flipX = false;
                        boxcoll.offset = new Vector2(origincolloffset, boxcoll.offset.y);
                        bite.position = new Vector2(transform.position.x + biteDistance, bite.position.y);
                        groundDetection.position = new Vector2(transform.position.x + groundDetectionDistance, groundDetection.position.y);
                        groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distanceGround * 1.5f);
                        if (groundInfo.collider == false)
                        {
                            transform.position = this.transform.position;
                            State = SnakeState.idle;
                        }
                        else
                        {
                            transform.position = Vector3.MoveTowards(transform.position, new Vector2(character.transform.position.x - 1, transform.position.y), 1.5f * speed * Time.deltaTime);
                            State = SnakeState.Walking;
                        }

                    }
                    else if (saw1.collider == true && saw1.collider.tag == "Player")
                    {
                        sprite.flipX = true;
                        boxcoll.offset = new Vector2(flipcolloffset, boxcoll.offset.y);
                        bite.position = new Vector2(transform.position.x - biteDistance, bite.position.y);
                        groundDetection.position = new Vector2(transform.position.x - groundDetectionDistance, groundDetection.position.y);
                        groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distanceGround * 1.5f);
                        if (groundInfo.collider == false)
                        {
                            transform.position = this.transform.position;
                            State = SnakeState.idle;
                        }
                        else
                        {
                            transform.position = Vector3.MoveTowards(transform.position, new Vector2(character.transform.position.x + 1, transform.position.y), 1.5f * speed * Time.deltaTime);
                            State = SnakeState.Walking;
                        }
                    }
                    else
                        State = SnakeState.idle;
                    
                }
                else
                    State = SnakeState.Attack;
            }
            else
            {
                if (sprite.flipX)
                {
                    direction.x = -1;
                    boxcoll.offset = new Vector2(flipcolloffset, boxcoll.offset.y);
                }
                else
                {
                    direction.x = 1;
                    boxcoll.offset = new Vector2(origincolloffset, boxcoll.offset.y);
                }
                isAttacking = false;
            }

        }

        if (isDead)
            StartCoroutine(Dying());
    }
}

public enum SnakeState
{
    idle,
    Walking,
    Attack,
    Hit,
    Dying
}
