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
using System.Drawing;
using System.Linq;

using FluentAssert;

using NUnit.Framework;

namespace GeneticAlgorithms
{
    public class RouteOptimizationSolver
    {
        private const string GeneSet = "abcdefghijklmnopqrstuvwxyz*";
        private const int Radius = 20;

        private static int CalculateRouteLength(
            IEnumerable<char> sequence,
            IDictionary<char, Point> pointLookup)
        {
            var points = sequence.Select(x => pointLookup[x]).ToList();
            int distinctCount = points.Distinct().Count();
            points.Add(points[0]);
            double routeLength = points
                .Select((x, i) => i == 0 ? 0 : GetDistance(x, points[i - 1]))
                .Sum();
            int fitness =
                1000 * (pointLookup.Count - distinctCount)
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

        private static double GetDistance(Point pointA, Point pointB)
        {
            int sideA = pointA.X - pointB.X;
            int sideB = pointA.Y - pointB.Y;
            double sideC = Math.Sqrt(sideA * sideA + sideB * sideB);
            return sideC;
        }

        public IList<Point> GetNPointsOnCircle(Point center, int radius, int n)
        {
            double alpha = Math.PI * 2 / (n + 1);
            var points = new List<Point>(n);
            int maxArrayIndex = 2 * radius - 1;
            Func<int, int> forceInBounds = x => Math.Min(Math.Max(0, x), maxArrayIndex);
            int i = n / 2;
            while (i < 3 * n / 2)
            {
                double theta = alpha * i++;
                int x = (int)(Math.Cos(theta) * radius);
                int y = (int)(Math.Sin(theta) * radius);
                var pointOnCircle = new Point(forceInBounds(center.X + x), forceInBounds(center.Y + y));
                points.Add(pointOnCircle);
            }

            return points;
        }

        public string Solve()
        {
            int pointOffset = 0;
            var pointLookup = GetNPointsOnCircle(new Point(Radius, Radius), Radius, GeneSet.Length - 1)
                .ToDictionary(x => GeneSet[pointOffset++], x => x);
            pointLookup.Add('*', new Point(0, pointLookup['a'].Y));

            Console.WriteLine("Expect optimal route " + GeneSet + " to have fitness " + CalculateRouteLength(GeneSet, pointLookup));

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var geneticSolver = new GeneticSolver
                {
                    GetCanonicalGenes = GetCanonicalGenes
                };
            string result = geneticSolver.GetBest(
                GeneSet.Length,
                GeneSet,
                child => CalculateRouteLength(child, pointLookup),
                (generation, fitness, genes, strategy) => Display(generation, fitness, genes, strategy, stopwatch));
            Console.WriteLine(result);

            return result;
        }
    }

    public class RouteOptimizationSolverTests
    {
        [TestFixture]
        public class GetCanonicalGenes
        {
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