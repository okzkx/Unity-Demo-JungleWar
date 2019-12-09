using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Common;
using System;

public class RoomListPanel : BasePanel {

    private RectTransform battleRes;
    private RectTransform roomList;
    private VerticalLayoutGroup roomLayout;
    private GameObject roomItemPrefab;
    private ListRoomRequest listRoomRequest;
    private CreateRoomRequest createRoomRequest;
    private JoinRoomRequest joinRoomRequest;
    private List<UserData> udList = null;

    private UserData ud1 = null;
    private UserData ud2 = null;

    private void Awake() {
        battleRes = transform.Find("BattleRes").GetComponent<RectTransform>();
        roomList = transform.Find("RoomList").GetComponent<RectTransform>();
        roomLayout = transform.Find("RoomList/ScrollRect/Layout").GetComponent<VerticalLayoutGroup>();
        roomItemPrefab = Resources.Load("UIPanel/RoomItem") as GameObject;

        transform.Find("RoomList/CloseButton").GetComponent<Button>().onClick.AddListener(OnCloseClick);
        transform.Find("RoomList/CreateRoomButton").GetComponent<Button>().onClick.AddListener(OnCreateRoomClick);
        transform.Find("RoomList/RefreshButton").GetComponent<Button>().onClick.AddListener(OnRefreshClick);

        listRoomRequest = gameObject.AddComponent<ListRoomRequest>();
        createRoomRequest = gameObject.AddComponent<CreateRoomRequest>();
        joinRoomRequest = gameObject.AddComponent<JoinRoomRequest>();
    }

    public override void OnEnter() {
        SetBattleRes();

        listRoomRequest.SendRequest();

        base.OnEnter();
    }

    public override void OnResume() {
        base.OnResume();
        listRoomRequest.SendRequest();
    }
    private void Update() {
        //更新房间列表
        if (udList != null) {
            LoadRoomItem(udList);
            udList = null;
        }
    }

    protected override void EnterAnimation() {
        gameObject.SetActive(true);

        battleRes.localPosition = new Vector3(-1000, 0);
        battleRes.DOLocalMoveX(-290, 0.5f);

        roomList.localPosition = new Vector3(1000, 0);
        roomList.DOLocalMoveX(171, 0.5f);
    }
    protected override void HideAnimation() {
        battleRes.DOLocalMoveX(-1000, 0.5f);
        roomList.DOLocalMoveX(1000, 0.5f).OnComplete(() => gameObject.SetActive(false));
    }

    //创建房间
    private void OnCreateRoomClick() {
        createRoomRequest.SendRequest();
    }

    //设置个人信息
    private void SetBattleRes() {
        UserData ud = facade.GetUserData();
        transform.Find("BattleRes/Username").GetComponent<Text>().text = ud.Username;
        transform.Find("BattleRes/TotalCount").GetComponent<Text>().text = "总场数:" + ud.TotalCount.ToString();
        transform.Find("BattleRes/WinCount").GetComponent<Text>().text = "胜利:" + ud.WinCount.ToString();
    }
    
    private void OnRefreshClick() {
        listRoomRequest.SendRequest();
    }

    public void OnUpdateResultResponse(int totalCount, int winCount) {
        facade.UpdateResult(totalCount, winCount);
        SetBattleRes();
    }

    //设置房间列表
    public void LoadRoomItemSync(List<UserData> udList) {
        this.udList = udList;
    }
    private void LoadRoomItem(List<UserData> udList) {
        RoomItem[] riArray = roomLayout.GetComponentsInChildren<RoomItem>();
        foreach (RoomItem ri in riArray) {
            ri.DestroySelf();
        }
        int count = udList.Count;
        for (int i = 0; i < count; i++) {
            GameObject roomItem = GameObject.Instantiate(roomItemPrefab);
            roomItem.transform.SetParent(roomLayout.transform);
            UserData ud = udList[i];
            roomItem.GetComponent<RoomItem>().SetRoomInfo(ud.Id, ud.Username, ud.TotalCount, ud.WinCount, this);
        }
        int roomCount = GetComponentsInChildren<RoomItem>().Length;
        Vector2 size = roomLayout.GetComponent<RectTransform>().sizeDelta;
        roomLayout.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x,
            roomCount * (roomItemPrefab.GetComponent<RectTransform>().sizeDelta.y + roomLayout.spacing));
    }

    //加入房间
    public void OnJoinClick(int id) {
        joinRoomRequest.SendRequest(id);
    }
}
