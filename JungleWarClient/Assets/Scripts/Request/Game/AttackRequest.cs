using System;
using System.Collections.Generic;
using Common;
using UnityEngine;


public class AttackRequest : BaseRequest {
    public override void Awake() {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.Attack;
        base.Awake();
    }
    public void SendRequest(RoleType rt,int damage) {
        base.SendRequest(rt + "," + damage.ToString());
    }
}

