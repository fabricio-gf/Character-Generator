using System;
using System.Collections.Generic;

public class GeneticAlgorithm<T>
{
    public List<DNA<T>> Population { get; private set; }
    public int Generation { get; private set; }
    public float BestFitness { get; private set; }
    public T[] BestGenes { get; private set; }

    public int Elitism;
    public float MutationRate;
    private List<DNA<T>> newPopulation;
    private Random random;
    private float fitnessSum;
    private int dnaSize;
    private Func<T> getRandomGene;
    private Func<int, float> fitnessFunction;

    public GeneticAlgorithm(int populationSize, int dnaSize, Random random, Func<T> getRandomGene, Func<int, float> fitnessFunction, int elitism, float mutationRate = 0.01f)
    {
        Generation = 1;
        Elitism = elitism;
        MutationRate = mutationRate;
        Population = new List<DNA<T>>(populationSize);
        newPopulation = new List<DNA<T>>(populationSize);
        this.random = random;
        this.dnaSize = dnaSize;
        this.getRandomGene = getRandomGene;
        this.fitnessFunction = fitnessFunction;

        BestGenes = new T[dnaSize];

        for(int i = 0; i < populationSize; i++)
        {
            Population.Add(new DNA<T>(dnaSize, random, getRandomGene, fitnessFunction, shouldInitGenes: true));
        }
    }

    // Creates a new population of individuals
    public void NewGeneration(int numNewDNA = 0, bool crossoverNewDNA = false)
    {
        int finalCount = Population.Count + numNewDNA;

        if(finalCount <= 0)
        {
            return;
        }

        // Sorts the population based on individual fitness
        if (Population.Count > 0)
        {
            CalculateFitness();
            Population.Sort(CompareDNA);
        }
        newPopulation.Clear();


        for(int i = 0; i < finalCount; i++)
        {
            // If elitism is enabled, copies the top individuals to the new population
            if (i < Elitism && i < Population.Count)
            {
                newPopulation.Add(Population[i]);
            }
            // Then, executes crossover on the old populations individuals to form a new population
            else if(i < Population.Count || crossoverNewDNA)
            {
                DNA<T> parent1 = ChooseParent();
                DNA<T> parent2 = ChooseParent();

                DNA<T> child = parent1.Crossover(parent2);

                child.Mutate(MutationRate);

                newPopulation.Add(child);
            }
            // Creates new individuals from scratch if the population is increasing
            else
            {
                newPopulation.Add(new DNA<T>(dnaSize, random, getRandomGene, fitnessFunction, shouldInitGenes: true));
            }

        }

        List<DNA<T>> tmpList = Population;
        Population = newPopulation;
        newPopulation = tmpList;

        Generation++;
    }

    public void SaveGeneration(string filePath)
    {
        GeneticSaveData<T> save = new GeneticSaveData<T>
        {
            Generation = Generation,
            PopulationGenes = new List<T[]>(Population.Count),

        };

        for( int i = 0; i < Population.Count; i++)
        {
            save.PopulationGenes.Add(new T[dnaSize]);
            Array.Copy(Population[i].Genes, save.PopulationGenes[i], dnaSize);
        }

        FileReadWrite.WriteToBinaryFile(filePath, save);
    }

    public bool LoadGeneration(string filePath)
    {
        if (!System.IO.File.Exists(filePath))
        {
            return false;
        }

        GeneticSaveData<T> save = FileReadWrite.ReadFromBinaryFile<GeneticSaveData<T>>(filePath);
        Generation = save.Generation;
        for(int i = 0; i < save.PopulationGenes.Count; i++)
        {
            if(i >= Population.Count)
            {
                Population.Add(new DNA<T>(dnaSize, random, getRandomGene, fitnessFunction, shouldInitGenes: false));
            }
            Array.Copy(save.PopulationGenes[i], Population[i].Genes, dnaSize);
        }
        return true;
    }

    public int CompareDNA(DNA<T> a, DNA<T> b)
    {
        if(a.Fitness > b.Fitness)
        {
            return -1;
        }
        else if (a.Fitness < b.Fitness)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public void CalculateFitness()
    {
        fitnessSum = 0;
        DNA<T> best = Population[0];

        for(int i = 0; i < Population.Count; i++)
        {
            fitnessSum += Population[i].CalculateFitness(i);

            if(Population[i].Fitness > best.Fitness)
            {
                best = Population[i];
            }
        }

        BestFitness = best.Fitness;
        best.Genes.CopyTo(BestGenes, 0); 
    }

    // Atualmente é feito por rankeamento. Porém, pode causar overfitting, cair no máximo local. Pode ser ruim, considerar mudar
    private DNA<T> ChooseParent()
    {
        double randomNumber = random.NextDouble() * fitnessSum;

        for(int i = 0; i < Population.Count; i++)
        {
            if(randomNumber < Population[i].Fitness)
            {
                return Population[i];
            }
            randomNumber -= Population[i].Fitness;

        }

        return Population[0];
    }
}
