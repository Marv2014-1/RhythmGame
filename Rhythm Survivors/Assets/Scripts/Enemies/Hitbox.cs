using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [HideInInspector] public int ArrowDamage;
    private Collider2D hitboxCollider;
    // [SerializeField] Rigidbody2D Hb;
    // private float knockback;

    private int damageAmount;
    // private Animator animator;

    public float knockbackForce = 5f;
    protected Transform playerTransform;


    // private void Start()
    // {
    //     animator = GetComponent<Animator>();

    //     playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

    // }
    private void Awake()
    {
        hitboxCollider = GetComponent<Collider2D>();
        hitboxCollider.enabled = false;  // Ensure the hitbox is initially disabled
    }
    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {

    }
    public void SetKnockback(float knockbackAmount)
    {
        knockbackForce = knockbackAmount;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MeleeEnemy enemy = GetComponentInParent<MeleeEnemy>();
        if (enemy != null)
        {
            damageAmount = enemy.GetAttackDamage();
            // damageAmount = meleeAttack.GetAttackDamage();
        }

        try
        {
            // Ensure you have tagged the player GameObject with "Player"
            if (collision.CompareTag("Player"))
            {
                Vector2 knockbackDirection = (playerTransform.position - transform.position).normalized;
                collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damageAmount, knockbackDirection, knockbackForce);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }
    public void ActivateHitbox()
    {
        hitboxCollider.enabled = true;
    }
    public void DeactivateHitbox()
    {
        hitboxCollider.enabled = false;
    }


}

