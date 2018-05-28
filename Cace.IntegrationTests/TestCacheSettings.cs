using Cache.Interfaces;

namespace Cache.IntegrationTests
{
	public class TestCacheSettings : ICacheSettings
	{
		public string Environment
		{
			get { return "integration-test-environment"; }
			set { Environment = value; }
		}

		public string FunctionalArea
		{
			get { return "cache-testing"; }
			set { FunctionalArea = value; }
		}

		public string ConnectionString
		{
			get { return "localhost,abortConnect=false"; }
			set { ConnectionString = value; }
		}

		public override string ToString()
		{
			return $"Env:{Environment} Area:{FunctionalArea}";
		}
	}
}