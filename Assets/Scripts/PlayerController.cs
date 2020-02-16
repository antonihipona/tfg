using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject PlayerUIPrefab;

    private Color[] myColors;

    private CustomGameManager gameManager;
    private PlayerStats playerStats;

    private Vector3 movement;
    private Vector3 networkPosition;
    private Quaternion networkRotationChasis;
    void Start()
    {
        SetPlayerColor();
        playerStats = GetComponent<PlayerStats>();
        gameManager = FindObjectOfType<CustomGameManager>();
        transform.GetChild(0).rotation = transform.rotation;
        transform.GetChild(1).rotation = transform.rotation;

        if (PlayerUIPrefab != null)
        {
            GameObject _uiGo = Instantiate(PlayerUIPrefab);
            _uiGo.GetComponent<PlayerUI>().SetTarget(this);
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
        if (!photonView.IsMine)
        {
            transform.position = Vector3.MoveTowards(transform.position, networkPosition, Time.deltaTime * playerStats.speed);
            transform.GetChild(0).rotation = Quaternion.RotateTowards(transform.GetChild(0).rotation, networkRotationChasis, Time.deltaTime * playerStats.rotationSpeed);
        }
        if (gameManager.gameStarted)
        {

            if (photonView.IsMine)
            {
                Vector3 oldPosition = transform.position;
                float v = Input.GetAxis("Vertical") * playerStats.speed * Time.deltaTime;
                float h = Input.GetAxis("Horizontal") * playerStats.rotationSpeed * Time.deltaTime;

                // Translate in the forward direction of the first child (chasis)
                if (v >= 0)
                    transform.Translate(transform.GetChild(0).forward * v);
                movement = transform.position - oldPosition;

                // Rotate the firt child (chasis)
                transform.GetChild(0).Rotate(0, h, 0);

            }

        }
    }

    [PunRPC]
    void ChangeColor(Vector3[] colors, PhotonMessageInfo info)
    {
        if (photonView.Owner.Equals(info.Sender))
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                foreach (Material mat in renderers[i].materials)
                {
                    mat.color = new Color(colors[i].x, colors[i].y, colors[i].z, 1.0f);
                }
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.GetChild(0).rotation); // Chasis
            stream.SendNext(movement);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotationChasis = (Quaternion)stream.ReceiveNext(); // Chasis
            movement = (Vector3)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            networkPosition += (movement * lag);
        }
    }
}
