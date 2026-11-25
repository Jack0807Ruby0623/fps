using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomItemButton : MonoBehaviour
{
    // 用于存储该按钮所代表的房间名称
    public string RoomName;

    // 当按钮被按下时调用的方法
    public void OnButtonPressed()
    {
        // 通过单例访问 RoomList，并调用其 JoinRoomByName 方法来加入房间
        RoomList.Instance.JoinRoomByName(RoomName);
    }
}
