using UnityEngine;
using System.Collections;

public class BasePanel : MonoBehaviour {
    public UIManager uiMng;
    public GameFacade facade;
    public UIPanelType uIPanelType;

    protected void PlayClickSound() {
        facade.PlayNormalSound(AudioManager.Sound_ButtonClick);
    }

    /// <summary>
    /// 界面被显示出来
    /// </summary>
    public virtual void OnEnter() {
        EnterAnimation();
    }

    /// <summary>
    /// 界面暂停
    /// </summary>
    public virtual void OnPause() {
        HideAnimation();
    }

    /// <summary>
    /// 界面继续
    /// </summary>
    public virtual void OnResume() {
        EnterAnimation();
    }

    /// <summary>
    /// 界面不显示,退出这个界面，界面被关闭
    /// </summary>
    public virtual void OnExit() {
        HideAnimation();
        Close();
    }

    protected virtual void EnterAnimation() {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 界面隐藏动画，包括设置界面是否是活动的
    /// </summary>
    protected virtual void HideAnimation() {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 界面关闭处理逻辑
    /// </summary>
    protected virtual void Close() {
        uiMng.panelDict.Remove(uIPanelType);
        Destroy(this.gameObject);
    }

    /// <summary>
    /// 注册进该界面关闭按钮点击事件
    /// </summary>
    protected virtual void OnCloseClick() {
        PlayClickSound();
        uiMng.PopPanel();
    }
}
