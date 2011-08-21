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
    public class CrossoverStrategy : AbstractStrategy, IGeneticStrategy
    {
        public Individual CreateChild(Individual parentA, Individual parentB, string geneSet)
        {
            int crossOverPoint = Random.Next(parentA.Genes.Length);
            var childGenes = parentA.Genes.ToCharArray();
            var parentBGenes = parentB.Genes.ToCharArray();
            childGenes[crossOverPoint] = parentBGenes[crossOverPoint];
            var child = new Individual
            {
                Genes = new String(childGenes),
                Strategy = this
            };
            return child;
        }
    }

    [TestFixture]
    public class CrossoverStrategyTests
    {
        private Individual _parentA;
        private Individual _parentB;
        private List<int> _randomToReturn;
        private Individual _result;
        private CrossoverStrategy _strategy;

        [SetUp]
        public void Before_each_test()
        {
            _randomToReturn = new List<int>();
            _strategy = new CrossoverStrategy();
        }

        [Test]
        public void Given_crossOverPoint_3()
        {
            Test.Verify(
                with_parent_a_as_ABCDEFGH,
                with_parent_b_as_QWERTYUI,
                random_crossOverPoint_gets_3,
                when_asked_to_create_child,
                should_return_ABCREFGH
                );
        }

        [Test]
        public void Given_crossOverPoint_5()
        {
            Test.Verify(
                with_parent_a_as_ABCDEFGH,
                with_parent_b_as_QWERTYUI,
                random_crossOverPoint_gets_5,
                when_asked_to_create_child,
                should_return_ABCDEYGH
                );
        }

        private void random_crossOverPoint_gets_3()
        {
            _randomToReturn.Add(3);
        }

        private void random_crossOverPoint_gets_5()
        {
            _randomToReturn.Add(5);
        }

        private void should_return_ABCDEYGH()
        {
            _result.Genes.ShouldBeEqualTo("ABCDEYGH");
        }

        private void should_return_ABCREFGH()
        {
            _result.Genes.ShouldBeEqualTo("ABCREFGH");
        }

        private void when_asked_to_create_child()
        {
            var random = new TestRandomSource(_randomToReturn);
            _strategy.SetRandomSource(random);
            _result = _strategy.CreateChild(_parentA, _parentB, null);
        }

        private void with_parent_a_as_ABCDEFGH()
        {
            _parentA = new Individual
            {
                Genes = "ABCDEFGH"
            };
        }

        private void with_parent_b_as_QWERTYUI()
        {
            _parentB = new Individual
            {
                Genes = "QWERTYUI"
            };
        }
    }
}