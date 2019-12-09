using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// 请求基类，Request，全局单例
/// 作为组件挂载在游戏物体上，
/// 当游戏物体需要向服务端发送某请求时，
/// 调用一个挂载在其身上的某请求类的请求方法，
/// 
/// 该请求类也具有处理服务端消息返回的方法，
/// 一般是调用Manager中的某个方法，如果该方法是有关于游戏物体的操作，
/// 需要在主线程（生命周期函数）中执行，（异步）
/// 异步操作尽量在Request类中实现，该Request无法实现才在Manager中实现异步
/// </summary>
public class BaseRequest : MonoBehaviour {
    protected RequestCode requestCode = RequestCode.None;
    protected ActionCode actionCode = ActionCode.None;
    protected GameFacade _facade;

    protected GameFacade facade {
        get {
            if (_facade == null)
                _facade = GameFacade.Instance;
            return _facade;
        }
    }
    public virtual void Awake() {
        facade.AddRequest(actionCode, this);
    }

    protected void SendRequest(string data) {
        facade.SendRequest(requestCode, actionCode, data);
    }

    public virtual void SendRequest() {
        facade.SendRequest(requestCode, actionCode, "r");
    }
    public virtual void OnResponse(string data) { }

    public virtual void OnDestroy() {
        if (facade != null)
            facade.RemoveRequest(actionCode);
    }
}
