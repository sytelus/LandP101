using System;
using System.Collections;

//TODO: after success rules have been found, failure rules needs to be cleaned up.
//TODO: instead of passing true random rule, do some extra polation (static games enhanced)
//TODO: what if send/req/score sequence is unexpected/incomplete.
//TODO: What is "Assumed score" for a move overrides actual builded rule.
//TODO: Handling for unperfact systems (glitches)

namespace LearnAndPlay
{
	/// <summary>
	/// Entity with wich interaction is made
	/// </summary>
	public class Player
	{
		private Game moGame;
		private RuleManager moRuleManager;
		private const double SCORE_MAX = 1.0;
		private const double SCORE_MIN = -1.0;
		private const double SCORE_MEAN = 0;
		private const double MAX_ALLOWED_ESTIMATED_SCORE_DEVIATION = 0.35;
		private const double HISTORY_DEPTH_TO_INCREASE_WHEN_RULE_FAILS = 1.35;
		private int miMaxHistoryDepthForReply = 7;
		public event EventHandler RulesChanged;
		public event EventHandler MoveHistoryChanged;

		public Player()
		{
			InitializePrivateMembers();
		}

		private void InitializePrivateMembers()
		{
			moGame = new Game();
			moRuleManager = new RuleManager();
			Rule.ResetIDCounter();			
		}
		public Game Game
		{
			get
			{
				return moGame;
			}
		}
		public void StartNewGame()
		{
			moGame.StartNewRound();

			if (MoveHistoryChanged != null) MoveHistoryChanged(this, null);
		}
		public void RegisterGameSymbols(ArrayList voAvailableSymbols)
		{
			for(int iAvailableSymbolIndex=0;iAvailableSymbolIndex < voAvailableSymbols.Count;iAvailableSymbolIndex++)
			{
				String sThisSymbolToAdd = (String) voAvailableSymbols[iAvailableSymbolIndex];
				if (moGame.AvailableSymbols.IndexOf(sThisSymbolToAdd) == -1)
				{
					moGame.AvailableSymbols.Add(sThisSymbolToAdd);
				}
			}
		}
        public void PutMessage(ArrayList voMessageContent)
		{
			Message oMessage = new Message(voMessageContent, Message.PLAYER_JUDGE);
			moGame.RecordMessage(oMessage);
			
			if (MoveHistoryChanged != null) MoveHistoryChanged(this, null);			
		}
		public Message GetMessage()
		{
			//Get the history until certain depth, since last score
			ArrayList oHistoryDepthToLookFor = moGame.History.GetHistoryByDepth(miMaxHistoryDepthForReply, true);
			
			//Find the applicable rules for history + current move
			ArrayList oMatchingRules = moRuleManager.GetMatchingRules(oHistoryDepthToLookFor, moGame.CurrentMove, false);

			//Analyse matching rules, pick the best applicable and send out response suggested by it
			Move oCorrentMove = moGame.CurrentMove;
			Message oReply = AnalyzeRulesAndSelectResponse(oMatchingRules, ref oCorrentMove);
			
			//Add this message in to the history
			oReply.Source = Message.PLAYER_THIS;
			moGame.RecordMessage(oReply);

			if (MoveHistoryChanged != null) MoveHistoryChanged(this, null);			
			
			//return our response
			return oReply;
		}
		
		public ArrayList GetMatchingRules()
		{
			//Get the history until certain depth, since last score
			ArrayList oHistoryDepthToLookFor = moGame.History.GetHistoryByDepth(miMaxHistoryDepthForReply, true);
			
			//Find the applicable rules for history + current move
			ArrayList oMatchingRules = moRuleManager.GetMatchingRules(oHistoryDepthToLookFor, moGame.CurrentMove, false);
		
			return 	oMatchingRules;	
		}
		
		private int SymbolCountInOnMessage(Move voMove)
		{
			int iSymbolCountInOnMessage=1;
			if (voMove.OnMessage != null)
			{
				iSymbolCountInOnMessage = voMove.OnMessage.Content.Count;
			}
			else
				iSymbolCountInOnMessage = 1;
			
			return iSymbolCountInOnMessage;
		}
				
		private Message AnalyzeRulesAndSelectResponse(ArrayList voMatchingRules, ref Move roMoveToUpdate)
		{
			//Go through each rule and get the 4 categories of rules
			RuleMatch oRuleMatchYieldingHighestScore = null;
			ArrayList aRulesYieldingHighestScore = new ArrayList();
			ArrayList aAvoidableRulesYieldingHighestScore = new ArrayList();
			RuleMatch oAvoidableRuleMatchYieldingHighestScore = null;
			RuleMatch oRuleMatchYieldingLowestScore = null;
			RuleMatch oAvoidableRuleMatchYieldingLowestScore = null;
			ArrayList aRulesYieldingLowestScore  = new ArrayList();
			ArrayList aAvoidableRulesYieldingLowestScore  = new ArrayList();

			Log.Write("Rule Matches found:", voMatchingRules.Count.ToString(), true,4);
			
			//Rules are sorted by least matching to most matching
			//Classify rules in 4 categories
			for(int iRuleIndex = voMatchingRules.Count-1; iRuleIndex >=0; iRuleIndex--)
			{
				RuleMatch oRuleMatchToTest =(RuleMatch) voMatchingRules[iRuleIndex];
				Rule oRuleToTest = oRuleMatchToTest.moRule;
				
				if (oRuleToTest.IsAvoidableObsolete == false)
				{
					//Is this success rule?
					if (oRuleToTest.ExpectedScore >= SCORE_MEAN)
					{
						//Is this rule avoidable
						if (oRuleToTest.IsAvoidable==false)
						{
							oRuleMatchYieldingHighestScore = RuleManager.SelectBetterRuleMatch(oRuleMatchYieldingHighestScore, oRuleMatchToTest);
							if (Rule.IsNumbersSimilar(oRuleMatchYieldingHighestScore.mdMatchLevel, oRuleMatchToTest.mdMatchLevel, 0.5))
							{
								if (false == aRulesYieldingHighestScore.Contains(oRuleToTest))
								{
									aRulesYieldingHighestScore.Add(oRuleToTest);
								}
								else
								{} // to less match to include
							}
						}
						else
						{
							oAvoidableRuleMatchYieldingHighestScore = RuleManager.SelectBetterRuleMatch(oAvoidableRuleMatchYieldingHighestScore, oRuleMatchToTest);
							if (Rule.IsNumbersSimilar(oAvoidableRuleMatchYieldingHighestScore.mdMatchLevel, oRuleMatchToTest.mdMatchLevel, 0.5))
							{
								if (false == aAvoidableRulesYieldingHighestScore.Contains(oRuleToTest))
								{
									aAvoidableRulesYieldingHighestScore.Add(oRuleToTest);
								}
								else
								{} // to less match to include
							}
						}
					}
					else
					{
						//this is a failure rule
						if (oRuleToTest.IsAvoidable==false)
						{
							oRuleMatchYieldingLowestScore = RuleManager.SelectBetterRuleMatch(oRuleMatchYieldingLowestScore, oRuleMatchToTest);
							if (Rule.IsNumbersSimilar(oRuleMatchYieldingLowestScore.mdMatchLevel, oRuleMatchToTest.mdMatchLevel, 0.95))
							{
								if (false == aRulesYieldingLowestScore.Contains(oRuleToTest))
								{
									aRulesYieldingLowestScore.Add(oRuleToTest);
								}
								else
								{
									Log.Write ("Dropped rule: ", oRuleToTest.RuleID, false, 3);
								}
							}
						}
						else
						{
							oAvoidableRuleMatchYieldingLowestScore  = RuleManager.SelectBetterRuleMatch(oAvoidableRuleMatchYieldingLowestScore, oRuleMatchToTest);
							if (Rule.IsNumbersSimilar(oAvoidableRuleMatchYieldingLowestScore.mdMatchLevel, oRuleMatchToTest.mdMatchLevel, 0.6))
							{
								if (false == aAvoidableRulesYieldingLowestScore.Contains(oRuleToTest))
								{
									aAvoidableRulesYieldingLowestScore.Add(oRuleToTest);
								}
								else
								{
									Log.Write ("Dropped rule ", oRuleToTest.RuleID, false, 3);
								}
							}
						}
					}
				} else {}; //ignore avoidable obsolete
			}
			
			
			//Now we have classified rule with max match level in each of 4 categories
			//Theory: If there is success rule with match level >= Thresolds.MIN_GOOD_RULE_MATCH_TO_MAKE_IT_AVOIDABLE
			// we would apply this rule even if there is failure rule with higher match. But if not, then we will see
			//if there is any non-avoidable rules exist. If it does, we will select one with the highest match level. Then we will 
			//make group of same kind of rules with similar match. If the selected rule was success, we will select randomly one.
			//If it was failure then we will apply whole group. If there were no non-avoidable rules then we will
			//select avoidable rule with best match, create it's group and do same thing as in non-avoidable rules.
			
			Message oSelectedMessageForReply = null;
			ArrayList aSelectedMessageForReply = null;
			ArrayList aAppliedRuleIDs = new ArrayList();
			double dAvgExpectedScore = double.NaN;
			double dAvgMatchLevelOfAppliedRules = double.NaN;
			double dAvgHistoryDepthOfAppliedRules = double.NaN;
			
			bool bIsAnyRuleSelected = false;
			
			if ((null != oRuleMatchYieldingHighestScore ) && (bIsAnyRuleSelected == false))
			{
				if (oRuleMatchYieldingHighestScore.mdMatchLevel >= Thresolds.MIN_GOOD_RULE_MATCH_TO_MAKE_IT_AVOIDABLE)
				{
					Log.Write("Success rule above thresold found","",true,2);
					ApplySuccessRule(oRuleMatchYieldingHighestScore, ref aSelectedMessageForReply, aAppliedRuleIDs, 
						ref dAvgExpectedScore, ref dAvgMatchLevelOfAppliedRules, ref dAvgHistoryDepthOfAppliedRules);
					bIsAnyRuleSelected = true;
				}
				else 
				{
					Log.Write("Success rule exist but below thresold","",true,2);
				} //see for other rules
			}
			
			//If there is no success rule with match level in avoidable markable range,
			//apply any other success of failure rules with highest match level
			if (
				(bIsAnyRuleSelected == false)
				&&
				((null != oRuleMatchYieldingLowestScore) || (null != oRuleMatchYieldingHighestScore ))
				)
			{
				Log.Write("Success/failure rule exist","",true,2);
				double dHighestMatch = double.NegativeInfinity;
				bool bIsSuccessRuleWithHighestMatch = false;
				
				if (null != oRuleMatchYieldingHighestScore )
				{
					if (dHighestMatch <= oRuleMatchYieldingHighestScore.mdMatchLevel)
					{
						Log.Write ("Success rule " + oRuleMatchYieldingHighestScore.moRule.RuleID + " with match ", oRuleMatchYieldingHighestScore.mdMatchLevel.ToString(), true, 1);						
						dHighestMatch = oRuleMatchYieldingHighestScore.mdMatchLevel;
						bIsSuccessRuleWithHighestMatch = true;
					}
				}
				
				if (null != oRuleMatchYieldingLowestScore )
				{
					if (dHighestMatch < oRuleMatchYieldingLowestScore.mdMatchLevel)
					{
						Log.Write ("Failure rule " + oRuleMatchYieldingLowestScore.moRule.RuleID + " with match ", oRuleMatchYieldingLowestScore.mdMatchLevel.ToString(), true, 1);
						dHighestMatch = oRuleMatchYieldingLowestScore.mdMatchLevel;
						bIsSuccessRuleWithHighestMatch = false;
					}
				}
				
				if (bIsSuccessRuleWithHighestMatch == false)
				{
					Log.Write("Failure Rule with best match " + oRuleMatchYieldingLowestScore.moRule.RuleID + " applied, ml=", oRuleMatchYieldingLowestScore.mdMatchLevel.ToString(), true, 4);
					aSelectedMessageForReply = SelectReplyForFailureRule(oRuleMatchYieldingLowestScore, aRulesYieldingLowestScore, SymbolCountInOnMessage(roMoveToUpdate), aAppliedRuleIDs, ref dAvgExpectedScore, ref dAvgMatchLevelOfAppliedRules, ref dAvgHistoryDepthOfAppliedRules);
				}
				else
				{
					Log.Write("Success rule with best match " + oRuleMatchYieldingHighestScore.moRule.RuleID + " ", oRuleMatchYieldingHighestScore.mdMatchLevel.ToString(),true,2);
					ApplySuccessRule(oRuleMatchYieldingHighestScore, ref aSelectedMessageForReply, aAppliedRuleIDs, 
						ref dAvgExpectedScore, ref dAvgMatchLevelOfAppliedRules, ref dAvgHistoryDepthOfAppliedRules);
				}
				bIsAnyRuleSelected = true;
			}	
			
			
			//if avoidable rules available, use them
			if (
				(bIsAnyRuleSelected == false)
				&&
				((null != oAvoidableRuleMatchYieldingLowestScore) || (null != oAvoidableRuleMatchYieldingHighestScore))
				)
			{
				Log.Write("Avoidable rule exist and will be applied","",true,2);
				double dHighestMatch = double.NegativeInfinity;
				bool bIsAvoidableSuccessRuleWithHighestMatch = false;
				
				if (null != oAvoidableRuleMatchYieldingHighestScore)
				{
					if (dHighestMatch <= oAvoidableRuleMatchYieldingHighestScore.mdMatchLevel)
					{
						dHighestMatch = oAvoidableRuleMatchYieldingHighestScore.mdMatchLevel;
						bIsAvoidableSuccessRuleWithHighestMatch = true;
					}
				}
				
				if (null != oAvoidableRuleMatchYieldingLowestScore )
				{
					if (dHighestMatch < oAvoidableRuleMatchYieldingLowestScore.mdMatchLevel)
					{
						dHighestMatch = oAvoidableRuleMatchYieldingLowestScore.mdMatchLevel;
						bIsAvoidableSuccessRuleWithHighestMatch = false;
					}
				}
				
				if (bIsAvoidableSuccessRuleWithHighestMatch == false)
				{
					Log.Write("Avoidable Failure Rule " + oAvoidableRuleMatchYieldingLowestScore.moRule.RuleID + " applied, ml=", oAvoidableRuleMatchYieldingLowestScore.mdMatchLevel.ToString(), true, 4);
					aSelectedMessageForReply = SelectReplyForFailureRule(oAvoidableRuleMatchYieldingLowestScore, aRulesYieldingLowestScore, SymbolCountInOnMessage(roMoveToUpdate), aAppliedRuleIDs , ref dAvgExpectedScore, ref dAvgMatchLevelOfAppliedRules, ref dAvgHistoryDepthOfAppliedRules);
				}
				else
				{
					ApplySuccessRule(oAvoidableRuleMatchYieldingHighestScore, ref aSelectedMessageForReply, aAppliedRuleIDs, 
						ref dAvgExpectedScore, ref  dAvgMatchLevelOfAppliedRules, ref dAvgHistoryDepthOfAppliedRules);
				}
				bIsAnyRuleSelected = true;
			}	
			

			if (bIsAnyRuleSelected == false)
			{
				Log.Write("No rule applied", "", true, 4);
				aSelectedMessageForReply = null;
			} else {} ; //some other rule was already applied
			
			//If no rules were applied or random rule was selected due to EBF
			if (aSelectedMessageForReply != null)
			{
				oSelectedMessageForReply = new Message(aSelectedMessageForReply, Message.PLAYER_THIS) ;
				//Update message to indicate which rule we had applied to it to get it
				roMoveToUpdate.AppliedRuleIDs = aAppliedRuleIDs;
				roMoveToUpdate.EstimatedScore = dAvgExpectedScore;
				roMoveToUpdate.AvgMatchLevelOfAppliedRules = dAvgMatchLevelOfAppliedRules;
				roMoveToUpdate.AvgHistoryDepthOfAppliedRules = dAvgHistoryDepthOfAppliedRules;
			}
			else
			{
				ArrayList oNewMessageContent = new ArrayList(1);
				oNewMessageContent.AddRange (moGame.GetRandomSymbol(true, SymbolCountInOnMessage(roMoveToUpdate)));
				oSelectedMessageForReply  = new Message(oNewMessageContent, Message.PLAYER_THIS );
				roMoveToUpdate.AppliedRuleIDs = new ArrayList();
				roMoveToUpdate.EstimatedScore = double.NaN;
				roMoveToUpdate.AvgMatchLevelOfAppliedRules = double.NaN;
				roMoveToUpdate.AvgHistoryDepthOfAppliedRules = double.NaN;
			}

			Log.Write("------------------------reply over for move-----",moGame.History.Moves.Count.ToString(),true,2);
			return oSelectedMessageForReply ;
		}
	
		private void ApplySuccessRule(RuleMatch voRuleMatchToApply, ref ArrayList raSelectedMessageForReply, ArrayList vaAppliedRuleIDs, ref double rdAvgExpectedScore, ref double rdAvgMatchLevelOfAppliedRules, ref double rdAvgHistoryDepthOfAppliedRules)
		{
			Log.Write("Success Rule " + voRuleMatchToApply.moRule.RuleID + " applied, ml=" + voRuleMatchToApply.mdMatchLevel.ToString(), "", true, 4);
			raSelectedMessageForReply = voRuleMatchToApply.moRule.ReplyMessage;
			vaAppliedRuleIDs.Add (voRuleMatchToApply.moRule.RuleID);
			rdAvgExpectedScore = voRuleMatchToApply.moRule.ExpectedScore;
			rdAvgMatchLevelOfAppliedRules = voRuleMatchToApply.mdMatchLevel;
			rdAvgHistoryDepthOfAppliedRules = voRuleMatchToApply.moRule.HistoryDepthUsed;
		}
		
		
		private ArrayList SelectReplyForFailureRule(RuleMatch voFailureBaseRuleMatch, ArrayList vaFailureRules, 
			int iSymbolCountToUseForRandomMessage, ArrayList vaAppliedRuleIDs, ref double rdAvgExpectedScore, ref double rdAvgMatchLevelOfAppliedRules,
			ref double rdAvgHistoryDepthOfAppliedRules )
		{
			bool bEBFOccured = false;
			ArrayList aSelectedMessageForReply = null;
			CombineSymbolsAndNegate(voFailureBaseRuleMatch, vaFailureRules,null,iSymbolCountToUseForRandomMessage, ref aSelectedMessageForReply, ref bEBFOccured);
			if (bEBFOccured==false)
			{
				for(int iRuleYieldingLowScore = 0;iRuleYieldingLowScore < vaFailureRules.Count;iRuleYieldingLowScore++)
				{
					//dTotalExpectedScore += 	((Rule)(aRulesYieldingLowestScore[iRuleYieldingLowScore])).ExpectedScore;
					vaAppliedRuleIDs.Add (((Rule)(vaFailureRules[iRuleYieldingLowScore])).RuleID);	
				}
				//If we apply failure rules, we can't predict the score
				rdAvgExpectedScore = double.NaN; //dTotalExpectedScore / aRulesYieldingLowestScore.Count;
				//Change code to calculate total match level and then calculate avg
				rdAvgMatchLevelOfAppliedRules = voFailureBaseRuleMatch.mdMatchLevel;
				rdAvgHistoryDepthOfAppliedRules = voFailureBaseRuleMatch.moRule.HistoryDepthUsed;
			}
			else
			{
				//EBF has occured
				//Scan the history to find the last move which had success score
				//and applied rules
				ArrayList oMoveHistory = moGame.History.Moves;
				for(int iMoveIndex=oMoveHistory.Count-1; iMoveIndex >= 0; iMoveIndex--)
				{
					Move oMoveToCheckForEBFCause = (Move) oMoveHistory[iMoveIndex];
					if ((oMoveToCheckForEBFCause.ActualScore > SCORE_MEAN) && (oMoveToCheckForEBFCause.AppliedRuleIDs.Count != 0))
					{
						//Make these rules avoidable and create failure rules
						bool rbWasRuleDissolvationDone = false; 
						MakeRulesAppliedToMoveAsAvoidable(oMoveToCheckForEBFCause, false, ref rbWasRuleDissolvationDone);
						if (rbWasRuleDissolvationDone == true)
							break;	//Change only last move
					}
				}
				
				//Select random reply
				aSelectedMessageForReply = null;
			}
			
			return aSelectedMessageForReply;
		}
	
		
		
	
//		public void PutScore1(double vdScore)
//		{
//
//			//First analyse moves before current move since last score
//			
//			//Get move history since last score
//			ArrayList voHistorySinceLastScore = moGame.History.GetHistoryFromLastScore(false);
//			
//			Log.Write("Histry depth to match for:", voHistorySinceLastScore.Count.ToString(), true);
//			
//			//For each move since last score, analyze whether rules we used were correct
//			int iHistorySinceLastScoreMoveCount = voHistorySinceLastScore.Count;
//			for(int i=0;i < iHistorySinceLastScoreMoveCount;i++)
//			{
//				//Get each of the moves until last score
//				Move oMoveToAnalyze = (Move)voHistorySinceLastScore[i];
//				
//				//This is not last move and we haven't assigned score, assume it to be a success
//				oMoveToAnalyze.ActualScore= SCORE_MAX;
//				
//				AnalyzeMoveForDissolvation(oMoveToAnalyze, voHistorySinceLastScore);
//			}
//			
//			//Now dissolve the history for current move
//			
//			//Note down score in to our current move
//			Move oUpdatedMove = moGame.RecordScore(vdScore);
//			
//			ArrayList aHistoryForCurrent = moGame.History.GetHistoryByDepth(iHistorySinceLastScoreMoveCount,true);
//			
//			AnalyzeMoveForDissolvation(oUpdatedMove,aHistoryForCurrent);
//
//		}
//
//
		public void PutScore(double vdScore)
		{
			//Note down score in to our current move
			Move oUpdatedMove = moGame.RecordScore(vdScore);
			
			//If score was not ignored
			if (null != oUpdatedMove)
			{
				ArrayList oAllMoveList = moGame.History.Moves;
				
				//Now from backward go to every move in our history that we haven't check yet
				for(int iAllMoveIndex = oAllMoveList.Count-1; iAllMoveIndex >= 0; iAllMoveIndex--)
				{
					Move oMoveToAnalyze = (Move) oAllMoveList[iAllMoveIndex];
					
					//If this move was not scored, make it as success
					if (double.IsNaN(oMoveToAnalyze.ActualScore))
					{
						oMoveToAnalyze.ActualScore = SCORE_MAX;
					}
					
					if (oMoveToAnalyze.IsAnalyzed == false)
					{
						//Now check if we applied any rule while giving a response
						if ((oMoveToAnalyze.AppliedRuleIDs.Count != 0))
						{
							//Check if we did this success check already
							if (oMoveToAnalyze.IsRuleSuccessChecked == false)
							{
								//Did we had prediction?
								if (double.IsNaN(oMoveToAnalyze.EstimatedScore)==false)
								{
									//Is what happened was complete opposite of failure/success predicted
									if (Math.Abs (Math.Sign(oMoveToAnalyze.EstimatedScore)-Math.Sign(oMoveToAnalyze.ActualScore)) > 1)
									{
										//Rule was complete failure
										
										//We should mark the rule as avoidable
										bool rbIsRuleDissolvationDone = false; //not used ref param
										MakeRulesAppliedToMoveAsAvoidable(oMoveToAnalyze, true, ref rbIsRuleDissolvationDone);
									}
									else
									{
										
										//There was no complete failure but see if expected and actual scores are differing by wide margin
										if (Math.Abs(oMoveToAnalyze.ActualScore-oMoveToAnalyze.EstimatedScore) > MAX_ALLOWED_ESTIMATED_SCORE_DEVIATION)
										{
											//Rule prediction is significantly different but still a success 
											//Try to dissolve this history with bigger depth
											DissolveHistory(oMoveToAnalyze.MoveIndex, SelectBiggerHistoryDepth(oMoveToAnalyze), null, false, false, false);
										}
										else
										{
											//Rule prediction was right
											for (int iAppliedRuleIndex = 0; iAppliedRuleIndex < oMoveToAnalyze.AppliedRuleIDs.Count; iAppliedRuleIndex++)
											{
												Rule oVarifiedRule = (Rule) moRuleManager.Rules[(String) oMoveToAnalyze.AppliedRuleIDs[iAppliedRuleIndex]];
												//Was that avoidable rule
												if (oVarifiedRule.IsAvoidable == false)
												{
													//Dissolve in to nearest existing rule
													DissolveHistory(oMoveToAnalyze.MoveIndex, (int) oMoveToAnalyze.AvgHistoryDepthOfAppliedRules + 1,null, false, false, false);
												}
												else
												{
													//Create the new rule out of current history
													DissolveHistory(oMoveToAnalyze.MoveIndex,SelectBiggerHistoryDepth(oMoveToAnalyze), null, false, false, false);
												}
											}
										}
									}
								}
								else
								{
									//We didn't had prediction for this rule. If the move has resulted in failure
									if (Math.Sign(oMoveToAnalyze.ActualScore) == -1)
									{
										//If we failed again, this might be same failure rule or a different failure rule
										//try to get dissolved with nearest matching or else create new
										DissolveHistory(oMoveToAnalyze.MoveIndex, SelectBiggerHistoryDepth(oMoveToAnalyze), null, false, false, false);
									}
									else	//move has resulted in success
									{
										//Create new rule
										DissolveHistory(oMoveToAnalyze.MoveIndex, miMaxHistoryDepthForReply, null, false, false, false);
									}
								}
								oMoveToAnalyze.IsRuleSuccessChecked=true;
							}
							else
							{};	//Don't check the success again
						}
						else
						{
							//Reply was selected randomly. Save this history for future purposes
							Rule oCreatedOrAppliedRule = DissolveHistory(oMoveToAnalyze.MoveIndex, miMaxHistoryDepthForReply, null, false, false, false);
							//oMoveToAnalyze.AppliedRuleIDs.Add(oCreatedOrAppliedRule.RuleID);
						}		
						
						//Mark this move as analyzed
						oMoveToAnalyze.IsAnalyzed = true;
					}
					else
						break;	//Assume that previous moves have been already analyzed
				}
			}
			else {} //no action needed if score was ignored
			
			if (RulesChanged != null) RulesChanged(this, null);
			if (MoveHistoryChanged != null) MoveHistoryChanged(this, null);
			Log.Write ("....................................","",true,1);
		}

		
		private void MakeRulesAppliedToMoveAsAvoidable(Move voMove, bool vbDissolveRuleEvenIfPoorMatch, ref bool rbWasRuleDissolvationDone)
		{
			bool bIsAvoidableRuleFailureFound = false;
			bool bIsPoorMatch = (voMove.AvgMatchLevelOfAppliedRules < Thresolds.MIN_GOOD_RULE_MATCH_TO_MAKE_IT_AVOIDABLE);
									
			//See if rule match we applied was quite matching
			if (bIsPoorMatch == false)
			{
				//We should mark the rule as avoidable
				Log.Write("Faulty rule found for move index", voMove.MoveIndex.ToString(), true, 4);
				for(int iAppliedRuleIndex = 0; iAppliedRuleIndex < voMove.AppliedRuleIDs.Count;iAppliedRuleIndex++)
				{
					//Our rule deduction was faulty
					//Mark the rule we applied as avoidable
					Rule oAppliedRuleToMove = (Rule)moRuleManager.Rules[(String)voMove.AppliedRuleIDs[iAppliedRuleIndex]];
					if (oAppliedRuleToMove.IsAvoidable==true)
					{
						//Avoidable rule has turned out to be faulty. There should
						//be opposite rule already created. We should try to dissolve with this rule
						//instead of creating new
						bIsAvoidableRuleFailureFound = true;
					}
					else
					{
						oAppliedRuleToMove.IsAvoidable=true;	//If this rule was avoidable, it will mark it again
					}
				}
			}
			else
			{
				//Match was not that good.
				//Do not rollback our rule yet.
				Log.Write ("Rule won't be marked avoidable because match level was poor","", true, 3);
			}
									
			rbWasRuleDissolvationDone = false;
			if ((vbDissolveRuleEvenIfPoorMatch==true) || (bIsPoorMatch == false))
			{
				int iHistoryDeplthToUse = SelectBiggerHistoryDepth(voMove);
					
				if (bIsAvoidableRuleFailureFound==false)
				{
					//Create the new rule out of this situation because we didn't got
					//expected score. This will create the opposite rule. Next time this rule have better match then the one we applied earlier.
					//??
					DissolveHistory(voMove.MoveIndex,iHistoryDeplthToUse, null, false, false, true);
					DissolveHistory(voMove.MoveIndex,iHistoryDeplthToUse, null, true, false, false);
				}
				else
				{
					//Try to dissolve with avoidable rule's companian
					DissolveHistory(voMove.MoveIndex, iHistoryDeplthToUse, null, false, true, false);
				}
				rbWasRuleDissolvationDone = true;
			}
			else {}; //do not dissolve
		}
				
		private int SelectBiggerHistoryDepth(Move voMove)
		{
			int iHistoryDeplthToUse = 0;
			if (miMaxHistoryDepthForReply > Math.Round(voMove.AvgHistoryDepthOfAppliedRules))
				iHistoryDeplthToUse = (int) Math.Round (miMaxHistoryDepthForReply * HISTORY_DEPTH_TO_INCREASE_WHEN_RULE_FAILS);
			else
				iHistoryDeplthToUse = (int) Math.Round (voMove.AvgHistoryDepthOfAppliedRules * HISTORY_DEPTH_TO_INCREASE_WHEN_RULE_FAILS);
				
			return iHistoryDeplthToUse;
		}
		
		private Rule DissolveHistory(int viForMoveIndex, int viHistoryDepth, String vsRuleIDToDissolveWith, bool vbForceCreateNew, bool vbForceDissolvation, bool vbPreventCreateNew)
		{
			ArrayList oHistoryForAvoidableMove = 
				moGame.History.GetHistoryByDepth(viHistoryDepth, viForMoveIndex, true);
			return moRuleManager.DisolveHistory(oHistoryForAvoidableMove, vsRuleIDToDissolveWith, vbForceCreateNew, vbForceDissolvation, vbPreventCreateNew);
		}
		
//		private void AnalyzeMoveForDissolvation(Move voMove, ArrayList voHistorySinceLastScore)
//		{
//			//Now check if we applied any rule while giving a response
//			if ((voMove.AppliedRuleIDs.Count != 0))
//			{
//				//Did we already checked the success?
//				if (voMove.IsRuleSuccessChecked==false)
//				{
//					//Was our estimated score is much less then actual score?
//					if ((voMove.EstimatedScore-voMove.ActualScore) > MAX_ALLOWED_ESTIMATED_SCORE_DEVIATION )
//					{
//						Log.Write("Faulty rule found for move index", voMove.MoveIndex.ToString(), true);
//						for(int iAppliedRuleIndex = 0; iAppliedRuleIndex < voMove.AppliedRuleIDs.Count;iAppliedRuleIndex++)
//						{
//							//Our rule deduction was faulty
//							//Mark the rule we applied as avoidable
//							Rule oAppliedRuleToMove = (Rule)moRuleManager.Rules[(String)voMove.AppliedRuleIDs[iAppliedRuleIndex]];
//							oAppliedRuleToMove.IsAvoidable=true;
//								
//							//Get bigger chunk of history for this move and disolve it in rule manager
//							ArrayList oBiggerHistoryForAvoidableMove = 
//								moGame.History.GetHistoryByDepth(
//								(int) Math.Round(oAppliedRuleToMove.HistoryDepthUsed * HISTORY_DEPTH_TO_INCREASE_WHEN_RULE_FAILS), 
//								voMove.MoveIndex,true);
//							moRuleManager.DisolveHistory(oBiggerHistoryForAvoidableMove);
//						}
//					}
//					else
//					{
//						Log.Write("Rule varified for move index", voMove.MoveIndex.ToString(), true);
//						//We assume that rule deduction was correct
//						//but we don't dissolve history because we really don't know actual reaction to this move
//					}
//					voMove.IsRuleSuccessChecked=true;
//				}
//				else
//				{
//					//do not check success again
//				}
//			}
//			else
//			{
//				//Reply was selected randomly. Save this history for future purposes
//				moRuleManager.DisolveHistory(voHistorySinceLastScore.GetRange(0,voMove.MoveIndex+1));
//			}		
//		}
//		
		private void CombineSymbolsAndNegate(RuleMatch voBaseFailureRuleMatch, ArrayList vaAllFailureRules, ArrayList vaSuccessRules, int iSymbolCountToUseForRandomMessage, ref ArrayList raCombinedAndNegatedMessage, ref bool rbIsEBFOccured)
		{
			//Build the set that of the length of message content
			ArrayList aBaseFailureMessage = voBaseFailureRuleMatch.moRule.ReplyMessage;
			//Create the jagged array of combined failure message
			ArrayList aCombinedLowestMessage = new ArrayList();
			for (int iEachSymbolInFailureMessage = 0;iEachSymbolInFailureMessage < aBaseFailureMessage.Count; iEachSymbolInFailureMessage++ )
			{
				aCombinedLowestMessage.Add(new ArrayList());
			}
			
			Log.Write ("Applied failure rules: ", "", false, 4);
			
			//for each of the failure message
			for (int iFailureMessageIndex = 0; iFailureMessageIndex < vaAllFailureRules.Count ; iFailureMessageIndex++)
			{
				//Add element in this message to our combined set
				Rule oThisFailureRule = (Rule)vaAllFailureRules[iFailureMessageIndex];
				Log.Write ("", oThisFailureRule.RuleID, false, 4);
				ArrayList aThisFailureMessage = oThisFailureRule.ReplyMessage;
				for(int iFailureMessageSymbolIndex = 0; (iFailureMessageSymbolIndex < aThisFailureMessage.Count) && (iFailureMessageSymbolIndex < aBaseFailureMessage.Count); iFailureMessageSymbolIndex++)
				{
					ArrayList aThisCombinedSymbols = (ArrayList) aCombinedLowestMessage[iFailureMessageSymbolIndex];
					if (aThisCombinedSymbols.Contains((String)aThisFailureMessage[iFailureMessageSymbolIndex]) == false)
					{
						aThisCombinedSymbols.Add((String) aThisFailureMessage[iFailureMessageSymbolIndex]);
					}
					
				}
			}
			
			//Now we have sets of symbols that we want to avoid
			//Construct a message that contains least numbers of common symbols in this set
			//If we can't find such message, it's "Failure By Exhaustan" condition
			
			//for each symbol is combined message set
			bool bIsAnyNonEmptySetFound = false;
			ArrayList aReplyMessageContent = new ArrayList();
			for (int iCombinedMessageSymbolSetIndex = 0; iCombinedMessageSymbolSetIndex < aCombinedLowestMessage.Count; iCombinedMessageSymbolSetIndex++)
			{
				ArrayList aAvailableSymbols = new ArrayList(moGame.AvailableSymbols.ToArray());
				
				ArrayList aThisSymbolSet = (ArrayList) aCombinedLowestMessage[iCombinedMessageSymbolSetIndex];
				for(int iSymbolIndex = 0; iSymbolIndex < aThisSymbolSet.Count; iSymbolIndex++)
				{
					String sThisSymbol = (String) aThisSymbolSet[iSymbolIndex];
					int iIndexInAvailableSymbolSet = aAvailableSymbols.IndexOf(sThisSymbol);
					if (iIndexInAvailableSymbolSet != -1)
					{
						aAvailableSymbols.RemoveAt(iIndexInAvailableSymbolSet);
						Log.Write("Removed symbol: ", sThisSymbol, true, 1);
					}
				}
				if (aAvailableSymbols.Count != 0) 
				{	
					Log.Write("Available sybols after negation: ", CommonFunctions.ArrayToCSV(aAvailableSymbols.ToArray()), true, 1);
					bIsAnyNonEmptySetFound = true;
					int iRandomMessageToSelect = CommonFunctions.GetRandomNumber(aAvailableSymbols.Count-1);
					aReplyMessageContent.Add((String) aAvailableSymbols[iRandomMessageToSelect]);
				}
				else
				{
					aReplyMessageContent.Add(moGame.GetRandomSymbol(true, iSymbolCountToUseForRandomMessage));
				}
			}
			
			if (bIsAnyNonEmptySetFound==false)
			{
				Log.Write("EBF has occured. ", "", false, 4);
			}
			
			raCombinedAndNegatedMessage = aReplyMessageContent;
			rbIsEBFOccured = (bIsAnyNonEmptySetFound==false);
		}		
		
		public override String ToString()
		{
			return moRuleManager.ToString();
		}
	}
}