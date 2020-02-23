using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class PlayerStats : MonoBehaviour
{
    public float maxLife = 10;
    public float currentLife;
    public float speed = 5;
    public float rotationSpeed = 100;
    public float shootDamage = 2;
    public float bombDamage = 5;
    public float turretRotationSpeed = 100;
    public float bulletSpeed = 5;
    public float shootCooldown = 2;

    private float cooldownTimer;
    private void Start()
    {
        currentLife = maxLife;
        cooldownTimer = 0;
    }

    public void Damage(float damage)
    {
        currentLife -= damage;
    }

    public bool IsDead()
    {
        return currentLife <= 0f;
    }

    public bool CanShoot()
    {
        return cooldownTimer <= 0;
    }

    public void ResetShootCooldown()
    {
        cooldownTimer = shootCooldown;
    }

    private void LateUpdate()
    {
        cooldownTimer -= Time.deltaTime;
    }
}
