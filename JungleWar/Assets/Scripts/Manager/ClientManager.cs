using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using Common;

/// <summary>
/// 这个是用来管理跟服务器端的Socket连接
/// </summary>
public class ClientManager : BaseManager {

    private const string IP = "127.0.0.1";
    private const int PORT = 6688;

    private Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private Message msg = new Message();

    public ClientManager(GameFacade facade) : base(facade) { }

    public bool Connect() {
        try {
            clientSocket.Connect(IP, PORT);
            clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallback, null);
            return true;
        } catch (Exception e) {
            facade.ShowMessage("无法连接到服务器端，请检查您的网络。。");
            Debug.LogError(e);
            return false;
        }
    }
    private void ReceiveCallback(IAsyncResult ar) {
        try {
            if (clientSocket == null || clientSocket.Connected == false) return;
            int count = clientSocket.EndReceive(ar);

            msg.ReadMessage(count, OnProcessDataCallback);

            clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallback, null);
        } catch (Exception e) {
            Debug.LogError(e);
            facade.ShowMessage("网络延时检测。。");
        }
    }

    /// <summary>
    /// 得到服务端消息后，交给RequestManager处理
    /// </summary>
    /// <param name="actionCode"></param>
    /// <param name="data"></param>
    private void OnProcessDataCallback(ActionCode actionCode, string data) {
        facade.HandleReponse(actionCode, data);
    }
    /// <summary>
    /// 由RequestManager发来的的消息，发送到服务器端
    /// </summary>
    /// <param name="requestCode"></param>
    /// <param name="actionCode"></param>
    /// <param name="data"></param>
    public void SendRequest(RequestCode requestCode, ActionCode actionCode, string data) {
        byte[] bytes = Message.PackData(requestCode, actionCode, data);
        clientSocket.Send(bytes);
    }

    public void Close() {
        try {
            //TODO
            //连接没法关闭
            clientSocket.Close();
        } catch (Exception e) {
            Debug.LogWarning("无法关闭跟服务器端的连接！！" + e);
        }

    }

    public override void OnDestroy() {
        Close();
    }
}