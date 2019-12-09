using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

    public GameObject arrowPrefab;
    private Animator anim;
    private Transform leftHandTrans;
    private Vector3 shootDir;
    private PlayerManager playerMng;

	void Start () {
        anim = GetComponent<Animator>();
        leftHandTrans = transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand");
	}

    void Update() {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Grounded")) {
            if (Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                bool isCollider = Physics.Raycast(ray, out hit);
                if (isCollider) {
                    Vector3 targetPoint = hit.point;
                    targetPoint.y = transform.position.y;
                    shootDir = targetPoint - transform.position;
                    transform.rotation = Quaternion.LookRotation(shootDir);
                    anim.SetTrigger("Attack");
                    Invoke("Shoot", 0.1f);
                }
            }
        }
    }
    public void SetPlayerMng(PlayerManager playerMng) {
        this.playerMng = playerMng;
    }

    /// <summary>
    /// 本地角色直接射击，
    /// 射击完成后，再通知服务端
    /// </summary>
    private void Shoot() {
        playerMng.Shoot(arrowPrefab, leftHandTrans.position, Quaternion.LookRotation(shootDir));
    }
}
