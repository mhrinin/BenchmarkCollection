namespace BenchmarkCollection.Models
{
	public class SyncedSignal
	{
		public int ParameterId { get; set; }
		public int ClinicId { get; set; }
		public int PatientId { get; set; }
		public int ParameterStageId { get; set; }
		public DateTime? LastSyncDate { get; set; }
	}
}
