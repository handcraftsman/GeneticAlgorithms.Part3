//  * **********************************************************************************
//  * Copyright (c) Clinton Sheppard
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **********************************************************************************
namespace GeneticAlgorithms
{
    public abstract class AbstractStrategy
    {
        protected IRandomSource Random;
        private string _description;

        protected AbstractStrategy()
        {
            SetRandomSource(new RandomSource());
        }

        public string Description
        {
            get { return _description ?? (_description = GetType().Name.Replace("Strategy", "")); }
        }

        internal void SetRandomSource(IRandomSource randomSource)
        {
            Random = randomSource;
        }
    }
}