using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class QuitRoomRequest : BaseRequest {
    private RoomPanel roomPanel;
    public override void Awake() {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.QuitRoom;
        roomPanel = GetComponent<RoomPanel>();
        base.Awake();
    }
    public override void SendRequest() {
        base.SendRequest("r");
    }
}
