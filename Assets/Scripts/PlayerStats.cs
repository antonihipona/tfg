using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class PlayerStats : MonoBehaviourPunCallbacks
{
    public float maxLife = 10;
    public float currentLife;
    public float speed = 5;
    public float rotationSpeed = 100;
    public float shootDamage = 2;
    public float bombDamage = 5;
    public float turretRotationSpeed = 100;
    public float bulletSpeed = 10;
    public float shootCooldown = 2;
    public float bombCooldown = 10;


    private CustomGameManager gameManager;
    private UIGameManager uiGameManager;
    private float cooldownTimer;
    private float cooldownBombTimer;
    private void Start()
    {
        gameManager = FindObjectOfType<CustomGameManager>();
        uiGameManager = FindObjectOfType<UIGameManager>();
        currentLife = maxLife;
        cooldownTimer = 0;
        cooldownBombTimer = 0;
    }

    public bool IsDead()
    {
        return currentLife <= 0f;
    }

    public bool CanShoot()
    {
        return cooldownTimer <= 0;
    }

    public bool CanPlaceBomb()
    {
        return cooldownBombTimer <= 0;
    }

    public void ResetShootCooldown()
    {
        cooldownTimer = shootCooldown;
    }

    private void LateUpdate()
    {
        cooldownTimer -= Time.deltaTime;
        cooldownBombTimer -= Time.deltaTime;
    }

    public void TakeDamage(float damage, Vector3 direction)
    {
        if (!this.IsDead())
        {
            this.RemoveHealth(damage);
            UIDamageManager.instance.InstantiateDamage(damage, this.transform.position);
        }
        if (this.IsDead())
            this.Die(direction);
    }
    private void RemoveHealth(float damage)
    {
        this.currentLife -= damage;
    }
    private void Die(Vector3 point)
    {
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
            var force = (child.position - point).normalized;
            child.gameObject.AddComponent<Rigidbody>();
            child.GetComponent<Rigidbody>().AddForceAtPosition(force * 5, point, ForceMode.Impulse);
            Destroy(child.gameObject, 4);
        }
        float r = 1f, g = 0.4f, b = 0.4f; // Red color
        uiGameManager.endText.color = new Color(r, g, b);
        uiGameManager.endText.text = "DEFEAT";
        Destroy(gameObject);
        gameManager.IncreaseDeadPlayers();
    }

    internal void ResetBombCooldown()
    {
        cooldownBombTimer = bombCooldown;
    }
}
