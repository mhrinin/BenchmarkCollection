using BenchmarkCollection.Benchmarks;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

//BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
BenchmarkRunner.Run<StaticNonStaticMethods>();