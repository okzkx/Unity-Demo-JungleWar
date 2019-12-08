using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
/// <summary>
/// 进入游戏开始倒计时请求
/// </summary>
public class StartGameRequest : BaseRequest {
    private RoomPanel roomPanel;
    public override void Awake() {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.StartGame;
        roomPanel = GetComponent<RoomPanel>();
        base.Awake();
    }

    public override void OnResponse(string data) {
        ReturnCode returnCode = (ReturnCode)int.Parse(data);
        if (returnCode == ReturnCode.Success) {
            facade.GetManager<UIManager>().PushPanelAsync(UIPanelType.Game);
            facade.EnterPlaying();
        } else {
            facade.GetManager<UIManager>().ShowMessage("您不是房主，无法开始游戏！！");
        }
    }
}
