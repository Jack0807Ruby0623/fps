using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


public class RoomList : MonoBehaviourPunCallbacks
{
    // 单例模式：确保场景中只有一个 RoomList 实例
    public static RoomList Instance;

    [Header("UI")]
    public Transform roomListParent;
    public GameObject roomListItemPrefab;

    // 用于引用 RoomManager 脚本的公共变量
    public GameObject roomManagerGameobject;
    public RoomManager roomManager;

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();

    public void ChangeRoomToCreateName(string _roomName)
    {
        roomManager.roomNameToJoin = _roomName;
    }


    // 在 Awake 中初始化单例
    private void Awake()
    {
        Instance = this;
    }

    
    // Event function
    IEnumerator Start()
    {
        // Precautions
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }

        yield return new WaitUntil(() => !PhotonNetwork.IsConnected);

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();
    }



    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (cachedRoomList.Count <= 0)
        {
            cachedRoomList = roomList;
        }
        else
        {
            foreach (var room in roomList)
            {
                for (int i = 0; i < cachedRoomList.Count; i++)
                {
                    if (cachedRoomList[i].Name == room.Name)
                    {
                        List<RoomInfo> newList = cachedRoomList;

                        if (room.RemovedFromList)
                        {
                            newList.Remove(newList[i]);
                        }
                        else
                        {
                            newList[i] = room;
                        }

                        cachedRoomList = newList;
                    }
                }
            }
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        foreach (Transform roomItem in roomListParent)
        {
            Destroy(roomItem.gameObject);
        }

        foreach (var room in cachedRoomList)
        {
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListParent);
            roomItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = room.Name;
            roomItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = room.PlayerCount + "/16";

            // 补充：将房间名称传递给按钮脚本
            roomItem.GetComponent<RoomItemButton>().RoomName = room.Name;
        }
    }

    // 更新后的方法：通过房间名称加入房间
    public void JoinRoomByName(string _name)
    {
        // 将要加入的房间名称赋值给 RoomManager 脚本中的变量
        roomManager.roomNameToJoin = _name;
        roomManagerGameobject.SetActive(true);
        gameObject.SetActive(false);
    }
}