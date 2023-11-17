using BenchmarkCollection.Models;
using BenchmarkDotNet.Attributes;
using Bogus;

namespace BenchmarkCollection.Benchmarks
{
	public class DtoObject
	{
		public int Id { get; set; }
	}

	[ShortRunJob]
	[MinColumn, MaxColumn, MeanColumn, MedianColumn]
	[MemoryDiagnoser]
	[MarkdownExporter]
	public class StaticNonStaticMethods
	{
		private StaticNonStaticMethodsContainer _c = new StaticNonStaticMethodsContainer();
		private List<DtoObject> _dtoObjects;

		public StaticNonStaticMethods()
		{
			Randomizer.Seed = new Random(2222222);
			var parameterStages = new[] { -1, 2, 3, 4, 6, 9 };

			var dtoObjectFaker = new Faker<DtoObject>()
				.RuleFor(u => u.Id, f => f.Random.Number(1, 999999));

			_dtoObjects = dtoObjectFaker.Generate(5000);
		}

		[Benchmark]
		public List<bool> Static()
		{
			return _dtoObjects.Select(StaticNonStaticMethodsContainer.StaticMethod).ToList();
		}

		[Benchmark]
		public List<bool> Dynamic()
		{
			return _dtoObjects.Select(_c.DynamicMethod).ToList();
		}
	}

	public class StaticNonStaticMethodsContainer
	{
		public static bool StaticMethod(DtoObject dto)
		{
			return dto.Id % 2 == 0;
		}

		public bool DynamicMethod(DtoObject dto)
		{
			return dto.Id % 2 == 0;
		}
	}
}