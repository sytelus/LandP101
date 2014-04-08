using System;

namespace LearnAndPlay
{
	/// <summary>
	/// Summary description for CommonFunctions.
	/// </summary>
	public class CommonFunctions
	{
		public static int GetRandomNumber(int viMax)
		{
			Random oRandomGenerator = new Random();
			return (int) Math.Round((viMax) *  oRandomGenerator.NextDouble());
		}
		public static String ArrayToCSV(Object[] vaValues)
		{
			String sReturn = String.Empty;
			for(int iValueIndex=0;iValueIndex < vaValues.Length;iValueIndex++)
			{
				sReturn += (String)(vaValues[iValueIndex]);
				if (iValueIndex < (vaValues.Length-1))
					sReturn += ",";
			}
			return sReturn;
		}		
	}
}
