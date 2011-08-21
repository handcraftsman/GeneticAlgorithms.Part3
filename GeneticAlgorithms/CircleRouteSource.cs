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
using System.Drawing;
using System.Linq;

namespace GeneticAlgorithms
{
    public class CircleRouteSource : IRouteSource
    {
        private const int Radius = 20;
        private readonly Dictionary<char, Point> _pointLookup;

        public CircleRouteSource()
        {
            int pointOffset = 0;
            _pointLookup = GetNPointsOnCircle(new Point(Radius, Radius), Radius, GeneSet.Length - 1)
                .ToDictionary(x => GeneSet[pointOffset++], x => x);
            _pointLookup.Add('*', new Point(0, _pointLookup['a'].Y));
        }

        public string GeneSet
        {
            get { return "abcdefghijklmnopqrstuvwxyz*"; }
        }

        public double GetDistance(char pointA, char pointB)
        {
            return GetDistance(_pointLookup[pointA], _pointLookup[pointB]);
        }

        public string OptimalRoute
        {
            get { return "*azyxwvutsrqponmlkjihgfedcb"; }
        }

        private static double GetDistance(Point pointA, Point pointB)
        {
            int sideA = pointA.X - pointB.X;
            int sideB = pointA.Y - pointB.Y;
            double sideC = Math.Sqrt(sideA * sideA + sideB * sideB);
            return sideC;
        }

        private static IEnumerable<Point> GetNPointsOnCircle(Point center, int radius, int n)
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
    }
}