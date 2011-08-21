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
using System.Linq;

using FluentAssert;

using NUnit.Framework;

namespace GeneticAlgorithms
{
    public class RandomStrategy : AbstractStrategy, IGeneticStrategy
    {
        private readonly int _length;

        public RandomStrategy(int length)
        {
            _length = length;
        }

        public Individual CreateChild(Individual parentA, Individual parentB, string geneSet)
        {
            return new Individual
            {
                Genes = GenerateSequence(geneSet),
                Strategy = this
            };
        }

        private string GenerateSequence(string geneSet)
        {
            Func<char> next = () => geneSet[Random.Next(0, geneSet.Length)];
            char initial = next();
            return new String(initial.Generate(next).Take(_length).ToArray());
        }
    }

    [TestFixture]
    public class RandomStrategyTests
    {
        private string _geneSet;
        private int _length;
        private List<int> _randomToReturn;
        private Individual _result;
        private RandomStrategy _strategy;

        [SetUp]
        public void Before_each_test()
        {
            _randomToReturn = new List<int>();
        }

        [Test]
        public void Given_length_2_and_geneNumbers_7_2()
        {
            Test.Verify(
                with_length_2,
                with_geneSet_ABCDEFGH,
                random_geneNumber_gets_7,
                random_geneNumber_gets_2,
                when_asked_to_create_child,
                should_return_HC
                );
        }

        [Test]
        public void Given_length_4_and_geneNumbers_3_1_2_3()
        {
            Test.Verify(
                with_length_4,
                with_geneSet_ABCDEFGH,
                random_geneNumber_gets_3,
                random_geneNumber_gets_1,
                random_geneNumber_gets_2,
                random_geneNumber_gets_3,
                when_asked_to_create_child,
                should_return_DBCD
                );
        }

        private void random_geneNumber_gets_1()
        {
            _randomToReturn.Add(1);
        }

        private void random_geneNumber_gets_2()
        {
            _randomToReturn.Add(2);
        }

        private void random_geneNumber_gets_3()
        {
            _randomToReturn.Add(3);
        }

        private void random_geneNumber_gets_7()
        {
            _randomToReturn.Add(7);
        }

        private void should_return_DBCD()
        {
            _result.Genes.ShouldBeEqualTo("DBCD");
        }

        private void should_return_HC()
        {
            _result.Genes.ShouldBeEqualTo("HC");
        }

        private void when_asked_to_create_child()
        {
            var random = new TestRandomSource(_randomToReturn);
            _strategy = new RandomStrategy(_length);
            _strategy.SetRandomSource(random);
            _result = _strategy.CreateChild(null, null, _geneSet);
        }

        private void with_geneSet_ABCDEFGH()
        {
            _geneSet = "ABCDEFGH";
        }

        private void with_length_2()
        {
            _length = 2;
        }

        private void with_length_4()
        {
            _length = 4;
        }
    }
}