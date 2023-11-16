using BenchmarkCollection.Models;
using BenchmarkDotNet.Attributes;
using Bogus;

namespace BenchmarkCollection.Benchmarks
{
	[ShortRunJob]
	[MinColumn, MaxColumn, MeanColumn, MedianColumn]
	[MemoryDiagnoser]
	[MarkdownExporter]
	public class StaticNonStaticMethods
	{
		private StaticNonStaticMethodsContainer _c = new StaticNonStaticMethodsContainer();
		private List<SignalSource> _signals;
		private List<SyncedSignal> _syncedSignals;

		public StaticNonStaticMethods()
        {
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

			_signals = signalFaker.Generate(100000);
			_syncedSignals = syncedSignalFaker.Generate(100000);
		}

        [Benchmark]
		public List<SignalSource> Static()
		{
			return StaticNonStaticMethodsContainer.StaticMethod(_signals, _syncedSignals);
		}

		[Benchmark]
		public List<SignalSource> Dynamic()
		{
			return _c.DynamicMethod(_signals, _syncedSignals);
		}
	}

	public class StaticNonStaticMethodsContainer
	{
		public static List<SignalSource> StaticMethod(IEnumerable<SignalSource> sources,
			IEnumerable<SyncedSignal> syncedSignals, int parameterId = 96)
		{
			List<SignalSource> notSyncedList = new();

			foreach (var source in sources)
			{
				var sourceParameterStageId = source.ParameterStageId ?? -1;

				var syncedSignal = syncedSignals.FirstOrDefault(x =>
				x.ParameterId == parameterId
				&& x.ClinicId == source.ClinicId
				&& x.PatientId == source.PatientId
				&& x.ParameterStageId == sourceParameterStageId);

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

		public List<SignalSource> DynamicMethod(IEnumerable<SignalSource> sources,
			IEnumerable<SyncedSignal> syncedSignals, int parameterId = 96)
		{
			List<SignalSource> notSyncedList = new();

			foreach (var source in sources)
			{
				var sourceParameterStageId = source.ParameterStageId ?? -1;

				var syncedSignal = syncedSignals.FirstOrDefault(x =>
				x.ParameterId == parameterId
				&& x.ClinicId == source.ClinicId
				&& x.PatientId == source.PatientId
				&& x.ParameterStageId == sourceParameterStageId);

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
	}
}
