using System.Collections.Generic;
using System.Text;
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
    [SerializeField] int numberOfFeatures = 5;
    [SerializeField] int numberOfProfileAxes = 5;
    [SerializeField] int axisSize = 5;

    [SerializeField] int[] profileValues = null;
    
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

        /*foreach(var v in tables)
        {
            Debug.Log("Key: " + v.Key + " - Value: " + v.Value);
        }*/
    }

    void Start()
    {
        random = new System.Random();
        ga = new GeneticAlgorithm<int>(populationSize, numberOfFeatures, random, GetRandomAttributeValue, FitnessFunction, elitism, mutationRate);

        fullPath = Application.persistentDataPath + "/Genetic Save";

        ga.LoadGeneration(fullPath);
    }

    // Update is called once per frame
    void Update()
    {
        ga.NewGeneration();

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
        Debug.Break();
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

        Debug.Log("Fitness score: " + score);

        return score;
    }
}
