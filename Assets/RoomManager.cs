using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;



    public GameObject player;
    [Space]
    public Transform[] spawnPoints;

    [Space]
    public GameObject roomCam;

    [Space]
    public GameObject nameUI;
    public GameObject connectingUI; 

    private string nickname = "unnamed";
    public string roomNameToJoin = "test";

    [HideInInspector]
    public int kills = 0; // Unchanged
    [HideInInspector]
    public int deaths = 0; // Unchanged


    void Awake()
    {
        instance = this;
    }

    public void ChangeNickname(string _name)
    {
        Debug.Log("HERE");
        nickname = _name;
    }

    public void JoinRoomButtonPressed()
    {
        Debug.Log(message: "Connecting..");
        PhotonNetwork.JoinOrCreateRoom(roomNameToJoin, null, null);

        nameUI.SetActive(false);
        connectingUI.SetActive(true);
        
    }



    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log(message: "We're connected and in a room!");


        roomCam.SetActive(false);

        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0,spawnPoints.Length)];


        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer();
        _player.GetComponent<Health>().isLocalPlayer = true;

        _player.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, nickname);
        

        PhotonNetwork.LocalPlayer.NickName = nickname;
        Debug.Log(nickname);
    }

    public void SetHashes()
    {
        try
        {
            Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;

            // 将 RoomManager 中的 kills 和 deaths 值存入哈希表
            hash["kills"] = kills;
            hash["deaths"] = deaths;

            // 使用 SetCustomProperties 方法将更新后的哈希表发送到服务器
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        catch
        {
            // 如果发生错误，可以在这里添加日志记录或其他错误处理逻辑
            Debug.LogError("Failed to set custom properties.");
        }
    }
}
