using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBullet : MonoBehaviourPunCallbacks, IPunObservable
{
    public PlayerStats myPlayerStats;
    public GameObject explosionPrefab;

    private float speed;


    private Vector3 networkPosition;
    private Quaternion networkRotation;

    float currentTime = 0;
    double currentPacketTime = 0;
    double lastPacketTime = 0;
    Vector3 positionAtLastPacket = Vector3.zero;
    Quaternion networkRotationAtLastPacket = Quaternion.identity;
    void Start()
    {
        Destroy(gameObject, 2f);
    }

    public void InitializeBullet(Quaternion rotation, float speed)
    {
        transform.rotation = rotation;
        this.speed = speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var hitNormal = collision.GetContact(0).normal;
        Vector3 r = Vector3.Reflect(transform.forward, hitNormal);
        transform.rotation = Quaternion.LookRotation(r);
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            double timeToReachGoal = currentPacketTime - lastPacketTime;
            currentTime += Time.deltaTime;
            transform.position = Vector3.Lerp(positionAtLastPacket, networkPosition, (float)(currentTime / timeToReachGoal));
            transform.rotation = networkRotation;
        }
        transform.position += transform.forward * speed * Time.deltaTime;
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
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();

            //Lag compensation
            currentTime = 0.0f;
            lastPacketTime = currentPacketTime;
            currentPacketTime = info.SentServerTime;
            positionAtLastPacket = transform.position;
            networkRotationAtLastPacket = transform.rotation;
        }
    }
}
