using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Pun.UtilityScripts;


public class WeaponSwitcher : MonoBehaviour
{
    public PhotonView playerSetupView;

    // 用于引用武器的动画组件和动画片段
    public Animation _animation;
    public AnimationClip draw;

    private int selectedWeapon = 0;

    // Start is called before the first frame update
    void Start()
    {
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedWeapon = 1;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedWeapon = 2;
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedWeapon = 3;
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            selectedWeapon = 4;
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            selectedWeapon = 5;
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            selectedWeapon = 6;
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            selectedWeapon = 7;
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            selectedWeapon = 8;
        }

        // 通过鼠标滚轮向上滚动来切换到下一把武器
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (selectedWeapon >= transform.childCount - 1)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon += 1;
            }
        }

        // 通过鼠标滚轮向下滚动来切换到上一把武器
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (selectedWeapon <= 0)
            {
                selectedWeapon = transform.childCount - 1;
            }
            else
            {
                selectedWeapon -= 1;
            }
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }

    void SelectWeapon()
    {
        playerSetupView.RPC("SetTPWeapon", RpcTarget.All, selectedWeapon);

        // 安全检查，确保 selectedWeapon 在有效范围内。
        if (selectedWeapon >= transform.childCount)
        {
            selectedWeapon = transform.childCount - 1;
        }

        // 新增：在切换武器前，先停止当前可能正在播放的动画。
        if (_animation != null)
        {
            _animation.Stop();
        }

        int i = 0;
        foreach (Transform _weapon in transform)
        {
            if (i == selectedWeapon)
            {
                _weapon.gameObject.SetActive(true);
                // 播放武器拔出动画
                if (_animation != null && draw != null)
                {
                    _animation.Play(draw.name);
                }
            }
            else
            {
                _weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
