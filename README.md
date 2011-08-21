## Travelling Salesperson Problem - Genetic Algorithms 3

This is the C# source code for [part 3 of my series on genetic algorithms][part3].

In this part we extend the genetic solver from [part 2][part2] to be able to solve the [Travelling Salesperson Problem][wikipedia] which belongs to a more generic class of real-world problems related to route or sequence optimization.  

We start with a series of points laid out in a circle 

	......................*.................
	........................................
	..................b...a.................
	..............c.........................
	........................................
	..........d...................z.........
	........................................
	.......e.........................y......
	........................................
	........................................
	........................................
	....f...............................x...
	........................................
	........................................
	........................................
	..g...................................w.
	........................................
	........................................
	........................................
	..h...................................v.
	........................................
	........................................
	........................................
	..i...................................u.
	........................................
	........................................
	........................................
	...j.................................t..
	........................................
	........................................
	........................................
	.....k.............................s....
	........................................
	........................................
	........l.......................r.......
	........................................
	............m...............q...........
	........................................
	................n.......p...............
	....................o...................

	* => home base
	
and update the solver to support this kind of problem.	

Sample output:

	Expect optimal route abcdefghijklmnopqrstuvwxyz* to have fitness 122
	generation     1 fitness  5752 *nzficw*vxjtlrkozbrhpgfyuep  elapsed: 00:00:00.0557000 by Random
	generation     4 fitness  4680 *kjdaizfvftwcngkrohsuybeumo  elapsed: 00:00:00.0745969 by Crossover
	generation     8 fitness  4654 *orkgncwtfvfziadjkhomuebyus  elapsed: 00:00:00.0833367 by Swap
	generation    10 fitness  4647 *adcxwkuqgpkumkheflyscoribn  elapsed: 00:00:00.0870942 by Crossover
	generation    10 fitness  4583 *orkgncwtfifzvadjkhomuebyus  elapsed: 00:00:00.0873425 by Swap
	generation    12 fitness  4578 *orkgxcwtfifzvadjkhomuebyus  elapsed: 00:00:00.0919437 by Mutation
	generation    13 fitness  4546 *adcxwvuqggpirocslyfehkmukn  elapsed: 00:00:00.0932691 by Mutation
	generation    14 fitness  3738 *ieuyrgphrbnokrltjxfswcvpqz  elapsed: 00:00:00.0943789 by Mutation
	generation    15 fitness  3680 *kjdaizfvftwcngkrohsxybeumo  elapsed: 00:00:00.0980285 by Mutation
	generation    16 fitness  3638 *elmohtjdkizfvftwcagnrobsuy  elapsed: 00:00:00.0994994 by Mutation
	generation    18 fitness  3620 *feuyrgjhrbnoxrltpkimwcvwqz  elapsed: 00:00:00.1022795 by Mutation
	generation    19 fitness  3606 *ieuykgptrbnokrlhjxfcwsvpqz  elapsed: 00:00:00.1044059 by Swap
	generation    19 fitness  3588 *elmohijdktzfvftwcagnrobsuy  elapsed: 00:00:00.1053475 by Swap
	generation    21 fitness  3538 *adcxwkvqgpkumkheflysnoribc  elapsed: 00:00:00.1086238 by Mutation
	generation    22 fitness  2661 *emlohtjvfzskdftwcagnrqbiuy  elapsed: 00:00:00.1102894 by Mutation
	generation    22 fitness  2611 *ieuydgptrbnokxlhjrfcwsvpqz  elapsed: 00:00:00.1107309 by Mutation
	generation    23 fitness  2610 *ieuydgptrbnokwlhjrfcxsvpqz  elapsed: 00:00:00.1129340 by Swap
	generation    24 fitness  2569 *ieuydgptrbnokwvsxcfrjhlpqz  elapsed: 00:00:00.1138274 by Reverse
	generation    26 fitness  2545 *amlohtjifzskdftwcegnrqbvuy  elapsed: 00:00:00.1177485 by Swap
	generation    30 fitness  2533 *amlhotjifzskdftwcegnrqbvuy  elapsed: 00:00:00.1252988 by Swap
	generation    31 fitness  2514 *ieuydgptvbnorxlhjkfcwsrpqz  elapsed: 00:00:00.1265510 by Swap
	generation    31 fitness  1640 *dqpvpxcfrlhjwkonbmstgzyuei  elapsed: 00:00:00.1279769 by Mutation
	generation    36 fitness  1635 *cqpvpxdfrlhjwkonbmstgzyuei  elapsed: 00:00:00.1349471 by Swap
	generation    36 fitness  1537 *amlthojifcwxfdkszegnrqbvuy  elapsed: 00:00:00.1354383 by Mutation
	generation    38 fitness  1521 *agnrqzvuoljdcbhtfisymkefxw  elapsed: 00:00:00.1383850 by Mutation
	generation    41 fitness  1518 *ablthojifcwxfdkszegnrqmvuy  elapsed: 00:00:00.1439563 by Swap
	generation    42 fitness  1510 *agnrqxvuoljdcbhtfisymkefzw  elapsed: 00:00:00.1451785 by Swap
	generation    42 fitness  1498 *kmlontjisbcwtpdgfaehrqzvuy  elapsed: 00:00:00.1463426 by Mutation
	generation    43 fitness  1435 *atsefghbcdjlouvzqrnymkifxw  elapsed: 00:00:00.1472218 by Swap
	generation    46 fitness  1434 *atsefghbcdjlorqzvunymkifxw  elapsed: 00:00:00.1520952 by Reverse
	generation    51 fitness  1426 *atsefghbcdjlouvzqrnymkixxw  elapsed: 00:00:00.1609032 by Mutation
	generation    51 fitness  1423 *avsefghifkmynutzqroljdcbxw  elapsed: 00:00:00.1614609 by Reverse
	generation    52 fitness  1414 *avsefghbcdjlorqwtunymkifxz  elapsed: 00:00:00.1622193 by Swap
	generation    54 fitness  1389 *avseffyzutnqroljdcbhgkmixw  elapsed: 00:00:00.1668875 by Swap
	generation    55 fitness  1388 *atsefghbcdfikmynjlorqzvuxw  elapsed: 00:00:00.1686067 by Reverse
	generation    56 fitness  1377 *atsefghbcdfikmjnuvzqrolyxw  elapsed: 00:00:00.1699975 by Swap
	generation    61 fitness  1338 *atsefghbcdfikmqnjloryzvuxw  elapsed: 00:00:00.1788435 by Swap
	generation    63 fitness   434 *agfeydcbhmjlouvzqrnstkipxw  elapsed: 00:00:00.1817006 by Mutation
	generation    68 fitness   402 *atswxpmkiynuvzqroljdcbhgfe  elapsed: 00:00:00.1905062 by Mutation
	generation    70 fitness   356 *atswxpmkiyzuvnqroljdcbhgfe  elapsed: 00:00:00.1941582 by Swap
	generation    85 fitness   348 *atswxpmnorqlvuzyikjdcbhgfe  elapsed: 00:00:00.2164192 by Reverse
	generation    89 fitness   334 *atswxpmkiyzuvrqnoljdcbhgfe  elapsed: 00:00:00.2232943 by Reverse
	generation    93 fitness   328 *azikmnvuytswxpqroljdcbhgfe  elapsed: 00:00:00.2302641 by Reverse
	generation    94 fitness   323 *atswxpmnorqlvuzydbcijkhgfe  elapsed: 00:00:00.2321492 by Reverse
	generation   105 fitness   314 *atswxpqronmlvuzyjkidefghbc  elapsed: 00:00:00.2489732 by Reverse
	generation   110 fitness   299 *adefghikmpxwsqroljncbztuvy  elapsed: 00:00:00.2582542 by Reverse
	generation   111 fitness   295 *atslmnorqpxwvuzyjkidefghbc  elapsed: 00:00:00.2603764 by Reverse
	generation   116 fitness   287 *adefghikjpxwsqrolmncbztuvy  elapsed: 00:00:00.2692063 by Swap
	generation   125 fitness   285 *adefghikjzxwsqronmlcbptuvy  elapsed: 00:00:00.2832807 by Reverse
	generation   126 fitness   282 *atslmnorqpxuvwzyjkihfgebcd  elapsed: 00:00:00.2851542 by Reverse
	generation   131 fitness   279 *axslmnorqptuvwzyjkidefghbc  elapsed: 00:00:00.2935830 by Swap
	generation   132 fitness   275 *atslmnorqpwuvxzyjkihfgebcd  elapsed: 00:00:00.2954973 by Swap
	generation   134 fitness   271 *abegfhikjyzwvuxrqponmlstcd  elapsed: 00:00:00.2984190 by Swap
	generation   135 fitness   264 *azyxpqronmlstwvujkidefghbc  elapsed: 00:00:00.3007587 by Reverse
	generation   136 fitness   260 *azyxvwtslmnorqpujkidefghbc  elapsed: 00:00:00.3018316 by Reverse
	generation   139 fitness   256 *azwuvxpqronmlstyjkihgfedbc  elapsed: 00:00:00.3074837 by Reverse
	generation   141 fitness   231 *azyxpqronmlstwvujkihgfedbc  elapsed: 00:00:00.3112030 by Swap
	generation   147 fitness   217 *azyxpqruvwtslmnojkihgfedbc  elapsed: 00:00:00.3200379 by Reverse
	generation   152 fitness   216 *azyxpqruvwtsjmnolkihgfedcb  elapsed: 00:00:00.3273526 by Swap
	generation   163 fitness   203 *azyxpqruvwtsomnljkihgfedbc  elapsed: 00:00:00.3447502 by Swap
	generation   182 fitness   202 *azyxsuvwtrqplmnoikjhgefdcb  elapsed: 00:00:00.3753730 by Reverse
	generation   186 fitness   195 *azyxpqruvwtsomnlkjihgfedbc  elapsed: 00:00:00.3815321 by Swap
	generation   189 fitness   187 *azyxstwvurqpomnljkihgfebdc  elapsed: 00:00:00.3875450 by Reverse
	generation   197 fitness   179 *azyxstwvurqpomnljkihgfedbc  elapsed: 00:00:00.3995630 by Swap
	generation   221 fitness   176 *azxywuvtrspqomnljkihgfedcb  elapsed: 00:00:00.4383912 by Swap
	generation   225 fitness   168 *azxywuvtrsqpomnljkihgfedcb  elapsed: 00:00:00.4460578 by Swap
	generation   240 fitness   164 *azyxwvtsurqpomnljkihgfedbc  elapsed: 00:00:00.4679619 by Reverse
	generation   255 fitness   163 *azyxwuvtsrqpolnmkijhgefdcb  elapsed: 00:00:00.4913439 by Reverse
	generation   264 fitness   157 *azyxwuvtsrqpomnlkjihfgedbc  elapsed: 00:00:00.5052970 by Reverse
	generation   269 fitness   156 *azyxwvustqrpomnljkihgfedcb  elapsed: 00:00:00.5135945 by Crossover
	generation   272 fitness   154 *azyxwuvtsrqpomnlkijhgefdcb  elapsed: 00:00:00.5190518 by Swap
	generation   296 fitness   145 *azyxwtvusrqopnmlkjihgfedcb  elapsed: 00:00:00.5544857 by Swap
	generation   335 fitness   139 *abcdefghikjlnmopqrstuvwxyz  elapsed: 00:00:00.6132106 by Swap
	generation   344 fitness   138 *azyxwvutsrqponmljkihgfedbc  elapsed: 00:00:00.6259218 by Reverse
	generation   349 fitness   130 *azyxwvutsrqponmljkihgfedcb  elapsed: 00:00:00.6340854 by Reverse
	generation   387 fitness   121 *azyxwvutsrqponmlkjihgfedcb  elapsed: 00:00:00.6923056 by Reverse
	
This shows that the solver determined the optimal route from home base, around the circle, and back to home base to be:

    (home) -> azyxwvutsrqponmlkjihgfedcb -> (home)	
	
After that we take on problem eil51 from the [TSPLIB][tsplib], a standard set of Travelling Salesperson Problems used in benchmarking solvers.
	
## License		

[MIT License][mitlicense]

[mitlicense]: http://www.opensource.org/licenses/mit-license.php
[part2]: http://handcraftsman.wordpress.com/2011/06/04/genetic-programming-part-2-8-queens/
[part3]: http://handcraftsman.wordpress.com/2011/06/05/genetic-programming-part-3-route-optimization/
[wikipedia]: http://en.wikipedia.org/wiki/Travelling_salesman_problem
[tsplib]: http://comopt.ifi.uni-heidelberg.de/software/TSPLIB95/