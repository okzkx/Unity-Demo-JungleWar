using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
/// <summary>
/// 控制人物请求
/// </summary>
public class StartPlayRequest : BaseRequest {

    private bool isStartPlaying = false;

    public override void Awake() {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.StartPlay;
        base.Awake();
    }

    private void Update() {
        if (isStartPlaying) {
            facade.StartPlaying();
            isStartPlaying = false;
        }
    }

    public override void OnResponse(string data) {
        isStartPlaying = true;
    }
}
