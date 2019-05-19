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

    [Header("References")]
    [Header("Generation Screen")]
    [SerializeField] private Text[] texts = null;
    [SerializeField] private Text generationText = null;
    [SerializeField] private Text fitnessText = null;
    [SerializeField] private Text genesText = null;
    [SerializeField] private Text titleText = null;

    [Header("Results Screen")]
    [SerializeField] private Text[] resultsTexts = null;
    [SerializeField] private Text resultsTitleText = null;


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
            topOptions.Add(localGA.Population[0]);
            ShowFinalScreen();
        }
        else
        {
            iterationNumber++;
            
            for (int i = 0; i < 3; i++)
            {
                topOptions.Add(localGA.Population[i]);
            }

            currentIndex = 0;
            ShowNextResult();
        }
    }

    public void ShowNextResult()
    {
        if(currentIndex * iterationNumber < topOptions.Count)
        {
            UpdateTexts(topOptions[currentIndex*iterationNumber].Genes, localGA.Generation, topOptions[currentIndex*iterationNumber].Fitness);
            currentIndex++;
        }
        else
        {
            if (iterationNumber <= 3)
            {
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
        sb.Append(optionIndex+1);
        resultsTitleText.text = sb.Append("º personagem gerado:").ToString();

        for(int i = 0; i < resultsTexts.Length; i++)
        {
            resultsTexts[i].text = generator.featureValues[i][topOptions[currentIndex].Genes[i]];
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

    public void ReturnToMenu()
    {
        Debug.Log("Return to menu");
    }

    // Updates UI text on screen
    private void UpdateTexts(int[] individual, int generation, float fitness)
    {
        sb.Clear();
        sb.Append(((currentIndex+1)+((iterationNumber-1)*3)));
        titleText.text = sb.Append("º personagem gerado:").ToString();

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
}
