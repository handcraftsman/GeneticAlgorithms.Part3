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

using FluentAssert;

using NUnit.Framework;

namespace GeneticAlgorithms
{
    public class RouteOptimizationSolver
    {
        private static int CalculateRouteLength(
            string sequence,
            IRouteSource routeSource)
        {
            var points = sequence.ToList();
            int distinctCount = points.Distinct().Count();
            points.Add(points[0]);
            double routeLength = points
                .Select((x, i) => i == 0 ? 0 : routeSource.GetDistance(x, points[i - 1]))
                .Sum();
            int fitness =
                1000 * (sequence.Length - distinctCount)
                + (int)Math.Floor(routeLength);
            return fitness;
        }

        private static void Display(int generation, int fitness, string sequence, string strategy, Stopwatch stopwatch)
        {
            Console.WriteLine("generation {0} fitness {1} {2}  elapsed: {3} by {4}",
                              generation.ToString().PadLeft(5, ' '),
                              fitness.ToString().PadLeft(5, ' '),
                              GetCanonicalGenes(sequence),
                              stopwatch.Elapsed,
                              strategy);
        }

        public static string GetCanonicalGenes(string input)
        {
            char first = input.OrderBy(x => x).First();
            string doubledForRotation = input + input;
            string forward = (-1).GenerateFrom(x => doubledForRotation.IndexOf(first, x + 1))
                .Skip(1)
                .TakeWhile(x => x < input.Length)
                .Select(x => doubledForRotation.Substring(x, input.Length))
                .OrderBy(x => x)
                .First();

            string reverseDoubledForRotation = new String(doubledForRotation.Reverse().ToArray());
            string backward = (-1).GenerateFrom(x => reverseDoubledForRotation.IndexOf(first, x + 1))
                .Skip(1)
                .TakeWhile(x => x < input.Length)
                .Select(x => reverseDoubledForRotation.Substring(x, input.Length))
                .OrderBy(x => x)
                .First();

            return String.Compare(forward, backward) == -1 ? forward : backward;
        }

        public string Solve(IRouteSource routeSource, int maxSecondsWithoutImprovement)
        {
            if (routeSource.OptimalRoute != null)
            {
                Console.WriteLine("Expect optimal route " + routeSource.OptimalRoute
                                  + " to have fitness " + CalculateRouteLength(routeSource.OptimalRoute, routeSource));
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var geneticSolver = new GeneticSolver
            {
                GetCanonicalGenes = GetCanonicalGenes,
                MaxSecondsWithoutImprovement = maxSecondsWithoutImprovement
            };
            string result = geneticSolver.GetBest(
                routeSource.GeneSet.Length,
                routeSource.GeneSet,
                child => CalculateRouteLength(child, routeSource),
                (generation, fitness, genes, strategy) => Display(generation, fitness, genes, strategy, stopwatch));
            Console.WriteLine(GetCanonicalGenes(result));

            return GetCanonicalGenes(result);
        }
    }

    public class RouteOptimizationSolverTests
    {
        [TestFixture]
        public class GetCanonicalGenes
        {
            [Test]
            public void Should_find_the_optimal_route_with_a_Circle_route_source()
            {
                var routeSource = new CircleRouteSource();
                string result = new RouteOptimizationSolver().Solve(routeSource, 3);
                result.ShouldBeEqualTo(routeSource.OptimalRoute);
            }

            [Test]
            public void Should_return_the_same_result_for_all_rotations_and_reversals_of__abcdef()
            {
                var sequences = new[]
                    {
                        "abcdef", "bcdefa", "cdefab",
                        "defabc", "efabcd", "fabcde",
                        "fedcba", "edcbaf", "dcbafe",
                        "cbafed", "bafedc", "afedcb",
                    };
                foreach (string sequence in sequences)
                {
                    RouteOptimizationSolver.GetCanonicalGenes(sequence)
                        .ShouldBeEqualTo("abcdef");
                }
            }
        }
    }
}