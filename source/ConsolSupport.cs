using System;

namespace LearnAndPlay
{
	/// <summary>
	/// Summary description for ConsolParser.
	/// </summary>
	public class ConsolSupport
	{
		public const String msMY_NAME = "LandP";
		
		public const String msCHANNEL_INFO_TYPE_NAME = "name";
		public const String msCHANNEL_INFO_TYPE_EXIT = "exit";
		
		public const String msCHANNEL_COMMAND_TYPE_EXIT = "exit";
		public const String msCHANNEL_COMMAND_TYPE_NEW_GAME = "new";
		public const String msCHANNEL_COMMAND_TYPE_REGISTER_SYMBOL = "symbol";
		public const String msCHANNEL_COMMAND_TYPE_GET_MESSAGE = "play";
		
		public const String msCHANNEL_SCORE = "@score";
		public const String msCHANNEL_INFO = "@info";
		public const String msCHANNEL_COMMAND = "@command";
		public const String msCHANNEL_INPUT = "@input";
		public const String msCHANNEL_OUTPUT = "@output";
		
		public void SendMyName()
		{
			SendMessage(msCHANNEL_INFO, msCHANNEL_INFO_TYPE_NAME , msMY_NAME);
			SendComment("Using Deductable Rules Algorithm. (C) Shital Shah, 2001");
			SendComment("Please report all errors and bugs to shital_s@usa.net");
		}
		public void SendMessage(String vsChannel, String vsCommandType, String vsCommandContent)
		{
			System.Console.WriteLine(vsChannel + " " + vsCommandType + " " + vsCommandContent);
		}
		public void ReadMessage(ref String rsChannel, ref String rsCommandType, ref String rsCommandContent)
		{
			bool bIsCommandReceived = false;
			do
			{
				String sCommandLine = System.Console.ReadLine();
				char[] aWhiteSpace = new char[] {(char)" "[0], (char)"/t"[0]};
				sCommandLine = sCommandLine.TrimStart(null);
				if (sCommandLine.StartsWith("#") == false)
				{
					//Does command line starts with @
					if (sCommandLine.StartsWith("@")==true)
					{
						//parse
						sCommandLine = sCommandLine.TrimStart(aWhiteSpace);
						int iIndexOfWhiteSpaceAfterCommand = sCommandLine.IndexOfAny(aWhiteSpace);
						if (iIndexOfWhiteSpaceAfterCommand > 1)
						{
							rsChannel = sCommandLine.Substring(0, iIndexOfWhiteSpaceAfterCommand);
							String sRemainingString = sCommandLine.Substring(iIndexOfWhiteSpaceAfterCommand);
							
							sRemainingString = sRemainingString.TrimStart(null);
							int iIndexOfWhiteSpaceAfterCommandType = sRemainingString.IndexOfAny(aWhiteSpace);
							if (iIndexOfWhiteSpaceAfterCommandType > 1)
							{
								rsCommandType = sRemainingString.Substring(0, iIndexOfWhiteSpaceAfterCommandType);
								rsCommandContent = sRemainingString.Substring(iIndexOfWhiteSpaceAfterCommandType);
								rsCommandContent = rsCommandContent.TrimStart(null);
							}
							else
							{
								rsCommandType = sRemainingString;
								rsCommandContent = String.Empty;
							}
						}
						else
						{
							rsChannel =  String.Empty;
							rsCommandType = String.Empty;
							rsCommandContent = String.Empty;			
						}
					}
					else
					{
						rsChannel = msCHANNEL_INPUT;
						rsCommandType = sCommandLine;
						rsCommandContent = String.Empty;
					}
					
					rsChannel.ToLower();
					//rsCommandType.ToLower();
					//rsCommandContent.ToLower();
					bIsCommandReceived = true;
				}
				else
				{
					//ignore comments
				};
			}
			while (bIsCommandReceived == false);
		}
		
		public void SendComment(String vsComment)
		{
			System.Console.WriteLine("#" + vsComment);
		}
	}
}
