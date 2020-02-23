using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBullet : MonoBehaviourPunCallbacks
{
    public GameObject explosionPrefab;
    private float speed;
    void Start()
    {
        Destroy(gameObject, 10f);
    }

    public void InitializeBullet(Quaternion rotation, float speed)
    {
        transform.rotation = rotation;
        this.speed = speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            photonView.RPC("DestroyInstant", RpcTarget.AllBuffered);
            return;
        }
        else
        {
            var hitNormal = collision.GetContact(0).normal;
            Vector3 r = Vector3.Reflect(transform.forward, hitNormal);
            transform.rotation = Quaternion.LookRotation(r);
        }
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnDestroy()
    {
        var particle = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        particle.transform.SetParent(null);
    }

    [PunRPC]
    void DestroyInstant()
    {
        Destroy(gameObject);
    }
}
