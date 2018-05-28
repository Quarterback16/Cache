namespace Cache.Interfaces
{
	public interface ICacheSettings
	{
		string Environment { get; set; }
		string FunctionalArea { get; set; }
		string ConnectionString { get; set; }
	}
}
