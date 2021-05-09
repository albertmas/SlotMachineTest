using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

/// <summary>
/// Class that controls the behaviour of a roller of the Slot Machine.
/// Loads the configured figures at the start.
/// Accelerates the figures down when the roller spins and decelerates them when it stops.
/// </summary>
public class Spin : MonoBehaviour
{
    [Tooltip("Max velocity of the roller.")]
    public int spinVelocity = 2000;
    [Tooltip("Acceleration of the roller.")]
    public int spinAcceleration = 70;
    [Tooltip("Deceleration of the roller.")]
    public int spinDeceleration = 40;
    [Tooltip("Velocity of the roller bounce.")]
    public int bounceVelocity = 700;
    [Tooltip("Figures of the roller in order.")]
    public FigureTypes[] rollerFigures;

    bool spinning = false;
    bool bouncing = false;

    float rollerVelocity = 0f; // Current velocity of the roller
    float rollerLength; // Length from the top of the first figure to the bottom of the last
    float spinTimer = 0f; // Time left of the spin
    readonly int offset = 10; // Pixel offset between each figure

    List<RectTransform> figuresRectTrans; // List with each figure's RectTransform

    FigureTypes[] lastFigurePattern; // The three figures gotten in the last spin, from top to bottom

    void Start()
    {
        figuresRectTrans = new List<RectTransform>();
        lastFigurePattern = new FigureTypes[3];

        // Create the figures of the roller
        CreateRollerFigures();

        // Set the length of the roller
        rollerLength = figuresRectTrans[0].rect.size.y * rollerFigures.Length + offset * rollerFigures.Length - 1;
    }

    void Update()
    {
        // Stop spinning when the time is up
        if (spinning && spinTimer < 0.0f)
            StopSpin();

        SpinFigures();

        if (bouncing)
            BounceFigures();
    }

    /// <summary>
    /// Spin the figures in the roller.
    /// Moves each figure down and resets to the top the ones that go too low.
    /// Creates a loop effect.
    /// </summary>
    void SpinFigures()
    {
        // Accelerate roller if it's spinning, decelerate it otherwise
        if (spinning)
        {
            rollerVelocity += spinAcceleration;
            spinTimer -= Time.deltaTime;
        }
        else if (rollerVelocity > 0f)
        {
            rollerVelocity -= spinDeceleration;

            // Activate bounce when the roller is about to stop
            if (rollerVelocity < spinVelocity / 3)
            {
                bouncing = true;
                rollerVelocity = 0f;
            }
        }

        // Cap roller velocity
        rollerVelocity = Mathf.Clamp(rollerVelocity, 0f, spinVelocity);

        // Move each figure down if roller is moving
        if (rollerVelocity > 0f)
        {
            for (int i = 0; i < figuresRectTrans.Count; i++)
            {
                figuresRectTrans[i].localPosition += Vector3.down * rollerVelocity * Time.deltaTime;

                // Move to the top figures that go out of screen
                if (figuresRectTrans[i].localPosition.y < -rollerLength)
                {
                    figuresRectTrans[i].localPosition += new Vector3(0f, rollerLength, 0f);
                }
            }
        }
    }

    /// <summary>
    /// Moves the figures up until one is at the top.
    /// This centers the figures in the slot machine's interface.
    /// Creates a bouncing effect.
    /// </summary>
    void BounceFigures()
    {
        for (int i = 0; i < figuresRectTrans.Count; i++)
        {
            figuresRectTrans[i].localPosition += Vector3.up * bounceVelocity * Time.deltaTime;

            if (figuresRectTrans[i].localPosition.y > 0f)
            {
                bouncing = false;
                EvaluateResult();
            }
        }
    }

    void EvaluateResult()
    {
        //RectTransform midFigure = figuresRectTrans[0];
        //
        //foreach (RectTransform figure in figuresRectTrans)
        //{
        //    if (Mathf.Abs(figure.position.y) < Mathf.Abs(midFigure.position.y))
        //        midFigure = figure;
        //}
        //Debug.Log(midFigure.name);
    }

    /// <summary>
    /// Creates the figures requested in the figure array.
    /// Sets the correct sprite to each of them and places them in order.
    /// Fills a list with all the new figures' RectTransforms.
    /// </summary>
    void CreateRollerFigures()
    {
        for (int i = 0; i < rollerFigures.Length; i++)
        {
            // Instantiate a new empty figure
            GameObject newFigure = Instantiate(Resources.Load<GameObject>("EmptyFigure"), transform);
            newFigure.name = "" + rollerFigures[i];

            // Load the corresponding sprite and set it to the new figure
            string texPath = "Figures/" + (int)rollerFigures[i];
            newFigure.GetComponent<Image>().sprite = Resources.Load<Sprite>(texPath);

            // Place the new figure under the previous one, with an offset in between
            RectTransform figureTrans = newFigure.GetComponent<RectTransform>();
            figureTrans.localPosition = new Vector3(0f, -(i * figureTrans.rect.size.y + i * offset), 0f);

            // Add the RectTransform of the new figure to the figure list
            figuresRectTrans.Add(figureTrans);
        }
    }

    /// <summary>
    /// Activates the spin after the warm-up time and sets the spin duration.
    /// </summary>
    /// <param name="duration">Duration of the spin.</param>
    /// <param name="warmUpTime">Time before the spin starts.</param>
    public void ActivateRoller(float duration, float warmUpTime = 0.0f)
    {
        // Set the duration of the spin
        spinTimer = duration;

        // If warm-up time is 0 start spinning, otherwise Invoke the start after warm-up time
        if (warmUpTime > 0f)
        {
            Invoke(nameof(StartSpin), warmUpTime);
        }
        else
        {
            StartSpin();
        }
    }

    /// <summary>
    /// Start spinning the roller.
    /// </summary>
    void StartSpin()
    {
        spinning = true;
    }

    /// <summary>
    /// Stop spinning the roller.
    /// Reset spin timer.
    /// </summary>
    void StopSpin()
    {
        spinning = false;
        spinTimer = 0f;
    }
}
