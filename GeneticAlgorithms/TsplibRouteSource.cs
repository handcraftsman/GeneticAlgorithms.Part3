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
using System.IO;
using System.Linq;

using NUnit.Framework;

namespace GeneticAlgorithms
{
    public class TsplibRouteSource : IRouteSource
    {
        private const string GenericGeneSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        private readonly Dictionary<char, Point> _pointLookup;

        public TsplibRouteSource(string filepath)
        {
            int pointCount = 0;
            _pointLookup = GetTsplibFilePoints(filepath)
                .ToDictionary(x => GenericGeneSet[pointCount++], x => x);
        }

        public string GeneSet
        {
            get { return new String(_pointLookup.Keys.ToArray()); }
        }

        public string OptimalRoute
        {
            get { return GeneSet; }
        }

        public double GetDistance(char pointA, char pointB)
        {
            return GetDistance(_pointLookup[pointA], _pointLookup[pointB]);
        }

        private static double GetDistance(Point pointA, Point pointB)
        {
            int sideA = pointA.X - pointB.X;
            int sideB = pointA.Y - pointB.Y;
            double sideC = Math.Sqrt(sideA * sideA + sideB * sideB);
            return (int)(.5 + sideC);
        }

        private static IEnumerable<Point> GetTsplibFilePoints(string filepath)
        {
            var separator = new[] { ' ' };
            var pointLookup = File.ReadAllLines(filepath + ".tsp")
                .SkipWhile(x => x != "NODE_COORD_SECTION")
                .Skip(1)
                .TakeWhile(x => x != "EOF")
                .Select(x => x.Split(separator).Select(y => Int32.Parse(y)).ToArray())
                .ToDictionary(x => x[0], x => new Point(x[1], x[2]));

            var optimalPath = File.ReadAllLines(filepath + ".opt.tour")
                .SkipWhile(x => x != "TOUR_SECTION")
                .Skip(1)
                .TakeWhile(x => x != "-1")
                .Select(x => Int32.Parse(x))
                .ToArray();

            return optimalPath.Select(x => pointLookup[x]);
        }
    }

    [TestFixture]
    public class TsplibRouteSourceTests
    {
        public void Benchmark()
        {
            var lengths = new List<int>();
            var routeOptimizationSolver = new RouteOptimizationSolver();
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var tsplibRouteSource = new TsplibRouteSource(@"Data\eil51");
            for (int i = 0; i < 100; i++)
            {
                string result = routeOptimizationSolver.Solve(tsplibRouteSource, 60);
                int length = RouteOptimizationSolver.CalculateRouteLength(result, tsplibRouteSource);
                lengths.Add(length);
                Console.WriteLine("-- round " + (i + 1) + " - " + lengths.Count(x => x == 426) + " optimal in " + stopwatch.Elapsed);

                foreach (var group in lengths.GroupBy(x => x).OrderBy(x => x.Key))
                {
                    Console.WriteLine(group.Key + " - " + group.Count() + " times");
                }
            }
        }
    }
}