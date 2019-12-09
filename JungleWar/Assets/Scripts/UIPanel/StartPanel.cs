using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class StartPanel : BasePanel {

    private Button loginButton;
    private Animator btnAnimator;

    private void Awake() {
        loginButton = transform.Find("LoginButton").GetComponent<Button>();
        btnAnimator = loginButton.GetComponent<Animator>();
        loginButton.onClick.AddListener(OnLoginClick);

    }

    private void OnLoginClick() {
        if (facade.GetManager<ClientManager>().Connect()) {
            PlayClickSound();
            uiMng.PushPanel(UIPanelType.Login);
        }
    }
    public override void OnPause() {
        btnAnimator.enabled = false;
        loginButton.transform.DOScale(0, 0.3f).OnComplete(() => loginButton.gameObject.SetActive(false));
    }
    public override void OnResume() {
        loginButton.gameObject.SetActive(true);
        loginButton.transform.DOScale(1, 0.3f).OnComplete(() => btnAnimator.enabled = true);
    }
}
