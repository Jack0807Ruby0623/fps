using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class Health : MonoBehaviourPun
{
    public int health; // "100"
    public bool isLocalPlayer; // Unchanged

    public RectTransform healthBar; // Unchanged
    private float originalHealthBarSize;

    [Header("UI")]
    public TextMeshProUGUI healthText; // Health (TextMeshProUGUI)


    private bool hasDied;

    private void Start()
    {
        originalHealthBarSize = healthBar.sizeDelta.x;
    }

    private void Update()
    {
        // healthBar.sizeDelta = new Vector2(originalHealthBarSize * health / 100f, healthBar.sizeDelta.y);
    }

    [PunRPC]
    public void TakeDamage(int _damage)
    {
        if (hasDied)
            return;

        health -= _damage;

        healthBar.sizeDelta = new Vector2(originalHealthBarSize * health / 100f, healthBar.sizeDelta.y);

        healthText.text = health.ToString();

        if (health <= 0)
        {
            hasDied = true;

            if (isLocalPlayer)
            {
                RoomManager.instance.SpawnPlayer();

                RoomManager.instance.deaths++;
                RoomManager.instance.SetHashes();
            }

            Destroy(gameObject);
        }
    }
}