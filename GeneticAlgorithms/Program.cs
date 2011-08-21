﻿//  * **********************************************************************************
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
    internal class Program
    {
        public static void Main()
        {
            new RouteOptimizationSolver().Solve(new TsplibRouteSource(@"Data\eil51"), 3);
//            new TsplibRouteSourceTests().Benchmark();
        }
    }
}