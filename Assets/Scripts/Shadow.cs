using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : Unit
{
    private Character character;
    private BoxCollider2D boxcoll;

    private Tentacle tentacle;

    private float sleepcollsize;
    private float sleepcolloffset;
    public float origincollsize;
    public float origincolloffset;

    public float nextShot = 1f;
    private float nextShotTime = 0f;

    private float distance = 0f;
    public float vision = 10f;

    private bool isAppeared = false;

    private RaycastHit2D hit;

    protected ShadowState State
    {
        get { return (ShadowState)animator.GetInteger("State"); }
        set { animator.SetInteger("State", (int)value); }
    }

    readonly int appearHash = Animator.StringToHash("Appearence");
    int idleHash = Animator.StringToHash("idle");
    readonly int disappHash = Animator.StringToHash("Disappearance");

    private void Awake()
    {
        character = FindObjectOfType<Character>();
        animator = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        tentacle = Resources.Load<Tentacle>("Tentacle");

        boxcoll = GetComponent<BoxCollider2D>();
        sleepcolloffset = boxcoll.offset.y;
        sleepcollsize = boxcoll.size.y;
    }

    private void Shoot()
    {
        Vector3 position = transform.position;
        position.x = character.transform.position.x;
        position.y = character.transform.position.y - 0.05f;
        hit = Physics2D.Raycast(position, -Vector3.up);
        if (hit)
        {
            position = hit.transform.position;
            position.x = character.transform.position.x;
            position.y += 1f;
        }

        Tentacle newTentacle = Instantiate(tentacle, position, tentacle.transform.rotation) as Tentacle;
        newTentacle.Parent = gameObject;
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

    private IEnumerator Appear()
    {
        if (!isAppeared)
        {
            animator.Play(appearHash);
            yield return new WaitForSeconds(1.2f);
            isAppeared = true;
        }
        else
        {
            animator.Play(idleHash);
            boxcoll.size = new Vector2(boxcoll.size.x, origincollsize);
            boxcoll.offset = new Vector2(boxcoll.offset.x, origincolloffset);
            RateShoot();
        }
    }

    private IEnumerator Dying()
    {
        idleHash = disappHash;
        animator.Play(disappHash);
        boxcoll.offset = new Vector2(boxcoll.offset.x, sleepcolloffset);
        boxcoll.size = new Vector2(boxcoll.offset.x, sleepcollsize);
        yield return new WaitForSeconds(deathTime);
        ReceiveDamage();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!character.isDead)
        {
            if (transform.position.x > character.transform.position.x)
            {
                sprite.flipX = false;
                distance = transform.position.x - character.transform.position.x;
            }
            else
            {
                sprite.flipX = true;
                distance = character.transform.position.x - transform.position.x;
            }

            //Debug.Log(appearHash);

            if (distance < vision)
            {

                StartCoroutine(Appear());


            }
            if (distance > vision)
            {
                isAppeared = false;
                boxcoll.size = new Vector2(boxcoll.size.x, sleepcollsize);
                boxcoll.offset = new Vector2(boxcoll.offset.x, sleepcolloffset);
            }
        }
        if (isDead)
            StartCoroutine(Dying());
    }
}

public enum ShadowState
{
    Appearence,
    idle,
    Disappearance
}
