using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachine : MonoBehaviour
{
    public Image top;
    public Image mid;
    public Image bot;

    public Sprite[] textures;

    //int currentNum = 0;
    //float spinSpeed = 0.3f;
    //float timer = 0f;

    void Start()
    {
        top.sprite = textures[0];
        mid.sprite = textures[1];
        bot.sprite = textures[2];
    }

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
}
