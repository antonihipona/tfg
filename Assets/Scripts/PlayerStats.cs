using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

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

    public void TakeDamage(float damage, Vector3 direction)
    {
        if (!this.IsDead())
            this.currentLife -= damage;
        if (this.IsDead())
            this.Die(direction);
    }

    private void Die(Vector3 point)
    {
        Debug.LogWarning("local execution");
        var rb = GetComponent<Rigidbody>();
        rb.constraints = 0;
        var colliders = GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }
        var children = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i));
        }
        foreach (var child in children)
        {
            child.SetParent(null);
            point.y = -0.5f;
            var force = child.position - point;
            child.gameObject.AddComponent<Rigidbody>();
            child.GetComponent<Rigidbody>().AddForce(force * 5, ForceMode.Impulse);
            Destroy(child.gameObject, 4);
        }
        Destroy(gameObject, 4);
    }
}
