using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace LearnAndPlay
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form, ILoggable
	{
		private static bool mbIsConsolApp = true;
		
		private Player moPlayer;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.TextBox txtSymbolsToSend;
		private System.Windows.Forms.TextBox txtReplyReceived;
		private System.Windows.Forms.TextBox txtAvailableSymbols;
		private System.Windows.Forms.TextBox txtScoreToSend;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.Button button6;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.RichTextBox txtLog;
		private System.Windows.Forms.RichTextBox txtRules;
		private System.Windows.Forms.ListView lsvMoveHistory;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.Button button7;
		private System.Windows.Forms.Button button8;
		private System.Windows.Forms.Button button9;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		
		void ILoggable.Write(String vsContent)
		{
			txtLog.Focus();
			txtLog.AppendText(vsContent);
			txtLog.SelectionStart = txtLog.TextLength;
			txtLog.SelectionLength = 0;
			txtLog.ScrollToCaret();
		}
		
		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lsvMoveHistory = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.txtRules = new System.Windows.Forms.RichTextBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.txtLog = new System.Windows.Forms.RichTextBox();
			this.button7 = new System.Windows.Forms.Button();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.txtSymbolsToSend = new System.Windows.Forms.TextBox();
			this.button8 = new System.Windows.Forms.Button();
			this.button9 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.button6 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.txtAvailableSymbols = new System.Windows.Forms.TextBox();
			this.txtReplyReceived = new System.Windows.Forms.TextBox();
			this.txtScoreToSend = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.SuspendLayout();
			// 
			// lsvMoveHistory
			// 
			this.lsvMoveHistory.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lsvMoveHistory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																							 this.columnHeader1});
			this.lsvMoveHistory.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lsvMoveHistory.FullRowSelect = true;
			this.lsvMoveHistory.GridLines = true;
			this.lsvMoveHistory.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lsvMoveHistory.HideSelection = false;
			this.lsvMoveHistory.Location = new System.Drawing.Point(512, 32);
			this.lsvMoveHistory.MultiSelect = false;
			this.lsvMoveHistory.Name = "lsvMoveHistory";
			this.lsvMoveHistory.Size = new System.Drawing.Size(176, 320);
			this.lsvMoveHistory.TabIndex = 6;
			this.lsvMoveHistory.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "columnHeader1";
			this.columnHeader1.Width = 200;
			// 
			// txtRules
			// 
			this.txtRules.Location = new System.Drawing.Point(2, 2);
			this.txtRules.Name = "txtRules";
			this.txtRules.Size = new System.Drawing.Size(470, 142);
			this.txtRules.TabIndex = 0;
			this.txtRules.Text = "";
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					  this.tabPage1,
																					  this.tabPage2});
			this.tabControl1.Location = new System.Drawing.Point(16, 168);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(480, 184);
			this.tabControl1.TabIndex = 8;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				   this.txtLog,
																				   this.button7});
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(472, 158);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Log";
			// 
			// txtLog
			// 
			this.txtLog.AcceptsTab = true;
			this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtLog.Location = new System.Drawing.Point(2, 0);
			this.txtLog.Name = "txtLog";
			this.txtLog.Size = new System.Drawing.Size(470, 160);
			this.txtLog.TabIndex = 0;
			this.txtLog.Text = "";
			// 
			// button7
			// 
			this.button7.Location = new System.Drawing.Point(48, -103);
			this.button7.Name = "button7";
			this.button7.Size = new System.Drawing.Size(112, 26);
			this.button7.TabIndex = 5;
			this.button7.Text = "Auto Game";
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.AddRange(new System.Windows.Forms.Control[] {
																				   this.txtRules});
			this.tabPage2.Location = new System.Drawing.Point(4, 4);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(472, 176);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Current Rules";
			// 
			// txtSymbolsToSend
			// 
			this.txtSymbolsToSend.Location = new System.Drawing.Point(16, 48);
			this.txtSymbolsToSend.Name = "txtSymbolsToSend";
			this.txtSymbolsToSend.Size = new System.Drawing.Size(280, 20);
			this.txtSymbolsToSend.TabIndex = 0;
			this.txtSymbolsToSend.Text = "";
			// 
			// button8
			// 
			this.button8.Location = new System.Drawing.Point(8, 360);
			this.button8.Name = "button8";
			this.button8.TabIndex = 9;
			this.button8.Text = "Auto Game";
			this.button8.Click += new System.EventHandler(this.button7_Click);
			// 
			// button9
			// 
			this.button9.Location = new System.Drawing.Point(96, 360);
			this.button9.Name = "button9";
			this.button9.TabIndex = 10;
			this.button9.Text = "All New";
			this.button9.Click += new System.EventHandler(this.button9_Click);
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(16, 132);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(184, 24);
			this.button4.TabIndex = 1;
			this.button4.Text = "Request Reply";
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(344, 360);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(160, 24);
			this.button5.TabIndex = 3;
			this.button5.Text = "Print Matching Rules";
			this.button5.Click += new System.EventHandler(this.button5_Click);
			// 
			// button6
			// 
			this.button6.Location = new System.Drawing.Point(192, 360);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(136, 24);
			this.button6.TabIndex = 4;
			this.button6.Text = "Start New Game";
			this.button6.Click += new System.EventHandler(this.button6_Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(304, 8);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(184, 24);
			this.button1.TabIndex = 1;
			this.button1.Text = "Register Symbols";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(304, 48);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(184, 24);
			this.button2.TabIndex = 1;
			this.button2.Text = "Send Symbols";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(304, 88);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(184, 24);
			this.button3.TabIndex = 1;
			this.button3.Text = "Send Score";
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// txtAvailableSymbols
			// 
			this.txtAvailableSymbols.Location = new System.Drawing.Point(8, 8);
			this.txtAvailableSymbols.Name = "txtAvailableSymbols";
			this.txtAvailableSymbols.Size = new System.Drawing.Size(280, 20);
			this.txtAvailableSymbols.TabIndex = 0;
			this.txtAvailableSymbols.Text = "a,b,c";
			// 
			// txtReplyReceived
			// 
			this.txtReplyReceived.Location = new System.Drawing.Point(208, 132);
			this.txtReplyReceived.Name = "txtReplyReceived";
			this.txtReplyReceived.Size = new System.Drawing.Size(280, 20);
			this.txtReplyReceived.TabIndex = 0;
			this.txtReplyReceived.Text = "";
			// 
			// txtScoreToSend
			// 
			this.txtScoreToSend.Location = new System.Drawing.Point(16, 88);
			this.txtScoreToSend.Name = "txtScoreToSend";
			this.txtScoreToSend.Size = new System.Drawing.Size(280, 20);
			this.txtScoreToSend.TabIndex = 0;
			this.txtScoreToSend.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(513, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(128, 16);
			this.label1.TabIndex = 7;
			this.label1.Text = "Move History:";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(696, 389);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.button9,
																		  this.button8,
																		  this.tabControl1,
																		  this.label1,
																		  this.lsvMoveHistory,
																		  this.button6,
																		  this.button5,
																		  this.button4,
																		  this.txtReplyReceived,
																		  this.button3,
																		  this.txtScoreToSend,
																		  this.txtSymbolsToSend,
																		  this.button2,
																		  this.button1,
																		  this.txtAvailableSymbols});
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.Closed += new System.EventHandler(this.Form1_Closed);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) 
		{
		
			mbIsConsolApp = (args.Length == 0);
		
			if (mbIsConsolApp == false)
			{
				Application.Run(new Form1());
			}	
			else
			{
				Player oPlayer = new Player();
//				oPlayer.RegisterGameSymbols(new ArrayList(new String[]{"a", "b", "c"}));
//				oPlayer.StartNewGame();
				//This is consol APP
				ConsolSupport oConsolSupport = new ConsolSupport();
				Log.OutputDevice = null;
				
				oConsolSupport.SendMyName();
				
				bool bIsExitRequested = false;
				
				Message oLastGetMessage = null;
				int iNextIndexInGetMessage = -1;
				bool bIsLastGetMessageStillUsable = false;
				
				do
				{
					try
					{
						String sChannel = String.Empty;
						String sCommandType = String.Empty;
						String sCommandContent = String.Empty;
						
						oConsolSupport.ReadMessage(ref sChannel, ref sCommandType, ref sCommandContent);
						
						bool bIsGetMessageExecuted = false;
						
						switch (sChannel)
						{
							case ConsolSupport.msCHANNEL_OUTPUT: 
								//Not concerned with other outputs
								break;
							case ConsolSupport.msCHANNEL_COMMAND: 
							{
								switch (sCommandType.ToLower())
								{
									case ConsolSupport.msCHANNEL_COMMAND_TYPE_EXIT:
										bIsExitRequested = true;
										break;
									case ConsolSupport.msCHANNEL_COMMAND_TYPE_GET_MESSAGE:
										bool bIsLastGetMessageToUse = false;
										if (bIsLastGetMessageStillUsable == true)
										{
											if (iNextIndexInGetMessage >= oLastGetMessage.Content.Count)
												bIsLastGetMessageToUse = false; 
											else bIsLastGetMessageToUse = true;
										}
										if (bIsLastGetMessageToUse == false)
										{
											oLastGetMessage = oPlayer.GetMessage();
											iNextIndexInGetMessage = 0;										
										}
										
										oConsolSupport.SendMessage(ConsolSupport.msCHANNEL_OUTPUT, (String) oLastGetMessage.Content[iNextIndexInGetMessage], String.Empty);
										iNextIndexInGetMessage++;
										bIsGetMessageExecuted = true;
										break;
									case ConsolSupport.msCHANNEL_COMMAND_TYPE_NEW_GAME:
										oPlayer = new Player();
										oPlayer.StartNewGame();
										break;
									case ConsolSupport.msCHANNEL_COMMAND_TYPE_REGISTER_SYMBOL:
										oPlayer.RegisterGameSymbols(new ArrayList(new String[] {sCommandContent}));
										break;
									default:
										oConsolSupport.SendComment("Don't know about command type " + sCommandType);
										break;
								}
							}
							break;
							case ConsolSupport.msCHANNEL_INPUT: 
								oPlayer.PutMessage(new ArrayList(new String[]{sCommandType}));
								break;
							case ConsolSupport.msCHANNEL_SCORE: 
								oPlayer.PutScore(double.Parse(sCommandType));
								break;
							case ConsolSupport.msCHANNEL_INFO: 
								//not concerted with info message received
								break;						
							default:
								oConsolSupport.SendComment("Don't know about channel " + sChannel);
								break;
						}
						
						bIsLastGetMessageStillUsable = bIsGetMessageExecuted;
					}
					catch (Exception e)
					{
						oConsolSupport.SendComment("Error occured in program: " + e.ToString());
					}
				}
				while(bIsExitRequested == false);
				
				oConsolSupport.SendMessage(ConsolSupport.msCHANNEL_INFO, ConsolSupport.msCHANNEL_INFO_TYPE_EXIT, String.Empty); 
				
				Log.OutputDevice=null;
				oPlayer = null;
			}
		}
		
		private void InitPlayer(ref Player roPlayerToSetup, bool vbConnectToDebugDevices, bool vbRegisterDefaultSymbols, bool vbStartNewGame)
		{
			roPlayerToSetup = new Player();
			
			if (vbConnectToDebugDevices==true)
			{
				roPlayerToSetup.RulesChanged += new EventHandler(Game_UpdateRules);
				roPlayerToSetup.MoveHistoryChanged += new EventHandler(Game_UpdateMoveHistory);
			}
			
			if (vbRegisterDefaultSymbols) roPlayerToSetup.RegisterGameSymbols(new ArrayList(new String[]{"a","b","c"}));
			if (vbStartNewGame) roPlayerToSetup.StartNewGame();
		
		}
		
		private void Form1_Load(object sender, System.EventArgs e)
		{
			Log.OutputDevice=this;
			InitPlayer(ref moPlayer, true, true, true);
//			moPlayer.PutMessage(new ArrayList(new String[]{"a"}));
//			moPlayer.GetMessage();
//			moPlayer.PutScore(1);
//			moPlayer.PutMessage(new ArrayList(new String[]{"a"}));	
//			moPlayer.GetMessage();					
//			moPlayer.PutScore(1);
			//todo: what if score given before reply?
		}

		private void Form1_Closed(object sender, System.EventArgs e)
		{
			moPlayer = null;
			Log.OutputDevice=null;
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			String sAvailableSymbols = txtAvailableSymbols.Text;
			char[] aSeperator = ",".ToCharArray();
			moPlayer.RegisterGameSymbols(new ArrayList(sAvailableSymbols.Split(aSeperator)));
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			String sSymbolsToSend = txtSymbolsToSend.Text;
			char[] aSeperator = ",".ToCharArray();
			moPlayer.PutMessage((new ArrayList(sSymbolsToSend.Split(aSeperator))));
		}

		private void button3_Click(object sender, System.EventArgs e)
		{
			moPlayer.PutScore(double.Parse(txtScoreToSend.Text));
		}

		private void button4_Click(object sender, System.EventArgs e)
		{
			Message oReply = moPlayer.GetMessage();
			Object[]  aReplySymbols = oReply.Content.ToArray();
			
			txtReplyReceived.Text = CommonFunctions.ArrayToCSV(aReplySymbols);
		}

		private void button5_Click(object sender, System.EventArgs e)
		{
			ArrayList aMatchingRules = moPlayer.GetMatchingRules();
			Log.Write("Matching rules ", aMatchingRules.Count.ToString(), true, 1000);
			for(int iMatchingRuleIndex=0;iMatchingRuleIndex < aMatchingRules.Count;iMatchingRuleIndex++)
			{
				RuleMatch oThisRuleMatch = (RuleMatch) aMatchingRules[iMatchingRuleIndex];
				Log.Write(oThisRuleMatch.moRule.RuleID, oThisRuleMatch.mdMatchLevel.ToString(), true, 1000);
			}
		}

		private void button6_Click(object sender, System.EventArgs e)
		{
			moPlayer.StartNewGame();
		}

		private void button7_Click(object sender, System.EventArgs e)
		{
			int iMoveCount = 50;
			Player oPlayer = null;
			InitPlayer(ref oPlayer,(iMoveCount <= 50), true, true);
			String sLastReply = "";
			int iLastfailure = 0;
			int iFailureCount =0;
			int iSuccessCount =0;
			if (iMoveCount > 50) Log.OutputDevice = null;
			for(int i=1;i < iMoveCount;i++)
			{
				int iRandom = CommonFunctions.GetRandomNumber(2);
				ArrayList aQue = oPlayer.Game.AvailableSymbols.GetRange(iRandom,1);
				String sQue = (String) aQue[0];
				oPlayer.PutMessage(aQue);
				String sReply = (String) oPlayer.GetMessage().Content[0];	//oAS.GetRange(CommonFunctions.GetRandomNumber(2),1)[0];
				
				if ((sReply==sQue) || ((sReply == sLastReply) && (sLastReply != "")))
				//if ((sReply != sLastReply) && (sLastReply != ""))
				{	
					oPlayer.PutScore(-1); 
					iLastfailure = i; iFailureCount++;
				}
				else
				{	
					oPlayer.PutScore(1); 
					iSuccessCount++;
				};
				sLastReply = sReply;
			}
			Log.OutputDevice = this;
			Log.Write("Rules\n", oPlayer.ToString(), true, 1000);
			Log.Write("Last failed on " + iLastfailure.ToString() + " fc=" + iFailureCount.ToString() + " sc=" + iSuccessCount.ToString(), "", true, 1000);
		}
		
		private void Game_UpdateRules(object sender, System.EventArgs e)
		{
			Player oThisPlayer = (Player) sender;
			txtRules.Text = oThisPlayer.ToString();
		}
		private void Game_UpdateMoveHistory(object sender, System.EventArgs e)
		{
			Player oThisPlayer = (Player) sender;
			lsvMoveHistory.Items.Clear();
			AddMovesToListView(oThisPlayer.Game.History.Moves, 0);
			return;
			if (lsvMoveHistory.Items.Count != oThisPlayer.Game.History.Moves.Count)
			{
				if (lsvMoveHistory.Items.Count < oThisPlayer.Game.History.Moves.Count)
				{
					//Just write extra entry
					AddMovesToListView(oThisPlayer.Game.History.Moves, lsvMoveHistory.Items.Count);
				}
				else if  (lsvMoveHistory.Items.Count == oThisPlayer.Game.History.Moves.Count)
				{
					//no action
				} else
				{
					lsvMoveHistory.Items.Clear();
					AddMovesToListView(oThisPlayer.Game.History.Moves, 0);
				}
			}
			else {}; //no need to update
		}		
		private void AddMovesToListView(ArrayList vaMoves, int viStartIndex)
		{
			for(int iMoveIndex = viStartIndex;iMoveIndex < vaMoves.Count; iMoveIndex++)
			{
				Move oThisMove =(Move) vaMoves[iMoveIndex];
				int iListItemToUpdate  = lsvMoveHistory.Items.Add(String.Empty).Index;
				UpdateListItemWithMoveInfo(iListItemToUpdate, oThisMove);
			}
		}
		private void UpdateListItemWithMoveInfo(int viListItemIndex, Move voMove)
		{
			String sSourceMessage = "";
			if (voMove.OnMessage != null)	sSourceMessage = voMove.OnMessage.ToString();
			String sMyMessage = "";
			if (voMove.ReplyMessage != null) sMyMessage = voMove.ReplyMessage.ToString();
			lsvMoveHistory.Items[viListItemIndex].Text = voMove.MoveIndex.ToString() + ".  " + sSourceMessage + "    " + sMyMessage  + "   -  " + voMove.ActualScore.ToString() + "   ..." +  CommonFunctions.ArrayToCSV(voMove.AppliedRuleIDs.ToArray());
		}

		private void button9_Click(object sender, System.EventArgs e)
		{
			InitPlayer(ref moPlayer, true, true, true);
		}
	}
}
