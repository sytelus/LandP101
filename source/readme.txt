What' This:
-----------
This is the program to play any game without knowing rules. It will learn from the scores passed on in several games and try to guess the rules and win.
The game is represented by set of messages. The program is passed on some messages, asked for response and then given the score depending on
how good response was. The details of this can be found at http://www.a-i.com which has organized the Learning Machine Challange. This program complies with
their interface specification for automated game play and is submitted in the competition as one of the entries.


Who Did it:
-----------
This is Shital Shah, 26 living in Fairfield, NJ, USA. I work as Software Engineer in one of top US business firm. I do not have MS in Computer Science and neither any acadamic background in the field of AI but to 
a person looking for the challanges, AI happens to be the greatest, most significant and important challange possed ever. You can email me at shital_s@usa.net. Visit my web site at http://www.ShitalShah.com.


How To Run:
-----------
To run the binaries, your machine must have Windows operating system (preferably Windows 2000) and Microsoft .Net Framework runtimes. This can be downloaded free of charge from http://www.microsoft.com or ordered on CD/DVD for a charge of $10.
Goto bin\Debug folder and run LearnAndPlay.exe. This exe is standalone i.e. doesn't requires any other support files like database (except Microsoft .Net runtime libraries).

To see GUI instead of consol, run
LearnAndPlay GUI


Running from SourceCode:
------------------------
This program is entirly written in C# language using Microsoft Visual Studio.Net Beta2 IDE. You can recompile
and build program these tools.


How It Works:
-------------
Please see the valgo.doc for theoraticle discussion. It's still in very primitive draft form however.

This program compares move histories and makes a guess for rules involved. To make a guess for rules it finds common factors in the history and weights them on few different bases. As scores keeps comming, it gets to compare more and more moves to find what was common thing between them which lead to perticular score. Then it uses these common factors to see how well current move history matches with them and apply the best match. As you might have already though, this process is not perfact. For example, while deriving common factors from history you might miss out part of it. To compensate for this errors, the program keeps track of what rules it guessed were failed. The rules whcih were failed are marked as "avoidable". The algorithm is little more complex to explain it in whole here but I hope you got the idea.


Known Problems:
---------------
1. Software bugs: 
	The program doesn't very well handle multi message inputs. For example, if a game like Tic-Tac-Toe consist of passing 2 symbols and then requesting 2 then program might crash. But it does handle 1 symbol messages well.
	The program might gather lots of failure rules. It doesn't have a way to dispose these rules to save memory and resources.


2. Logical bugs: 
	It might not survive the mistakes that judge might make. For example when judge is supposed to give score 1 but it gives 0 by rare mistake, program might have hard time to figure out correct rules - unlike humans.
	It's rigidly created for my-turn-then-your-turn - kind of sequence. I would instead like to have a program that can scan a message stream which might have some junk as well as say multiple symboles for input and output.
	The part of program that scans rules it guessed so far to extrapolate other possible rules in not yet made.
	Program might suffer from bad performance in case of get-killed-and-win kind of moves. For example, in chess, you might want to give up something in turn of possible bigger gain. In such situation, this program will eventually figure this out but it will take lots of time before it does so.
	I wanted to have a kind of feedback loop in the program so that even if there are some coding - algorithmic mistakes, the program still servives and eventually learns - but now with longer time. This feedback loop doesn't seems to be fully operational.


Version History:
----------------

30-Oct-2001	1.0.668.39387	First version submitted to the Challange.
09-Nov-2001	1.0.1.61348	Second submission to Challange. Fixed critical algorithmic bugs which improved winning chance by 4 times. Still other bugs are detected but not yet fixed. UI and Logs are much enhanced to debug. 
