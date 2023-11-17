using BenchmarkCollection.Models;
using BenchmarkDotNet.Attributes;
using Bogus;

namespace BenchmarkCollection.Benchmarks
{
	[ShortRunJob]
	[MinColumn, MaxColumn, MeanColumn, MedianColumn]
	[MemoryDiagnoser]
	[MarkdownExporter]
	public class FilterCollection
	{
		private FilterCollectionContainer _c = new FilterCollectionContainer();
		private List<SignalSource> _signals;
		private List<SyncedSignal> _syncedSignals;

		public FilterCollection()
		{
			Randomizer.Seed = new Random(1111111);
			var parameterStages = new[] { -1, 2, 3, 4, 6, 9 };

			var signalFaker = new Faker<SignalSource>()
				.RuleFor(u => u.PatientId, f => f.Random.Number(1, 999999))
				.RuleFor(u => u.ClinicId, f => f.Random.Number(1, 3))
				.RuleFor(u => u.CaseId, f => f.Random.Number(1, 999999))
				.RuleFor(u => u.UserId, f => f.Random.Number(1, 9000))
				.RuleFor(u => u.ParameterStageId, f => f.PickRandom(parameterStages))
				.RuleFor(u => u.ParameterId, f => f.Random.Number(1, 300))
				.RuleFor(u => u.ProgramId, f => f.Random.Number(1, 9000))
				.RuleFor(u => u.CreatedDate, f => f.Date.Between(new DateTime(2022, 10, 1), new DateTime(2023, 11, 1)));

			var syncedSignalFaker = new Faker<SyncedSignal>()
				.RuleFor(u => u.PatientId, f => f.Random.Number(1, 999999))
				.RuleFor(u => u.ClinicId, f => f.Random.Number(1, 3))
				.RuleFor(u => u.ParameterId, f => f.Random.Number(1, 300))
				.RuleFor(u => u.ParameterStageId, f => f.PickRandom(parameterStages))
				.RuleFor(u => u.LastSyncDate, f => f.Date.Between(new DateTime(2022, 10, 1), new DateTime(2023, 9, 1)));

			_signals = signalFaker.Generate(5000);
			_syncedSignals = syncedSignalFaker.Generate(500);
		}

		[Benchmark]
		public List<SignalSource> AlgoritmA()
		{
			return _c.AlgoritmA(_signals, _syncedSignals);
		}

		[Benchmark]
		public List<SignalSource> AlgoritmB()
		{
			return _c.AlgoritmB(_signals, _syncedSignals);
		}
	}

	public class FilterCollectionContainer
	{
		public List<SignalSource> AlgoritmA(IEnumerable<SignalSource> sources,
			IEnumerable<SyncedSignal> syncedSignals, int parameterId = 96)
		{
			List<SignalSource> notSyncedList = new();

			foreach (var source in sources)
			{
				var syncedSignal = syncedSignals.FirstOrDefault(x =>
					x.ParameterId == parameterId
					&& x.ClinicId == source.ClinicId
					&& x.PatientId == source.PatientId
					&& x.ParameterStageId == source.ParameterStageId);

				if (syncedSignal == null)
				{
					notSyncedList.Add(source);
					continue;
				}

				if (source.CreatedDate > syncedSignal.LastSyncDate)
				{
					notSyncedList.Add(source);
				}
			}

			return notSyncedList;
		}

		public List<SignalSource> AlgoritmB(IEnumerable<SignalSource> sources,
			IEnumerable<SyncedSignal> syncedSignals, int parameterId = 96)
		{
			return sources.Where(source =>
			{
				var syncedSignal = syncedSignals.FirstOrDefault(x =>
					x.ParameterId == parameterId
					&& x.ClinicId == source.ClinicId
					&& x.PatientId == source.PatientId
					&& x.ParameterStageId == source.ParameterStageId);

				return syncedSignal == null || source.CreatedDate > syncedSignal.LastSyncDate;
			}).ToList();
		}
	}
}
