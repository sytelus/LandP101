using System;
using System.Collections;

namespace LearnAndPlay
{
	/// <summary>
	/// Summary description for Move.
	/// </summary>
	public class Move
	{
		private ArrayList moHistory;
		private double mdActualScore;
		private double mdEstimatedScore;
		private int miMoveIndex;
		private bool mbIsUsed;
		private bool mbIsRuleSuccessChecked;
		private ArrayList maAppliedRuleIDs;
		private Message moOnMessage;
		private Message moReplyMessage;
		private bool mbIsAnalyzed;
		private double mdAvgMatchLevelOfAppliedRules;
		private double mdAvghistoryDepthOfAppliedRules;

		//Constructors
		public Move()
		{
			moHistory=new ArrayList();
			InitializePrivateMembers();
		}
		
		//Set defaults in to private members
		private void InitializePrivateMembers()
		{
			mbIsUsed=false;
			miMoveIndex=-1;
			mdEstimatedScore=double.NaN;
			mdActualScore=double.NaN;
			mbIsRuleSuccessChecked = false;
			maAppliedRuleIDs = new ArrayList();
			mbIsAnalyzed = false;
			mdAvgMatchLevelOfAppliedRules = double.NaN;
		}
		public ArrayList History
		{
			get
			{
				return moHistory;
			}
		}

		public void AddMessage(Message voMessage)
		{
			AddMessage(voMessage, null);
		}
		public void AddMessage(Message voMessage, String Source)
		{
			if (null != Source)
			{
				voMessage.Source=Source;
			}
			else {} //assume that Source is already set in to voMessage
			moHistory.Add(voMessage);
			
			if (voMessage.Source==Message.PLAYER_THIS)
			{
				moReplyMessage=voMessage;
			}
			else
			{
				moOnMessage=voMessage;
			}
			
			mbIsUsed = true;
		}

		public double EstimatedScore
		{
			get
			{
				return mdEstimatedScore;
			}
			set
			{
				mdEstimatedScore=value;
			}
		}
		
		public int MoveIndex
		{
			get
			{
				return miMoveIndex;
			}
			set
			{
				miMoveIndex=value;
			}
		}
		public double ActualScore
		{
			get
			{
				return mdActualScore;
			}
			set
			{
				mdActualScore=value;
				mbIsUsed=true;
			}
		}
		public bool IsRuleSuccessChecked
		{
			get
			{
				return mbIsRuleSuccessChecked;
			}
			set
			{
				mbIsRuleSuccessChecked=value;
			}
		}
		public ArrayList AppliedRuleIDs
		{
			get
			{
				return maAppliedRuleIDs;
			}
			set
			{
				maAppliedRuleIDs = value;
			}
		}
		public bool IsUsed
		{
			get
			{
				return mbIsUsed;
			}
		}
		
		public Message OnMessage
		{
			get
			{
				return moOnMessage;
			}
		}
		
		public Message ReplyMessage
		{
			get
			{
				return moReplyMessage;
			}
		}

		public bool IsSimilar(Move voMoveToCompare, double vdMoveSimilarityThresold, double vdMessageSimilarityThresold)
		{
			ArrayList MessageList1;
			ArrayList MessageList2;
			
			if (moHistory.Count > voMoveToCompare.History.Count)
			{
				MessageList1 = voMoveToCompare.History;
				MessageList2 = moHistory;
			}
			else
			{
				MessageList1 = moHistory;
				MessageList2 = voMoveToCompare.History;			}
		
			int iTotalMessages = MessageList1.Count ;
			int iSimilarMessages = 0;
			
			foreach(Message oMessage1 in MessageList1)
			{
				foreach(Message oMessage2 in MessageList2)
				{
					if (oMessage1.IsSimilar(oMessage2, vdMessageSimilarityThresold)==true)
					{
						iSimilarMessages++;
						break;
					}
				}
			}
			
			bool bIsSimilar;
			if (iTotalMessages != 0)
			{
				bIsSimilar = ((iSimilarMessages/iTotalMessages) >= vdMoveSimilarityThresold);
			}
			else bIsSimilar=false;
			
			return bIsSimilar;
		}
		
		public override String ToString()
		{
			String sReturn = String.Empty;
			for(int iMessageIndex = 0; iMessageIndex < moHistory.Count; iMessageIndex++)
			{
				Message oThisMessage = (Message) moHistory[iMessageIndex];
				sReturn += oThisMessage.ToString();				
				if (iMessageIndex < moHistory.Count-1)
				{
					sReturn += ";";
				}
			}
			
			return sReturn;
		}
		
		public bool IsAnalyzed
		{
			get
			{
				return mbIsAnalyzed;
			}
			set
			{
				mbIsAnalyzed = value;	
			}
		}
		public double AvgMatchLevelOfAppliedRules
		{
			get
			{
				return mdAvgMatchLevelOfAppliedRules;
			}
			set
			{
				mdAvgMatchLevelOfAppliedRules = value;	
			}
		}
		public double AvgHistoryDepthOfAppliedRules
		{
			get
			{
				return mdAvghistoryDepthOfAppliedRules;
			}
			set
			{
				mdAvghistoryDepthOfAppliedRules = value;	
			}
		}
	}
}
