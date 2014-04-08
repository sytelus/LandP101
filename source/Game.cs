using System;
using System.Collections;

namespace LearnAndPlay
{
	/// <summary>
	/// Summary description for Game.
	/// </summary>
	public class Game
	{
		private MoveHistory moMoveHistory;
		private ArrayList moAvailableSymbols;
		private Move moCurrentMove;
		public Game()
		{
			moAvailableSymbols = new ArrayList();
		}
		
		public ArrayList AvailableSymbols
		{
			get
			{
				return moAvailableSymbols;
			}
		}

		public MoveHistory History
		{
			get
			{
				return moMoveHistory ;
			}
		}
		public void StartNewRound()
		{
			//erase history and reset other round specific class members
			moMoveHistory = new MoveHistory();
			this.StartNewMove();
		}
		private void StartNewMove()
		{
			this.StartNewMove(true, true);
		}
		private void StartNewMove(bool vbAddCurrentMoveInHistory)
		{
			this.StartNewMove(vbAddCurrentMoveInHistory, true);
		}
		private void StartNewMove(bool vbAddCurrentMoveInHistory, bool vbIgnoreCurrentMoveIfEmpty)
		{
			//If current move is available
			if (moCurrentMove != null) 
			{
				//see if it used at all
				if ((true==moCurrentMove.IsUsed ) || (false==vbIgnoreCurrentMoveIfEmpty))
				{
					if (true==vbAddCurrentMoveInHistory) 
					{
						//add this move in to history
						moMoveHistory.AddMove(moCurrentMove);
					}
					else {};//do not add this move in history 
				}
			}
			//Now start new move
			moCurrentMove = new Move();
		}
		public Move CurrentMove
		{
			get
			{
				return moCurrentMove;
			}
		}
		public ArrayList GetRandomSymbol(bool vbIsForReply, int viSymbolCountRequired)
		{
			ArrayList aSymbolsToReturn = new ArrayList();
			for(int iSymbolIndex = 1;iSymbolIndex <= viSymbolCountRequired; iSymbolIndex++)
			{
				int iRandomSymbolIndex = CommonFunctions.GetRandomNumber(moAvailableSymbols.Count-1);
				aSymbolsToReturn.Add((String)moAvailableSymbols[iRandomSymbolIndex]);
			}
			return aSymbolsToReturn;
		}

		public Move RecordScore(double vdScore)
		{
			Move oModifiedMove = null;
			
			if ((null != moCurrentMove.OnMessage) && (null == moCurrentMove.ReplyMessage))
			{
				//This move has no reply but only question. Ignore this move
				Log.Write ("Error: ", "Score is received before reply was asked. This move will be ignored", true, 1000);
				oModifiedMove = null;
				StartNewMove(false, true);
			}
			//else if current move only had reply
			else if ((null == moCurrentMove.OnMessage) && (null != moCurrentMove.ReplyMessage) && double.IsNaN(moCurrentMove.ActualScore))
			{
				moCurrentMove.ActualScore = vdScore;
				oModifiedMove = moCurrentMove;
				StartNewMove(true, true);
			}
			//else if current move had reply + score
			else if	 ((null != moCurrentMove.ReplyMessage) && (double.IsNaN(moCurrentMove.ActualScore)==false))
			{
				Log.Write ("Error: ", "Score is received before quetion or reply. This score will be ignored", true, 1000);
				oModifiedMove = null;
				StartNewMove(true, true);
			}
			//else if current move had ques + reply + no score
			else if	 ((null != moCurrentMove.OnMessage) && (null != moCurrentMove.ReplyMessage) && (double.IsNaN(moCurrentMove.ActualScore)==true))
			{
				moCurrentMove.ActualScore = vdScore;
				oModifiedMove = moCurrentMove;
				StartNewMove(true, true);
			}
			//else if current move has only score
			else if	 ((null == moCurrentMove.OnMessage) && (null == moCurrentMove.ReplyMessage) && (double.IsNaN(moCurrentMove.ActualScore)==false))
			{
				Log.Write ("Error: ", "Score is received again before quetion or reply. This score will be ignored", true, 1000);
				oModifiedMove = null;
				StartNewMove(false, true);
			}
			//else if current move is empty
			else if	 ((null == moCurrentMove.OnMessage) && (null == moCurrentMove.ReplyMessage) && (double.IsNaN(moCurrentMove.ActualScore)==true))
			{
				//record in to prev move else ignore
				oModifiedMove = moMoveHistory.RecordScore(moMoveHistory.Moves.Count-1, vdScore);
			}
			return oModifiedMove;
		}
		
		public void RecordMessage(Message voMessage)
		{
			if (voMessage.Source == Message.PLAYER_JUDGE)
			{
				//If cuurent move is already scored
				if (double.IsNaN(moCurrentMove.ActualScore)==false)
				{
					//If current move doesn't have reply
					if (null == moCurrentMove.ReplyMessage)
					{
						Log.Write("Error: ", " Current move had score but no reply. New move will be started.", true, 1000);
						//Drop current move
						StartNewMove(false, true);
						moCurrentMove.AddMessage(voMessage);
					}
					else
					{
						StartNewMove(true, true);
						moCurrentMove.AddMessage(voMessage);
					}
				}
				else
				{
					//Current move is not scored
					
					if (null == moCurrentMove.ReplyMessage )
					{
						if (null == moCurrentMove.OnMessage)
						{
							moCurrentMove.AddMessage(voMessage);
						}
						else
						{
							moCurrentMove.OnMessage.Content.AddRange(voMessage.Content);
						}
					}
					else
					{
						if (null == moCurrentMove.OnMessage)
						{
							moCurrentMove.AddMessage(voMessage);
						}
						else
						{
							//start new move
							StartNewMove(true, true);
							moCurrentMove.AddMessage(voMessage);
						}
					}
				}
			}
			else if (voMessage.Source == Message.PLAYER_THIS )
			{
				//If this move is not empty
				if ((null != moCurrentMove.OnMessage) || (null != moCurrentMove.ReplyMessage) || (double.IsNaN(moCurrentMove.ActualScore)==false))
				{
					//If current move is not scored
					if (double.IsNaN(moCurrentMove.ActualScore)==true)
					{
						//Record in to current
						if (null == moCurrentMove.ReplyMessage)
						{
							moCurrentMove.AddMessage(voMessage);
						}
						else
						{
							moCurrentMove.ReplyMessage.Content.AddRange(voMessage.Content);
						}
					}
					else
					{
						//If current move is scored but has no reply
						if (null == moCurrentMove.ReplyMessage)
						{	
							Log.Write("Error: ", "Current move is cored but has no reply. It will be ignored", true, 1000);
							StartNewMove(false, true);
							moCurrentMove.AddMessage(voMessage);
						}
						else
						{
							StartNewMove(true, true);
							moCurrentMove.AddMessage(voMessage);
						}
					}
				}
				else
				{
					//Try to record in to previous move
					Move oUpdatedLastMove = moMoveHistory.RecordReply(moMoveHistory.Moves.Count -1 ,voMessage);
					
					if (null == oUpdatedLastMove)
					{
						//else record to current
						moCurrentMove.AddMessage(voMessage);
					} else {} // it's recorded in to last move
				}
			}
			else
			{
				Log.Write("Error: ", "Message Source is not judge or player. This message will be ignored", true, 1000);							
			}
		}
		
	}
}
