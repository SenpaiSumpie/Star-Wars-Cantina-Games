using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class GamePot : MonoBehaviour
{
    [SerializeField] public string pot_name;
    [SerializeField] public int pot_total;
    [SerializeField] TextMeshProUGUI name_of_pot;
    [SerializeField] public TextMeshProUGUI credit_total;
    // Start is called before the first frame update
    void Start()
    {
        name_of_pot.text = pot_name;
        //pot_total = 0;
        //credit_total.text = pot_total.ToString();
    }

    void Update()
    {
        if(pot_total > 0)
        {
            credit_total.text = pot_total.ToString();
        }
    }

    public void AddToPot(int total_to_add)
    {
        pot_total = pot_total + total_to_add;
    }

    public int WonPot()
    {
        int temp_pot_total = pot_total;
        pot_total = 0;
        return temp_pot_total;
    }
}
