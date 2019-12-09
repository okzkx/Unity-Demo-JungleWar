using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
public class RequestManager : BaseManager {
    public RequestManager(GameFacade facade) : base(facade) { }

    private Dictionary<ActionCode, BaseRequest> requestDict = new Dictionary<ActionCode, BaseRequest>();

    public void AddRequest(ActionCode actionCode, BaseRequest request) {
        requestDict.Add(actionCode, request);
    }
    public void RemoveRequest(ActionCode actionCode) {
        requestDict.Remove(actionCode);
    }

    /// <summary>
    /// ActionCode得到对应的 Request，
    /// 调用该 Request 处理返回值的方法
    /// </summary>
    /// <param name="actionCode"></param>
    /// <param name="data"></param>
    public void HandleReponse(ActionCode actionCode, string data) {
        BaseRequest request = requestDict.TryGet(actionCode);
        if (request == null) {
            Debug.LogWarning("无法得到ActionCode[" + actionCode + "]对应的Request类"); return;
        }
        request.OnResponse(data);
    }
}
