using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class UnitStats : MonoBehaviour
{
    public float maxHealth = 50;
    public float health;
    public float accuracy;
    public float weaponDamage;
    public float evasion;

    public bool isEnemy;
    public GameManagerr manager;

    public Image healthBar;
    public GameObject enemyUI;
    private GameObject cameraTgt;
    private Vector3 TargetCamPosition;
    private void Start()
    {
        manager = GameObject.Find("/GameManagerr").GetComponent<GameManagerr>();
        health = maxHealth;
        cameraTgt = Camera.main.gameObject;
    }
    private void Update()
    {
        if (health <= 0.0f)
        {
            if (isEnemy)
            {
                manager.enemyUnits.Remove(this.transform.gameObject);
                Destroy(this.transform.gameObject);
                manager.checkForWin();
            }
            else
            {
                manager.switchStats();
                Debug.Log(manager.switchAction);
                Debug.Log(manager.switchAlly);
                Debug.Log(manager.switchChar);
            }
        }
    }

    private void LateUpdate()
    {
        if (TargetCamPosition != cameraTgt.transform.position)
        {
            TargetCamPosition = cameraTgt.transform.position;
            enemyUI.transform.LookAt(TargetCamPosition);
        }
    }

    public void takeDamage(float dmg) 
    {
        health -= dmg;
        healthBar.fillAmount = health / maxHealth;

    }
}
