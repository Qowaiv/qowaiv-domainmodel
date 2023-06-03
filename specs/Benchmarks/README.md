## Creation
By making both the event buffer and its only underlying append only collection
structs the overhead of object allocation is minimized. Locks and null checks
still take time.

|       Method |   Count |         Mean | Ratio |      Gen0 |     Gen1 |     Gen2 |    Allocated |
|-------------:|--------:|-------------:|------:|----------:|---------:|---------:|-------------:|
|         List |   1,000 |     2.665 us |  1.00 |    1.3237 |   0.0381 |        - |     16.21 KB |
| event buffer |   1,000 |    28.691 us | 10.94 |    1.3123 |   0.0305 |        - |     16.13 KB |
|  previous[1] |   1,000 |     9.373 us |  3.39 |    5.7373 |   0.3357 |        - |     70.35 KB |
|              |         |              |       |           |          |          |              |
|         List |  10,000 |    58.536 us |  1.00 |   41.6260 |  41.6260 |  41.6260 |    256.32 KB |
| event buffer |  10,000 |   291.107 us |  4.97 |   41.5039 |  41.5039 |  41.5039 |    256.23 KB |
|  previous[1] |  10,000 |   106.066 us |  1.81 |   57.3730 |  24.2920 |        - |    703.16 KB |
|              |         |              |       |           |          |          |              |
|         List | 100,000 |   629.386 us |  1.00 |  114.2578 | 104.4922 | 104.4922 |  2,048.82 KB |
| event buffer | 100,000 | 2,863.705 us |  4.56 |  503.9063 | 496.0938 | 496.0938 |  2,048.44 KB |
|  previous[1] | 100,000 | 3,013.979 us |  4.73 |  570.3125 | 425.7813 |        - |  7,031.29 KB |
|              |         |              |       |           |          |          |              |
|         List | 200,000 | 1,384.525 us |  1.00 |  128.9063 | 119.1406 | 119.1406 |  4,096.82 KB |
| event buffer | 200,000 | 6,171.884 us |  4.40 |  484.3750 | 476.5625 | 476.5625 |  4,096.45 KB |
|  previous[1] | 200,000 | 9,733.000 us |  7.08 | 1140.6250 | 984.3750 |        - | 14,062.55 KB |

## Iteration
The event buffer can be looped faster than a list, and almost as fast as an
array. This is possible as the immutable event buffer does not have the check
for changes, where a list has too.

|              |   1,000 |   10,000 |    100,000 |
|-------------:|--------:|---------:|-----------:|
|        array | 11.2 µs | 107.5 µs | 1.071,9 µs |
| event buffer | 12.7 µs | 121.4 µs | 1.211,0 µs |
|         list | 15.4 µs | 148.9 µs | 1.501,9 µs |
|    aggregate | 20.5 µs | 196.0 µs | 1.994,8 µs |
|  previous[1] | 36.4 µs | 507.7 µs |     N/A    |

1. `EventBuffer` of v0.2.0.
