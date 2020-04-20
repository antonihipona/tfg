using System;
using UnityEngine;
using UnityEngine.UI;

public enum PowerUpType { Speed, ShootSpeed, FireRate, Invisibility };

public class PowerUp : MonoBehaviour
{
    public PowerUpType type { get; private set; }
    public bool isActive;

    private SpriteRenderer spriteRenderer;
    private PowerUpManager powerUpManager;
    private void Start()
    {
        powerUpManager = FindObjectOfType<PowerUpManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Spawn(PowerUpType type)
    {
        this.type = type;
        switch (type)
        {
            case PowerUpType.Speed:
                this.spriteRenderer.sprite = powerUpManager.speedPowerUpSprite;
                break;
            case PowerUpType.ShootSpeed:
                this.spriteRenderer.sprite = powerUpManager.shootSpeedPowerUpSprite;
                break;
            case PowerUpType.FireRate:
                this.spriteRenderer.sprite = powerUpManager.fireRatePowerUpSprite;
                break;
            case PowerUpType.Invisibility:
                this.spriteRenderer.sprite = powerUpManager.invisibilityPowerUpSprite;
                break;
            default:
                break;
        }
    }

}
