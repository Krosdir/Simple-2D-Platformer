using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int lives = 5;
    public float HP = 100;
    public float deathTime = 1.35f;
    public bool isDead = false;

    protected Animator animator;
    public SpriteRenderer sprite;
    public virtual void ReceiveDamage()
    {
        Die();
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }

    protected virtual void  OnTriggerEnter2D(Collider2D collider)
    {
        Bullet bullet = collider.GetComponent<Bullet>();
        if (bullet && gameObject != bullet.Parent)
            isDead = true;

        Character character = collider.GetComponent<Character>();
        if (character)
        {
            character.ReceiveDamage();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
