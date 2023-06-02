## Creation
|       Method |   Count |          Mean | Ratio |
|-------------:|--------:|--------------:|------:|
|         list |   1,000 |      2.646 µs |  1.00 |
| event buffer |   1,000 |     29.114 µs | 11.05 |
|              |         |               |       |
|         list |  10,000 |     59.020 µs |  1.00 |
| event buffer |  10,000 |    385.066 µs |  6.60 |
|              |         |               |       |
|         list | 100,000 |    629.436 µs |  1.00 |
| event buffer | 100,000 | 15,640.531 µs | 25.04 |

## Iteration
The event buffer can be looped faster than a list, and almost as fast as an
array. This is possible as the immutable event buffer does have the check
for changes, where a list has too.

|              |   1,000 |   10,000 |    100,000 |
|-------------:|--------:|---------:|-----------:|
|        array | 11.2 µs | 107.5 µs | 1.071,9 µs |
| event buffer | 12.7 µs | 121.4 µs | 1.211,0 µs |
|         list | 15.4 µs | 148.9 µs | 1.501,9 µs |
|    aggregate | 20.5 µs | 196.0 µs | 1.994,8 µs |
|  previous[1] | 36.4 µs | 507.7 µs |     N/A    |

1. `EventBuffer` of v0.2.0.
