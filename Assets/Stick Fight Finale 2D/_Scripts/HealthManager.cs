using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Image healthBar;
    public float healAmount = 100f;

    void Start()
    {
    }

    void Update()
    {
        if (healAmount <= 0)
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            TakeDamage(20);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Heal(5);
        }
    }

    public void TakeDamage(float damage)
    {
        healAmount -= damage;
        healthBar.fillAmount = healAmount / 100f;
    }

    public void Heal(float healingAmount)
    {
        healAmount += healingAmount;
        healAmount = Mathf.Clamp(healAmount, 0, 100);
        healthBar.fillAmount = healAmount / 100f;
    }
}
