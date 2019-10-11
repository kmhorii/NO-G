using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthandAmmo : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;

    public Slider healthbar;
    public Text healthtext;
    // Start is called before the first frame update
    void Start()
    {
        if(maxHealth == 0)
        {
            maxHealth = 100f;
        }
        currentHealth = maxHealth;

        healthbar.value = CalculateHealth();
        healthtext.text = ConvertHealthFloattoString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private float CalculateHealth()
    {
        return currentHealth / maxHealth;
    }

    private string ConvertHealthFloattoString()
    {
        float converthealth = CalculateHealth() * 100;
        return converthealth.ToString("f00");
    }
}
