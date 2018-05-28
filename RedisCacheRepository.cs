using Cache.Interfaces;
using System;
using StackExchange.Redis;

namespace Cache
{
	/// <summary>
	///   A local Redis cache implementation based on the Department of Jobs implementation
	/// </summary>
	public class RedisCacheRepository : ICacheRepository
	{
		public ILog Logger { get; set; }

		public bool IsActive { get; set; }

		private ConnectionMultiplexer _redis;

		public string Environment { get; set; }
		public string FunctionalArea { get; set; }
		public string ConnectionString { get; set; }

		private const string Separator = ":";
		private const string NamespacedKeyFormat = "{1}{0}{2}{0}{3}";

		//private const string DefaultEnvironment = "local";
		//private const string DefaultFunctionalArea = "tfl";
		//private const string DefaultConnectionString = "localhost,abortConnect=false";

		protected readonly TimeSpan DefaultTimeToLive;
		protected readonly TimeSpan MaxTimeToLive;

		protected readonly ISerializer Serializer;

		protected IDatabase Cache
		{
			get
			{
				var cache = _redis.GetDatabase();
				if (cache == null)
					throw new InvalidOperationException("Could not get Redis database.");
				return cache;
			}
		}

		public RedisCacheRepository(
			string connectionString,
			string environment,
			string functionalArea,
			ISerializer serializer, 
			ILog logger)
		{
			Logger = logger;
			try
			{
				ConnectionString = connectionString;
				Environment = environment;
				FunctionalArea = functionalArea;
				if (!string.IsNullOrEmpty(ConnectionString))
				{
					_redis = ConnectionMultiplexer.Connect(
						ConnectionString);

					DefaultTimeToLive = new TimeSpan(2, 0, 0);
					MaxTimeToLive = new TimeSpan(4, 0, 0);

					Serializer = serializer;
					IsActive = true;
					Logger.Info("Redis Cache Repository initialised");
				}
				else
					Logger.Info("NO connection string");
			}
			catch (Exception ex)
			{
				IsActive = false;
				Logger.ErrorException("RedisCacheRepository failed to initialise", ex);
			}
		}

		public bool Add<T>(
			string key, 
			T o, 
			TimeSpan? timeToLive = default(TimeSpan?))
		{
			if (!IsActive) return false;

			if (string.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException("key");

			key = NamespacedKey(key);
			timeToLive = EnsureTimeToLive(timeToLive);

			return Cache.StringSet(
			   key,
			   Serialize(o),
			   timeToLive.Value,
			   When.NotExists);
		}

		public void Remove(string key)
		{
			if (!IsActive) return;

			if (string.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException("key");
			RemoveInternal(key);
		}

		public void Set<T>(
			string key,
			T o,
			TimeSpan? timeToLive = default(TimeSpan?))
		{
			if (!IsActive) return;

			if (string.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException("key");

			key = NamespacedKey(key);
			timeToLive = EnsureTimeToLive(timeToLive);

			Cache.StringSet(
			   key,
			   Serialize(o),
			   timeToLive.Value,
			   When.Always,
			   CommandFlags.FireAndForget);
		}

		public bool TryGet<T>(
			string key,
			out T value)
		{
			if (string.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException("key");

			try
			{
				value = default(T);

				if (!IsActive) return false;

				key = NamespacedKey(key);

				RedisValue cachedValue = Cache.StringGet(key);

				if (cachedValue.HasValue)
				{
					value = Deserialize<T>(cachedValue);
				}

				return !Equals(value, default(T));
			}
			catch (Exception ex)
			{
				Logger.ErrorException(
					$"RedisCacheRepository TryGet failed for key {key}",
					ex);
			}
			value = default(T);
			return false;
		}

		private void RemoveInternal(string key)
		{
			key = NamespacedKey(key);
			Cache.KeyDelete(key, CommandFlags.FireAndForget);
		}

		private TimeSpan EnsureTimeToLive(TimeSpan? timeToLive)
		{
			var ttl = timeToLive ?? DefaultTimeToLive;
			if (ttl > MaxTimeToLive)
			{
				ttl = MaxTimeToLive;
			}
			return ttl;
		}

		private string Serialize(object value)
		{
			return Serializer.Serialize(value);
		}

		private T Deserialize<T>(string value)
		{
			var deserialized = Serializer.Deserialize(value);

			return (deserialized != null) ? (T)deserialized : default(T);
		}

		private string NamespacedKey(string key)
		{
			return (!string.IsNullOrWhiteSpace(key))
			   ? string.Format(
				  NamespacedKeyFormat,
				  Separator,
				  Environment,
				  FunctionalArea,
				  key) : key;
		}

	}
}