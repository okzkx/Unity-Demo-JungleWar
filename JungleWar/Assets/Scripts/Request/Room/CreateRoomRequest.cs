using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
public class CreateRoomRequest : BaseRequest {

    public override void Awake() {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.CreateRoom;
        base.Awake();
    }

    public override void OnResponse(string data) {
        string[] strs = data.Split(',');
        ReturnCode returnCode = (ReturnCode)int.Parse(strs[0]);
        if (returnCode == ReturnCode.Success) {

            RoleType roleType = (RoleType)int.Parse(strs[1]);
            facade.SetCurrentRoleType(roleType);

            facade.GetManager<UIManager>().PushPanelAsync(UIPanelType.Room);
        }
    }
}
