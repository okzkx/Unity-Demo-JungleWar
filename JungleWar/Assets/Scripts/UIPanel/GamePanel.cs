using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Common;

public class GamePanel : BasePanel {

    private Text timer;
    private int time = -1;

    private Button successBtn;
    private Button failBtn;
    private Button exitBtn;

    private QuitBattleRequest quitBattleRequest;
    private void Awake() {
        timer = transform.Find("Timer").GetComponent<Text>();
        successBtn = transform.Find("SuccessButton").GetComponent<Button>();
        failBtn = transform.Find("FailButton").GetComponent<Button>();
        exitBtn = transform.Find("ExitButton").GetComponent<Button>();

        timer.gameObject.SetActive(false);
        successBtn.gameObject.SetActive(false);
        failBtn.gameObject.SetActive(false);
        exitBtn.gameObject.SetActive(false);

        exitBtn.onClick.AddListener(OnExitClick);
        successBtn.onClick.AddListener(OnResultClick);
        failBtn.onClick.AddListener(OnResultClick);

        quitBattleRequest = gameObject.AddComponent<QuitBattleRequest>();

    }
    public override void OnExit() {
        successBtn.gameObject.SetActive(false);
        failBtn.gameObject.SetActive(false);
        exitBtn.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
    private void Update() {
        if (time > -1) {
            ShowTime(time);
            time = -1;
        }
    }
    private void OnResultClick() {
        uiMng.PopPanel();
        uiMng.PopPanel();
        facade.GameOver();
    }
    private void OnExitClick() {
        quitBattleRequest.SendRequest();
    }
    public void OnExitResponse() {
        OnResultClick();
    }
    public void ShowTimeSync(int time) {
        this.time = time;
    }
    public void ShowTime(int time) {
        if (time == 3) {
            exitBtn.gameObject.SetActive(true);
        }
        timer.gameObject.SetActive(true);
        timer.text = time.ToString();
        timer.transform.localScale = Vector3.one;
        Color tempColor = timer.color;
        tempColor.a = 1;
        timer.color = tempColor;
        timer.transform.DOScale(2, 0.3f).SetDelay(0.3f);
        timer.DOFade(0, 0.3f).SetDelay(0.3f).OnComplete(() => timer.gameObject.SetActive(false));
        facade.PlayNormalSound(AudioManager.Sound_Alert);
    }
    public void OnGameOverResponse(ReturnCode returnCode) {
        Button tempBtn = null;
        switch (returnCode) {
            case ReturnCode.Success:
                tempBtn = successBtn;
                break;
            case ReturnCode.Fail:
                tempBtn = failBtn;
                break;
        }
        tempBtn.gameObject.SetActive(true);
        tempBtn.transform.localScale = Vector3.zero;
        tempBtn.transform.DOScale(1, 0.5f);
    }

}
