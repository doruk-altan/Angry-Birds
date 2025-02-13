using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piggy : MonoBehaviour
{
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float damageThreshold = .2f;
    [SerializeField] private GameObject piggyDeathParticles;
    [SerializeField] private AudioClip deathClip;

    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void damagePiggy(float damageAmount)
    {
        currentHealth -= damageAmount;

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.instance.RemovePiggy(this);

        Instantiate(piggyDeathParticles, transform.position, Quaternion.identity);

        AudioSource.PlayClipAtPoint(deathClip, transform.position);

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float impactVelocity = collision.relativeVelocity.magnitude;

        if(impactVelocity > damageThreshold)
        {
            damagePiggy(impactVelocity);
        }
    }
}
