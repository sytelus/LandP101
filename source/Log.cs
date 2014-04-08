using System;
using System.Collections;

namespace LearnAndPlay
{
	/// <summary>
	/// Summary description for Log.
	/// </summary>
	
	public interface ILoggable
	{
		void Write(String vsString);
	}
	
	public class Log
	{
		public static ILoggable OutputDevice;
		public static int MinImportanceLevel = -1;
		public static void Write(String vsHeading, String vsContent, bool vbAddLine, int viImportanceLevel)
		{
			if (OutputDevice != null)
			{
				String sLogMessage = vsHeading + vsContent;
				if(vbAddLine) sLogMessage += "\n";
				OutputDevice.Write(sLogMessage);		
			}
		}
		public static void Write(String vsHeading, ArrayList vaContent, bool vbAddLine, int viImportanceLevel)
		{
			String sContent = "{ArrayList(" + vaContent.Count + "}";
			Write(vsHeading, sContent, vbAddLine, viImportanceLevel);
		}	
	}
}
