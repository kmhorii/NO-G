using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public float CurrentHealth;
    public float MaxHealth;

    public Slider healthbar;

    public Text healthtext;

    // Start is called before the first frame update
    void Start()
    {
        MaxHealth = 100f;
        CurrentHealth = MaxHealth;

        healthbar.value = CalculateHealth();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
            DealDamage(10);
    }

    void DealDamage(float damagevalue)
    {
        //Minus player health w/ damage value
        CurrentHealth -= damagevalue;
        healthbar.value = CalculateHealth();
        healthtext.text = ConvertHealthFloattoString();
        Debug.Log(CurrentHealth);

        //If player health =0, trigger death
        if (CurrentHealth <= 0)
            Die();
    }

    float CalculateHealth()
    {
        return CurrentHealth / MaxHealth;
    }

    string ConvertHealthFloattoString()
    {
        float converthealth = CalculateHealth() * 100;
        return converthealth.ToString("f00");
    }

    void Die()
    {
        CurrentHealth= 0;
        Debug.Log("Die");
        healthtext.text = "Dead";
   
    }
}