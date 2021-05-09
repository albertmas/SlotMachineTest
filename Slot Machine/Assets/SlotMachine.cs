using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that controls the Slot Machine behaviour.
/// </summary>
public class SlotMachine : MonoBehaviour
{
    [Tooltip("Time between the activation of each roller.")]
    public float rollersOffsetTime = 0.2f;
    [Tooltip("Rollers to spin.")]
    public Spin[] rollers;
    [Tooltip("Button to activate the rollers.")]
    public Button spinButton;

    bool machineReady = true;

    void Update()
    {
        // Spin rollers when pressing SPIN or space
        if (machineReady && Input.GetKeyDown(KeyCode.Space))
            SpinRollers();
    }

    /// <summary>
    /// Spin each roller from left to right
    /// </summary>
    public void SpinRollers()
    {
        // Get a random spin time between two and four seconds
        float spinTime = Random.Range(2.0f, 4.0f);

        // Start spinning each roller, from left to right with a offset time between them
        for (int i = 0; i < rollers.Length; i++)
        {
            rollers[i].ActivateRoller(spinTime, rollersOffsetTime * i);
        }

        // Disable spinning until current spin is over
        machineReady = false;
        spinButton.interactable = false;
        Invoke(nameof(MachineReady), spinTime + 1.5f);
    }

    /// <summary>
    /// Allow the player to spin again the slot machine
    /// </summary>
    void MachineReady()
    {
        machineReady = true;
        spinButton.interactable = true;
    }
}
