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

using NUnit.Framework;

namespace GeneticAlgorithms
{
    public class ReverseStrategy : AbstractStrategy, IGeneticStrategy
    {
        public Individual CreateChild(Individual parentA, Individual parentB, string geneSet)
        {
            int reversePointA = Random.Next(parentA.Genes.Length);
            int reversePointB = Random.Next(parentA.Genes.Length);
            if (reversePointA == reversePointB)
            {
                reversePointB = Random.Next(parentA.Genes.Length);
                if (reversePointA == reversePointB)
                {
                    return parentA;
                }
            }
            int min = Math.Min(reversePointA, reversePointB);
            int max = Math.Max(reversePointA, reversePointB);
            var childGenes = parentA.Genes.ToCharArray();
            for (int i = 0; i <= (max - min) / 2; i++)
            {
                int lowIndex = i + min;
                int highIndex = max - i;
                char temp = childGenes[lowIndex];
                childGenes[lowIndex] = childGenes[highIndex];
                childGenes[highIndex] = temp;
            }
            var child = new Individual
            {
                Genes = new String(childGenes),
                Strategy = this,
                Parent = parentA
            };
            return child;
        }
    }

    [TestFixture]
    public class ReverseStrategyTests
    {
        private Individual _parentA;
        private List<int> _randomToReturn;
        private Individual _result;
        private ReverseStrategy _strategy;

        [SetUp]
        public void Before_each_test()
        {
            _randomToReturn = new List<int>();
            _strategy = new ReverseStrategy();
        }

        [Test]
        public void Given_reversePointB_equal_reversePointA_twice()
        {
            Test.Verify(
                with_parent_a_as_ABCDEFGH,
                random_reversePointA_gets_2,
                random_reversePointB_gets_2,
                random_reversePointB_gets_2,
                when_asked_to_create_child,
                should_return_parent_a
                );
        }

        [Test]
        public void Given_reversePointB_equal_to_then_higher_than_reversePointA()
        {
            Test.Verify(
                with_parent_a_as_ABCDEFGH,
                random_reversePointA_gets_0,
                random_reversePointB_gets_0,
                random_reversePointB_gets_3,
                when_asked_to_create_child,
                should_return_DCBAEFGH
                );
        }

        [Test]
        public void Given_reversePointB_equal_to_then_lower_than_reversePointA()
        {
            Test.Verify(
                with_parent_a_as_ABCDEFGH,
                random_reversePointA_gets_2,
                random_reversePointB_gets_2,
                random_reversePointB_gets_0,
                when_asked_to_create_child,
                should_return_CBADEFGH
                );
        }

        [Test]
        public void Given_reversePointB_higher_than_reversePointA()
        {
            Test.Verify(
                with_parent_a_as_ABCDEFGH,
                random_reversePointA_gets_0,
                random_reversePointB_gets_3,
                when_asked_to_create_child,
                should_return_DCBAEFGH
                );
        }

        [Test]
        public void Given_reversePointB_lower_than_reversePointA_and_difference_is_even()
        {
            Test.Verify(
                with_parent_a_as_ABCDEFGH,
                random_reversePointA_gets_3,
                random_reversePointB_gets_0,
                when_asked_to_create_child,
                should_return_DCBAEFGH
                );
        }

        [Test]
        public void Given_reversePointB_lower_than_reversePointA_and_difference_is_odd()
        {
            Test.Verify(
                with_parent_a_as_ABCDEFGH,
                random_reversePointA_gets_2,
                random_reversePointB_gets_0,
                when_asked_to_create_child,
                should_return_CBADEFGH
                );
        }

        private void random_reversePointA_gets_0()
        {
            _randomToReturn.Add(0);
        }

        private void random_reversePointA_gets_2()
        {
            _randomToReturn.Add(2);
        }

        private void random_reversePointA_gets_3()
        {
            _randomToReturn.Add(3);
        }

        private void random_reversePointB_gets_0()
        {
            _randomToReturn.Add(0);
        }

        private void random_reversePointB_gets_2()
        {
            _randomToReturn.Add(2);
        }

        private void random_reversePointB_gets_3()
        {
            _randomToReturn.Add(3);
        }

        private void should_return_CBADEFGH()
        {
            _result.Genes.ShouldBeEqualTo("CBADEFGH");
        }

        private void should_return_DCBAEFGH()
        {
            _result.Genes.ShouldBeEqualTo("DCBAEFGH");
        }

        private void should_return_parent_a()
        {
            _result.ShouldBeSameInstanceAs(_parentA);
        }

        private void when_asked_to_create_child()
        {
            var random = new TestRandomSource(_randomToReturn);
            _strategy.SetRandomSource(random);
            _result = _strategy.CreateChild(_parentA, null, null);
        }

        private void with_parent_a_as_ABCDEFGH()
        {
            _parentA = new Individual
            {
                Genes = "ABCDEFGH"
            };
        }
    }
}