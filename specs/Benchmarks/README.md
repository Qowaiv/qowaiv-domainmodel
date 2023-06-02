## Creation
|       Method |   Count |          Mean | Ratio |
|-------------:|--------:|--------------:|------:|
|         list |   1,000 |      2.646 us |  1.00 |
| event buffer |   1,000 |     29.114 us | 11.05 |
|              |         |               |       |
|         list |  10,000 |     59.020 us |  1.00 |
| event buffer |  10,000 |    385.066 us |  6.60 |
|              |         |               |       |
|         list | 100,000 |    629.436 us |  1.00 |
| event buffer | 100,000 | 15,640.531 us | 25.04 |

## Iteration
The event buffer can be looped faster than a list, and almost as fast as an
array. This is possible as the immutable event buffer does have the check
for changes, where a list has too.

|       Method |   Count |         Mean | Ratio |
|-------------:|--------:|-------------:|------:|
|        array |   1,000 |     9.891 us |  1.00 |
| event buffer |   1,000 |    11.384 us |  1.16 |
|         list |   1,000 |    13.623 us |  1.38 |
|              |         |              |       |
|        array |  10,000 |   101.403 us |  1.00 |
| event buffer |  10,000 |   117.457 us |  1.16 |
|         list |  10,000 |   136.235 us |  1.34 |
|              |         |              |       |
|        array | 100,000 | 1,013.059 us |  1.00 |
| event buffer | 100,000 | 1,188.769 us |  1.17 |
|         list | 100,000 | 1,375.805 us |  1.36 |

## Aggregate
|         Method | Count |           Mean |    Ratio |
|--------------- |------ |---------------:|---------:|
|           List |    10 |       0.023 ms |     1.00 |
|  'Append Only' |    10 |       5.207 ms |   221.64 |
| 'Event Buffer' |    10 |       9.876 ms |   419.92 |
|                |       |                |          |
|           List |   100 |       1.961 ms |     1.00 |
|  'Append Only' |   100 |      50.382 ms |    25.66 |
| 'Event Buffer' |   100 |     742.614 ms |   378.64 |
|                |       |                |          |
|           List |  1000 |     219.400 ms |     1.00 |
|  'Append Only' |  1000 |     515.754 ms |     2.34 |
| 'Event Buffer' |  1000 | 299,534.578 ms | 1,351.20 |
|                |       |                |          |
|           List | 10000 |  23,832.072 ms |    1.000 |
|  'Append Only' | 10000 |   5,093.584 ms |    0.214 |
| 'Event Buffer' | 10000 |           N/A  |          |
