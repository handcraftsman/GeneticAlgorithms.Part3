﻿//  * **********************************************************************************
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
            NumberOfParentLines = 2;
        }

        public Func<string, string> GetCanonicalGenes { private get; set; }
        public double MaxSecondsWithoutImprovement { get; set; }
        public int NumberOfParentLines { get; set; }

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

            int lastParentLine = NumberOfParentLines - 1;
            int currentParentLine = lastParentLine;

            var parentLines = Enumerable.Range(0, NumberOfParentLines)
                .Select(x => parents.ToList())
                .ToArray();

            var strategies = Enumerable.Range(0, NumberOfParentLines)
                .Select(x => _strategies.ToList())
                .ToArray();

            double halfMaxAllowableSecondsWithoutImprovement = MaxSecondsWithoutImprovement / 2;
            IEqualityComparer<Individual> individualGenesComparer = new IndividualGenesComparer();

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
                        strategies[currentParentLine] = GetStrategyPool(ancestors);
                        if (ancestors.Count > maxIndividualsInPool)
                        {
                            maxIndividualsInPool = ancestors.Count;
                        }
                        displayChild(generationCount,
                                     bestParent.Fitness,
                                     bestParent.Genes,
                                     bestParent.Strategy.Description);
                    }
                }
                generationCount++;
                parentLines[currentParentLine] = parentLines[currentParentLine]
                    .Concat(potentialParents)
                    .OrderBy(x => x.Fitness)
                    .Take(maxIndividualsInPool)
                    .ToList();

                if (parentLines[currentParentLine][0].Fitness == bestParent.Fitness ||
                    stopwatch.Elapsed.TotalSeconds < halfMaxAllowableSecondsWithoutImprovement)
                {
                    if (--currentParentLine < 0)
                    {
                        currentParentLine = lastParentLine;
                    }
                }
                else
                {
                    parentLines[currentParentLine] = parentLines
                        .SelectMany(x => x.Take(5))
                        .Distinct(individualGenesComparer)
                        .ToList();
                }

                children = GenerateChildren(parentLines[currentParentLine], strategies[currentParentLine], geneSet)
                    .Where(x => uniqueIndividuals.Add(GetCanonicalGenes(x.Genes)));
            } while (bestParent.Fitness > 0 &&
                     stopwatch.Elapsed.TotalSeconds <= MaxSecondsWithoutImprovement);

            return bestParent.Genes;
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