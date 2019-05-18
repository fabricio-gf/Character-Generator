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

    StringBuilder sb = new StringBuilder();
    GeneticAlgorithm<int> localGA = null;

    [Header("References")]
    [SerializeField] private Text[] texts = null;
    [SerializeField] private Text generationText = null;
    [SerializeField] private Text fitnessText = null;
    [SerializeField] private Text genesText = null;

    /*[SerializeField] private GameObject GoodButton = null;
    [SerializeField] private GameObject BadButton = null;
    [SerializeField] private GameObject NextGenerationButton = null;
    */

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

    private void ShowFinalScreen()
    {
        windowBehaviours.ChangeActiveWindow(3);
        ShowListOfOptions();
    }

    private void ShowListOfOptions()
    {
        Debug.Break();
    }

    public void ExportToPDF(int index)
    {

    }

    // Updates UI text on screen
    private void UpdateTexts(int[] individual, int generation, float fitness)
    {
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
