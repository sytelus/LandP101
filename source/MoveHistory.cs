using System;
using System.Collections ;

namespace LearnAndPlay
{
	/// <summary>
	/// Summary description for MoveHistory.
	/// </summary>
	public class MoveHistory
	{
		private ArrayList moMoves;
		private Move moMoveWithLastScore;
		
		public MoveHistory()
		{
			moMoves=new ArrayList();
			moMoveWithLastScore = null;
		}
		
		public MoveHistory(ArrayList voMoves, Move voMoveWithLastScore)
		{
			moMoves=voMoves;
			moMoveWithLastScore = voMoveWithLastScore;
		}
		
		public ArrayList Moves
		{
			get
			{
				return moMoves;
			}
		}
		public void AddMove(Move voMove)
		{
			voMove.MoveIndex = moMoves.Count;
			moMoves.Add(voMove);
			if (double.IsNaN(voMove.ActualScore)==false)
			{
				moMoveWithLastScore = voMove;
			}
		}
		public void AddMoveHistory(MoveHistory voMoveHistoryToAdd)
		{
			//Update move indexes
			int iNewMoveIndex = moMoves.Count;
			foreach(Move oNewMoveToAdd in voMoveHistoryToAdd.Moves)
			{
				oNewMoveToAdd.MoveIndex = iNewMoveIndex;
				iNewMoveIndex++;
			}	
			
			//Add moves in to our array list
			moMoves.AddRange(voMoveHistoryToAdd.Moves);
			
			//Update the move with last score
			if (voMoveHistoryToAdd.MoveWithLastScore != null)
			{
				moMoveWithLastScore = voMoveHistoryToAdd.MoveWithLastScore;
			}
		}
		
		
		public ArrayList GetHistoryByDepth(int viDepth, bool vbIncludeLastMove)
		{
			return GetHistoryByDepth(viDepth, -1, vbIncludeLastMove);	
		}
		public ArrayList GetHistoryByDepth(int viDepth, int viForMoveIndex, bool vbIncludeLastMove)
		{
			int iStartIndex;
			int iLength;
			
			if (viForMoveIndex != -1)
			{
				iStartIndex = viForMoveIndex-viDepth;
			}
			else
			{
				//Assume start move is the last one
				iStartIndex = (moMoves.Count-1) - viDepth ;
			};
				
			iLength = viDepth;
			
			if (vbIncludeLastMove == true)
			{
				iLength++;
			}
			else {};	//Length is ok
			
			return GetMovesInRange(iStartIndex, iLength);
		}
		
		//Automatically adjusts boundries if wrong boundries are specified
		public ArrayList GetMovesInRange(int viStartIndex, int viLength)
		{
			int iStartIndex = viStartIndex;
			int iLength = viLength;

			//If StartIndex is going in to -ve, make it zero
			if (iStartIndex < 0) 
			{
				//Decrease the length accordingly
				iLength = iLength + iStartIndex; // iStartIndex is -ve
				//Reset the start index to start of array
				iStartIndex = 0;
			}

			//If iLength is going beyond array length, srunk it to array length
			if ((iStartIndex + iLength) > moMoves.Count) iLength = moMoves.Count - iStartIndex;
			
			ArrayList oMovesToReturn = new ArrayList();	//By default, say that no history is available
			
			//if start is not beyond count
			if (iStartIndex+1 <= moMoves.Count)
			{
				oMovesToReturn = moMoves.GetRange(iStartIndex, iLength);
			}
			else {}; //return default emty set of moves

			return oMovesToReturn;
		}

		public Move MoveWithLastScore
		{
			get
			{
//				Move oMoveToCheck = null;
//				
//				//Find the move which was assigned score
//				for(int iMoveIndex=moMoves.Count-1;iMoveIndex >= 0;iMoveIndex--)
//				{
//					oMoveToCheck = (Move)(moMoves[iMoveIndex]);
//					if (oMoveToCheck.ActualScore != double.NaN )
//					{
//						//Score is assigned
//						break;
//					}
//				}
				return moMoveWithLastScore;
			}
			
		}
		
		public Move RecordScore(int viMoveIndex, double vdScore)
		{
			Move oMoveToUpdate = null;
			if ((viMoveIndex < moMoves.Count) && (viMoveIndex >=0))
			{
				oMoveToUpdate =(Move) moMoves[viMoveIndex];
				
				//If this move is not scored
				if (double.IsNaN(oMoveToUpdate.ActualScore)==true)
				{
					oMoveToUpdate.ActualScore = vdScore;
					if (moMoveWithLastScore != null)
					{
						if (moMoveWithLastScore.MoveIndex < oMoveToUpdate.MoveIndex)
						{
							moMoveWithLastScore = oMoveToUpdate;
						}
						else {}; //Keep the current move with last score
					}
					else
					{
						moMoveWithLastScore = oMoveToUpdate;
					}
				}
				else
				{
					//Ignore this score
					Log.Write ("Error: ", "Previous move already scored", true, 1000);
					oMoveToUpdate = null;
				}
			}
			else
			{
				Log.Write ("Error: ", "No previous move to record score into", true, 1000);
				oMoveToUpdate = null;
			}
			
			return oMoveToUpdate;
		}
		
		public Move RecordReply(int viMoveIndex, Message voMessage)
		{ 
			Move oMoveToUpdate = null;
			if ((viMoveIndex < moMoves.Count) && (viMoveIndex >=0))
			{
				oMoveToUpdate =(Move) moMoves[viMoveIndex];
				
				//If this move is not scored
				if (double.IsNaN(oMoveToUpdate.ActualScore)==true)
				{
					oMoveToUpdate.AddMessage(voMessage);
				}
				else
				{
					//Can not use this move
					oMoveToUpdate = null;
				} 
			}
			else
			{
				oMoveToUpdate = null;
			}
			
			return oMoveToUpdate;
		}


		public ArrayList GetHistoryFromLastScore(int viUntilMoveIndex, bool vbIncludeLastMove)
		{
			Move oMoveWithLastScore = this.MoveWithLastScore;
			
			ArrayList oMovesToReturn = new ArrayList();

			int iStartIndex;

			//if we found any moves with assigned score
			if (oMoveWithLastScore != null)
			{
				//Start from next move which was scored
				iStartIndex = oMoveWithLastScore.MoveIndex + 1;
			}
			else
			{
				//no moves found having score. This means we were given score for the first time.
				//Return all moves
				iStartIndex = 0;
			} 

			int iLength;
			if (viUntilMoveIndex == -1) //Get all moves until end
			{
				iLength = moMoves.Count-iStartIndex;
			}
			else 
			{
				iLength=viUntilMoveIndex-iStartIndex;
			}

			if (vbIncludeLastMove==false) iLength--;

			oMovesToReturn=GetMovesInRange(iStartIndex, iLength);

			return oMovesToReturn;
		}
		public ArrayList GetHistoryFromLastScore(bool vbIncludeLastMove)
		{
			return GetHistoryFromLastScore(-1, vbIncludeLastMove);
		}
		public double LastScore
		{
			get
			{
				return this.MoveWithLastScore.ActualScore;
			}
		}
		public Move LastMove
		{
			get
			{
				if (moMoves.Count >= 1)
				{
					return (Move)moMoves[moMoves.Count-1];
				}
				else return null;
			}
		}
		
		public override String ToString()
		{
			String sReturn = String.Empty;
			for(int iMoveIndex = 0; iMoveIndex < moMoves.Count; iMoveIndex++)
			{
				Move oThisMove = (Move) moMoves[iMoveIndex];
				sReturn += oThisMove.ToString();
			}
		
			return "{" + sReturn + "}";
		}
		
	}
}
