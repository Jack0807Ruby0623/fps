using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Pun.UtilityScripts;


public class Weapon : MonoBehaviour
{
    public int damage;
    public int pelletsCount = 1;
    public float sprayMultiplier = 0f;

    public Camera camera;
    public float fireRate;


    [Header("VFX")]
    public GameObject hitVFX;

    private float nextFire;



    [Header("Ammo")]
    public int mag = 5; // 弹夹容量
    public int ammo = 30; // 总弹药量
    public int magAmmo = 30; // 弹夹最大弹药量

    [Header("SFX")] public int shootSFXIdex = 0;
    public PlayerPhotonSoundManager playerPhotonSoundManager;


    [Header("UI")]
    public TextMeshProUGUI magText;
    public TextMeshProUGUI ammoText;


    [Header("Animation")]
    public Animation animation;
    public AnimationClip reload;

    [Header("Projectile Weapon Settings")]
    public bool isProjectileWeapon = false;
    public GameObject projectile;
    public Transform projectileExit;

    [Header("Recoil Settings")]


    [Range (0, 2)]
    public float recoverPercent = 0.7f;

    [Space]
    public float recoilUp = 1f;

    public float recoilBack = 0f;



    private Vector3 originalPosition;
    private Vector3 recoilVelocity=Vector3.zero;

    private float recoilLength;
    private float recoverLenth;


    private bool recoiling;
    public bool recovering;

    void Start()
    {
        // 初始化UI显示
        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;


        originalPosition = transform.localPosition;

        recoilLength = 0;
        recoverLenth = 1 / fireRate * recoverPercent;
    }

    // Update is called once per frame
    void Update()
    {
        if (nextFire > 0)
            nextFire -= Time.deltaTime;

        // 射击逻辑
        if (Input.GetButton("Fire1") && nextFire <= 0 && ammo > 0 && animation.isPlaying == false)
        {
            Debug.Log("Firing..."); // 看看射击有没有触发
            nextFire = 1 / fireRate;
            ammo--;

            magText.text = mag.ToString();
            ammoText.text = ammo + "/" + magAmmo;
            if (isProjectileWeapon)
            {
                ProjectileFire();
            }
            else
            {
                Fire();
            }
        }

        // 重装弹药
        if (Input.GetKeyDown(KeyCode.R)&& mag>0)
        {
            Debug.Log("Reloading..."); // 看看重装有没有触发
            Reload();
        }


        if (recoiling)
        {
            Recoil();
        }


        if (recovering)
        {
            Recovering();
        }
    }

    void ProjectileFire()
    {
        GameObject myProjectile= PhotonNetwork.Instantiate(projectile.name, projectileExit.position, projectileExit.rotation);
        myProjectile.GetComponent<Explosive>().isLocalExplosive = true;
    }


    void Reload()
    {

        animation.Play(reload.name);
        // 从总弹药中补充弹夹
        if (mag > 0)
        {
            mag--;
            ammo = magAmmo;
        }

        // 更新UI
        magText.text = mag.ToString();
        ammoText.text = ammo + "/" + magAmmo;
    }

    void Fire()
    {
        recoiling = true;
        recovering = false;

        // 安全检查 playerPhotonSoundManager
        if (playerPhotonSoundManager != null)
            playerPhotonSoundManager.PlayShootSFX(shootSFXIdex);

        for (int i = 0; i < pelletsCount; i++)
        {
            Vector3 sprayOffset = (Random.insideUnitSphere * sprayMultiplier);
            sprayOffset.z = 0;

            Ray ray = new Ray(origin: camera.transform.position, direction: camera.transform.forward + sprayOffset);

            RaycastHit hit;

            if (Physics.Raycast(ray.origin, ray.direction, out hit, maxDistance: 100f))
            {
                PhotonNetwork.Instantiate(hitVFX.name, hit.point, Quaternion.identity);

                // 获取 Health 组件
                Health targetHealth = hit.transform.gameObject.GetComponent<Health>();
                if (targetHealth != null)
                {
                    // 关键：先获取 PhotonView 组件并检查它是否存在
                    PhotonView targetPhotonView = hit.transform.gameObject.GetComponent<PhotonView>();
                    if (targetPhotonView != null)
                    {
                        PhotonNetwork.LocalPlayer.AddScore(damage);

                        if (damage >= targetHealth.health)
                        {
                            // 安全检查 RoomManager.instance
                            if (RoomManager.instance != null)
                            {
                                RoomManager.instance.kills++;
                                RoomManager.instance.SetHashes();
                            }
                            PhotonNetwork.LocalPlayer.AddScore(100);
                        }

                        // 现在可以安全地调用 RPC
                        targetPhotonView.RPC("TakeDamage", RpcTarget.All, damage);
                    }
                }
            }
        }
    }





    

    void Recovering()
    {
        Vector3 finalPosition = originalPosition;

        transform.localPosition =
            Vector3.SmoothDamp(current: transform.localPosition, target: finalPosition, ref recoilVelocity, recoverLenth);

        if (transform.localPosition == finalPosition)
        {
            recoiling = false;
            recovering = false;
        }
    }

    void Recoil()
    {
        Vector3 finalPosition = new Vector3(originalPosition.x, y: originalPosition.y + recoilUp, z: originalPosition.z - recoilBack);

        transform.localPosition =
            Vector3.SmoothDamp(current: transform.localPosition, target: finalPosition, ref recoilVelocity,recoilLength );

        if (transform.localPosition == finalPosition)
        {
            recoiling = false;
            recovering = true;
        }
    }
}
