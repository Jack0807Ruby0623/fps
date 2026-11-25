using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public Movement movement; // Player (Movement)

    public GameObject camera; // Main Camera --- DISABLE

    public string nickname; // Unchanged

    public TextMeshPro nicknameText; // Text (TMP) (TextMeshPro)

    public Transform TPweaponHolder; // Unchanged

    // Frequently called
    public void IsLocalPlayer()
    {
        TPweaponHolder.gameObject.SetActive(false);

        movement.enabled = true;
        camera.SetActive(true);
    }

    [PunRPC]
    public void SetTPWeapon(int _weaponIndex)
    {
        foreach (Transform _weapon in TPweaponHolder)
        {
            _weapon.gameObject.SetActive(false);
        }

        TPweaponHolder.GetChild(_weaponIndex).gameObject.SetActive(true);
    }

    [PunRPC]
    public void SetNickname(string _name)
    {
        nickname = _name;
        nicknameText.text = nickname;
    }
}