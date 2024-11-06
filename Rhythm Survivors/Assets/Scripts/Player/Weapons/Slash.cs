using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{
    private int damage;
    private float knockback;
    private Transform anchor;
    private float size;

    public void SetDamage(int damageAmount)
    {
        damage = damageAmount;
    }

    public void SetKnockback(float knockbackAmount)
    {
        knockback = knockbackAmount;
    }

    public void SetTransform(Transform anchorPoint)
    {
        anchor = anchorPoint;
    }

    public void SetSize(float sizeAmount)
    {
        size = sizeAmount;
    }

    private void Start()
    {
        transform.localScale = new Vector3 (anchor.localScale.x * size / 10, anchor.localScale.y * size / 10, anchor.localScale.z);
    }

    private void Update()
    {
        transform.position = anchor.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the arrow hit an enemy
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if (enemy != null)
            {
                Vector2 direction = (enemy.transform.position - transform.position).normalized;
                enemy.Knockback(direction, knockback);
                enemy.TakeDamage(damage);
                Debug.Log($"Sword hit {enemy.gameObject.name} and dealt {damage} damage.");
            }
        }
    }
}
