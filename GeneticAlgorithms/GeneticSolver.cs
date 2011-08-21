//  * **********************************************************************************
//  * Copyright (c) Clinton Sheppard
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GeneticAlgorithms
{
    public class GeneticSolver
    {
        private readonly Random _random = new Random();
        private readonly IGeneticStrategy[] _strategies;
        private RandomStrategy _randomStrategy;

        public GeneticSolver()
        {
            _strategies = (from t in Assembly.GetExecutingAssembly().GetTypes()
                           where t.GetInterfaces().Contains(typeof(IGeneticStrategy))
                           where t.GetConstructor(Type.EmptyTypes) != null
                           select Activator.CreateInstance(t) as IGeneticStrategy).ToArray();
        }

        private IEnumerable<Individual> GenerateChildren(
            IList<Individual> parents,
            IGeneticStrategy strategy,
            string geneSet)
        {
            int count = 0;
            while (count < parents.Count)
            {
                int parentAIndex = _random.Next(parents.Count);
                int parentBIndex = _random.Next(parents.Count);
                if (parentAIndex == parentBIndex)
                {
                    continue;
                }
                var parentA = parents[parentAIndex];
                var parentB = parents[parentBIndex];
                yield return strategy.CreateChild(parentA, parentB, geneSet);
                count++;
            }
        }

        private IEnumerable<Individual> GenerateParents(string geneSet)
        {
            Func<Individual> next = () => _randomStrategy.CreateChild(null, null, geneSet);
            var initial = next();
            return initial.Generate(next);
        }

        public string GetBest(int length,
                              string geneSet,
                              Func<string, int> getFitness,
                              Action<int, int, string, string> displayChild)
        {
            _randomStrategy = new RandomStrategy(length);

            int maxIndividualsInPool = geneSet.Length * 3;
            int generationCount = 1;
            var uniqueIndividuals = new HashSet<string>();
            var parents = GenerateParents(geneSet)
                .Where(x => uniqueIndividuals.Add(x.Genes))
                .Take(maxIndividualsInPool)
                .ToList();
            foreach (var individual in parents)
            {
                individual.Fitness = getFitness(individual.Genes);
            }
            parents = parents.OrderBy(x => x.Fitness).ToList();

            var worstParent = parents.Last();
            displayChild(generationCount,
                         worstParent.Fitness,
                         worstParent.Genes,
                         worstParent.Strategy.Description);

            int worstParentFitness = worstParent.Fitness;

            IGeneticStrategy strategy = _randomStrategy;
            var children = GenerateChildren(parents, strategy, geneSet);
            do
            {
                var improved = new List<Individual>();
                foreach (var child in children.Where(x => uniqueIndividuals.Add(x.Genes)))
                {
                    child.Fitness = getFitness(child.Genes);
                    if (worstParentFitness >= child.Fitness)
                    {
                        improved.Add(child);
                        if (worstParentFitness > child.Fitness)
                        {
                            displayChild(generationCount,
                                         child.Fitness,
                                         child.Genes,
                                         child.Strategy.Description);
                            worstParentFitness = child.Fitness;
                        }
                    }
                }
                generationCount++;
                if (improved.Any())
                {
                    parents = parents
                        .Concat(improved)
                        .OrderBy(x => x.Fitness)
                        .Take(maxIndividualsInPool)
                        .ToList();
                    children = GenerateChildren(parents, strategy, geneSet);
                }
                else
                {
                    var previousStrategy = strategy;
                    strategy = _strategies
                        .Where(x => !ReferenceEquals(x, previousStrategy))
                        .Shuffle()
                        .First();
                    children = GenerateChildren(parents, strategy, geneSet);
                }
            } while (parents[0].Fitness > 0);
            return parents[0].Genes;
        }
    }
}