using NLog;

namespace TycoonBus.Server
{
	public class NLogAdaptor : ILog
	{
		public Logger Logger { get; set; }

		public NLogAdaptor()
		{
			Logger = LogManager.GetCurrentClassLogger();
		}

		public void Info( string message )
		{
			Logger.Info( message );
		}

		public void Debug( string message )
		{
			Logger.Debug( message );
		}

		public void Error( string message )
		{
			Logger.Error( message );
		}

		public void Warning( string message )
		{
			Logger.Warn( message );
		}
	}
}
