using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class Credits : MonoBehaviour
{
    [SerializeField] string playerName;
    [SerializeField] public int totalPlayerCredits;
    [SerializeField] public int selectedCreditAmount;
    [SerializeField] public int betTotal;

    //[SerializeField] Slider creditAdjuster;
    [SerializeField] GamePot chipPot;
    [SerializeField] TextMeshProUGUI creditTotal;
    [SerializeField] TextMeshProUGUI betRatio;

    [SerializeField] GameObject creditImage;

    private void Start()
    {
        selectedCreditAmount = betTotal;
    }

    private void Update()
    {
        //creditAdjuster.maxValue = totalPlayerCredits;
        //creditAdjuster.minValue = betTotal;
        creditTotal.text = totalPlayerCredits.ToString();
        //UpdateBetRatio();
    }

    //private void UpdateBetRatio()
    //{
    //    betRatio.text = creditAdjuster.value.ToString() + "/" + totalPlayerCredits.ToString();
    //}

    public void ReducePlayerCredits(int creditReduction)
    {
        totalPlayerCredits = totalPlayerCredits - creditReduction;

        if(totalPlayerCredits < 0)
        {
            totalPlayerCredits = 0;
        }
    }

    public int ReturnCreditTotal()
    {
        return totalPlayerCredits;
    }

    public void BetChips()
    {
        //chipPot.AddToPot((int)creditAdjuster.value);
        creditImage.transform.position = new Vector3(creditTotal.rectTransform.position.x, creditTotal.rectTransform.position.y, creditTotal.rectTransform.position.z);
        creditImage.SetActive(true);
        creditImage.transform.DOMove(new Vector2(chipPot.credit_total.rectTransform.position.x, chipPot.credit_total.rectTransform.position.y), 0.35f);
        StartCoroutine(ChipMoveToPileAnimation(0.35f));
        //ReducePlayerCredits((int)creditAdjuster.value);
    }

    IEnumerator ChipMoveToPileAnimation(float seconds)
    {

        yield return new WaitForSeconds(seconds);
        creditImage.SetActive(false);
    }
}
