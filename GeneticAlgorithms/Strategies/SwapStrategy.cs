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
    public class SwapStrategy : AbstractStrategy, IGeneticStrategy
    {
        public Individual CreateChild(Individual parentA, Individual parentB, string geneSet)
        {
            int swapPointA = Random.Next(parentA.Genes.Length);
            int swapPointB = Random.Next(parentA.Genes.Length);
            if (swapPointA == swapPointB)
            {
                swapPointB = Random.Next(parentA.Genes.Length);
                if (swapPointA == swapPointB)
                {
                    return parentA;
                }
            }
            var childGenes = parentA.Genes.ToCharArray();
            char temp = childGenes[swapPointA];
            childGenes[swapPointA] = childGenes[swapPointB];
            childGenes[swapPointB] = temp;
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
    public class SwapStrategyTests
    {
        private Individual _parentA;
        private List<int> _randomToReturn;
        private Individual _result;
        private SwapStrategy _strategy;

        [SetUp]
        public void Before_each_test()
        {
            _randomToReturn = new List<int>();
            _strategy = new SwapStrategy();
        }

        [Test]
        public void Given_swapPointB_equal_to_swapPointA_twice()
        {
            Test.Verify(
                with_parent_a_as_ABCDEFGH,
                random_swapPointA_gets_3,
                random_swapPointB_gets_3,
                random_swapPointB_gets_3,
                when_asked_to_create_child,
                should_return_parent_a
                );
        }

        [Test]
        public void Given_swapPointB_equal_to_then_higher_than_swapPointA()
        {
            Test.Verify(
                with_parent_a_as_ABCDEFGH,
                random_swapPointA_gets_0,
                random_swapPointA_gets_0,
                random_swapPointB_gets_3,
                when_asked_to_create_child,
                should_return_DBCAEFGH
                );
        }

        [Test]
        public void Given_swapPointB_equal_to_then_lower_than_swapPointA()
        {
            Test.Verify(
                with_parent_a_as_ABCDEFGH,
                random_swapPointA_gets_3,
                random_swapPointB_gets_3,
                random_swapPointB_gets_0,
                when_asked_to_create_child,
                should_return_DBCAEFGH
                );
        }

        [Test]
        public void Given_swapPointB_higher_than_swapPointA()
        {
            Test.Verify(
                with_parent_a_as_ABCDEFGH,
                random_swapPointA_gets_0,
                random_swapPointB_gets_3,
                when_asked_to_create_child,
                should_return_DBCAEFGH
                );
        }

        [Test]
        public void Given_swapPointB_lower_than_swapPointA()
        {
            Test.Verify(
                with_parent_a_as_ABCDEFGH,
                random_swapPointA_gets_3,
                random_swapPointB_gets_0,
                when_asked_to_create_child,
                should_return_DBCAEFGH
                );
        }

        private void random_swapPointA_gets_0()
        {
            _randomToReturn.Add(0);
        }

        private void random_swapPointA_gets_3()
        {
            _randomToReturn.Add(3);
        }

        private void random_swapPointB_gets_0()
        {
            _randomToReturn.Add(0);
        }

        private void random_swapPointB_gets_3()
        {
            _randomToReturn.Add(3);
        }

        private void should_return_DBCAEFGH()
        {
            _result.Genes.ShouldBeEqualTo("DBCAEFGH");
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