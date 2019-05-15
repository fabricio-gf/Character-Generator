﻿using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class CharacterGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Text[] texts = null;
    [SerializeField] private Text generationText = null;
    [SerializeField] private Text fitnessText = null;
    [SerializeField] private Text genesText = null;

    [Header("GeneticAlgorithm")]
    [SerializeField] int populationSize = 5;
    [SerializeField] float mutationRate = 0.01f;
    [SerializeField] int elitism = 1;
    [SerializeField] int GenerationBatch = 10;

    [Header("Recommendation System")]
    [SerializeField] int numberOfFeatures = 6;
    [SerializeField] int numberOfProfileAxes = 1;
    [SerializeField] int axisSize = 5;

    //[SerializeField] int[] profileValues = null;
    public static int[] profileValues = null;

    Dictionary<string, int[,]> tables = new Dictionary<string, int[,]>();

    [Header("Tables")]
    [SerializeField] string[] profileAxes = null;
    [SerializeField] string[] features = null;
    string[][] featureValues = null; 

    // OTHER
    private GeneticAlgorithm<int> ga;
    private System.Random random;
    private string fullPath;
    private StringBuilder sb = new StringBuilder();

    void Awake()
    {
        //load matrixes from file to memory
        var files = Resources.LoadAll<TextAsset>("Tables/");
        string[] lines;
        int[,] matrix;
        string[] temp;
        foreach (var v in files)
        {
            lines = v.text.Split('\n');

            matrix = new int[lines.Length, axisSize];

            for (int i = 0; i < lines.Length; i++)
            {
                temp = lines[i].Split(',');
                for (int j = 0; j < temp.Length; j++)
                {
                    matrix[i, j] = int.Parse(temp[j]);
                }
            }

            tables.Add(v.name, matrix);
        }

        featureValues = new string[numberOfFeatures][];
        files = Resources.LoadAll<TextAsset>("FeatureValues/");
        int k = 0;
        foreach (var v in files)
        {
            lines = v.text.Split('\n');
            featureValues[k] = lines;
            k++;
        }
    }

    void Start()
    {
        random = new System.Random();
        ga = new GeneticAlgorithm<int>(populationSize, numberOfFeatures, random, GetRandomAttributeValue, FitnessFunction, elitism, mutationRate);
    }

    // Update is called once per frame
    void Update()
    {
        //ga.NewGeneration();

        /*
        sb.Clear();
        string bestGenes;

        sb.Append("| ");
        for (int i = 0; i < numberOfFeatures; i++) {
            sb.Append(ga.BestGenes[i]);
            sb.Append(" | ");
        }
        bestGenes = sb.ToString();
        Debug.Log("Best genes in generation " + ga.Generation + " : " + bestGenes);

        for(int i = 0; i < featureValues.Length; i++)
        {
            Debug.Log("Selected value: " + featureValues[i][ga.BestGenes[i]]);
            texts[i].text = featureValues[i][ga.BestGenes[i]];
        }
        Debug.Break();*/
        //update text

        /*if (ga.BestFitness == 1)
        {
            this.enabled = false;
        }*/

        /*if (ga.Generation % 10 == 0)
        {
            ga.SaveGeneration(fullPath);
            this.enabled = false;
        }*/
    }

    public void LoadGeneration(string path)
    {
        DefineGenerationPath(path);
        ga.LoadGeneration(fullPath);
    }

    public void DefineGenerationPath(string path)
    {
        fullPath = path;
    }

    public void NewGeneration()
    {
        ga.NewGeneration();

        UpdateTexts(ga);
    }

    public void NextGenerationBatch()
    {
        ga.NewGeneration();

        while(ga.Generation % GenerationBatch != 0)
        {
            ga.NewGeneration();
        }
        UpdateTexts(ga);

        ga.SaveGeneration(fullPath);
    }

    private int GetRandomAttributeValue()
    {
        int size = 5;
        int i = random.Next(size);
        return i;
    }

    private float FitnessFunction(int index)
    {
        float score = 0;
        DNA<int> dna = ga.Population[index];
        string tableName;
        int[,] tempTable;

        for(int i = 0; i < profileAxes.Length; i++)
        {
            for(int j = 0; j < features.Length; j++)
            {
                sb.Clear();
                sb.Append(profileAxes[i]);
                sb.Append("_");
                tableName = sb.Append(features[j]).ToString();

                tempTable = tables[tableName];
                score += tempTable[dna.Genes[j],profileValues[i]];
            }
        }

        //score = (Mathf.Pow(2, score) - 1) / (2 - 1);

        //Debug.Log("Fitness score: " + score);

        return score;
    }

    public void UpdateTables(int valueChange)
    {
        int[] bestGenes = ga.BestGenes;
        string tableName;

        for (int i = 0; i < profileAxes.Length; i++)
        {
            for (int j = 0; j < features.Length; j++)
            {
                sb.Clear();
                sb.Append(profileAxes[i]);
                sb.Append("_");
                tableName = sb.Append(features[j]).ToString();

                tables[tableName][bestGenes[j], profileValues[i]] += valueChange;
            }
        }
    }

    private void UpdateTexts(GeneticAlgorithm<int> ga)
    {
        sb.Clear();
        sb.Append("Genes: ");
        for(int i = 0; i < texts.Length; i++)
        {
            texts[i].text = featureValues[i][ga.BestGenes[i]];
            sb.Append(ga.BestGenes[i]);
            if(i < texts.Length-1)
                sb.Append(" | ");
        }
        genesText.text = sb.ToString();

        sb.Clear();
        sb.Append("Generation: ");
        generationText.text = sb.Append(ga.Generation).ToString();

        sb.Clear();
        sb.Append("Fitness: ");
        fitnessText.text = sb.Append(ga.BestFitness).ToString();


    }   
}
