using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WebSocketSharp;
using Photon.Pun;

public class GameChat : MonoBehaviour
{
    public TextMeshProUGUI chatText; // 聊天显示文本组件
    public TMP_InputField inputField; // 输入框组件

    private bool isInputFieldToggled; // 标记输入框是否处于激活状态

    void Update()
    {
        // 按下 'Y' 键来打开输入框
        if (Input.GetKeyDown(KeyCode.Y) && !isInputFieldToggled)
        {
            isInputFieldToggled = true;
            inputField.Select(); // 选中输入框
            inputField.ActivateInputField(); // 激活输入框，使其可以接收输入
            Debug.Log("Toggled on");
        }

        // 按下 'Escape' 键来关闭输入框
        if (Input.GetKeyDown(KeyCode.Escape) && isInputFieldToggled)
        {
            isInputFieldToggled = false;
            // 取消选中当前的 UI 元素
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            Debug.Log("Toggled off");
        }

        // 发送消息的逻辑：当按下回车键或小键盘回车键，且输入框处于激活状态且有内容时
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && isInputFieldToggled && !string.IsNullOrEmpty(inputField.text))
        {
            // 构建要发送的消息字符串，格式为 "玩家昵称: 消息内容"
            string messageToSend = $"{PhotonNetwork.LocalPlayer.NickName}: {inputField.text}";

            // 通过 Photon RPC 调用 SendChatMessage 方法，并将消息发送给所有客户端 (RpcTarget.All)
            GetComponent<PhotonView>().RPC("SendChatMessage", RpcTarget.All, messageToSend);

            // 清空输入框文本
            inputField.text = "";
            // 关闭输入框状态
            isInputFieldToggled = false;
            // 取消选中当前的 UI 元素
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

            Debug.Log("Message sent");
        }

    }
    [PunRPC]
    public void SendChatMessage(string _message)
    {
        chatText.text = chatText.text + "\n" + _message;
    }
}
