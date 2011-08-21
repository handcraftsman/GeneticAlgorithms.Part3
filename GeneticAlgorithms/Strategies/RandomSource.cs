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

using FluentAssert;

namespace GeneticAlgorithms
{
    public interface IRandomSource
    {
        int Next(int inclusiveMin, int exclusiveMax);
        int Next(int exclusiveMax);
    }

    public class RandomSource : IRandomSource
    {
        private readonly Random _random = new Random();

        public int Next(int inclusiveMin, int exclusiveMax)
        {
            return _random.Next(inclusiveMin, exclusiveMax);
        }

        public int Next(int exclusiveMax)
        {
            return _random.Next(exclusiveMax);
        }
    }

    internal class TestRandomSource : IRandomSource
    {
        private readonly IList<int> _toReturn;
        private int _index;

        public TestRandomSource(IList<int> toReturn)
        {
            _toReturn = toReturn;
        }

        public int Next(int inclusiveMin, int exclusiveMax)
        {
            int next = _toReturn[_index++];
            next.ShouldBeGreaterThanOrEqualTo(inclusiveMin);
            if (exclusiveMax > 0)
            {
                next.ShouldBeLessThan(exclusiveMax);
            }
            return next;
        }

        public int Next(int exclusiveMax)
        {
            return Next(0, exclusiveMax);
        }
    }
}