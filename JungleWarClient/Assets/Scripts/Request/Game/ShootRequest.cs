using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class ShootRequest : BaseRequest {
    public PlayerManager playerMng;
    private bool isShoot = false;
    private RoleType rt;
    private Vector3 pos;
    private Vector3 rotation;
    public override void Awake() {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.Shoot;
        base.Awake();
    }
    private void Update() {
        if (isShoot) {
            playerMng.RemoteShoot(rt, pos, rotation);
            isShoot = false;
        }
    }
    /// <summary>
    /// 本地角色射出箭的transform信息
    /// </summary>
    /// <param name="rt"></param>
    /// <param name="pos"></param>
    /// <param name="rotation"></param>
    public void SendRequest(RoleType rt, Vector3 pos, Vector3 rotation) {
        string data = string.Format("{0}|{1},{2},{3}|{4},{5},{6}", (int)rt, pos.x, pos.y, pos.z, rotation.x, rotation.y, rotation.z);
        base.SendRequest(data);
    }

    /// <summary>
    /// 非本地角色射出箭的transform信息
    /// </summary>
    /// <param name="data"></param>
    public override void OnResponse(string data) {
        string[] strs = data.Split('|');
        RoleType rt = (RoleType)int.Parse(strs[0]);
        Vector3 pos = UnityTools.ParseVector3(strs[1]);
        Vector3 rotation = UnityTools.ParseVector3(strs[2]);
        isShoot = true;
        this.rt = rt;
        this.pos = pos;
        this.rotation = rotation;
    }
}
