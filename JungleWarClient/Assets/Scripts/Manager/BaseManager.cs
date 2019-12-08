using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理类，
/// 1.存储有关的信息
/// 2.具备有关的方法，以便被request调用
/// </summary>
public class BaseManager {
    protected GameFacade facade;
    public BaseManager(GameFacade facade) {
        this.facade = facade;
    }
    public virtual void OnInit() { }
    public virtual void Update() { }
    public virtual void OnDestroy() { }
}
