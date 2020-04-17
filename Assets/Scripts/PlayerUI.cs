using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

    public Text playerNameText;
    public Slider playerHealthSlider;

    private PlayerController target;
    private PlayerStats playerStats;

    private float characterControllerHeight = 0f;
    private Transform targetTransform;
    private Renderer targetRenderer;
    private CanvasGroup _canvasGroup;
    private Vector3 targetPosition;

    void Awake()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        _canvasGroup = this.GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (target == null || playerStats.IsDead())
        {
            Destroy(this.gameObject);
            return;
        }
        // Reflect the Player Health
        if (playerHealthSlider != null && playerStats != null)
        {
            playerHealthSlider.value = playerStats.currentLife / playerStats.maxLife;
        }
    }

    void LateUpdate()
    {
        // Do not show the UI if we are not visible to the camera, thus avoid potential bugs with seeing the UI, but not the player itself.
        if (targetRenderer != null)
        {
            this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
        }


        // #Critical
        // Follow the Target GameObject on screen.
        if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y += characterControllerHeight;
            this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
        }
    }

    public void SetTarget(PlayerController _target)
    {
        if (_target == null)
        {
            Destroy(this);
            return;
        }
        // Cache references for efficiency
        target = _target;
        playerStats = target.gameObject.GetComponent<PlayerStats>();
        if (playerNameText != null)
        {
            var playerName = target.photonView.Owner.NickName;
            if (AuthenticationManager.instance.Username.Equals(playerName))
                playerNameText.color = Color.green;
            else
                playerNameText.color = Color.red;

            playerNameText.text = playerName;
        }
        targetTransform = this.target.GetComponent<Transform>();
        targetRenderer = this.target.GetComponent<Renderer>();
        CharacterController characterController = _target.GetComponent<CharacterController>();
        // Get data from the Player that won't change during the lifetime of this Component
        if (characterController != null)
        {
            characterControllerHeight = characterController.height;
        }
    }
}
