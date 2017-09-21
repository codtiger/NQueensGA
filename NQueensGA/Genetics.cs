using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Xml.Serialization;
using NUnit.Framework;

namespace NQueensGA
{
    class Genetics
    {
        private Chromosome[] currentpopulation;
        private List<Chromosome> nextGen = new List<Chromosome>();
        private int boardSize = 8;
        private int popSize = 200;
        public int PopSize { get { return popSize;} }
        private double mutationRate = 0.2;
        private double crossoverRate = 0.6;
        public double MutationRate { get { return mutationRate;} }
        public double CrossOverRate { get { return crossoverRate; } }
        public int BoardSize { get { return boardSize;
            
        } }
        public Genetics(int initPopSize,double crossoverRate,double mutationRate,int boardSize)
        {
            currentpopulation=new Chromosome[initPopSize];
            popSize = initPopSize;
            this.mutationRate = mutationRate;
            this.crossoverRate = crossoverRate;
            this.boardSize = boardSize;
        }

       public  Genetics(Chromosome[] Population, double crossoverRate, double mutationRate)
        {
            if(Population==null)
                throw new NullReferenceException();
            popSize = Population.Length;
            currentpopulation = Population;
            this.mutationRate = mutationRate;
            this.crossoverRate = crossoverRate;
            this.boardSize = Population[0].Dimension;
        }

        public void InitPop()
        {
            
            for(int i=0;i<popSize;i++)
               currentpopulation[i]=new Chromosome(BoardSize); 
        }

        public Chromosome Start(int iterations)
        {
            
            if (currentpopulation[0]==null)
                InitPop();
            Chromosome BestChromosome = currentpopulation[0];
            Random rng = new Random();
            nextGen.AddRange(currentpopulation);
            Chromosome[] selected=new Chromosome[10];
            for (int i = 0; i < iterations; i++)
            {
                for (int j = 0; j < CrossOverRate * popSize; j++)
                {
                    for (int k = 0; k < 10; k++)
                        selected[k] = currentpopulation[rng.Next(0, popSize)];
                    var selectedTwo=Tournament(selected);
                    Chromosome child=selectedTwo.Item1.Crossover(selectedTwo.Item2);
                    nextGen.Add(child);
                    child=child.Mutation(MutationRate);
                    nextGen.Add(child);
                }
                nextGen=nextGen.OrderByDescending(a => a.Fitness()).ToList();
                nextGen=nextGen.GetRange(0, popSize);
                currentpopulation = nextGen.ToArray();
                if (currentpopulation[0] .Fitness()> BestChromosome.Fitness())
                    BestChromosome = currentpopulation[0];
            }
            return BestChromosome;
        }
        private Tuple<Chromosome,Chromosome> Tournament(Chromosome[] selected)
        {
            Chromosome max1 = null, max2 = null;
            int cmp=0, cmp1 = 0;
            foreach (var i in selected)
            {
                if (i.Fitness() >= cmp)
                {
                    max1 = i;
                    cmp = i.Fitness();
                    ///max1 steps down to max2
                    max2 = max1;
                    cmp1 = max1.Fitness();
                }
                else if (i.Fitness() >= cmp1)
                {
                    max2 = i;
                    cmp1 = i.Fitness();
                }

            }
            return new Tuple<Chromosome, Chromosome>(max1,max2);
        }
    }
}
