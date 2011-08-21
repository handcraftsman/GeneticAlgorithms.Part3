//  * **********************************************************************************
//  * Copyright (c) Clinton Sheppard
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **********************************************************************************

using System.Collections.Generic;

using FluentAssert;

using NUnit.Framework;

namespace GeneticAlgorithms
{
    public class ShiftStrategy : AbstractStrategy, IGeneticStrategy
    {
        public Individual CreateChild(Individual parentA, Individual parentB, string geneSet)
        {
            const int charsToShift = 1;
            if (parentA.Genes.Length < charsToShift + 1)
            {
                return parentA;
            }
            bool shiftingPairLeft = Random.Next(2) == 1;
            string childGenes;
            int segmentStart = Random.Next(parentA.Genes.Length - charsToShift - 1);
            int segmentLength = Random.Next(charsToShift + 1, parentA.Genes.Length + 1 - segmentStart);
            string childGenesBefore = parentA.Genes.Substring(0, segmentStart);

            if (shiftingPairLeft)
            {
                string shiftedSegment = parentA.Genes.Substring(segmentStart, segmentLength - charsToShift);
                string shiftedPair = parentA.Genes.Substring(segmentStart + segmentLength - charsToShift, charsToShift);
                string childGenesAfter = parentA.Genes.Substring(segmentStart + segmentLength);
                childGenes = childGenesBefore + shiftedPair + shiftedSegment + childGenesAfter;
            }
            else
            {
                string shiftedPair = parentA.Genes.Substring(segmentStart, charsToShift);
                string shiftedSegment = parentA.Genes.Substring(segmentStart + charsToShift, segmentLength - charsToShift);
                string childGenesAfter = parentA.Genes.Substring(segmentStart + segmentLength);
                childGenes = childGenesBefore + shiftedSegment + shiftedPair + childGenesAfter;
            }
            var child = new Individual
            {
                Genes = childGenes,
                Strategy = this,
                Parent = parentA
            };
            return child;
        }
    }

    [TestFixture]
    public class ShiftStrategyTests
    {
        private Individual _parentA;
        private List<int> _randomToReturn;
        private Individual _result;
        private ShiftStrategy _strategy;

        [SetUp]
        public void Before_each_test()
        {
            _randomToReturn = new List<int>();
            _strategy = new ShiftStrategy();
        }

        [Test]
        public void Given_segmentLength_3_segmentStart_1_shifting_right()
        {
            Test.Verify(
                with_parent_a_as_ABCDEFGH,
                random_shift_direction_gets_Right,
                random_segmentStart_gets_0,
                random_segmentLength_gets_4,
                when_asked_to_create_child,
                should_return_BCDAEFGH
                );
        }

        [Test]
        public void Given_segmentLength_3_segmentStart_1_shifting_left()
        {
            Test.Verify(
                with_parent_a_as_ABCDEFGH,
                random_shift_direction_gets_Left,
                random_segmentStart_gets_1,
                random_segmentLength_gets_4,
                when_asked_to_create_child,
                should_return_AEBCDFGH
                );
        }

        [Test]
        public void Given_segmentLength_Length_minus_1_segmentStart_1_shifting_right()
        {
            Test.Verify(
                with_parent_a_as_ABCDEFGH,
                random_shift_direction_gets_Right,
                random_segmentStart_gets_0,
                random_segmentLength_gets_Length,
                when_asked_to_create_child,
                should_return_BCDEFGHA
                );
        }

        [Test]
        public void Given_segmentLength_Length_minus_1_segmentStart_1_shifting_left()
        {
            Test.Verify(
                with_parent_a_as_ABCDEFGH,
                random_shift_direction_gets_Left,
                random_segmentStart_gets_0,
                random_segmentLength_gets_Length,
                when_asked_to_create_child,
                should_return_HABCDEFG
                );
        }

        private void random_segmentLength_gets_4()
        {
            _randomToReturn.Add(4);
        }

        private void random_segmentLength_gets_Length()
        {
            _randomToReturn.Add(_parentA.Genes.Length);
        }

        private void random_segmentStart_gets_0()
        {
            _randomToReturn.Add(0);
        }

        private void random_segmentStart_gets_1()
        {
            _randomToReturn.Add(1);
        }

        private void random_shift_direction_gets_Left()
        {
            _randomToReturn.Add(1);
        }

        private void random_shift_direction_gets_Right()
        {
            _randomToReturn.Add(0);
        }

        private void should_return_AEBCDFGH()
        {
            _result.Genes.ShouldBeEqualTo("AEBCDFGH");
        }

        private void should_return_BCDAEFGH()
        {
            _result.Genes.ShouldBeEqualTo("BCDAEFGH");
        }

        private void should_return_BCDEFGHA()
        {
            _result.Genes.ShouldBeEqualTo("BCDEFGHA");
        }

        private void should_return_HABCDEFG()
        {
            _result.Genes.ShouldBeEqualTo("HABCDEFG");
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