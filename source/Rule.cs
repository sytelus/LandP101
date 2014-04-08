using System;
using System.Collections;

//TODO: blank constructor should initialize estimated score as -ve infinity
//TODO: avoidable handling while dissolve
namespace LearnAndPlay
{
	/// <summary>
	/// Summary description for Rule.
	/// </summary>
	public class Rule
	{
		private ArrayList moMoveHistory;
		private ArrayList moOnMessage;
		private ArrayList moReplyMessage;
		private double mdExpectedScore = 0;	//no success/failure predicted by empty rule
		private String msRuleID;
		private double mdAvoidableSimilarityThresold;
		private double mdMoveSimilarityThresold;
		private double mdMessageSimilarityThresold;
		
		private int miHistoryDepthUsed = 0;
		private bool mbIsAvoidableObsolete = false;
		private bool mbIsAvoidable = false;
		private bool mbIsRuleDissolvedOnce = false;
		
		private static int mlNextRuleID = 0;
		
		public static void ResetIDCounter()
		{
			mlNextRuleID = 0;
		}
		
		public Rule()
		{
			moMoveHistory = new ArrayList();
			msRuleID = Rule.mlNextRuleID.ToString();
			Rule.mlNextRuleID = mlNextRuleID;
			mlNextRuleID++;
			mdAvoidableSimilarityThresold = 0.85;
			mdMoveSimilarityThresold = 0.4;
			mdMessageSimilarityThresold = 0.85;
		}
		
		public bool IsDissolvedOnce
		{
			get
			{
				return mbIsRuleDissolvedOnce;
			}
		}
		
		public String RuleID
		{
			get
			{
				return msRuleID;
			}
		}

		public ArrayList HistoryPattern
		{
			get
			{
				return moMoveHistory;
			}
		}
		public ArrayList OnMessage
		{
			get
			{
//				Move oLastMove = moMoveHistory.LastMove;
//				Message oOnMessage;
//				
//				if (oLastMove != null)
//				{
//					oOnMessage = oLastMove.OnMessage;
//				}
//				else
//				{
//					oOnMessage = null;
//				}
				return 	moOnMessage;
			}
		}
		public ArrayList ReplyMessage
		{
			get
			{
//				Move oLastMove = moMoveHistory.LastMove;
//				Message oReplyMessage;
//				
//				if (oReplyMessage != null)
//				{
//					oReplyMessage = oLastMove.ReplyMessage;
//				}
//				else
//				{
//					oReplyMessage = null;
//				}
				return 	moReplyMessage;
			}
		}
		
		public double ExpectedScore
		{
			get
			{
				return mdExpectedScore;
			}
		}
		
		public bool IsAvoidable
		{
			get
			{
				return mbIsAvoidable;
			}
			set
			{
				mbIsAvoidable = value;
			}
		}
		public bool IsAvoidableObsolete
		{
			get
			{
				return mbIsAvoidableObsolete;
			}
			set
			{
				mbIsAvoidableObsolete=value;
			}
		}
		public int HistoryDepthUsed
		{
			get
			{
				return miHistoryDepthUsed;
			}
		}
		
		private bool CompareStringArrayLists(ArrayList voList1, ArrayList voList2, bool vbReturnTrueIfAnyListIsNull)
		{
			bool bIsAnyListNull = false;
			if ((voList1 == null) || (voList2 == null))
				bIsAnyListNull=true;
			
			bool bIsEqual = false;
			if (bIsAnyListNull == false)
			{
				if (voList1.Count != voList2.Count)
				{
					bIsEqual = false;
				}
				else
				{
					bIsEqual = true;
					for(int iArrayIndex=0; iArrayIndex < voList1.Count; iArrayIndex++)
					{
						if (((String) (voList1[iArrayIndex])) != ((String) (voList2[iArrayIndex])))
						{
							bIsEqual = false;
							break;
						}
					}
				}
			}
			else
			{
				bIsEqual = vbReturnTrueIfAnyListIsNull;
			}			
			return bIsEqual;
		}
		
		public double EvaluateMatch(ArrayList voRuleHistoryPart, ArrayList voOnMessagePart, ArrayList voReplyMessagePart, double vdScorePart, IEnumerable voRuleListToCheckForAvoidables)
		{
			double dMatchLevel = -1; //Match level = -1 to 1
			
			if ((mbIsRuleDissolvedOnce == false) || (mbIsAvoidableObsolete == true))
			{ 
				dMatchLevel = -1;	//No match if rule wasn't initialized
			}
			else
			{
				
				bool bIsHistoryEndsSimilar = false;
				
				//Is history ends similar?
				if (
					((CompareStringArrayLists(moOnMessage, voOnMessagePart, true)==true)) &&
					((CompareStringArrayLists(moReplyMessage,voReplyMessagePart, true)==true)) &&
					((double.IsNaN(vdScorePart)==true) || (IsNumbersSimilar(mdExpectedScore, vdScorePart, 0.85) == true))
					)
				{
					bIsHistoryEndsSimilar = true;
				}
				else
				{
					bIsHistoryEndsSimilar = false;
				}

				//Find the similarity between rule history and current situation
				double dHistoryMatchLevel = 0;
				
				//If this rule has history
				if (moMoveHistory.Count > 0)
				{
					//If passed situation has history
					if (voRuleHistoryPart.Count > 0)
					{
						ArrayList oLargestCommonMoveSet = FindLargestCommonMoveSet(moMoveHistory, voRuleHistoryPart, ref dHistoryMatchLevel);				

						//See if we need to check for avoidable rule matches
						bool bIsAvoidableRuleMatchFound = false;
						if (voRuleListToCheckForAvoidables != null)
						{
							//see if the common factor belongs to any avoidable rule
							foreach(Rule oAvoidableRule in voRuleListToCheckForAvoidables)
							{
								if (oAvoidableRule.IsAvoidable == true)
								{
									if (oAvoidableRule.EvaluateMatch(oLargestCommonMoveSet,voOnMessagePart, voReplyMessagePart, vdScorePart, new ArrayList()) > mdAvoidableSimilarityThresold)
									{
										bIsAvoidableRuleMatchFound = true;
										//Avoid this rule match
										dMatchLevel = -1;
									}
								}
								else {}; //no need to check with non-avoidable rules
							}
							//If no avoidable rules were given to process
						}
						
						//If no avoidable rule processing required
						if (bIsAvoidableRuleMatchFound == false)
						{
							//If there was any common moves found
							if (oLargestCommonMoveSet.Count != 0)
								//Match level is the product of match quality and match quantity. 
								//Match quanity is equal to ratio of rule length and common factor
								dMatchLevel = (Math.Pow(dHistoryMatchLevel,2.5) * ((double)oLargestCommonMoveSet.Count/(double)moMoveHistory.Count));
							else
								dMatchLevel = 0;	//Incase of similar ends - it will be retained else it will be made -1
								
							if (bIsHistoryEndsSimilar == true)
							{
								//Keep the match level
							}
							else
							{
								//Todo: Thresold to include rule if ends are not same but history is similar
								if (dMatchLevel > 0.5)
								{
									dMatchLevel = 0; //can't decide
								}
								else
								{
									dMatchLevel = -1; //It's not similar
								}
							}
						}
						else {}; //Do not reassign match level

					}
					else
					{
						//Move history is zero, rule history is non-0
						if (bIsHistoryEndsSimilar == true)
						{
							//Nothing to match in move history. 0 -> can't decide match
							dMatchLevel = 0;
						}
						else
						{
							dMatchLevel = -1;
						}
					}
				}
				else
				{
					//Move history is non-0, rule history is 0
					if (bIsHistoryEndsSimilar == true)
					{
						//In case rule has no histoty, it matches with any moves
						dMatchLevel = 1;
					}
					else
					{
						dMatchLevel = -1;
					}
				}
				
			}			
			
			return dMatchLevel;
		}
		
		public double EvaluateMatch(ArrayList voHistory, Move voCurrentMove, IEnumerable voRuleListToCheckForAvoidables)
		{
			//A rule is matched to history, if history ends are same as On/ReplyMessage AND score is similar.
			//The match level is determined by numbers of ordered common elements ratio with total length of 			
			//rule or history
			
			ArrayList oRuleHistoryPart = null;
			ArrayList oOnMessagePart = null;
			ArrayList oReplyMessagePart = null;
			double dScorePart = double.NaN;
			
			//If current move is not availble, take end points from history's last message
			//otherwise, take it from current move
			if(voCurrentMove == null)
			{
				GetParts(voHistory,ref  oRuleHistoryPart,ref  oOnMessagePart,ref  oReplyMessagePart,ref dScorePart); 
			}
			else
			{
				//Whole history is prospectus rule's history
				oRuleHistoryPart = new ArrayList(voHistory);
				
				//Extract end points
				//In current move we want to find out similarity of history with other rules
				//Current move might not have generated reply yet

				if (voCurrentMove.OnMessage != null)
				{
					oOnMessagePart = new ArrayList(voCurrentMove.OnMessage.Content.ToArray());
				}
				else
				{
					oOnMessagePart = null;
				}
				
				if (voCurrentMove.ReplyMessage != null)
				{
					oReplyMessagePart = new ArrayList(voCurrentMove.ReplyMessage.Content.ToArray());
				}
				else
				{
					oReplyMessagePart = null;
				}
				dScorePart = voCurrentMove.ActualScore;
			}
			
			return EvaluateMatch(oRuleHistoryPart, oOnMessagePart, oReplyMessagePart, dScorePart, voRuleListToCheckForAvoidables);
		}
		
		public double EvaluateMatch(ArrayList voHistory, IEnumerable voRuleListToCheckForAvoidables)
		{
			return EvaluateMatch(voHistory, null, voRuleListToCheckForAvoidables );
		}
		
		private void GetParts(ArrayList voMoveHistory, ref ArrayList roMoveHistoryBeforeLastMove, ref ArrayList roOnMessage, ref ArrayList roReplyMessage, ref double rdScore)
		{
			if (voMoveHistory.Count > 1)
			{
				roMoveHistoryBeforeLastMove = new ArrayList(voMoveHistory);
				roMoveHistoryBeforeLastMove.RemoveAt(roMoveHistoryBeforeLastMove.Count-1);
			}
			else
			{
				roMoveHistoryBeforeLastMove = new ArrayList();
			}
			
			if (voMoveHistory.Count != 0)
			{
				Move oLastMove = (Move) voMoveHistory[voMoveHistory.Count-1];
				if (oLastMove.OnMessage != null)
					roOnMessage = new ArrayList(oLastMove.OnMessage.Content.ToArray()); 
				else
					roReplyMessage = null;
					
				if (oLastMove.ReplyMessage != null)
					roReplyMessage = new ArrayList(oLastMove.ReplyMessage.Content.ToArray()); 
				else
					roReplyMessage = null;
				rdScore = oLastMove.ActualScore;
			}
			else
			{
				roOnMessage = null;
				roReplyMessage = null;
				rdScore = double.NaN;
			}
		}

		public static bool IsNumbersSimilar(double vdNumber1, double vdNumber2, double vdSimilarityThresold)
		{
			
			if (vdNumber1>1) vdNumber1 = 1; else if (vdNumber1 < -1) vdNumber1 = -1;
			if (vdNumber2>1) vdNumber2 = 1; else if (vdNumber2 < -1) vdNumber2 = -1;
			
			double dRelativeMatch = double.NaN;
			dRelativeMatch = System.Math.Abs((vdNumber1-vdNumber2)/2);
			
			return ((1-dRelativeMatch) > vdSimilarityThresold);
		}
		
//		private bool IsHistoryCompatibleToRule(MoveHistory voHistoryToCheck)
//		{
//			return  (
//				(moOnMessage.IsSimilar(voHistoryToCheck.LastMove.OnMessage)) &&
//				(moReplyMessage.IsSimilar(voHistoryToCheck.LastMove.ReplyMessage)) &&
//				(IsNumbersSimilar(mdExpectedScore, voHistoryToCheck.LastMove.ActualScore) == true)
//				);
//		}
		
		private ArrayList FindLargestCommonMoveSet(ArrayList voHistory1, ArrayList voHistory2, ref double rdSecondaryMatchLevel)
		{
			
			//
			//Find common set AND evaluate how much common is this really. If common elements
			//are more near recent history then it's better quality common
			//
			
			ArrayList oMain;
			ArrayList oSecondary;
			//Choose the smaller history as the main
			if (voHistory1.Count <= voHistory2.Count )
			{
				oMain = voHistory1;
				oSecondary = voHistory2;
			}
			else
			{
				oMain = voHistory2;
				oSecondary = voHistory1;
			}
			
			//For each main move, get the common moves
			ArrayList oLargestCommonMoves = new ArrayList();
			int iLastMostIndexMatchedInSecondary = -1;
			double dBestMatch = double.NegativeInfinity;
			for(int iMainMoveIndex = 0; iMainMoveIndex <= oMain.Count-1; iMainMoveIndex++)
			{
				//Start searching at each of secondary
				for (int iSecondaryMoveIndex=0; iSecondaryMoveIndex <= oSecondary.Count-1; iSecondaryMoveIndex++)
				{
					int iLastIndexMatchedInSecondary = -1;
					ArrayList oCommonMoves = GetCommonSequencialMovesBetweenRange(oMain,oSecondary,iMainMoveIndex, oMain.Count-1, iSecondaryMoveIndex, ref iLastIndexMatchedInSecondary);
					
					double dThisMatch = (double)oCommonMoves.Count * (double)iLastIndexMatchedInSecondary * (double)iLastIndexMatchedInSecondary;
					if (dBestMatch < dThisMatch )
					{
						oLargestCommonMoves = oCommonMoves;
						iLastMostIndexMatchedInSecondary = iLastIndexMatchedInSecondary;
					}
				}
			}
			
			//Calculate secondary match level
			if ((oSecondary.Count > 0) && (iLastMostIndexMatchedInSecondary != -1))
				rdSecondaryMatchLevel = (double)(iLastMostIndexMatchedInSecondary+1)/(double)oSecondary.Count;
			else
				rdSecondaryMatchLevel = 0;
				
			return oLargestCommonMoves;
		}
		
		public void DisolveHistory(ArrayList voHistory)
		{
//			if rule is empty, any histoy can be dissolved,
//		    otherwise ends of history to dissolve must be same.
//			To dissolve, disassemble history in to parts and store
//			those parts.
//
			ArrayList oRuleHistoryPart = null;
			ArrayList oOnMessagePart = null;
			ArrayList oReplyMessagePart = null;
			double dScorePart = double.NaN;
			
			GetParts(voHistory,ref  oRuleHistoryPart,ref  oOnMessagePart,ref  oReplyMessagePart,ref dScorePart); 
			
			if (mbIsRuleDissolvedOnce == false)
			{
				moMoveHistory = oRuleHistoryPart;
				mdExpectedScore = dScorePart;

				moOnMessage = oOnMessagePart;
				moReplyMessage = oReplyMessagePart;
				mbIsRuleDissolvedOnce = true;
				miHistoryDepthUsed = oRuleHistoryPart.Count;
			}
			else
			{
				double dSecondaryMatchLevel = double.NaN; //unused ref param
				moMoveHistory = FindLargestCommonMoveSet(moMoveHistory,oRuleHistoryPart, ref dSecondaryMatchLevel);
				mdExpectedScore = (mdExpectedScore + dScorePart)/2;
			}
		}
		
		
//		private ArrayList GetCommonSequencialMovesBetweenRange(ArrayList voMain, ArrayList voSecondary, int viStartIndexForMain, int viEndIndexForMain, int viStartIndexForSecondary)
//		{
//			ArrayList oCommonMoves = new ArrayList();
//			
//			for(int iMainScanIndex = viStartIndexForMain ;iMainScanIndex <= viEndIndexForMain; iMainScanIndex++)
//			{
//				for(int iSecondaryScanIndex = viStartIndexForSecondary; iSecondaryScanIndex <= voSecondary.Count-1; iSecondaryScanIndex++)
//				{	
//					Move oMoveToCheck =(Move) voMain[iMainScanIndex];
//					if(oMoveToCheck.IsSimilar(oMoveToCheck, mdMoveSimilarityThresold,  mdMessageSimilarityThresold))
//					{
//						oCommonMoves.Add(oMoveToCheck);
//						//Is this is the last main move to check
//						if ((iMainScanIndex < (voMain.Count-1)) && (iSecondaryScanIndex < (voSecondary.Count-1)))
//						{
//							ArrayList oCommonMovesBetweenLaterMoves = GetCommonSequencialMovesBetweenRange(voMain,voSecondary,iMainScanIndex+1, iMainScanIndex+1, iSecondaryScanIndex+1);
//							oCommonMoves.AddRange(oCommonMovesBetweenLaterMoves);
//						}
//						else break; //end secondary scanning
//					} 
//					else break;
//				}
//			}
//			
//			return oCommonMoves;
//		}

		
		private ArrayList GetCommonSequencialMovesBetweenRange(ArrayList voMain, ArrayList voSecondary, int viStartIndexForMain, int viEndIndexForMain, int viStartIndexForSecondary, ref int riLastIndexMatchedInSecondary)
		{
			ArrayList oCommonMoves = new ArrayList();
			
			int iMainScanIndex = viStartIndexForMain;
			int iSecondaryScanIndex = viStartIndexForSecondary;
			riLastIndexMatchedInSecondary = -1;
				
			while ((iMainScanIndex <= viEndIndexForMain) && (iSecondaryScanIndex <= voSecondary.Count-1))
			{
				bool bIsSimilarMoveFoundForThisMoveInMain = false;
				do
				{
					Move oMoveInMain = (Move) voMain[iMainScanIndex];
					Move oMoveInSecondary =  (Move) voSecondary[iSecondaryScanIndex];
					
					if (oMoveInMain.IsSimilar(oMoveInSecondary, mdMoveSimilarityThresold, mdMessageSimilarityThresold)==true)
					{
						oCommonMoves.Add(oMoveInMain);
						bIsSimilarMoveFoundForThisMoveInMain = true;
						if (iSecondaryScanIndex > riLastIndexMatchedInSecondary)
						{
							riLastIndexMatchedInSecondary = iSecondaryScanIndex;
						}
						break;
					}
					else
					{
						//try next secondary
						iSecondaryScanIndex++;
					}
				}
				while (iSecondaryScanIndex <= voSecondary.Count-1);

				if (bIsSimilarMoveFoundForThisMoveInMain == true)
				{
					//Try next
					iMainScanIndex++;
					iSecondaryScanIndex++;
				}
				//else break; //One of the move in main doesn't have similar move in secondary. This breaks sequencially similar moves
			}
			
			return oCommonMoves;
		}

		
		public override String ToString()
		{
			String sReturn = String.Empty;
			sReturn += msRuleID + ":  ";
			sReturn += "{";
			for(int iMoveIndex=0;iMoveIndex < moMoveHistory.Count; iMoveIndex++)
			{
				Move oThisMove = (Move) moMoveHistory[iMoveIndex];
				sReturn += oThisMove.ToString();
				if (iMoveIndex < moMoveHistory.Count-1)
				{
					sReturn += " | ";
				}
			}
			sReturn += "}";
						
			if (null != moOnMessage)
				sReturn += ", (" + CommonFunctions.ArrayToCSV(moOnMessage.ToArray()) + ") -> ";
			else
				sReturn += "null"
					;
				
			if (null != moReplyMessage)
				sReturn += CommonFunctions.ArrayToCSV(moReplyMessage.ToArray());
			else
				sReturn += "null"
			;

			sReturn += "..." + mdExpectedScore;
			if (mbIsAvoidable==true)
				sReturn += "  (avoid)";
			
			return sReturn;
		}
	}
}
