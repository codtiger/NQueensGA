using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace NQueensGA
{
    class Chromosome
    {
        private static Random rng=new Random();
        private int dimension=8;
        private int[] genes;
        public int Dimension { get {return dimension;;}
            private set
        {
            dimension = value;genes=new int[value];
            }
        }
        public int[] Genes { get { return genes; } }
        public Chromosome(int n)
        {
            Dimension = n;
            InitializeGenes();
        }

        public Chromosome(int[] genes)
        {
 
            dimension = genes.Length;
            this.genes = genes;
        }
        private void InitializeGenes()
        {
            List<int> numbers = new List<int>();
           
            for(int i=0;i<dimension;i++)
                numbers.Add(i);
            for (int i = dimension - 1; i >= 0; i--)
            {
                int random = (rng.Next(0, i+1));
                genes[i] = numbers[random];
                numbers.RemoveAt(random);
            }


        }
                    

        public void SetGenes(int[] genes)
        {
            this.genes = genes;
        }
        public Chromosome Crossover(Chromosome second)
        {
            if (second.Dimension != this.Dimension)
                throw new Exception("Chromosome Dimensions not equal");
            int dim = second.Dimension;
            int division = Convert.ToInt32(Math.Ceiling(dim / 2.0f));
            int [] newGenes=new int[dimension];
            int randomNum = rng.Next() % 2;
            int [] Occupied=new int[dimension];
            /////////////this is for the first half of the chromosome
            if (randomNum==0)
            {
                for (int i = 0; i < division; i++)
                {
                    newGenes[i] = this.Genes[i];
                    Occupied[genes[i]] = 1;
                }   
            }
            else
            {
                for (int i = 0; i < division; i++)
                {
                    newGenes[i] = second.genes[i];
                    Occupied[second.genes[i]] = 1;
                }
            }
            ////////////this one for the second half and checking the occupied positions
            for (int i = division; i < dim; i++)
            {
                /////if the first half of first chromosome was chosen
                if (randomNum == 0)
                {
                    int j = i;
                    if (Occupied[second.genes[i]] == 1)
                        for (j = 0; j < dim; j++)
                            if (Occupied[second.genes[j]] != 1)
                            {
                                break;

                            }
                    newGenes[i] = second.genes[j];
                    Occupied[second.genes[j]] = 1;
                }
                else
                {
                    int j = i;
                    if (Occupied[this.genes[i]] == 1)
                        for (j = 0; j < dim; j++)
                            if (Occupied[this.genes[j]] != 1)
                            {
                                break;

                            }
                    newGenes[i] = this.genes[j];
                    Occupied[this.genes[j]] = 1;
                }

            }
            return new Chromosome(newGenes);
        }

        public Chromosome Mutation(double MutationRate)
        {
            int []newgenes=new int[dimension];
            genes.CopyTo(newgenes,0);
            for (int i = 0; i < dimension; i++)
            {
                if (rng.Next() % 100 > MutationRate * 100)
                {
                    continue;
                }
                else
                {
                    int index = rng.Next(0, dimension);
                    int hold = newgenes[i];
                    newgenes[i] = newgenes[index];
                    newgenes[index] = hold;
                }
            }
            return new Chromosome(newgenes);
        }
        public int Fitness()
        {
            int clash = 0;
            for (int i = 0; i < dimension; i++)
            for (int j = i + 1; j < dimension; j++)
            {
                if (  Math.Abs(genes[j] - genes[i]) == Math.Abs(j - i)) clash++;
            }
            return (dimension * (dimension - 1)) / 2 - clash;
        }

        public override string ToString()
        {
           StringBuilder s=new StringBuilder();
            for (int i = 0; i < genes.Length; i++)
                s.Append(genes[i].ToString());
            s.Append("\n" + "Fitness:" + Fitness().ToString());
            return s.ToString();
        }
    }
}
