using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
    private GameObject parent;
    public GameObject Parent { get { return parent; } set { parent = value; } }

    private Shadow shadow;
    private SpriteRenderer sprite;
    private BoxCollider2D boxcoll;
    private Animator animator;

    private bool isGrounded = false;

    private float sleepcollsize;
    private float sleepcolloffset;
    public float origincollsize;
    public float origincolloffset;

    readonly int idleHash = Animator.StringToHash("idle");

    private float lifeTime = 3.2f;

    private void Awake()
    {
        shadow = FindObjectOfType<Shadow>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();

        boxcoll = GetComponent<BoxCollider2D>();
        sleepcolloffset = boxcoll.offset.y;
        sleepcollsize = boxcoll.size.y;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Unit unit = collider.GetComponent<Unit>();
        if (unit && unit.gameObject != Parent)
        {
            unit.ReceiveDamage();
        }
    }

    private void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.2f);

        isGrounded = colliders.Length > 1;
        Vector3 position = transform.position;
        
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    // Start is called before the first frame update
    void Start()
    {
        sprite.flipX = (shadow.transform.position.x > transform.position.x ? false : true);
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        

        if (stateInfo.shortNameHash == idleHash)
        {
            boxcoll.size = new Vector2(boxcoll.size.x, origincollsize);
            boxcoll.offset = new Vector2(boxcoll.offset.x, origincolloffset);
        }
        else
        {
            boxcoll.size = new Vector2(boxcoll.size.x, sleepcollsize);
            boxcoll.offset = new Vector2(boxcoll.offset.x, sleepcolloffset);
        }
    }
}
