using System;
using Cache.Interfaces;

namespace Cache.IntegrationTests
{
	public class TestLogger : ILog
	{
		public void Debug(string message)
		{
			Console.WriteLine($"debug:{message}");
		}

		public void Error(string message)
		{
			Console.WriteLine($"error:{message}");
		}

		public void ErrorException(string message, Exception ex)
		{
			Console.WriteLine($"error:{message} ex:{ex.Message}");
		}

		public void Info(string message)
		{
			Console.WriteLine($"info:{message}");
		}

		public void Warning(string message)
		{
			Console.WriteLine($"warn:{message}");
		}
	}
}
