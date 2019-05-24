using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class CharacterGenerator : MonoBehaviour
{

    [Header("GeneticAlgorithm")]
    [SerializeField] int populationSize = 5;
    [SerializeField] float mutationRate = 0.01f;
    [SerializeField] int elitism = 1;
    [SerializeField] int GenerationBatch = 10;

    [Header("Recommendation System")]
    [SerializeField] private RecommendationSystem recommendationSystem = null;
    [SerializeField] int numberOfFeatures = 6;
    [SerializeField] int numberOfProfileAxes = 6;
    [SerializeField] int axisSize = 5;

    public static int[] profileValues = null;

    Dictionary<string, int[,]> tables = new Dictionary<string, int[,]>();

    [Header("Tables")]
    [SerializeField] string[] profileAxes = null;
    [SerializeField] string[] features = null;
    [HideInInspector] public string[][] featureValues = null; 

    // OTHER
    private GeneticAlgorithm<int> ga;
    private System.Random random;
    private string fullPath;
    private StringBuilder sb = new StringBuilder();

    // USED TO WRITE THE GENERATION VALUES IN A FILE (on windows, located at "C:\Users\<user>\AppData\LocalLow\FabricioGuedes\Generations_Visualization")
    string testFileName = "Generations_Visualization";
    List<string> testLines = new List<string>();
    string testTablesFileName = "Tables_Visualization";
    private string profile = null;

    void Awake()
    {
        //load weight matrixes from file to memory
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

        //loads feature values to memory
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

        testLines.AddRange(ConvertGenerationToArray());
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

        testLines.AddRange(ConvertGenerationToArray());
    }

    public void NextGenerationBatch()
    {
        testLines.Clear();
        NewGeneration();

        while(ga.Generation % GenerationBatch != 0)
        {
            NewGeneration();
        }
        recommendationSystem.ShowTopThree(ga);

        GenerationsToFile(testLines.ToArray());

        ga.SaveGeneration(fullPath);
    }

    // Generates a random number between 0 and the number of available values for the gene
    private int GetRandomAttributeValue(int currentGene)
    {
        int size = featureValues[currentGene].Length;
        //int size = 7;
        int i = random.Next(size);
        return i;
    }

    // To calculate the individual's fitness, checks on each table the value corresponding to the generated characteristic and answered profile attribute. Simple sum of all values to get the fitness
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
                //print("Table name " + tableName + " size " + tempTable.Length + " index " + dna.Genes[j]);
                score += tempTable[dna.Genes[j],profileValues[i]];
            }
        }

        //score = (Mathf.Pow(2, score) - 1) / (2 - 1);
        return score;
    }

    // Updates weighted tables based on user evaluation of top individual
    public void UpdateTables(int valueChange, int[] currentGenes)
    {
        string tableName;

        for (int i = 0; i < profileAxes.Length; i++)
        {
            for (int j = 0; j < features.Length; j++)
            {
                sb.Clear();
                sb.Append(profileAxes[i]);
                sb.Append("_");
                tableName = sb.Append(features[j]).ToString();

                tables[tableName][currentGenes[j], profileValues[i]] += valueChange;
            }
        }

        SaveTables(profile, false);
    }

    // METHODS USED TO SAVE/LOAD TABLES INFO

    public void SaveTables(string profileName, bool newTables)
    {
        profile = profileName;
        string filePath = Path.Combine(Application.persistentDataPath, "Tables", profileName, profileName);
        
        foreach(var v in tables)
        {
            sb.Clear();
            sb.Append(filePath);
            sb.Append("_" + v.Key);

            FileReadWrite.WriteToBinaryFile(sb.ToString(), v.Value);
        }

        TablesToFile();
    }

    public void LoadTables(string profileName)
    {
        profile = profileName;
        string filePath = Path.Combine(Application.persistentDataPath, "Tables", profileName, profileName);

        List<string> keyList = new List<string>();
        List<int[,]> valueList = new List<int[,]>();

        foreach (var v in tables)
        {
            keyList.Add(v.Key);

            sb.Clear();
            sb.Append(filePath);
            sb.Append("_" + v.Key);
            //Debug.Log("Load path: " + sb.ToString());

            valueList.Add(FileReadWrite.ReadFromBinaryFile<int[,]>(sb.ToString()));
        }
        
        for(int i = 0; i < keyList.Count; i++)
        {
            tables[keyList[i]] = valueList[i];
        }

        TablesToFile();
    }

    private void TablesToFile()
    {
        List<string> linesList = new List<string>();
        string currentLine = null;
        string name = null;
        int[,] tempTable = null;
        string fileName = null;

        for (int i = 0; i < profileAxes.Length; i++)
        {
            for(int j = 0; j < features.Length; j++)
            {
                name = profileAxes[i] + "_" + features[j];
                tempTable = tables[name];

                currentLine = "Table: " + name;
                linesList.Add(currentLine);
                currentLine = "";
                linesList.Add(currentLine);
                for (int k = 0; k < tempTable.GetLength(0); k++)
                {
                    currentLine = featureValues[j][k] + " | ";
                    for(int l = 0; l < tempTable.GetLength(1); l++)
                    {
                        currentLine += tempTable[k, l];
                        if (l < tempTable.GetLength(1) - 1)
                        {
                            currentLine += " | ";
                        }
                    }
                    linesList.Add(currentLine);
                }
                currentLine = "";
                linesList.Add(currentLine);
                currentLine = "------------------------------";
                linesList.Add(currentLine);
                currentLine = "";
                linesList.Add(currentLine);
            }
        }

        fileName = Path.Combine(Application.persistentDataPath, testTablesFileName + ".txt");

        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }

        File.Create(fileName).Dispose();
        File.WriteAllLines(fileName, linesList.ToArray());


    }

    // METHODS USED TO WRITE GENERATIONS INFO TO FILE

    private string[] ConvertGenerationToArray()
    {
        string[] lines = new string[ga.Population.Count+4];
        int l = 0;

        /*for(int i = 0; i < GenerationBatch; i++)
        {*/
            lines[l] = "Generation " + ga.Generation + ": ";
            l++;
            lines[l] = "--------------------";
            l++;

            for (int j = 0; j < ga.Population.Count; j++)
            {
                lines[l] = "Individual " + j + ": ";
                for(int k = 0; k < ga.Population[j].Genes.Length; k++)
                {
                    lines[l] += ga.Population[j].Genes[k];
                    if (k < ga.Population[j].Genes.Length - 1)
                    {
                        lines[l] += " | ";
                    }
                }
                l++;
            }
            lines[l] = "-------------------";
            l++;
            lines[l] = "";
            l++;
        //}

        return lines;
    }

    private void GenerationsToFile(string[] lines)
    {
        int i = lines.Length;
        Array.Resize<string>(ref lines, i + 3);
        i++;
        lines[i] = "Mutation rate: " + ga.MutationRate.ToString();
        i++;
        lines[i] = "Elitism: " + ga.Elitism.ToString();

        string fileName = Path.Combine(Application.persistentDataPath, testFileName+".txt");

        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }

        File.Create(fileName).Dispose();
        File.WriteAllLines(fileName, lines);
    }
}
