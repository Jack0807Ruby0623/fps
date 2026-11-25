using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    public GameObject playersHolder; // 用于在Unity编辑器中引用包含所有玩家UI槽位的父对象

    [Header("Options")]
    public float refreshRate = 1f; // 刷新排行榜的频率（秒）

    [Header("UI")]
    public GameObject[] slots; // 存储所有UI槽位的数组
    [Space]
    public TextMeshProUGUI[] scoreTexts; // 存储所有分数文本组件的数组
    public TextMeshProUGUI[] nameTexts; // 存储所有名称文本组件的数组
    public TextMeshProUGUI[] kdTexts;

    // Start is called before the first frame update
    private void Start()
    {
        // 每隔 refreshRate 秒调用一次 Refresh 方法来更新排行榜
        InvokeRepeating(nameof(Refresh), time: 1f, refreshRate);
    }

    // Update is called once per frame
    private void Update()
    {
        // 当玩家按下 Tab 键时，激活或禁用整个排行榜UI
        playersHolder.SetActive(Input.GetKey(KeyCode.Tab));
    }

    // 刷新排行榜的方法
    public void Refresh()
    {
        // 首先，将所有UI槽位设为非激活状态，以便只显示有玩家数据的槽位
        foreach (var slot in slots)
        {
            slot.SetActive(false);
        }

        // 从 PhotonNetwork.PlayerList 中获取所有玩家，并按分数降序排列
        var sortedPlayerList = (
            from player in PhotonNetwork.PlayerList
            orderby player.GetScore() descending
            select player
        ).ToList();

        int i = 0;
        // 遍历排序后的玩家列表
        foreach (var player in sortedPlayerList)
        {
            // 如果当前索引超出了UI槽位的数量，则跳出循环
            if (i >= slots.Length) break;

            // 激活对应的UI槽位
            slots[i].SetActive(true);

            // 如果玩家没有设置昵称，则将其昵称设为 "unnamed"
            if (player.NickName == "")
            {
                player.NickName = "unnamed";
            }

            // 将玩家的昵称和分数赋值给对应的UI文本组件
            nameTexts[i].text = player.NickName;
            scoreTexts[i].text = player.GetScore().ToString();

            if (player.CustomProperties["kills"] != null)
            {
                kdTexts[i].text = player.CustomProperties["kills"] + "/" + player.CustomProperties["deaths"];
            }
            else
            {
                kdTexts[i].text = "0/0";
            }

            // 索引递增，指向下一个UI槽位
            i++;
        }
    }
}