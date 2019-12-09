using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// 角色管理器
/// 1.当前用户信息
/// 2.游戏中所有角色信息
/// 3.控制所有非本地角色的行为（同步服务端）
/// 4.上传本地角色的行为
/// </summary>
public class PlayerManager : BaseManager {
    public PlayerManager(GameFacade facade) : base(facade) { }

    private UserData userData;//个人信息
    private Dictionary<RoleType, RoleData> roleDataDict 
        = new Dictionary<RoleType, RoleData>();//所有的角色信息，

    //当前角色位置
    private Transform rolePositions;

    //游戏中的所有角色
    private RoleType currentRoleType;
    private GameObject currentRoleGameObject;
    private GameObject remoteRoleGameObject;

    //管理的请求
    private GameObject playerSync;

    public void UpdateResult(int totalCount, int winCount) {
        userData.TotalCount = totalCount;
        userData.WinCount = winCount;
    }
    public void SetCurrentRoleType(RoleType rt) {
        currentRoleType = rt;
    }
    public UserData UserData {
        set { userData = value; }
        get { return userData; }
    }
    public override void OnInit() {
        rolePositions = GameObject.Find("RolePositions").transform;
        InitRoleDataDict();
    }

    /// <summary>
    /// roleDataDict保存有所有角色的信息，每个角色的信息对应一个 RoleData
    /// 每个游戏物体持有一个 PlayInfo 组件，PlayInfo 和 RoleData 通过 RoleType属性相关联
    /// </summary>
    private void InitRoleDataDict() {
        roleDataDict.Add(RoleType.Blue, new RoleData(RoleType.Blue, "Hunter_BLUE", "Arrow_BLUE", "Explosion_BLUE", rolePositions.Find("Position1")));
        roleDataDict.Add(RoleType.Red, new RoleData(RoleType.Red, "Hunter_RED", "Arrow_RED", "Explosion_RED", rolePositions.Find("Position2")));
    }
    public void SpawnRoles() {
        foreach (RoleData rd in roleDataDict.Values) {
            GameObject go = GameObject.Instantiate(rd.RolePrefab, rd.SpawnPosition, Quaternion.identity);
            go.tag = "Player";
            if (rd.RoleType == currentRoleType) {
                currentRoleGameObject = go;
                currentRoleGameObject.GetComponent<PlayerInfo>().isLocal = true;
            } else {
                remoteRoleGameObject = go;
            }
        }
    }
    public GameObject GetCurrentRoleGameObject() {
        return currentRoleGameObject;
    }
    private RoleData GetRoleData(RoleType rt) {
        RoleData rd = null;
        roleDataDict.TryGetValue(rt, out rd);
        return rd;
    }

    /// <summary>
    /// 根据当前roleType，得到本地 RoleData 和 PlayInfo
    /// 为本地角色添加控制脚本
    /// </summary>
    public void AddControlScript() {
        currentRoleGameObject.AddComponent<PlayerMove>();
        RoleType rt = currentRoleGameObject.GetComponent<PlayerInfo>().roleType;
        RoleData rd = GetRoleData(rt);

        PlayerAttack playerAttack = currentRoleGameObject.AddComponent<PlayerAttack>();
        playerAttack.arrowPrefab = rd.ArrowPrefab;
        playerAttack.SetPlayerMng(this);
    }

    /// <summary>
    /// 场景新实例化一个游戏物体：角色同步器，
    /// 挂载MoveRequest，ShootRequest，attackRequest组件
    /// 这些组件被 PlayManager 持有
    /// </summary>
    public void CreateSyncRequest() {
        playerSync = new GameObject("PlayerSyncRequest");

        playerSync.AddComponent<MoveRequest>().SetPlayers(currentRoleGameObject, remoteRoleGameObject);
        playerSync.AddComponent<ShootRequest>().playerMng = this;
        playerSync.AddComponent<AttackRequest>();
    }

    /// <summary>
    /// 本地角色实例化箭，进行直接射击，
    /// 然后再通知服务端
    /// </summary>
    /// <param name="arrowPrefab"></param>
    /// <param name="pos"></param>
    /// <param name="rotation"></param>
    public void Shoot(GameObject arrowPrefab, Vector3 pos, Quaternion rotation) {
        facade.PlayNormalSound(AudioManager.Sound_Timer);
        GameObject.Instantiate(arrowPrefab, pos, rotation).GetComponent<Arrow>().isLocal = true;
        playerSync.GetComponent<ShootRequest>().
            SendRequest(arrowPrefab.GetComponent<Arrow>().roleType, pos, rotation.eulerAngles);
    }
    /// <summary>
    /// 非本地角色实例化箭，进行射击
    /// </summary>
    /// <param name="rt"></param>
    /// <param name="pos"></param>
    /// <param name="rotation"></param>
    public void RemoteShoot(RoleType rt, Vector3 pos, Vector3 rotation) {
        GameObject arrowPrefab = GetRoleData(rt).ArrowPrefab;
        Transform transform = GameObject.Instantiate(arrowPrefab).GetComponent<Transform>();
        transform.position = pos;
        transform.eulerAngles = rotation;
    }

    public void SendAttack(RoleType rt, int damage) {
        playerSync.GetComponent<AttackRequest>() .SendRequest(rt, damage);
    }

    public void GameOver() {
        GameObject.Destroy(playerSync);
        GameObject.Destroy(currentRoleGameObject);
        GameObject.Destroy(remoteRoleGameObject);
    }
}
