using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviourPunCallbacks
{
    public GameObject PlayerUIPrefab;

    private Color[] myColors;

    private CustomGameManager gameManager;
    private PlayerStats playerStats;

    void Start()
    {
        SetPlayerColor();
        playerStats = GetComponent<PlayerStats>();
        gameManager = FindObjectOfType<CustomGameManager>();
        if (PlayerUIPrefab != null)
        {
            GameObject _uiGo = Instantiate(PlayerUIPrefab);
            _uiGo.GetComponent<PlayerUI>().SetTarget(this);
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }
    }

    private void SetPlayerColor()
    {
        if (photonView.IsMine)
        {
            myColors = new Color[4];
            myColors[0] = Random.ColorHSV();
            myColors[1] = Random.ColorHSV();
            myColors[2] = Random.ColorHSV();
            myColors[3] = Random.ColorHSV();
            Vector3[] colors = new Vector3[4];
            colors[0] = new Vector3(myColors[0].r, myColors[0].g, myColors[0].b);
            colors[1] = new Vector3(myColors[1].r, myColors[1].g, myColors[1].b);
            colors[2] = new Vector3(myColors[2].r, myColors[2].g, myColors[2].b);
            colors[3] = new Vector3(myColors[3].r, myColors[3].g, myColors[3].b);

            photonView.RPC("ChangeColor", RpcTarget.AllBuffered, colors as object);

            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                foreach (Material mat in renderers[i].materials)
                {
                    mat.color = myColors[i];
                }
            }
        }
        Renderer[] renderers2 = gameObject.GetComponentsInChildren<Renderer>();
        Debug.Log(renderers2.Length);

    }

    void Update()
    {
        if (gameManager.gameStarted)
        {
            if (photonView.IsMine)
            {
                float v = Input.GetAxis("Vertical") * playerStats.speed * Time.deltaTime;
                float h = Input.GetAxis("Horizontal") * playerStats.rotationSpeed * Time.deltaTime;
                // Translate in the forward direction of the first child (chasis)
                transform.Translate(transform.GetChild(0).forward * v);
                // Rotate everything but the last child (turret)
                for (int i = 0; i < transform.childCount - 1; i++)
                {
                    transform.GetChild(i).Rotate(0, h, 0);
                }
            }
        }
#if false
        float v2 = Input.GetAxis("Vertical") * 10 * Time.deltaTime;
        float h2 = Input.GetAxis("Horizontal") * 100 * Time.deltaTime;
        // Translate in the forward direction of the first child (chasis)
        transform.Translate(transform.GetChild(0).forward * v2);
        // Rotate everything but the last child (turret)
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            transform.GetChild(i).Rotate(0, h2, 0);
        }
#endif
    }

    [PunRPC]
    void ChangeColor(Vector3[] colors, PhotonMessageInfo info)
    {
        if (photonView.Owner.Equals(info.Sender)){
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                foreach (Material mat in renderers[i].materials)
                {
                    mat.color = new Color(colors[i].x, colors[i].y, colors[i].z, 1.0f);
                }
            }

            //Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
            //foreach (Renderer r in renderers)
            //{
            //    foreach (Material mat in r.materials)
            //    {
            //        mat.color = new Color(color.x, color.y, color.z, 1.0f);
            //    }
            //}
        }
    }
}
