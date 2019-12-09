using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessagePanel :BasePanel {

    private Text text;
    public float showTime = 1;
    private string message = null;

    private void Update() {
        if (message != null) {
            text.CrossFadeAlpha(1, 0.2f, false);
            text.text = message;
            message = null;
            text.enabled = true;
            Invoke("Hide", showTime);
        }
    }

    public override void OnEnter() {
        base.OnEnter();
        text = GetComponent<Text>();
        text.enabled = false;
    }

    //异步调用的方法里不能直接调用UI控件中的方法，
    //需要通过Update 调用UI控件中的方法
    //如果该控件是 ！active的，要在其他控件的Update方法中激活这个控件
    //所以该方法也无法激活UI控件
    //暂时先不实现，信息显示界面不激活
    public void ShowMessage(string msg) {
        message = msg;
    }
    private void Hide() {
        text.CrossFadeAlpha(0, 1, false);
    }

}
