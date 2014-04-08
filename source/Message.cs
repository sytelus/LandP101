using System;
using System.Collections;

namespace LearnAndPlay
{
	/// <summary>
	/// Summary description for Message.
	/// </summary>
	public class Message
	{
		private String msSource;
		private ArrayList moContent;

		public const string PLAYER_THIS = "player_this";
		public const string PLAYER_JUDGE = "player_judge";

		public Message()
		{
			moContent=new ArrayList();
		}
		
		public Message(ArrayList voContent, String vsSource)
		{
			moContent = voContent;
			msSource = vsSource;
		}

		public String Source
		{
			get
			{
				return msSource;
			}
			set
			{
				msSource = value;
			}
		}
		
		public ArrayList  Content
		{
			get
			{
				return moContent;
			}
		}
		public void AddSymbol(String voSymbol)
		{
			moContent.Add(voSymbol);
		}
		public bool IsSimilar(Message voMessageToCompare, double vdSimilarityThresold)
		{
			bool bIsSimilar;
			
			if (msSource == voMessageToCompare.Source)
			{
				ArrayList SymbolList1;
				ArrayList SymbolList2;
				
				if (moContent.Count > voMessageToCompare.Content.Count)
				{
					SymbolList1 = voMessageToCompare.Content;
					SymbolList2=moContent;
				}
				else
				{
					SymbolList1 = moContent;
					SymbolList2 = voMessageToCompare.Content;			
				}
			
				int iTotalSymbols = SymbolList1.Count ;
				int iSimilarSymbols = 0;
				
				foreach(String Symbol1 in SymbolList1  )
				{
					if (SymbolList2.Contains(Symbol1)==true) iSimilarSymbols++;
				}
				
				if (iTotalSymbols  != 0)
				{
					bIsSimilar = ((iSimilarSymbols/iTotalSymbols) >= vdSimilarityThresold);
				}
				else bIsSimilar=false;
			}
			else
			{
				//Message sources are not similar
				bIsSimilar=false;
			}				
			
			return bIsSimilar;
		}
		
		public bool IsEqual(ArrayList voMessageToCompare)
		{
			bool bIsEqual = false;
			if (moContent.Count != voMessageToCompare.Count)
			{
				bIsEqual = false;
			}
			else
			{
				bIsEqual = true;
				for(int iArrayIndex=0; iArrayIndex <= moContent.Count; iArrayIndex++)
				{
					if (moContent[iArrayIndex] != voMessageToCompare[iArrayIndex])
					{
						bIsEqual = false;
						break;
					}
				}
			}
			
			return bIsEqual;
		}	

		public override String ToString()
		{
			String sReturn = String.Empty;
			sReturn = CommonFunctions.ArrayToCSV(moContent.ToArray());
			
			return sReturn;
		}
	}
}
