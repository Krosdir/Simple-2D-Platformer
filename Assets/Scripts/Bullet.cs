using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject parent;
    public GameObject Parent { get { return parent; } set { parent = value; } }

    private float speed = 8f;
    private Vector3 direction;
    public Vector3 Direction { get { return direction; } set { direction = value; } }

    private RaycastHit2D hit;

    private SpriteRenderer sprite;
    public Color Color
    { set { sprite.color = value; } }

    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        tag = "Unit";
        Destroy(gameObject, 1.65f);
    }

    // Update is called once per frame
    void Update()
    {
        //hit = Physics2D.Raycast(transform.position + transform.up * 0.5f + transform.right * direction.x * 0.5f, direction);
        //if (hit.transform.tag != tag)
        //{
        //    Debug.Log(hit.transform.name);
        //    Destroy(gameObject);
        //}

        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);
        sprite.flipX = Direction.x < 0.1f;
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Unit unit = collider.GetComponent<Unit>();
        if ((unit && unit.gameObject != Parent) || collider.gameObject.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }

    

    
}

