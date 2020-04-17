using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MyBomb : MonoBehaviourPunCallbacks
{
    public PlayerStats myPlayerStats;
    public GameObject explosionPrefab;

    void Start()
    {
        Destroy(gameObject, 4f);
    }

    private void OnDestroy()
    {
        var particle = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        particle.transform.SetParent(null);
    }

    public void Explode()
    {
        photonView.RPC("DestroyInstant", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void DestroyInstant()
    {
        Destroy(gameObject);
    }
}
