using Cache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace Cache.IntegrationTests
{
	[TestClass]
	public class RedisCacheRepositoryIntegrationTests
	{
		private RedisCacheRepository _sut;

		[TestInitialize]
		public void Setup()
		{
			_sut = new RedisCacheRepository(
				connectionString: "localhost,abortConnect=false",
				environment: "integration-test-environment",
				functionalArea: "cache-testing",
				serializer: new XmlSerializer(),
				logger: new TestLogger());
		}

		[TestMethod]
		public void Repository_OnInstantiation_ConnectsWithRedis()
		{
			Assert.IsTrue(_sut.IsActive, "Should connect to the database");
		}

		[TestMethod]
		public void Repository_OnInstantiation_EnvironmentIsTesting()
		{
			Assert.AreEqual(
				expected: "integration-test-environment",
				actual: _sut.Environment);
		}

		[TestMethod]
		public void Repository_WhenSetting_SavesAndReadsDataSets()
		{
			var cacheKey = "test-key-dataset";
			var objectToCache = new DataSet();
			_sut.Set(
				key: cacheKey,
				o: objectToCache );  // empty dataset will be serialized to XML

			Assert.IsTrue(_sut.TryGet(cacheKey, out DataSet dsRead));
		}

		[TestMethod]
		public void Repository_WhenSetting_SavesAndReadsStrings()
		{
			var cacheKey = "test-key-string";
			var objectToCache = "Hello World";
			_sut.Set(
				key: cacheKey,
				o: objectToCache);  // empty dataset will be serialized to XML

			Assert.IsTrue(_sut.TryGet(cacheKey, out string stringOut));
			Assert.IsTrue(stringOut == objectToCache);
		}
	}
}
