using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{
    private int damage;
    private Transform anchor;

    public void SetDamage(int damageAmount)
    {
        damage = damageAmount;
    }

    public void SetTransform(Transform anchorPoint)
    {
        anchor = anchorPoint;
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
                enemy.TakeDamage(damage);
                Debug.Log($"Sword hit {enemy.gameObject.name} and dealt {damage} damage.");
            }
        }
    }
}
