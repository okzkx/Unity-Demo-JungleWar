using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
public class MoveRequest : BaseRequest {

    private Transform localPlayerTransform;
    private PlayerMove localPlayerMove;
    private int syncRate = 30;

    private Transform remotePlayerTransform;
    private Animator remotePlayerAnim;

    //保存除本地角色外，另外一个角色的位置信息
    private bool isSyncRemotePlayer = false;
    private Vector3 pos;
    private Vector3 rotation;
    private float forward;
    public override void Awake() {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.Move;
        base.Awake();
    }

    //调用请求和处理返回
    private void Start() {
        InvokeRepeating("SyncLocalPlayer", 1f, 1f / syncRate);
    }
    private void FixedUpdate() {
        if (isSyncRemotePlayer) {
            SyncRemotePlayer();
            isSyncRemotePlayer = false;
        }
    }
    public MoveRequest SetLocalPlayer(GameObject go) {
        this.localPlayerTransform = go.transform;
        this.localPlayerMove = go.GetComponent<PlayerMove>();
        return this;
    }
    public MoveRequest SetRemotePlayer(GameObject remotePlayer) {
        this.remotePlayerTransform = remotePlayer.transform;
        this.remotePlayerAnim = remotePlayerTransform.GetComponent<Animator>();
        return this;
    }
    public void SetPlayers(GameObject localPlayer, GameObject remotePlayer) {
        SetLocalPlayer(localPlayer);
        SetRemotePlayer(remotePlayer);
    }
    
    //请求和返回，调用同步本地角色和非本地角色
    private void SyncLocalPlayer() {
        SendRequest(localPlayerTransform.position, localPlayerTransform.eulerAngles, localPlayerMove.forward);
    }
    private void SyncRemotePlayer() {
        remotePlayerTransform.position = pos;
        remotePlayerTransform.eulerAngles = rotation;
        remotePlayerAnim.SetFloat("Forward", forward);
    }

    /// <summary>
    /// 向服务端同步本地角色位置信息，
    /// 在start方法中开启线程调用
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    /// <param name="forward"></param>
    private void SendRequest(Vector3 pos, Vector3 rot, float forward) {
        string data = string.Format("{0},{1},{2}|{3},{4},{5}|{6}"
            , pos.x, pos.y, pos.z, rot.x, rot.y, rot.z, forward);
        base.SendRequest(data);
    }

    /// <summary>
    /// 接收服务端同步回来的除本地角色外所有角色的位置信息
    /// </summary>
    /// <param name="data"></param>
    public override void OnResponse(string data) {//27.75,0,1.41-0,0,0-0
        //print(data);
        string[] strs = data.Split('|');
        pos = UnityTools.ParseVector3(strs[0]);
        rotation = UnityTools.ParseVector3(strs[1]);
        forward = float.Parse(strs[2]);
        isSyncRemotePlayer = true;
    }

}
