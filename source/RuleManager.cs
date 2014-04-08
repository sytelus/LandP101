using System;
using System.Collections;

namespace LearnAndPlay
{
	/// <summary>
	/// Summary description for RuleManager.
	/// </summary>
	/// 

	public class RuleMatch : IComparable
	{
		public RuleMatch(Rule voRule, double vdMatchLevel) 
		{
			moRule=voRule;
			mdMatchLevel=vdMatchLevel;
		}
		public RuleMatch()
		{
			moRule=null;
			mdMatchLevel=double.NaN;
		}
		int IComparable.CompareTo(object obj)
		{
			RuleMatch oCompareTo = (RuleMatch)obj;
			double dDifference = mdMatchLevel-oCompareTo.mdMatchLevel;
			int iSign = 0;
			try
			{
				iSign = Math.Sign(dDifference);
			}
			catch (Exception e)
			{
				Log.Write("Sign exception occured ", dDifference.ToString(), true, 1000);
			}
			return iSign;
		}
		public Rule moRule;
		public double mdMatchLevel;
	}
	public class RuleManager
	{
		private Hashtable moRules;
		private double mdMatchThresold;
		public RuleManager()
		{
			moRules = new Hashtable();
			mdMatchThresold = 0.65;
		}
		public double MatchThresold
		{
			get
			{
				return mdMatchThresold;
			}
			set
			{
				mdMatchThresold=value;
			}
		}
		public ArrayList GetMatchingRules(ArrayList voMoveHistory, bool vbIsAvoidableResultCheckNeeded)
		{
			return GetMatchingRules(voMoveHistory, null, vbIsAvoidableResultCheckNeeded);
		}
		public ArrayList GetMatchingRules(ArrayList voMoveHistory, Move voCurrentMove, bool vbIsAvoidableResultCheckNeeded)
		{
			ArrayList oMatchingRules = new ArrayList();
			double dHighestMatchLevel = double.NegativeInfinity;
			
			//Go thr' each rule to find the match
			foreach(Rule oRule in moRules.Values)
			{
				double dMatchLevelForRule;
				if (vbIsAvoidableResultCheckNeeded == false)
				{
					dMatchLevelForRule = oRule.EvaluateMatch(voMoveHistory, voCurrentMove, null);
				}
				else
				{
					dMatchLevelForRule = oRule.EvaluateMatch(voMoveHistory, voCurrentMove, moRules.Values);
				}
				
				//Donot include avoidable rule's ratings to calculate highest match because
				//avoidable rules will usually have smaller history giving high levels of matches
				if ((dHighestMatchLevel < dMatchLevelForRule) && (oRule.IsAvoidable == false))
					dHighestMatchLevel = dMatchLevelForRule;
				
				if (dMatchLevelForRule >= 0 )
				{
					oMatchingRules.Add(new RuleMatch (oRule, dMatchLevelForRule));
				}
			}
			oMatchingRules.Sort(null);
			
			//Now take of the rule matches which is far beyond highest level match
			//First element has lowest match. Start from end of the list because we will be REMOVING elements from the array
			for (int iRuleMatchIndex = oMatchingRules.Count-1; iRuleMatchIndex >= 0; iRuleMatchIndex--)
			{
				RuleMatch oRuleMatchToTest = (RuleMatch) oMatchingRules[iRuleMatchIndex];
				if (
					(Rule.IsNumbersSimilar(dHighestMatchLevel, oRuleMatchToTest.mdMatchLevel, 0.65)==true)
					||
					(oRuleMatchToTest.mdMatchLevel > dHighestMatchLevel)	//Case of avoidable rule
					)
				{
					//Keep this one
				}
				else
				{
					//Remove this one
					oMatchingRules.RemoveAt(iRuleMatchIndex);
				}
			}
			
			return oMatchingRules;
		}
		
		
		public Rule DisolveHistory(ArrayList voMoveHistory, String vsRuleIDToDissolveWith, bool vbForceCreateNew, bool vbForceDissolvation, bool vbPreventCreateNew)
		{
		
			//Disolving history will be done only when a score is generated. Specf:
			//1. For last scored move
			//2. For all other moves before scored move but no rules applied to it
			
			//Find matching rule with highest match. If no match found then create a new rule.
			//If matches found then get the common factor  and search if that common factor belongs to
			//any avoidable rules. If not then disolve history with that rule else find the next matching rule
			//and repeat the process.
			//
		
			//Get matching rules, sorted by match level low to high

			bool bIsNewRuleRequired = false;
			Rule oRuleWithHighestMatch = null;
			Rule oRuleToReturn = null;
			if (vbForceCreateNew==false)
			{
				if (null == vsRuleIDToDissolveWith)
				{
					ArrayList oMatchingRules = GetMatchingRules(voMoveHistory, true);
					
					//If we found matching rules,
					if (oMatchingRules.Count > 0)
					{
						//Get the one with highest match
						RuleMatch oRuleMatchWithHighestMatch = (RuleMatch)oMatchingRules[oMatchingRules.Count-1];
						oRuleWithHighestMatch = oRuleMatchWithHighestMatch.moRule;
						if (vbForceDissolvation == false)
						{
							bIsNewRuleRequired = (oRuleMatchWithHighestMatch.mdMatchLevel <= 0);
						}
						else
						{
							//Dissolve even if poor match
							bIsNewRuleRequired = false;
						}
					}
					else
					{
						bIsNewRuleRequired = true;
					};
				}
				else
				{
					oRuleWithHighestMatch = (Rule) moRules[vsRuleIDToDissolveWith];
				}
			}
			else
			{
				bIsNewRuleRequired = true;
			}
			
			if (bIsNewRuleRequired == false)
			{
				Log.Write("Rule match found, move will be dissolved", "", true, 4);

				//Dissolve history with this one
				oRuleWithHighestMatch.DisolveHistory(voMoveHistory);
				oRuleToReturn = oRuleWithHighestMatch;
			}
			else
			{
				if (vbPreventCreateNew == false)
				{
					Log.Write("Creating new rule", "", true, 4);
					//Create a new rule
					Rule oNewRule = new Rule();
					//Set it up by dissolving history in to it
					oNewRule.DisolveHistory(voMoveHistory);
					//Add it in to our collection
					moRules.Add(oNewRule.RuleID,oNewRule);
					oRuleToReturn = oNewRule;
				}
				else {}; //Do not create new
			};
			
			return oRuleToReturn;
		}

//		private RuleMatch FindRuleMatchWithHighestMatch(ArrayList voRuleMatchesToSearch)
//		{
//			double dHighestMatchLevelFound = double.NegativeInfinity;
//			int iRuleMatchIndexFound = -1;
//			for(int i;i < voRuleMatchesToSearch.Length;i++)
//			{
//				if(vaRuleMatchesToSearch[i].mdMatchLevel > dHighestMatchLevelFound)
//				{
//					dHighestMatchLevelFound = voRuleMatchesToSearch[i].mdMatchLevel;
//					iRuleMatchIndexFound = i;
//				}
//			}
//			if(iRuleMatchIndexFound != -1)
//			{
//				return voRuleMatchesToSearch[iRuleMatchIndexFound];
//			}
//			else return null;
//		}

		public Hashtable Rules
		{
			get
			{
				return moRules;
			}
		}
		
		//Select the better rulematch of given two
		public static RuleMatch SelectBetterRuleMatch(RuleMatch voRuleMatch1, RuleMatch voRuleMatch2)
		{
			RuleMatch oBetterRuleMatch = null;
			
			//If we haven't got any success rule match, get one
			if (null == voRuleMatch1)
			{
				oBetterRuleMatch = voRuleMatch2;
			}
			else if (null == voRuleMatch2)
			{
				oBetterRuleMatch = voRuleMatch1;
			}
			else
			{
				//See if our previos rule match had same match but lower score
				if (Rule.IsNumbersSimilar(voRuleMatch1.mdMatchLevel, voRuleMatch2.mdMatchLevel, 0.9)==true)
				{
					if (voRuleMatch1.moRule.ExpectedScore < voRuleMatch2.moRule.ExpectedScore)
					{
						oBetterRuleMatch = voRuleMatch2;
					}
					else if (voRuleMatch1.moRule.ExpectedScore == voRuleMatch2.moRule.ExpectedScore)
					{
						if (CommonFunctions.GetRandomNumber(2) > 1)
						{
							oBetterRuleMatch = voRuleMatch2;
						} 
						else 
						{
							oBetterRuleMatch = voRuleMatch1;
						}
					}
					else
					{
						oBetterRuleMatch = voRuleMatch1;
					}
				}
				else
				{
					//Select the one with higher rule match
					if (voRuleMatch1.mdMatchLevel > voRuleMatch2.mdMatchLevel)
					{
						oBetterRuleMatch = voRuleMatch1;
					}
					else
					{
						oBetterRuleMatch = voRuleMatch2;
					}
				}
			}		
			
			return oBetterRuleMatch;
		}		
		
		public override String ToString()
		{
			String sReturn = String.Empty;
			foreach(Rule oRule in moRules.Values)
			{
				sReturn += oRule.ToString() + "\n";
			}
			return sReturn;
		}
	}
}
