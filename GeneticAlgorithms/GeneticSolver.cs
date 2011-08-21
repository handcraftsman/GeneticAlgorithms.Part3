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
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace GeneticAlgorithms
{
    public class GeneticSolver
    {
        private readonly int _numberOfAncestorStrategiesToUse;
        private readonly Random _random = new Random();
        private readonly IGeneticStrategy[] _strategies;
        private RandomStrategy _randomStrategy;

        public GeneticSolver()
        {
            _strategies = (from t in Assembly.GetExecutingAssembly().GetTypes()
                           where t.GetInterfaces().Contains(typeof(IGeneticStrategy))
                           where t.GetConstructor(Type.EmptyTypes) != null
                           select Activator.CreateInstance(t) as IGeneticStrategy).ToArray();

            _numberOfAncestorStrategiesToUse = 3 * _strategies.Length;

            GetCanonicalGenes = genes => genes;
            MaxSecondsWithoutImprovement = 20;
        }

        public Func<string, string> GetCanonicalGenes { private get; set; }
        public double MaxSecondsWithoutImprovement { get; set; }

        private IEnumerable<Individual> GenerateChildren(
            IList<Individual> parents,
            IList<IGeneticStrategy> strategyPool,
            string geneSet)
        {
            int parentBIndex = _random.Next(parents.Count);
            var parentB = parents[parentBIndex];
            while (true)
            {
                int parentAIndex = _random.Next(parents.Count);
                var parentA = parents[parentAIndex];
                var strategy = strategyPool[_random.Next(strategyPool.Count)];
                var child = strategy.CreateChild(parentA, parentB, geneSet);
                child.Genes = GetCanonicalGenes(child.Genes);
                yield return child;
                parentB = parentA;
            }
        }

        private IEnumerable<Individual> GenerateParents(string geneSet)
        {
            Func<Individual> next = () => _randomStrategy.CreateChild(null, null, geneSet);
            var initial = next();
            return initial.Generate(next);
        }

        private static IEnumerable<Individual> GetAncestors(Individual bestIndividual)
        {
            while (bestIndividual != null)
            {
                yield return bestIndividual;
                bestIndividual = bestIndividual.Parent;
            }
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

            var bestParent = parents.First();
            displayChild(generationCount,
                         bestParent.Fitness,
                         bestParent.Genes,
                         bestParent.Strategy.Description);

            var children = GenerateChildren(parents, new[] { _randomStrategy }, geneSet)
                .Where(x => uniqueIndividuals.Add(x.Genes));
            var strategies = _strategies.ToList();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            do
            {
                var potentialParents = new List<Individual>();
                foreach (var child in children
                    .TakeWhile(x => potentialParents.Count < maxIndividualsInPool))
                {
                    child.Fitness = getFitness(child.Genes);
                    potentialParents.Add(child);
                    if (bestParent.Fitness > child.Fitness)
                    {
                        bestParent = child;
                        var ancestors = GetAncestors(bestParent).ToList();
                        strategies = GetStrategyPool(ancestors);
                        if (ancestors.Count > maxIndividualsInPool)
                        {
                            maxIndividualsInPool = ancestors.Count;
                        }
                        displayChild(generationCount,
                                     bestParent.Fitness,
                                     bestParent.Genes,
                                     bestParent.Strategy.Description);
                        stopwatch.Reset();
                        stopwatch.Start();
                    }
                }
                generationCount++;
                parents = parents
                    .Concat(potentialParents)
                    .OrderBy(x => x.Fitness)
                    .Take(maxIndividualsInPool)
                    .ToList();
                children = GenerateChildren(parents, strategies, geneSet)
                    .Where(x => uniqueIndividuals.Add(x.Genes));
            } while (parents[0].Fitness > 0 &&
                     stopwatch.Elapsed.TotalSeconds <= MaxSecondsWithoutImprovement);
            return parents[0].Genes;
        }

        private List<IGeneticStrategy> GetStrategyPool(IEnumerable<Individual> ancestors)
        {
            var ancestorStrategies = ancestors
                .Select(x => x.Strategy)
                .Where(x => x != _randomStrategy)
                .ToList();
            int scaleTo = Math.Min(ancestorStrategies.Count, _numberOfAncestorStrategiesToUse);
            var successfulStrategies = ancestorStrategies
                .GroupBy(x => x)
                .SelectMany(x => Enumerable.Repeat(x.Key, (int)(scaleTo * x.Count() * 1.0 / ancestorStrategies.Count)));

            var strategies = _strategies.Concat(successfulStrategies).ToList();
            return strategies;
        }
    }
}