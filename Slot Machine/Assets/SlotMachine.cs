using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachine : MonoBehaviour
{
    public Spin[] rollers;
    public Button spinButton;

    void Update()
    {
        //timer += Time.deltaTime;
        //if (timer >= spinSpeed)
        //{
        //    timer = 0f;
        //    currentNum++;
        //    if (currentNum >= textures.Length)
        //        currentNum = 0;

        //    top.sprite = textures[currentNum - 1 >= 0 ? currentNum - 1 : textures.Length - 1];
        //    mid.sprite = textures[currentNum];
        //    bot.sprite = textures[currentNum + 1 < textures.Length ? currentNum + 1 : 0];
        //}
    }

    public void SpinWheels()
    {
        float spinTime = Random.Range(2.0f, 4.0f);

        for (int i = 0; i < rollers.Length; i++)
        {
            rollers[i].StartSpin(spinTime, 0.2f * i);
        }

        spinButton.interactable = false;
        Invoke("MachineReady", spinTime + 1.5f);
    }

    void MachineReady()
    {
        spinButton.interactable = true;
    }
}
