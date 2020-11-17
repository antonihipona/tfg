using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MyBomb : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector] public PlayerStats myPlayerStats;
    public GameObject explosionPrefab;

    private float activateCooldown = 2f;
    private Collider bombCollider;
    private Material[] mats;
    private bool activated;
    void Start()
    {
        activated = false;
        mats = gameObject.GetComponentInChildren<MeshRenderer>().materials;
        foreach (Material mat in mats)
        {
            Color color = mat.color;
            color.a = 0.5f;
            mat.color = color;
        }

        bombCollider = GetComponent<Collider>();
        bombCollider.enabled = false;
        Destroy(gameObject, 10f);
    }

    private void Update()
    {
        activateCooldown -= Time.deltaTime;

        if (activateCooldown <= 0 && !activated)
        {
            activated = true;
            bombCollider.enabled = true;
            foreach (Material mat in mats)
            {
                Color color = mat.color;
                color.a = 1f;
                mat.color = color;
            }
        }
    }

    private void OnDestroy()
    {
        var particle = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        particle.transform.SetParent(null);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
        }
    }
}
