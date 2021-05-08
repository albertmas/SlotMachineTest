using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spin : MonoBehaviour
{
    public bool spinning = false;
    public int spinVelocity = 100;
    public int spinAcceleration = 50;
    public int spinDeceleration = 20;
    public FigureTypes[] rollerFigures;

    float wheelVelocity = 0f;
    float wheelLength; // Length from the top of the first figure to the bottom of the last
    readonly int offset = 10;
    List<RectTransform> figuresRectTrans;

    public enum FigureTypes
    {
        none,
        bell,
        watermelon,
        grapes,
        eggplant,
        orange,
        lemon,
        cherry
    };

    void Start()
    {
        figuresRectTrans = new List<RectTransform>();

        CreateWheelFigures();

        // Set the length of the wheel
        wheelLength = figuresRectTrans[0].rect.size.y * rollerFigures.Length + offset * rollerFigures.Length - 1;
    }

    void Update()
    {
        SpinFigures();
    }

    void SpinFigures()
    {
        if (spinning)
        {
            wheelVelocity += spinAcceleration;
        }
        else if (wheelVelocity > 0f)
        {
            wheelVelocity -= spinDeceleration;
        }

        wheelVelocity = Mathf.Clamp(wheelVelocity, 0f, spinVelocity);

        if (wheelVelocity > 0f)
        {
            for (int i = 0; i < figuresRectTrans.Count; i++)
            {
                figuresRectTrans[i].localPosition += Vector3.down * wheelVelocity * Time.deltaTime;

                // Move to the top figures that go out of screen
                if (figuresRectTrans[i].localPosition.y < -wheelLength)
                {
                    figuresRectTrans[i].localPosition += new Vector3(0f, wheelLength, 0f);
                    //int next = i + 1 < figuresTextures.Count ? i + 1 : 0;
                    //figuresTextures[i].localPosition = new Vector2(0f,
                    //    figuresTextures[next].localPosition.y + figuresTextures[i].rect.size.y + offset);
                }
            }
        }
    }

    void CreateWheelFigures()
    {
        for (int i = 0; i < rollerFigures.Length; i++)
        {
            GameObject newFigure = Instantiate(Resources.Load<GameObject>("EmptyFigure"), transform);
            newFigure.name = "" + rollerFigures[i];

            string texPath = "Figures/" + (int)rollerFigures[i];
            newFigure.GetComponent<Image>().sprite = Resources.Load<Sprite>(texPath);

            RectTransform figureTrans = newFigure.GetComponent<RectTransform>();
            figureTrans.localPosition = new Vector3(0f, -(i * figureTrans.rect.size.y + i * offset), 0f);
            figuresRectTrans.Add(figureTrans);
        }
    }
}
