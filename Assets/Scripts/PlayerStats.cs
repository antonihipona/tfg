using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class PlayerStats : MonoBehaviourPunCallbacks
{
    #region Public Stats
    public float maxLife = 10;
    public float currentLife = 10;
    public float speed = 5;
    public float rotationSpeed = 100;
    public float shootDamage = 2;
    public float bombDamage = 5;
    public float turretRotationSpeed = 100;
    public float bulletSpeed = 10;
    public float shootCooldown = 2;
    public float bombCooldown = 10;
    #endregion

    #region Back-Up Stats
    private float originalSpeed;
    private float originalRotationSpeed;
    private float originalFireRate;
    private float originalShootSpeed;
    #endregion

    #region Power Ups Multipliers
    private float speedMultiplier = 1.4f;
    private float rotationSpeedMultiplier = 1.4f;
    private float fireRateMultiplier = 2f;
    private float shootSpeedMultiplier = 1.4f;
    #endregion

    #region Power Ups Duration
    private float speedPowerUpDuration = 10;
    private float fireRatePowerUpDuration = 10;
    private float shootSpeedPowerUpDuration = 10;
    private float invisibilityPowerUpDuration = 10;
    #endregion

    #region Timers
    private float cooldownShootTimer;
    private float cooldownBombTimer;
    private float speedPowerUpTimer;
    private float fireRatePowerUpTimer;
    private float shootSpeedPowerUpTimer;
    private float invisibilityPowerUpTimer;
    #endregion

    private CustomGameManager gameManager;
    private UIGameManager uiGameManager;
    private MeshRenderer[] meshes;

    private void Start()
    {
        // Back-up stats
        originalSpeed = speed;
        originalRotationSpeed = rotationSpeed;
        originalFireRate = shootCooldown;
        originalShootSpeed = bulletSpeed;
        // -----------

        gameManager = FindObjectOfType<CustomGameManager>();
        uiGameManager = FindObjectOfType<UIGameManager>();
        meshes = GetComponentsInChildren<MeshRenderer>();
        currentLife = maxLife;
    }

    public bool IsDead()
    {
        return currentLife <= 0f;
    }

    public bool CanShoot()
    {
        return cooldownShootTimer <= 0;
    }

    public bool CanPlaceBomb()
    {
        return cooldownBombTimer <= 0;
    }

    public void ResetShootCooldown()
    {
        cooldownShootTimer = shootCooldown;
    }

    private void LateUpdate()
    {
        // Decrease timers
        float dt = Time.deltaTime;
        cooldownShootTimer -= dt;
        cooldownBombTimer -= dt;
        speedPowerUpTimer -= dt;
        fireRatePowerUpTimer -= dt;
        shootSpeedPowerUpTimer -= dt;
        invisibilityPowerUpTimer -= dt;
        // ------------

        if (!InvisibilityPowerUpActive())
        {
            SetMeshAlpha(1f);
            GetComponent<PlayerController>()._uiGo.SetActive(true);
        }
        if (uiGameManager != null && photonView.IsMine)
        {
            // Deactivate powerups
            if (!SpeedPowerUpActive() && uiGameManager.speedPowerUp.activeSelf)
            {
                uiGameManager.speedPowerUp.SetActive(false);
                speed = originalSpeed;
                rotationSpeed = originalRotationSpeed;
            }
            if (!FireRatePowerUpActive() && uiGameManager.fireRatePowerUp.activeSelf)
            {
                uiGameManager.fireRatePowerUp.SetActive(false);
                shootCooldown = originalFireRate;
            }
            if (!ShootSpeedPowerUpActive() && uiGameManager.shootSpeedPowerUp.activeSelf)
            {
                uiGameManager.shootSpeedPowerUp.SetActive(false);
                bulletSpeed = originalShootSpeed;
            }
            if (!InvisibilityPowerUpActive() && uiGameManager.invisibilityPowerUp.activeSelf)
            {
                uiGameManager.invisibilityPowerUp.SetActive(false);
            }
            // ---------------------

            // Activate powerups
            if (SpeedPowerUpActive() && !uiGameManager.speedPowerUp.activeSelf)
            {
                uiGameManager.speedPowerUp.SetActive(true);
            }
            if (FireRatePowerUpActive() && !uiGameManager.fireRatePowerUp.activeSelf)
            {
                uiGameManager.fireRatePowerUp.SetActive(true);
            }
            if (ShootSpeedPowerUpActive() && !uiGameManager.shootSpeedPowerUp.activeSelf)
            {
                uiGameManager.shootSpeedPowerUp.SetActive(true);
            }
            if (InvisibilityPowerUpActive() && !uiGameManager.invisibilityPowerUp.activeSelf)
            {
                uiGameManager.invisibilityPowerUp.SetActive(true);
            }
            // ------------------
            // Update images
            uiGameManager.shootCooldown.fillAmount = 1 - Mathf.Max(0, cooldownShootTimer / shootCooldown);
            uiGameManager.bombCooldown.fillAmount = 1 - Mathf.Max(0, cooldownBombTimer / bombCooldown);
            uiGameManager.speedPowerUp.GetComponent<Image>().fillAmount = Mathf.Max(0, speedPowerUpTimer / speedPowerUpDuration);
            uiGameManager.fireRatePowerUp.GetComponent<Image>().fillAmount = Mathf.Max(0, fireRatePowerUpTimer / fireRatePowerUpDuration);
            uiGameManager.shootSpeedPowerUp.GetComponent<Image>().fillAmount = Mathf.Max(0, shootSpeedPowerUpTimer / shootSpeedPowerUpDuration);
            uiGameManager.invisibilityPowerUp.GetComponent<Image>().fillAmount = Mathf.Max(0, invisibilityPowerUpTimer / invisibilityPowerUpDuration);
            // -------
        }
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("PowerUp"))
        {
            PowerUp powerUp = other.transform.GetComponent<PowerUp>();
            if (powerUp != null)
            {
                powerUp.Deactivate();
                BoostStats(powerUp.type);
            }
        }
    }

    internal void ResetBombCooldown()
    {
        cooldownBombTimer = bombCooldown;
    }

    private void BoostStats(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.Speed:
                speedPowerUpTimer = speedPowerUpDuration;
                if (!SpeedPowerUpActive())
                {
                    speed *= speedMultiplier;
                    rotationSpeed *= rotationSpeedMultiplier;
                }
                break;
            case PowerUpType.ShootSpeed:
                shootSpeedPowerUpTimer = shootSpeedPowerUpDuration;
                if (!ShootSpeedPowerUpActive())
                {
                    bulletSpeed *= shootSpeedMultiplier;
                }
                break;
            case PowerUpType.FireRate:
                fireRatePowerUpTimer = fireRatePowerUpDuration;
                if (!FireRatePowerUpActive())
                {
                    shootCooldown /= fireRateMultiplier;
                }
                break;
            case PowerUpType.Invisibility:
                invisibilityPowerUpTimer = invisibilityPowerUpDuration;
                if (!photonView.IsMine)
                {
                    SetMeshAlpha(0f);
                    GetComponent<PlayerController>()._uiGo.SetActive(false);
                }
                else
                {
                    SetMeshAlpha(0.5f);
                }

                break;
            default:
                break;
        }
    }

    private void SetMeshAlpha(float alpha)
    {
        foreach (MeshRenderer mesh in meshes)
        {
            foreach (Material mat in mesh.materials)
            {
                Color c = mat.color;
                c.a = alpha;
                mat.color = c;
            }
        }
    }

    private bool SpeedPowerUpActive()
    {
        return originalSpeed != speed && speedPowerUpTimer > 0;
    }

    private bool FireRatePowerUpActive()
    {
        return originalFireRate != shootCooldown && fireRatePowerUpTimer > 0;
    }

    private bool ShootSpeedPowerUpActive()
    {
        return originalShootSpeed != bulletSpeed && shootSpeedPowerUpTimer > 0;
    }

    private bool InvisibilityPowerUpActive()
    {
        return invisibilityPowerUpTimer > 0;
    }
}
