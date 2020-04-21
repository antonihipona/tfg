using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject PlayerUIPrefab;
    public GameObject bulletPrefab;
    public GameObject bombPrefab;

    private Color[] myColors;

    private CustomGameManager gameManager;
    private PlayerStats playerStats;

    private Vector3 networkPosition;
    private Quaternion networkRotationChasis;
    private Quaternion networkRotationTurret;
    private Vector3 turretRotationPoint;
    private Transform chasis;
    private Transform turret;
    private Transform bombSpawn;

    new private Rigidbody rigidbody;

    float currentTime = 0;
    double currentPacketTime = 0;
    double lastPacketTime = 0;
    Vector3 positionAtLastPacket = Vector3.zero;
    Quaternion networkRotationChasisAtLastPacket = Quaternion.identity;
    Quaternion networkRotationTurretAtLastPacket = Quaternion.identity;

    public GameObject _uiGo;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        networkPosition = rigidbody.position;
        chasis = transform.GetChild(0);
        turret = transform.GetChild(1);
        bombSpawn = chasis.GetChild(3);
        chasis.rotation = transform.rotation;
        turret.rotation = transform.rotation;
        bombSpawn.rotation = transform.rotation;
    }

    void Start()
    {
        SetPlayerColor();
        playerStats = GetComponent<PlayerStats>();
        gameManager = FindObjectOfType<CustomGameManager>();

        if (PlayerUIPrefab != null)
        {
            _uiGo = Instantiate(PlayerUIPrefab);
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
    }

    void Update()
    {
        if (!photonView.IsMine && !playerStats.IsDead())
        {
            double timeToReachGoal = currentPacketTime - lastPacketTime;
            currentTime += Time.deltaTime;
            // We can try to add some motion to compensate lag
            rigidbody.position = Vector3.Lerp(positionAtLastPacket, networkPosition, (float)(currentTime / timeToReachGoal));
            chasis.rotation = Quaternion.Lerp(networkRotationChasisAtLastPacket, networkRotationChasis, (float)(currentTime / timeToReachGoal));
            turret.rotation = Quaternion.Lerp(networkRotationTurretAtLastPacket, networkRotationTurret, (float)(currentTime / timeToReachGoal));
        }
        if (gameManager.gameStarted && !gameManager.gameEnded)
        {
            if (photonView.IsMine && !playerStats.IsDead())
            {
                MovementAndRotation();
                Shoot();
                PlaceBomb();
            }
        }
    }

    private void MovementAndRotation()
    {
        float v = Input.GetAxis("Vertical") * playerStats.speed * Time.deltaTime;
        float h = Input.GetAxis("Horizontal") * playerStats.rotationSpeed * Time.deltaTime;


        rigidbody.MovePosition(rigidbody.position + chasis.forward * v);

        var chasisRot = chasis.rotation;
        chasisRot *= Quaternion.Euler(0, h, 0);
        chasis.rotation = chasisRot;
        rigidbody.rotation = Quaternion.Euler(0, 0, 0);

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            turretRotationPoint = hit.point;
        }

        Vector3 targetDirection = turretRotationPoint - transform.position;
        float singleStep = playerStats.turretRotationSpeed * Time.deltaTime;

        targetDirection.y = 0f;
        Vector3 newDirection = Vector3.RotateTowards(turret.forward, targetDirection, singleStep, 0.0f);
        turret.rotation = Quaternion.RotateTowards(turret.rotation, Quaternion.LookRotation(newDirection), Time.deltaTime * playerStats.turretRotationSpeed);
    }

    private void Shoot()
    {
        if (Input.GetMouseButton(0))
        {
            if (playerStats.CanShoot())
            {
                // Check if we are too close to a wall
                if (!TooCloseToAWall()){
                    var bullet = PhotonNetwork.Instantiate(this.bulletPrefab.name, turret.GetChild(0).position, Quaternion.identity, 0);
                    bullet.GetComponent<MyBullet>().myPlayerStats = playerStats;
                    bullet.GetComponent<MyBullet>().InitializeBullet(turret.rotation, playerStats.bulletSpeed);
                    playerStats.ResetShootCooldown();
                }
            }
        }
    }

    private bool TooCloseToAWall()
    {
        Vector3 startPos = turret.position;
        Vector3 dir = turret.forward;
        float len = 1.7f;
        RaycastHit hit;
        if (Physics.Raycast(startPos, dir, out hit, len))
        {
            if (hit.collider.CompareTag("Level"))
                return true;
        }
        return false;
    }

    private void PlaceBomb()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (playerStats.CanPlaceBomb())
            {
                var bomb = PhotonNetwork.Instantiate(this.bombPrefab.name, bombSpawn.position, Quaternion.identity, 0);
                bomb.GetComponent<MyBomb>().myPlayerStats = playerStats;
                playerStats.ResetBombCooldown();
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
        if (playerStats != null && playerStats.IsDead())
            return;
        if (stream.IsWriting)
        {
            stream.SendNext(rigidbody.position);
            stream.SendNext(chasis.rotation);
            stream.SendNext(turret.rotation);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotationChasis = (Quaternion)stream.ReceiveNext();
            networkRotationTurret = (Quaternion)stream.ReceiveNext();

            //Lag compensation
            currentTime = 0.0f;
            lastPacketTime = currentPacketTime;
            currentPacketTime = info.SentServerTime;
            positionAtLastPacket = rigidbody.position;
            networkRotationChasisAtLastPacket = chasis.rotation;
            networkRotationTurretAtLastPacket = turret.rotation;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameManager == null || !gameManager.gameStarted || gameManager.gameEnded || playerStats.IsDead())
            return;
        if (collision.transform.CompareTag("Bullet"))
        {
            Vector3 point = Vector3.zero;
            float shootDamage = 0;
            MyBullet bullet = collision.transform.GetComponent<MyBullet>();
            if (bullet != null && bullet.myPlayerStats != null)
            {
                shootDamage = bullet.myPlayerStats.shootDamage;
                point = collision.GetContact(0).point;
                photonView.RPC("TakeDamage", RpcTarget.AllBuffered, shootDamage, point);
                PhotonNetwork.Destroy(bullet.photonView);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameManager == null || !gameManager.gameStarted || gameManager.gameEnded || playerStats.IsDead())
            return;
        if (other.transform.CompareTag("Land Mine"))
        {
            Vector3 point = Vector3.zero;
            float bombDamage = 0;
            MyBomb bomb = other.transform.GetComponent<MyBomb>();
            if (bomb != null && bomb.myPlayerStats != null)
            {
                bombDamage = bomb.myPlayerStats.bombDamage;
                point = other.transform.position;
                photonView.RPC("TakeDamage", RpcTarget.AllBuffered, bombDamage, point);
                PhotonNetwork.Destroy(bomb.photonView);
            }
        }
    }

    [PunRPC]
    void TakeDamage(float damage, Vector3 point, PhotonMessageInfo info)
    {
        playerStats.TakeDamage(damage, point);
    }

    public void Leave()
    {
        photonView.RPC("LeaveGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void LeaveGame(PhotonMessageInfo info)
    {
        if (!photonView.IsMine)
            gameManager.CheckGameEnded(true);
    }
}
