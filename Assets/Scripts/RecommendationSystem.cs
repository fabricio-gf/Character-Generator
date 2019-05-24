using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class RecommendationSystem : MonoBehaviour
{
    [SerializeField] CharacterGenerator generator = null;
    [SerializeField] WindowBehaviours windowBehaviours = null;

    List<DNA<int>> topOptions = new List<DNA<int>>();
    private int currentIndex = 0;
    private int iterationNumber = 0;

    private int optionIndex = 0;

    StringBuilder sb = new StringBuilder();
    GeneticAlgorithm<int> localGA = null;

    [SerializeField] private Sprite[] iconList = null;

    [Header("References")]
    [Header("Generation Screen")]
    [SerializeField] private Text[] texts = null;
    [SerializeField] private Text generationText = null;
    [SerializeField] private Text fitnessText = null;
    [SerializeField] private Text genesText = null;
    [SerializeField] private Text titleText = null;
    [SerializeField] private Image icon = null;

    [Header("Results Screen")]
    [SerializeField] private Text[] resultsTexts = null;
    [SerializeField] private Text resultsTitleText = null;
    [SerializeField] private Image resultsIcon = null;


    /*[SerializeField] private GameObject GoodButton = null;
    [SerializeField] private GameObject BadButton = null;
    [SerializeField] private GameObject NextGenerationButton = null;
    */

    // METHODS DURING CHARACTER GENERATIONS
    public void ShowTopThree(GeneticAlgorithm<int> ga)
    {
        localGA = ga;
        localGA.Population.Sort(ga.CompareDNA);
        if (iterationNumber == 3)
        {
            localGA.Population[0].CalculateFitness(0);
            topOptions.Add(localGA.Population[0]);
            ShowFinalScreen();
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                localGA.Population[i].CalculateFitness(i);
                topOptions.Add(localGA.Population[i]);
            }

            currentIndex = 0;
            ShowNextResult();
        }
    }

    public void ShowNextResult()
    {
        if(currentIndex + iterationNumber*3 < topOptions.Count)
        {
            UpdateTexts(topOptions[currentIndex + iterationNumber*3].Genes, localGA.Generation, topOptions[currentIndex + iterationNumber*3].Fitness);
            currentIndex++;
        }
        else
        {
            if (iterationNumber < 3)
            {
                iterationNumber++;
                generator.NextGenerationBatch();
            }
        }
    }

    // METHODS FOR RESULTS SCREEN
    private void ShowFinalScreen()
    {
        windowBehaviours.ChangeActiveWindow(3);
        optionIndex = topOptions.Count - 1;
        ShowOption(optionIndex);
    }

    private void ShowOption(int index)
    {
        sb.Clear();
        sb.Append("Personagem: ");
        sb.Append(optionIndex+1);
        resultsTitleText.text = sb.Append("/10").ToString();

        resultsIcon.sprite = iconList[topOptions[index].Genes[0]];

        for(int i = 0; i < resultsTexts.Length; i++)
        {
            resultsTexts[i].text = generator.featureValues[i][topOptions[index].Genes[i]];
        }
    }

    public void ChangeOption(int change)
    {
        optionIndex += change;
        if(optionIndex >= 10)
        {
            optionIndex = 0;
        }
        else if(optionIndex < 0)
        {
            optionIndex = 9;
        }
        ShowOption(optionIndex);
    }

    public void ExportToPDF()
    {
        Debug.Log("Export to pdf");
    }

    public void GoToQuestionnaire()
    {
        Debug.Log("https://forms.gle/ohMJ8MYWKa3KWayJ7");
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLScshIlTAedg8HS6dm4Cr8LvuX5PYJhNAictWinqkShs3j6hpA/viewform?usp=sf_link");
    }

    public void ReturnToMenu()
    {
        topOptions.Clear();
        currentIndex = 0;
        iterationNumber = 0;
        optionIndex = 0;
    }

    // Updates UI text on screen
    private void UpdateTexts(int[] individual, int generation, float fitness)
    {
        sb.Clear();
        sb.Append(currentIndex+1+(iterationNumber*3));
        titleText.text = sb.Append("º personagem gerado:").ToString();

        UpdateIcon(individual[0]);

        sb.Clear();
        sb.Append("Genes: ");
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].text = generator.featureValues[i][individual[i]];
            sb.Append(individual[i]);
            if (i < texts.Length - 1)
                sb.Append(" | ");
        }
        genesText.text = sb.ToString();

        sb.Clear();
        sb.Append("Generation: ");
        generationText.text = sb.Append(generation).ToString();

        sb.Clear();
        sb.Append("Fitness: ");
        fitnessText.text = sb.Append(fitness).ToString();


    }

    private void UpdateIcon(int index)
    {
        icon.sprite = iconList[index];
    }
}
