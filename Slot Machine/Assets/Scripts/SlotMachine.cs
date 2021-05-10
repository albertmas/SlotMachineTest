using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Class that controls the Slot Machine behaviour.
/// </summary>
public class SlotMachine : MonoBehaviour
{
    [Tooltip("Time between the activation of each roller.")]
    public float rollersOffsetTime = 0.2f;
    [Tooltip("Rollers to spin.")]
    public Spin[] rollers;
    [Tooltip("Time each reward is shown.")]
    public float rewardTime = 1.5f;

    [Space, Tooltip("Button to activate the rollers.")]
    public Button spinButton;
    [Tooltip("GameObject with the last credits won.")]
    public GameObject creditsWon;
    [Tooltip("Text with the total credits won.")]
    public TextMeshProUGUI totalCredits;

    [Header("Rewards   [ Double | Triple | Quadruple ]")]
    [Tooltip("Rewards the for 2, 3 or 4 bells patterns.")]
    public Vector3Int bellsRewards;
    [Tooltip("Rewards the for 2, 3 or 4 watermelon patterns.")]
    public Vector3Int watermelonRewards;
    [Tooltip("Rewards the for 2, 3 or 4 grapes patterns.")]
    public Vector3Int grapesRewards;
    [Tooltip("Rewards the for 2, 3 or 4 eggplant patterns.")]
    public Vector3Int eggplantRewards;
    [Tooltip("Rewards the for 2, 3 or 4 orange patterns.")]
    public Vector3Int orangeRewards;
    [Tooltip("Rewards the for 2, 3 or 4 lemon patterns.")]
    public Vector3Int lemonRewards;
    [Tooltip("Rewards the for 2, 3 or 4 cherry patterns.")]
    public Vector3Int cherryRewards;

    int credits = 0; // Total credits won
    bool machineReady = true; // When deactivated the spin button can't be pressed
    FigureTypes[,] figuresCombination; // 2D Array with the figure combination from the last spin
    Dictionary<FigureTypes, Vector3Int> figureRewards; // Dictionary to figures with their 3 possible rewards

    void Start()
    {
        figuresCombination = new FigureTypes[rollers.Length, 3];

        // Create dictionary with the rewards for each figure
        figureRewards = new Dictionary<FigureTypes, Vector3Int>();
        figureRewards.Add(FigureTypes.bell, bellsRewards);
        figureRewards.Add(FigureTypes.watermelon, watermelonRewards);
        figureRewards.Add(FigureTypes.grapes, grapesRewards);
        figureRewards.Add(FigureTypes.eggplant, eggplantRewards);
        figureRewards.Add(FigureTypes.orange, orangeRewards);
        figureRewards.Add(FigureTypes.lemon, lemonRewards);
        figureRewards.Add(FigureTypes.cherry, cherryRewards);
    }

    void Update()
    {
        // Spin rollers when pressing SPIN or space
        if (machineReady && Input.GetKeyDown(KeyCode.Space))
            SpinRollers();

        // Quit app with Esc
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
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

        // Start a coroutine to evaluate the figure combinations after the spinning time
        IEnumerator evaluation = EvaluateCombinations(spinTime + 1.5f);
        StartCoroutine(evaluation);
    }

    /// <summary>
    /// Allow the player to spin again the slot machine
    /// </summary>
    void MachineReady()
    {
        machineReady = true;
        spinButton.interactable = true;
    }

    /// <summary>
    /// Coroutine that evaluates each possible patter one by one.
    /// If a pattern contains a reward, the execution waits for the rewarded figures to be shown before resuming.
    /// </summary>
    /// <param name="warmUpTime">Time that the rollers take to spin and stop.</param>
    /// <returns></returns>
    private IEnumerator EvaluateCombinations(float warmUpTime = 0f)
    {
        yield return new WaitForSeconds(warmUpTime);

        // Get each roller's result
        for (int i = 0; i < rollers.Length; i++)
        {
            rollers[i].GetResult(out FigureTypes[] result);
            figuresCombination[i, 0] = result[0];
            figuresCombination[i, 1] = result[1];
            figuresCombination[i, 2] = result[2];
        }

        // Check TOP line
        if (EvaluateLinePattern(0))
            yield return new WaitForSeconds(rewardTime);
        // Check MIDDLE line
        if (EvaluateLinePattern(1))
            yield return new WaitForSeconds(rewardTime);
        // Check BOTTOM line
        if (EvaluateLinePattern(2))
            yield return new WaitForSeconds(rewardTime);
        // Check W pattern
        if (EvaluateWPattern())
            yield return new WaitForSeconds(rewardTime);
        // Check V pattern
        if (EvaluateVPattern())
            yield return new WaitForSeconds(rewardTime);

        // Allow the machine to be spinned again
        MachineReady();

        yield return null;
    }

    /// <summary>
    /// Check a line for a winning combination.
    /// </summary>
    /// <param name="line">Line to check. Top = 0, Mid = 1 and Bottom = 2</param>
    /// <returns>Returns true if there is a winning combination.</returns>
    bool EvaluateLinePattern(int line)
    {
        // First figure in the line is saved
        FigureTypes figure = figuresCombination[0, line];
        int figureCount = 1;
        int reward = 0;

        // Each subsequent figure is checked
        for (int i = 1; i < rollers.Length; i++)
        {
            // If figures match, count is increased
            if (figuresCombination[i, line] == figure && figureCount < 4)
            {
                figureCount++;
            }
            else
            {
                break;
            }
        }

        // Show rewards if there is a winning combination
        if (figureCount > 1)
        {
            // Calculate rewards for the figure pattern
            if (figureRewards.TryGetValue(figure, out Vector3Int rewards))
                reward = rewards[figureCount - 2];

            // Show reward credits and frame the figures in the combination
            ShowRewardFigures(line, figureCount);
            UpdateCredits(reward);
        }

        return reward > 0;
    }

    /// <summary>
    /// Check the W pattern for a winning combination.
    /// </summary>
    /// <returns>Returns true if there is a winning combination.</returns>
    bool EvaluateWPattern()
    {
        int reward = 0;
        FigureTypes figure = figuresCombination[0, 0];
        // List for the pattern figures
        List<Vector2Int> figuresList = new List<Vector2Int>();
        figuresList.Add(new Vector2Int(0, 0));

        // Checks each figure in the pattern and adds it if they match.
        if (figuresCombination[1, 2] == figure)
        {
            figuresList.Add(new Vector2Int(1, 2));

            if (figuresCombination[2, 0] == figure)
            {
                figuresList.Add(new Vector2Int(2, 0));

                if (figuresCombination[3, 2] == figure)
                {
                    figuresList.Add(new Vector2Int(3, 2));
                }
            }
        }

        // Show rewards if there is a winning combination
        if (figuresList.Count > 1)
        {
            // Calculate rewards for the figure pattern
            if (figureRewards.TryGetValue(figure, out Vector3Int rewards))
                reward = rewards[figuresList.Count - 2];

            // Show reward credits and frame the figures in the combination
            ShowRewardFigures(figuresList);
            UpdateCredits(reward);
        }

        return reward > 0;
    }



    /// <summary>
    /// Check the V pattern for a winning combination.
    /// </summary>
    /// <returns>Returns true if there is a winning combination.</returns>
    bool EvaluateVPattern()
    {
        int reward = 0;
        FigureTypes figure = figuresCombination[0, 0];
        // List for the pattern figures
        List<Vector2Int> figuresList = new List<Vector2Int>();
        figuresList.Add(new Vector2Int(0, 0));

        // Checks each figure in the pattern and adds it if they match.
        if (figuresCombination[1, 1] == figure)
        {
            figuresList.Add(new Vector2Int(1, 1));

            if (figuresCombination[2, 2] == figure)
            {
                figuresList.Add(new Vector2Int(2, 2));

                if (figuresCombination[3, 1] == figure)
                {
                    figuresList.Add(new Vector2Int(3, 1));
                }
            }
        }

        // Show rewards if there is a winning combination
        if (figuresList.Count > 1)
        {
            // Calculate rewards for the figure pattern
            if (figureRewards.TryGetValue(figure, out Vector3Int rewards))
                reward = rewards[figuresList.Count - 2];

            // Show reward credits and frame the figures in the combination
            ShowRewardFigures(figuresList);
            UpdateCredits(reward);
        }

        return reward > 0;
    }

    /// <summary>
    /// Show figures in the winning combination with a frame.
    /// </summary>
    /// <param name="line">Line of the combination.</param>
    /// <param name="count">Number of figures in the winning combination.</param>
    void ShowRewardFigures(int line, int count)
    {
        for (int n = 0; n < count; n++)
        {
            rollers[n].FrameResultFigure(line, rewardTime);
        }
    }

    /// <summary>
    /// Show figures in the winning combination with a frame.
    /// </summary>
    /// <param name="figures">Figures to frame.</param>
    void ShowRewardFigures(List<Vector2Int> figures)
    {
        for (int i = 0; i < figures.Count; i++)
        {
            rollers[figures[i].x].FrameResultFigure(figures[i].y, rewardTime);
        }
    }

    /// <summary>
    /// Show last won credits on screen.
    /// Update total credits counter.
    /// </summary>
    /// <param name="newCredits">Last credits won.</param>
    void UpdateCredits(int newCredits)
    {
        // Update last credits won
        creditsWon.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = newCredits.ToString();

        // Change the colour of the highlight according to the amount won
        if (newCredits <= 15)
            creditsWon.GetComponent<Image>().color = new Color(0.94f, 1f, 0.04f, 0.6f);
        else if (newCredits <= 50)
            creditsWon.GetComponent<Image>().color = new Color(0.04f, 0.55f, 1f, 0.6f);
        else
            creditsWon.GetComponent<Image>().color = new Color(1f, 0.04f, 0.24f, 0.6f);

        // Update total credits
        credits += newCredits;
        // Update total credits in screen
        totalCredits.text = credits.ToString();
    }
}
