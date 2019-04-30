using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestCharacterGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Text[] texts = null;

    [Header("GeneticAlgorithm")]
    [SerializeField] int populationSize = 200;
    [SerializeField] float mutationRate = 0.01f;
    [SerializeField] int elitism = 5;

    [Header("Recommendation System")]
    [SerializeField] int numberOfFeatures = 10;
    [SerializeField] int numberOfProfileAxes = 5;
    Dictionary<string, int[,]> tables = new Dictionary<string, int[,]>();

    // OTHER
    private GeneticAlgorithm<int> ga;
    private System.Random random;
    private string fullPath;

    void Awake()
    {
        //load matrixes from file to memory
        var files = Resources.LoadAll<TextAsset>("");
        string[] lines;
        int[,] matrix;
        string[] temp;
        foreach (var v in files)
        {
            lines = v.text.Split('\n');

            matrix = new int[lines.Length, numberOfProfileAxes];

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

        foreach(var v in tables)
        {
            Debug.Log("Key: " + v.Key + " - Value: " + v.Value);
        }
    }

    void Start()
    {
        /*random = new System.Random();
        ga = new GeneticAlgorithm<int>(populationSize, numberOfAttributes, random, GetRandomAttributeValue, FitnessFunction, elitism, mutationRate);

        fullPath = Application.persistentDataPath + "/Genetic Save";

        ga.LoadGeneration(fullPath);*/
    }

    // Update is called once per frame
    void Update()
    {
        //ga.NewGeneration();

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

    private int GetRandomAttributeValue()
    {
        int size = 10;
        int i = random.Next(size);
        return i;
    }

    private float FitnessFunction(int index)
    {
        float score = 0;
        DNA<int> dna = ga.Population[index];

        /*for (int i = 0; i < dna.Genes.Length; i++)
        {
            if (dna.Genes[i] == targetString[i])
            {
                score += 1;
            }
        }

        score /= targetString.Length;*/

        score = (Mathf.Pow(2, score) - 1) / (2 - 1);

        return score;
    }
}
