using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class Explosive : MonoBehaviour
{
    [HideInInspector]
    public bool isLocalExplosive = false;

    private bool alreadyExploded = false;

    // 爆炸属性
    [Header("Stats")]
    public float explosionRadius = 5f; // 爆炸半径
    public int damage = 30; // 造成的伤害值

    // 发射设置
    [Header("Fire Settings")]
    public float fireForce; // 发射时的初始推力

    // 视觉特效
    [Header("VFX")]
    public GameObject exposalVFX; // 爆炸时播放的视觉特效预制体

    // 事件函数：当游戏对象被实例化后立即调用
    private void Start()
    {
        // 给爆炸物添加一个向前的初始速度
        GetComponent<Rigidbody>().AddForce(transform.forward * fireForce);
    }

    // 事件函数：当此对象与其他碰撞器发生碰撞时调用
    private void OnCollisionEnter(Collision other)
    {
        if (!isLocalExplosive)
            return;


        // 实例化爆炸视觉特效
        PhotonNetwork.Instantiate(exposalVFX.name, transform.position, Quaternion.identity);

        // 调用爆炸逻辑
        Explode();
    }

    // 爆炸的核心逻辑
    void Explode()
    {
        if (alreadyExploded)
            return;


        alreadyExploded = true;

        // 使用 Physics.OverlapSphere 检测爆炸半径内所有碰撞器
        foreach (var collider in Physics.OverlapSphere(transform.position, explosionRadius))
        {
            // 检查该碰撞器所属的游戏对象上是否有 Health 组件
            if (collider.transform.gameObject.GetComponent<Health>()&& collider.transform.gameObject.GetComponent<Health>().isLocalPlayer == false)
            {
                // 如果有，给本地玩家增加与伤害值相等的分数
                PhotonNetwork.LocalPlayer.AddScore(damage);

                // 检查伤害值是否大于或等于目标的生命值
                if (damage >= collider.transform.gameObject.GetComponent<Health>().health)
                {
                    // 如果是，则视为“击杀”
                    RoomManager.instance.kills++; // 增加击杀计数
                    RoomManager.instance.SetHashes(); // 更新房间状态（可能用于同步）
                    PhotonNetwork.LocalPlayer.AddScore(100); // 击杀额外奖励100分
                }

                // 通过 RPC 调用，向网络上的所有客户端广播，让目标受到伤害
                collider.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
            }
        }

        PhotonNetwork.Destroy(gameObject);
    }
}