using System;

namespace Cache.Interfaces
{
	public interface ILog
	{
		void Info( string message );
		void Debug( string message );
		void Error( string message );
		void ErrorException(string message, Exception ex);
		void Warning( string message );
	}
}
