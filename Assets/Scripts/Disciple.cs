using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disciple : Unit
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private Color bulletColor = Color.white;

    private Character character;
    private Bullet bullet;
    private Vector3 direction;
    private BoxCollider2D boxcoll;

    public float sleepcolloffset;
    public float sleepcollsize;
    private float origincolloffset;
    public float flipcolloffset;

    public float nextShot = 1f;
    private float nextShotTime = 0f;

    //public float checkTime = 3f;
    //private float chillTime;

    private float distance = 0f;
    public float vision = 10f;
    

    readonly int dieHash = Animator.StringToHash("Dying");

    new private Rigidbody2D rigidbody;

    protected DiscipleState State
    {
        get { return (DiscipleState)animator.GetInteger("State"); }
        set { animator.SetInteger("State", (int)value); }
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        character = FindObjectOfType<Character>();
        bullet = Resources.Load<Bullet>("Bullet");

        boxcoll = GetComponent<BoxCollider2D>();
        origincolloffset = boxcoll.offset.x;
    }

    private void Move()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + transform.up * 0.5f + transform.right * direction.x * 0.5f, 0.1f);
        if (colliders.Length > 0)
            direction.x *= -1f;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
    }

    private void Shoot()
    {
        Vector3 position = transform.position;
        position.x = (sprite.flipX ? position.x += -1f : position.x += 0.5f);
        position.y += 0.3f;
        Bullet newBullet = Instantiate(bullet, position, bullet.transform.rotation) as Bullet;
        newBullet.Parent = gameObject;
        newBullet.Direction = newBullet.transform.right * (sprite.flipX ? -1.0f : 1.0f);
        newBullet.Color = bulletColor;
    }

    private void RateShoot()
    {
        if (!isDead)
            if (nextShotTime == 0 || Time.time > nextShotTime)
            {
                Shoot();
                nextShotTime = Time.time + nextShot;
            }
    }
    
    private IEnumerator Dying()
    {
        direction.x = 0;
        State = DiscipleState.Dying;
        boxcoll.offset = new Vector2(boxcoll.offset.x, sleepcolloffset);
        boxcoll.size = new Vector2(boxcoll.offset.x, sleepcollsize);
        yield return new WaitForSeconds(deathTime);
        ReceiveDamage();
    }

    //private void CheckChar()
    //{
    //    if (distanceToChar > vision)
    //    {

    //        chillTime = Time.time + checkTime;
    //        if (Time.time > chillTime)
    //        {
    //            direction.x = 1f;

    //        }
    //        if (Time.time < chillTime && distanceToChar < vision)
    //        {
    //            RateShoot();
    //            Debug.Log("yay");
    //        }
    //    }
    //}

    // Start is called before the first frame update
    void Start()
    {
        direction = transform.right;

    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        State = DiscipleState.Walking;
        

        Move();
        if (!character.isDead)
        {
            if (transform.position.x > character.transform.position.x)
                distance = transform.position.x - character.transform.position.x;

            else
                distance = character.transform.position.x - transform.position.x;
            if (distance < vision && character.transform.position.y < transform.position.y + 2.5f && character.transform.position.y >= transform.position.y - 0.01f)
            {
                direction.x = 0;
                if (transform.position.x > character.transform.position.x)
                    sprite.flipX = true;
                else
                    sprite.flipX = false;
                State = DiscipleState.Shooting;
                RateShoot();

            }
            else
                if (direction.x == 0)
                direction.x = 1f;
        }
        if (direction.x > 0)
        {
            sprite.flipX = false;
            boxcoll.offset = new Vector2(origincolloffset, boxcoll.offset.y);
        }
        else if (direction.x < 0)
        {
            sprite.flipX = true;
            boxcoll.offset = new Vector2(flipcolloffset, boxcoll.offset.y);
        }
        if (isDead)
            StartCoroutine(Dying());
    }
}

public enum DiscipleState
{
    Walking,
    Shooting,
    Dying
}