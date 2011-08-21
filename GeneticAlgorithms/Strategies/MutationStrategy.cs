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
    public class MutationStrategy : AbstractStrategy, IGeneticStrategy
    {
        public Individual CreateChild(Individual parentA, Individual parentB, string geneSet)
        {
            var parentGenes = parentA.Genes.ToCharArray();
            int location = Random.Next(0, parentGenes.Length);
            parentGenes[location] = geneSet[Random.Next(0, geneSet.Length)];
            return new Individual
            {
                Genes = new String(parentGenes),
                Strategy = this,
                Parent = parentA
            };
        }
    }

    [TestFixture]
    public class MutationStrategyTests
    {
        private string _geneSet;
        private Individual _parentA;
        private List<int> _randomToReturn;
        private Individual _result;
        private MutationStrategy _strategy;

        [SetUp]
        public void Before_each_test()
        {
            _randomToReturn = new List<int>();
            _strategy = new MutationStrategy();
        }

        [Test]
        public void Given_location_0_and_geneNumber_7()
        {
            Test.Verify(
                with_parent_a_as_ABCDEFGH,
                with_geneSet_ABCDEFGH,
                random_location_gets_0,
                random_geneNumber_gets_7,
                when_asked_to_create_child,
                should_return_HBCDEFGH
                );
        }

        [Test]
        public void Given_location_3_and_geneNumber_7()
        {
            Test.Verify(
                with_parent_a_as_ABCDEFGH,
                with_geneSet_ABCDEFGH,
                random_location_gets_3,
                random_geneNumber_gets_7,
                when_asked_to_create_child,
                should_return_ABCHEFGH
                );
        }

        private void random_geneNumber_gets_7()
        {
            _randomToReturn.Add(7);
        }

        private void random_location_gets_0()
        {
            _randomToReturn.Add(0);
        }

        private void random_location_gets_3()
        {
            _randomToReturn.Add(3);
        }

        private void should_return_ABCHEFGH()
        {
            _result.Genes.ShouldBeEqualTo("ABCHEFGH");
        }

        private void should_return_HBCDEFGH()
        {
            _result.Genes.ShouldBeEqualTo("HBCDEFGH");
        }

        private void when_asked_to_create_child()
        {
            var random = new TestRandomSource(_randomToReturn);
            _strategy.SetRandomSource(random);
            _result = _strategy.CreateChild(_parentA, null, _geneSet);
        }

        private void with_geneSet_ABCDEFGH()
        {
            _geneSet = "ABCDEFGH";
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