```

BenchmarkDotNet v0.14.0, macOS 27.0 (26A428) [Darwin 27.0.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 10.0.400
  [Host]   : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD
  ShortRun : .NET 8.0.8 (8.0.824.36612), Arm64 RyuJIT AdvSIMD

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                           | Categories | Mean        | Error      | StdDev    | Ratio  | RatioSD | Gen0   | Allocated | Alloc Ratio |
|--------------------------------- |----------- |------------:|-----------:|----------:|-------:|--------:|-------:|----------:|------------:|
| proxy.Func(arg)                  | call       |  12.6107 ns |  0.3760 ns | 0.0206 ns |   1.72 |    0.01 | 0.0051 |      32 B |        1.00 |
| declared.Func(arg)               | call       |   7.3218 ns |  0.5879 ns | 0.0322 ns |   1.00 |    0.01 | 0.0051 |      32 B |        1.00 |
| &#39;dynamic .Func(arg)&#39;             | call       |  11.0369 ns |  0.0566 ns | 0.0031 ns |   1.51 |    0.01 | 0.0051 |      32 B |        1.00 |
| MethodInfo.Invoke                | call       |  32.0110 ns |  4.3380 ns | 0.2378 ns |   4.37 |    0.03 | 0.0102 |      64 B |        2.00 |
|                                  |            |             |            |           |        |         |        |           |             |
| &#39;ActLike&lt;IPoco&gt;() (type cached)&#39; | create     | 561.8601 ns | 11.7317 ns | 0.6431 ns | 129.62 |    1.73 | 0.1516 |     952 B |       29.75 |
| &#39;new Direct() (no proxy)&#39;        | create     |   4.3355 ns |  1.2244 ns | 0.0671 ns |   1.00 |    0.02 | 0.0051 |      32 B |        1.00 |
|                                  |            |             |            |           |        |         |        |           |             |
| proxy.Prop1                      | get        |   5.2981 ns |  0.0775 ns | 0.0042 ns |   6.39 |    0.09 |      - |         - |          NA |
| declared.Prop1                   | get        |   0.8293 ns |  0.2359 ns | 0.0129 ns |   1.00 |    0.02 |      - |         - |          NA |
| &#39;dynamic .Prop1&#39;                 | get        |   2.7250 ns |  0.1193 ns | 0.0065 ns |   3.29 |    0.04 |      - |         - |          NA |
| PropertyInfo.GetValue            | get        |   8.3386 ns |  1.6267 ns | 0.0892 ns |  10.06 |    0.16 |      - |         - |          NA |
|                                  |            |             |            |           |        |         |        |           |             |
| &#39;proxy.Prop1 = v&#39;                | set        |   3.9849 ns |  0.2447 ns | 0.0134 ns |   1.71 |    0.01 |      - |         - |          NA |
| &#39;declared.Prop1 = v&#39;             | set        |   2.3270 ns |  0.0184 ns | 0.0010 ns |   1.00 |    0.00 |      - |         - |          NA |
| PropertyInfo.SetValue            | set        |  15.4092 ns |  0.1478 ns | 0.0081 ns |   6.62 |    0.00 |      - |         - |          NA |
