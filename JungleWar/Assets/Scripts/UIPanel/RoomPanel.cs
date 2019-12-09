using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Common;

public class RoomPanel : BasePanel {

    private Text localPlayerUsername;
    private Text localPlayerTotalCount;
    private Text localPlayerWinCount;

    private Text enemyPlayerUsername;
    private Text enemyPlayerTotalCount;
    private Text enemyPlayerWinCount;

    private Transform bluePanel;
    private Transform redPanel;
    private Transform startButton;
    private Transform exitButton;

    private QuitRoomRequest quitRoomRequest;
    private StartGameRequest startGameRequest;

    private bool isPopPanel = false;

    private void Awake() {
        localPlayerUsername = transform.Find("BluePanel/Username").GetComponent<Text>();
        localPlayerTotalCount = transform.Find("BluePanel/TotalCount").GetComponent<Text>();
        localPlayerWinCount = transform.Find("BluePanel/WinCount").GetComponent<Text>();
        enemyPlayerUsername = transform.Find("RedPanel/Username").GetComponent<Text>();
        enemyPlayerTotalCount = transform.Find("RedPanel/TotalCount").GetComponent<Text>();
        enemyPlayerWinCount = transform.Find("RedPanel/WinCount").GetComponent<Text>();

        bluePanel = transform.Find("BluePanel");
        redPanel = transform.Find("RedPanel");
        startButton = transform.Find("StartButton");
        exitButton = transform.Find("ExitButton");

        transform.Find("StartButton").GetComponent<Button>().onClick.AddListener(OnStartClick);
        transform.Find("ExitButton").GetComponent<Button>().onClick.AddListener(OnExitClick);

        quitRoomRequest = gameObject.AddComponent<QuitRoomRequest>();
        startGameRequest = gameObject.AddComponent<StartGameRequest>();
        gameObject.AddComponent<UpdateRoomRequest>();
    }
    public override void OnEnter() {
        uiMng.ud1 = facade.GetUserData();
        base.OnEnter();
    }
    private void Update() {
        //根据uiMng.ud 实时更新房间玩家信息
        if (uiMng.ud1 != null) {
            SetLocalPlayerRes(uiMng.ud1.Username, uiMng.ud1.TotalCount.ToString(), uiMng.ud1.WinCount.ToString());
            uiMng.ud1 = null;
            if (uiMng.ud2 != null) {
                SetEnemyPlayerRes(uiMng.ud2.Username, uiMng.ud2.TotalCount.ToString(), uiMng.ud2.WinCount.ToString());
                uiMng.ud2 = null;
            } else {
                ClearEnemyPlayerRes();
            }
        }
    }

    public void SetPlayerRes(UserData ud1, UserData ud2 = null) {
        uiMng.ud1 = ud1;
        uiMng.ud2 = ud2;
    }
    //ui内容设置
    private void SetLocalPlayerRes(string username, string totalCount, string winCount) {
        localPlayerUsername.text = username;
        localPlayerTotalCount.text = "总场数：" + totalCount;
        localPlayerWinCount.text = "胜利：" + winCount;
    }
    private void SetEnemyPlayerRes(string username, string totalCount, string winCount) {
        enemyPlayerUsername.text = username;
        enemyPlayerTotalCount.text = "总场数：" + totalCount;
        enemyPlayerWinCount.text = "胜利：" + winCount;
    }
    public void ClearEnemyPlayerRes() {
        enemyPlayerUsername.text = "";
        enemyPlayerTotalCount.text = "等待玩家加入....";
        enemyPlayerWinCount.text = "";
    }

    //按钮点击
    private void OnStartClick() {
        startGameRequest.SendRequest();
    }
    private void OnExitClick() {
        quitRoomRequest.SendRequest();
        uiMng.PopPanel();
    }
    


    private void EnterAnim() {
        gameObject.SetActive(true);
        bluePanel.localPosition = new Vector3(-1000, 0, 0);
        bluePanel.DOLocalMoveX(-174, 0.4f);
        redPanel.localPosition = new Vector3(1000, 0, 0);
        redPanel.DOLocalMoveX(174, 0.4f);
        startButton.localScale = Vector3.zero;
        startButton.DOScale(1, 0.4f);
        exitButton.localScale = Vector3.zero;
        exitButton.DOScale(1, 0.4f);
    }
    private void ExitAnim() {
        bluePanel.DOLocalMoveX(-1000, 0.4f);
        redPanel.DOLocalMoveX(1000, 0.4f);
        startButton.DOScale(0, 0.4f);
        exitButton.DOScale(0, 0.4f).OnComplete(() => gameObject.SetActive(false));
    }
}
