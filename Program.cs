using System;
using System.Linq;

namespace textChess
{
    class Program
    {
        static void Main(string[] args)
        {
            chessGame game = new chessGame();
            game.runChessGameSequence();           
        }
    }        

    class Piece
    {
        bool isPieceWhite = true;
        public Piece(bool isWhitePiece)
        {
            this.isPieceWhite = isWhitePiece;
        }
        public bool isWhitePiece()
        {
            return isPieceWhite;
        }
        public virtual Piece copyPiece()
        {
            return null;
        }
        public virtual bool isMoveLegal(Move moveToMake, Piece[,] testBoard, bool displayMassages)
        {
            if (isPieceToMoveNull(moveToMake, testBoard, displayMassages))
                return false;
            if (isPieceToMoveWrongColor(moveToMake, testBoard, displayMassages))
                return false;
            if (isMoveToSameLocation(moveToMake, displayMassages))
                return false;
            if (doesDestinationContainFriendlyPiece(moveToMake, testBoard, displayMassages))
                return false;
            return true;
        }
        protected bool doesDestinationContainFriendlyPiece(Move moveToMake, Piece[,] testBoard, bool displayMassages)
        {
            if (testBoard[moveToMake.to.row, moveToMake.to.column] != null &&
                testBoard[moveToMake.to.row, moveToMake.to.column].isPieceWhite == moveToMake.isWhitesTurn)
            {
                if (displayMassages)
                    textChessGraphics.displayText("Destination square is occupied");
                return true;
            }
            return false;
        }
        protected bool doesDestinationContainEnemyPiece(Move moveToMake, Piece[,] testBoard, bool displayMassages)
        {
            if (testBoard[moveToMake.to.row, moveToMake.to.column]!=null &&
                testBoard[moveToMake.to.row, moveToMake.to.column].isPieceWhite != moveToMake.isWhitesTurn)
                return true;
            if (displayMassages)
                textChessGraphics.displayText("No enemy at destination");
            return false;
        }
        bool isMoveToSameLocation(Move moveToMake, bool displayMassages)
        {
            if(moveToMake.from==moveToMake.to)
            {
                if (displayMassages)
                    textChessGraphics.displayText("You selected the same square twice");
                return true;
            }
            return false;
        }
        bool isPieceToMoveWrongColor(Move moveToMake, Piece[,] testBoard, bool displayMassages)
        {
            if (getPieceToMove(moveToMake, testBoard).isPieceWhite != moveToMake.isWhitesTurn)
            {
                if (displayMassages)
                    textChessGraphics.displayText("You selected a " + (!moveToMake.isWhitesTurn ? "white" : "black") + " piece");
                return true;
            }
            return false;
        }
        bool isPieceToMoveNull(Move moveToMake, Piece[,] testBoard, bool displayMassages)
        {
            if (getPieceToMove(moveToMake, testBoard) == null)
            {
                if (displayMassages)
                    textChessGraphics.displayText("You selected an empty square");
                return true;
            }
            return false;
        }
        Piece getPieceToMove(Move moveToMake, Piece[,] testBoard)
        {
            return testBoard[moveToMake.from.row, moveToMake.from.column];
        }
        public virtual bool isLegalMovePossible(Piece[,] testBoard, pos pieceLocation)
        {
            return false;
        }        
        protected bool isEmptyOrContainingEnPassantTempRef(pos positionToTest, Piece[,] testBoard, bool displayMassages)
        {
            if (testBoard[positionToTest.row, positionToTest.column] == null)
                return true;
            if ((positionToTest.column == 2 || positionToTest.column == 5) &&
                (testBoard[positionToTest.row, positionToTest.column] is P && ((P)testBoard[positionToTest.row, positionToTest.column]).isEnPassant()))
                return true;
            if (displayMassages)
                textChessGraphics.displayText("Path is blocked");
            return false;
        }
        protected bool doesMoveEndInCheck(Move move, Piece[,] testBoard)
        {
            return new chessGame(move.isWhitesTurn, testBoard).doesMoveEndInCheck(move);
        }
    }
    class K : Piece
    {
        bool moved = false;
        public K(bool whitePiece)
            : base(whitePiece) { }
        public bool hasMoved()
        {
            return moved;
        }
        public void setMoved(bool moved)
        {
            this.moved = moved;
        } 
        public override Piece copyPiece()
        {
            K result = new K(this.isWhitePiece());
            result.moved = this.moved;
            return result;
        }
        public override string ToString()
        {
            return ((isWhitePiece() ? "W" : "B") + "|K");
        }
        public override bool isLegalMovePossible(Piece[,] testBoard, pos pieceLocation)
        {
            Move testMove;
            for (int row = 0; row < testBoard.GetLength(0); row++)
                for (int column = 0; column < testBoard.GetLength(1); column++)
                {
                    if ((Math.Abs(pieceLocation.row - row) < 2 && Math.Abs(pieceLocation.column - column) < 2) ||
                        ((Math.Abs(pieceLocation.row - row) == 0 && Math.Abs(pieceLocation.column - column) == 2)))
                    {
                        testMove = new Move(pieceLocation, new pos(row, column), isWhitePiece());
                        if (isMoveLegal(testMove, testBoard, false))
                            if (!doesMoveEndInCheck(testMove, testBoard))
                            {
                                Console.WriteLine(testMove.from.row + "" + testMove.from.column + "" + testMove.to.row + "" + testMove.to.column + "" + testMove.isWhitesTurn);
                                return true;
                            }
                    }
                }
            return false;
        }
        public override bool isMoveLegal(Move moveToMake, Piece[,] testBoard, bool displayMassages)
        {
            if (base.isMoveLegal(moveToMake, testBoard, displayMassages))
                if (Math.Abs(moveToMake.from.row - moveToMake.to.row) < 2 && Math.Abs(moveToMake.from.column - moveToMake.to.column) < 2)
                    return true;
                else if (Math.Abs(moveToMake.from.row - moveToMake.to.row) == 0 && Math.Abs(moveToMake.from.column - moveToMake.to.column) == 2)
                    if (new chessGame(moveToMake.isWhitesTurn, testBoard).isLegalCastling(moveToMake))
                        return true;
            if (displayMassages)
                textChessGraphics.displayText("Illegal king move");
            return false;
        }
    }
    class Q : Piece
    {
        public Q(bool whitePiece)
            : base(whitePiece) { }
        public override Piece copyPiece()
        {
            return new Q(this.isWhitePiece());
        }
        public override string ToString()
        {
            return ((isWhitePiece() ? "W" : "B") + "|Q");
        }
        public override bool isMoveLegal(Move moveToMake, Piece[,] testBoard, bool displayMassages)
        {
            if (base.isMoveLegal(moveToMake, testBoard, displayMassages))
            {
                B queenAsBishop = new B(isWhitePiece());
                R queenAsRook = new R(isWhitePiece());
                if (queenAsBishop.isMoveLegal(moveToMake, testBoard, displayMassages) ||
                    queenAsRook.isMoveLegal(moveToMake, testBoard, displayMassages))
                    return true;
            }
            if (displayMassages)
                textChessGraphics.displayText("Illegal queen move");
            return false;
        }
        public override bool isLegalMovePossible(Piece[,] testBoard, pos pieceLocation)
        {
            B queenAsBishop = new B(isWhitePiece());
            R queenAsRook = new R(isWhitePiece());
            if (queenAsBishop.isLegalMovePossible(testBoard, pieceLocation) || 
                queenAsRook.isLegalMovePossible(testBoard, pieceLocation))
                return true;
            return false;
        }
    }
    class B : Piece
    {
        public B(bool whitePiece)
            : base(whitePiece) { }
        public override Piece copyPiece()
        {
            return new B(this.isWhitePiece());
        }
        public override string ToString()
        {
            return ((isWhitePiece() ? "W" : "B") + "|B");
        }
        public override bool isMoveLegal(Move moveToMake, Piece[,] testBoard, bool displayMassages)
        {
            if (base.isMoveLegal(moveToMake, testBoard, displayMassages))
                if (isValidDiagonalMove(moveToMake, displayMassages))
                    if (isPathClearForMove(moveToMake, testBoard, displayMassages))
                        return true;
            if (displayMassages)
                textChessGraphics.displayText("Illegal bishop move");
            return false;
        }
        bool isValidDiagonalMove(Move moveToMake, bool displayMassages)
        {
            if(Math.Abs(moveToMake.from.row-moveToMake.to.row)!=Math.Abs(moveToMake.from.column-moveToMake.to.column))
            {
                if (displayMassages)
                    textChessGraphics.displayText("Illegal diagonal move");
                return false;
            }
            return true;
        }
        bool isPathClearForMove(Move moveToMake, Piece[,] testBoard, bool displayMassages)
        {
            int rowModifier = moveToMake.from.row < moveToMake.to.row ? 1 : -1;
            int columnModifier = moveToMake.from.column < moveToMake.to.column ? 1 : -1;
            int lengthOfPathToCheck = Math.Abs(moveToMake.from.row - moveToMake.to.row);
            int row = moveToMake.from.row;
            int column = moveToMake.from.column;
            for (int i=1;i<lengthOfPathToCheck;i++)
            {
                row += rowModifier;
                column += columnModifier;
                if (!isEmptyOrContainingEnPassantTempRef(new pos(row, column), testBoard, displayMassages))
                {
                    if (displayMassages)
                        textChessGraphics.displayText("Path is blocked");
                    return false;
                }
            }
            return true;
        }
        public override bool isLegalMovePossible(Piece[,] testBoard, pos pieceLocation)
        {
            Move testMove;
            for(int row=0;row<testBoard.GetLength(0);row++)
                for(int column=0;column<testBoard.GetLength(1);column++)
                {
                    if (Math.Abs(pieceLocation.row - row) == Math.Abs(pieceLocation.column - column))
                    {
                        testMove = new Move(pieceLocation, new pos(row, column), isWhitePiece());
                        if (isMoveLegal(testMove, testBoard, false))
                            if (!doesMoveEndInCheck(testMove, testBoard))
                            {
                                Console.WriteLine(testMove.from.row + "" + testMove.from.column + "" + testMove.to.row + "" + testMove.to.column + "" + testMove.isWhitesTurn);
                                return true;
                            }
                    }
                }
            return false;
        }
    }
    class N : Piece
    {
        public N(bool whitePiece)
            : base(whitePiece) { }
        public override Piece copyPiece()
        {
            return new N(this.isWhitePiece());
        }
        public override string ToString()
        {
            return ((isWhitePiece() ? "W" : "B") + "|N");
        }
        public override bool isMoveLegal(Move moveToMake, Piece[,] testBoard, bool displayMassages)
        {
            if (base.isMoveLegal(moveToMake, testBoard, displayMassages))
            {
                int moveLengthInXaxis = Math.Abs(moveToMake.from.row - moveToMake.to.row);
                int moveLengthInYaxis = Math.Abs(moveToMake.from.column - moveToMake.to.column);
                if (moveLengthInXaxis == 2 && moveLengthInYaxis == 1 || moveLengthInXaxis == 1 && moveLengthInYaxis == 2)
                    return true;
            }
            if (displayMassages)
                textChessGraphics.displayText("Illegal move for a knight");
            return false;
        }
        public override bool isLegalMovePossible(Piece[,] testBoard, pos pieceLocation)
        {
            Move testMove;
            for (int row = 0; row < testBoard.GetLength(0); row++)
                for (int column = 0; column < testBoard.GetLength(1); column++)
                {
                    if (Math.Abs(pieceLocation.row - row) + Math.Abs(pieceLocation.column - column) == 3)
                    {
                        testMove = new Move(pieceLocation, new pos(row, column), isWhitePiece());
                        if (isMoveLegal(testMove, testBoard, false))
                            if (!doesMoveEndInCheck(testMove, testBoard))
                            {
                                Console.WriteLine(testMove.from.row + "" + testMove.from.column + "" + testMove.to.row + "" + testMove.to.column + "" + testMove.isWhitesTurn);
                                return true;
                            }
                    }
                }
            return false;
        }
    }
    class R : Piece
    {
        bool moved = false;
        public R(bool whitePiece)
            : base(whitePiece) { }
        public bool hasMoved()
        {
            return moved;
        }
        public void setMoved(bool moved)
        {
            this.moved = moved;
        }
        public override Piece copyPiece()
        {
            R result = new R(this.isWhitePiece());
            result.moved = this.moved;
            return result;
        }
        public override string ToString()
        {
            return ((isWhitePiece() ? "W" : "B") + "|R");
        }
        public override bool isMoveLegal(Move moveToMake, Piece[,] testBoard, bool displayMassages)
        {
            if (base.isMoveLegal(moveToMake, testBoard, displayMassages))
                if (moveToMake.from.row == moveToMake.to.row || moveToMake.from.column == moveToMake.to.column)
                    if (isPathClearForMove(moveToMake, testBoard, displayMassages))
                        return true;
            if (displayMassages)
                textChessGraphics.displayText("Illegal rook move");
            return false;
        }
        bool isPathClearForMove(Move moveToMake, Piece[,] testBoard, bool displayMassages)
        {
            if (Math.Abs(moveToMake.from.row - moveToMake.to.row) > 1 || Math.Abs(moveToMake.from.column - moveToMake.to.column) > 1)
            {
                bool isMoveOnRowAxis = moveToMake.from.column == moveToMake.to.column;
                int lengthOfPathToCheck = isMoveOnRowAxis ? Math.Abs(moveToMake.from.row - moveToMake.to.row) : Math.Abs(moveToMake.from.column - moveToMake.to.column);
                int rowModifier = isMoveOnRowAxis ? (moveToMake.from.row < moveToMake.to.row ? 1 : -1) : 0;
                int columnModifier = isMoveOnRowAxis ? 0 : (moveToMake.from.column < moveToMake.to.column ? 1 : -1);
                int row = moveToMake.from.row;
                int column = moveToMake.from.column;
                for(int i=1;i<lengthOfPathToCheck;i++)
                {
                    row += rowModifier;
                    column += columnModifier;
                    if (!isEmptyOrContainingEnPassantTempRef(new pos(row, column), testBoard, displayMassages))
                    {
                        if (displayMassages)
                            textChessGraphics.displayText("Path is blocked");
                        return false;
                    }                    
                }
            }
            return true;
        }
        public override bool isLegalMovePossible(Piece[,] testBoard, pos pieceLocation)
        {
            Move testMove;
            for (int row = 0; row < testBoard.GetLength(0); row++)
                for (int column = 0; column < testBoard.GetLength(1); column++)
                {
                    if (Math.Abs(pieceLocation.row - row) == 0 || Math.Abs(pieceLocation.column - column) == 0)
                    {
                        testMove = new Move(pieceLocation, new pos(row, column), isWhitePiece());
                        if (isMoveLegal(testMove, testBoard, false))
                            if (!doesMoveEndInCheck(testMove, testBoard))
                            {
                                Console.WriteLine(testMove.from.row + "" + testMove.from.column + "" + testMove.to.row + "" + testMove.to.column + "" + testMove.isWhitesTurn);
                                return true;
                            }
                    }
                }
            return false;
        }
    }
    class P : Piece
    {
        bool moved = false;
        bool enPassant;
        public P(bool whitePiece)
            : base(whitePiece)
        {
            setEnPassant(false);
        }
        public bool hasMoved()
        {
            return moved;
        }
        public void setMoved(bool moved)
        {
            this.moved = moved;
        }
        public bool isEnPassant()
        {
            return enPassant;
        }
        public void setEnPassant(bool enPassant)
        {
            this.enPassant = enPassant;
        }
        public override Piece copyPiece()
        {
            P result = new P(this.isWhitePiece());
            result.enPassant = this.enPassant;
            return result;
        }
        public override string ToString()
        {
            return ((isWhitePiece() ? "W" : "B") + "|P");
        }
        public override bool isMoveLegal(Move moveToMake, Piece[,] testBoard, bool displayMassages)
        {
            if (base.isMoveLegal(moveToMake, testBoard, displayMassages))
                if (isMoveInRightDirection(moveToMake, displayMassages))
                {
                    if (moveToMake.from.column != moveToMake.to.column)
                    {
                        if (isValidDiagonalMove(moveToMake, testBoard, displayMassages))
                            return true;
                    }
                    else if(isValidStraightMove(moveToMake, testBoard, displayMassages))
                            return true;
                }
            if (displayMassages)
                textChessGraphics.displayText("Illegal pawn move");
            return false;
        }
        bool isValidStraightMove(Move moveToMake, Piece[,] testBoard, bool displayMassages)
        {
            if (Math.Abs(moveToMake.from.row - moveToMake.to.row) == 1)
                if (testBoard[moveToMake.to.row, moveToMake.to.column] == null)
                    return true;
            if (Math.Abs(moveToMake.from.row - moveToMake.to.row) == 2)
                if (!((P)testBoard[moveToMake.from.row, moveToMake.from.column]).moved)
                {
                    int modifier = moveToMake.from.row > moveToMake.to.row ? -1 : 1;
                    if (testBoard[moveToMake.from.row + modifier, moveToMake.from.column] == null)
                        if (testBoard[moveToMake.to.row, moveToMake.to.column] == null)
                            return true;
                }
            return false;
        }
        bool isValidDiagonalMove(Move moveToMake, Piece[,] testBoard, bool displayMassages)
        {
            if (Math.Abs(moveToMake.from.row - moveToMake.to.row) == 1 &&
                Math.Abs(moveToMake.from.column - moveToMake.to.column) == 1)
                if (doesDestinationContainEnemyPiece(moveToMake, testBoard, displayMassages))
                    return true;
            return false;
        }
        bool isMoveInRightDirection(Move moveToMake,  bool displayMassages)
        {
            if (moveToMake.isWhitesTurn ? 
                moveToMake.from.row > moveToMake.to.row : moveToMake.from.row < moveToMake.to.row)
                return true;
            if (displayMassages)
                textChessGraphics.displayText("Move in wrong direction");
            return false;
        }
        public override bool isLegalMovePossible(Piece[,] testBoard, pos pieceLocation)
        {
            Move testMove;
            int pawnDirectionModifier = isWhitePiece() ? -1 : 1;
            for (int row = 0; row < testBoard.GetLength(0); row++)
                for (int column = 0; column < testBoard.GetLength(1); column++)
                {
                    if ((pieceLocation.row + pawnDirectionModifier == row && Math.Abs(pieceLocation.column - column) == 1) ||
                        (pieceLocation.row + (2 * pawnDirectionModifier) == row && Math.Abs(pieceLocation.column - column) == 0))
                    {
                        testMove = new Move(pieceLocation, new pos(row, column), isWhitePiece());
                        if (isMoveLegal(testMove, testBoard, false))
                            if (!doesMoveEndInCheck(testMove, testBoard))
                            {
                                Console.WriteLine(testMove.from.row + "" + testMove.from.column + "" + testMove.to.row + "" + testMove.to.column + "" + testMove.isWhitesTurn);
                                return true;
                            }
                    }
                }
            return false;
        }
    }

    class chessGame
    {
        Player whitePlayer;
        Player blackPlayer;
        bool isWhitesTurn;
        int turn;
        int turnsWithNoAttacOrPawnkMoveInARow = 0;
        Piece[,] chessBoard = new Piece[8, 8];
        string[] boardHistory = new string[50];
        int boardHistoryCounter = 0;
        public chessGame(bool isWhitesTurn, Piece[,] chessBoard)
        {
            this.isWhitesTurn = isWhitesTurn;
            this.chessBoard = chessBoard;
        }
        public chessGame()
        {
            textChessGraphics.displaySplashScreen();
            whitePlayer = new Player(true);
            whitePlayer = new Player(false);
            isWhitesTurn = true;
            chessBoard = populateNewChessBoard();
        }
        public void runChessGameSequence()
        {
            Move move = null;
            textChessGraphics.PrintBoard(this.ToString());
            textChessGraphics.displayChessNotation();
            while (true)
            {
                resetEnPassantPawns();
                this.setCurrentPlayerIsInCheck(this.isKingInCheck());
                if (!this.isLegalMovePossible())
                {
                    if (this.isCurrentPlayerInCheck())
                        textChessGraphics.displayChackMate(this);
                    else
                        textChessGraphics.displayStaleMate(this);
                    textChessGraphics.displayExit();
                    return;
                }                
                    move = getValidMoveFromPlayer();
                if (move == null)
                {
                    textChessGraphics.displayExit();
                    return;
                }
                this.addBoardToHistory();
                this.makeVerifiedMove(move);
                this.turn++;
                this.isWhitesTurn = !this.isWhitesTurn;
                textChessGraphics.PrintBoard(this.ToString());
            }

        }
        Move getValidMoveFromPlayer()
        {
            Move result;
            while (true)
            {
                result = chessUtils.getMoveFromPlayerOrNullForExit(this.getPlayer());
                if (result == null)
                    return result;
                Piece pieceToMove = this.getPieceToMove(result);
                if (pieceToMove != null && pieceToMove.isMoveLegal(result, chessBoard, true))
                    if (this.doesMoveEndInCheck(result))
                        textChessGraphics.displayText("Move will end in check");
                    else
                        return result;
            }
        }
        Piece[,] populateNewChessBoard()
        {
            Piece[,] result = {//8*8                
                { new R(false), new N(false), new B(false), new Q(false), new K(false), new B(false), new N(false), new R(false)}, //row 1
                {new P(false), new P(false), new P(false), new P(false),  new P(false), new P(false), new P(false), new P(false)}, //row 2
                { null, null, null, null,null, null, null,null },                                                                   //row 3
                { null, null, null, null,null, null, null,null },                                                                   //row 4
                { null, null, null, null,null, null, null,null },                                                                   //row 5
                { null, null, null, null,null, null, null,null },                                                                   //row 6
                { new P(true), new P(true), new P(true), new P(true), new P(true), new P(true), new P(true), new P(true)},          //row 7
                { new R(true), new N(true), new B(true), new Q(true), new K(true), new B(true), new N(true), new R(true)},};       //row 8
            return result;
        }
        public override string ToString()
        {
            string boxesToString = "";
            for (int row = 0; row < chessBoard.GetLength(0); row++)
                for (int column = 0; column < chessBoard.GetLength(1); column++)
                {
                    if (chessBoard[row, column] == null)
                        boxesToString += "   " + "_";
                    else if ((row == 2 || row == 5) && chessBoard[row, column] is P && ((P)chessBoard[row, column]).isEnPassant())
                        boxesToString += "   " + "_";
                    else
                        boxesToString += chessBoard[row, column].ToString() + "_";
                }
            return boxesToString;
        }
        public void setTurnsWithNoAttacOrPawnkMoveInARow(bool didThisTurnIncludeAttacOrPawnkMove)
        {
            if (didThisTurnIncludeAttacOrPawnkMove)
                resetTurnsWithNoAttacOrPawnkMoveInARow();
            else
                turnsWithNoAttacOrPawnkMoveInARow++;
        }
        public void resetTurnsWithNoAttacOrPawnkMoveInARow()
        {
            turnsWithNoAttacOrPawnkMoveInARow = 0;
        }
        public Player getPlayer(bool isWhite)
        {
            if (isWhite)
                return whitePlayer;
            else
                return blackPlayer;
        }
        public Player getPlayer()
        {
            return getPlayer(isWhitesTurn);
        }
        public bool IsWhitesTurn()
        {
            return isWhitesTurn;
        }
        bool isMoveEnPassantKill(Move m)
        {
            Piece pieceToMove = chessBoard[m.from.row, m.from.column];
            Piece destinationPiece = chessBoard[m.to.row, m.to.column];
            if (!(pieceToMove is P))
                return false;
            if (destinationPiece is P && ((P)destinationPiece).isEnPassant()
                && (m.to.row == 5 || m.to.row == 2))
                return true;
            else return false;
        }
        public bool isMoveLegalKill(Move m)
        {
            Piece pieceToMove = chessBoard[m.from.row, m.from.column];
            Piece destinationPiece = chessBoard[m.to.row, m.to.column];
            if (isMoveEnPassantKill(m))
                return true;
            if (destinationPiece != null && destinationPiece.isWhitePiece() != pieceToMove.isWhitePiece())
                return true;
            return false;
        }
        public bool isMoveKillOrPawnMove(Move m)
        {
            if (isMoveLegalKill(m))
                return true;
            if (chessBoard[m.from.row, m.from.column] is P)
                return true;
            return false;
        }
        Piece getAppropriateRookForCastling(Move m)
        {
            switch (m.to.column)
            {
                case 2:
                    return chessBoard[m.isWhitesTurn ? chessBoard.GetLength(0) - 1 : 0, 0];
                case 6:
                    return chessBoard[m.isWhitesTurn ? chessBoard.GetLength(0) - 1 : 0, chessBoard.GetLength(1) - 1];
                default:
                    return null;
            }
        }
        bool isRoadClearForCastling(Move m)
        {
            K king = (K)chessBoard[m.from.row, m.from.column];
            if (isKingInCheck(king.isWhitePiece(), chessBoard))
                return false;
            Piece[,] testBoard;
            bool castlingToTheLeft = m.from.column > m.to.column;
            int modifier = castlingToTheLeft ? -1 : 1;
            int column = m.from.column;
            for (int i = 0; i < 2; i++)
            {
                column += modifier;
                if (chessBoard[m.from.row, column] != null)
                    return false;
                testBoard = this.copyBoard();
                makeTestMove(new Move(m.from, new pos(m.to.row, column), king.isWhitePiece()), testBoard);
                if (isKingInCheck(king.isWhitePiece(), testBoard))
                    return false;
            }
            return true;
        }
        public bool isLegalCastling(Move m)
        {
            Piece king = chessBoard[m.from.row, m.from.column];
            if (!(king is K && !((K)king).hasMoved()))
                return false;//checks king never moved
            Piece rook = getAppropriateRookForCastling(m);
            if (!(rook != null && rook is R && !((R)rook).hasMoved()))
                return false;//checks rook never moved
            if (!isRoadClearForCastling(m))
                return false;
            return true;
        }
        public bool isKingInCheck(bool isWhiteKing, Piece[,] testBoard)
        {
            Piece testPiece;
            Move testMove;
            for (int row = 0; row < testBoard.GetLength(0); row++)
                for (int column = 0; column < testBoard.GetLength(1); column++)
                {
                    testPiece = testBoard[row, column];
                    if (testPiece != null && testPiece.isWhitePiece() != isWhiteKing)
                    {
                        testMove = new Move(new pos(row, column), getKingsPositionForPlayer(isWhiteKing, testBoard), testPiece.isWhitePiece());
                        if (testPiece.isMoveLegal(testMove, testBoard, false))
                            return true;
                    }
                }
            return false;
        }
        public bool isKingInCheck()
        {
            return isKingInCheck(isWhitesTurn, chessBoard);
        }
        pos getKingsPositionForPlayer(bool isWhitePlayer, Piece[,] testBoard)
        {
            for (int row = 0; row < chessBoard.GetLength(0); row++)
                for (int column = 0; column < chessBoard.GetLength(1); column++)
                {
                    if (testBoard[row, column] is K && testBoard[row, column].isWhitePiece() == isWhitePlayer)
                        return new pos(row, column);
                }
            return null;
        }
        public Piece[,] copyBoard()
        {
            Piece[,] result = new Piece[8, 8];
            for (int row = 0; row < result.GetLength(0); row++)
                for (int column = 0; column < result.GetLength(1); column++)
                {
                    if (chessBoard[row, column] == null)
                        result[row, column] = null;
                    else
                        result[row, column] = chessBoard[row, column].copyPiece();
                }
            return result;
        }
        void makeTestMove(Move m, Piece[,] testBoard)
        {
            testBoard[m.to.row, m.to.column] = testBoard[m.from.row, m.from.column];
            testBoard[m.from.row, m.from.column] = null;
        }
        void makeVerifiedMove(Move m)
        {
            Piece pieceToMove = chessBoard[m.from.row, m.from.column];
            setMovedIfAvailableForPiece(pieceToMove);
            if (verifiedMoveIsCastling(m))
                moveRookForVerifiedCastlingMove(m);
            else if (verifideMoveIsPawn2Step(m))
                addEnPassantTempRef(m);
            if (isMoveEnPassantKill(m))
                killPawnInEnPassant(m);
            setTurnsWithNoAttacOrPawnkMoveInARow(isMoveKillOrPawnMove(m));
            if (isMovePawnPromotion(m))
                promotePawn(m);
            chessBoard[m.to.row, m.to.column] = chessBoard[m.from.row, m.from.column];
            chessBoard[m.from.row, m.from.column] = null;
        }
        void setMovedIfAvailableForPiece(Piece p)
        {
            if (p is K)
                ((K)p).setMoved(true);
            if (p is R)
                ((R)p).setMoved(true);
            if (p is P)
                ((P)p).setMoved(true);
        }
        bool verifiedMoveIsCastling(Move m)
        {
            return (chessBoard[m.from.row, m.from.column] is K && Math.Abs(m.from.column - m.to.column) == 2);
        }
        void moveRookForVerifiedCastlingMove(Move m)
        {
            bool castlingToTheLeft = m.from.column > m.to.column;
            R rook = (R)chessBoard[m.from.row, castlingToTheLeft ? 0 : chessBoard.GetLength(1) - 1];
            chessBoard[m.from.row, castlingToTheLeft ? 0 : chessBoard.GetLength(1) - 1] = null;
            chessBoard[m.from.row, castlingToTheLeft ? 3 : 5] = rook;
            rook.setMoved(true);
        }
        bool verifideMoveIsPawn2Step(Move m)
        {
            return (chessBoard[m.from.row, m.from.column] is P && Math.Abs(m.from.row - m.to.row) == 2);
        }
        void addEnPassantTempRef(Move m)
        {
            P pawn = (P)chessBoard[m.from.row, m.from.column];
            chessBoard[isWhitesTurn ? 5 : 2, m.to.column] = pawn;
            pawn.setEnPassant(true);
        }
        void killPawnInEnPassant(Move m)
        {
            chessBoard[(isWhitesTurn ? 1 : -1) + m.to.row, m.to.column] = null;
        }
        public bool is50MoveRule()
        {
            if (turnsWithNoAttacOrPawnkMoveInARow >= 50)
            {
                textChessGraphics.displayText("50 moves with no attack or pawn move");
                return true;
            }
            else return false;
        }
        public bool isLegalMovePossible()
        {
            if (is50MoveRule())
                return false;
            if (isThreefoldRepetition())
                return false;
            if (isNotEnoughMaterial())
                return false;
            for (int row = 0; row < chessBoard.GetLength(0); row++)
                for (int column = 0; column < chessBoard.GetLength(1); column++)
                {
                    if (chessBoard[row, column] != null &&
                        chessBoard[row, column].isWhitePiece() == isWhitesTurn)
                        if (chessBoard[row, column].isLegalMovePossible(chessBoard, new pos(row, column)))
                            return true;
                }
            return false;
        }
        public bool isNotEnoughMaterial()
        {
            string boardString = this.ToString().Replace(" ", "").Replace("_", "");
            if (boardString.Length < 12)
            {
                if (boardString.Contains("|B") || boardString.Contains("|N"))
                {
                    textChessGraphics.displayText("There is not enough material for a win");
                    return true;
                }
                else if (boardString.Length == 8)
                {
                    textChessGraphics.displayText("There is not enough material for a win");
                    return true;
                }

            }
            return false;
        }
        public void resetEnPassantPawns()
        {
            int rowToCheck = isWhitesTurn ? 5 : 2;
            Piece testPiece;
            for (int column = 0; column < chessBoard.GetLength(1); column++)
            {
                testPiece = chessBoard[rowToCheck, column];
                if (testPiece is P && ((P)testPiece).isEnPassant())
                {
                    ((P)testPiece).setEnPassant(false);
                    chessBoard[rowToCheck, column] = null;
                }
            }
        }
        public void promotePawn(Move m)
        {
            P pawnToPromote = (P)chessBoard[m.from.row, m.from.column];
            string input = chessUtils.requestPieceTypeForPromotion(getPlayer());
            Piece result = null;
            switch (input)
            {
                case "QUEEN":
                    result = new Q(pawnToPromote.isWhitePiece());
                    break;
                case "BISHOP":
                    result = new B(pawnToPromote.isWhitePiece());
                    break;
                case "KNIGHT":
                    result = new N(pawnToPromote.isWhitePiece());
                    break;
                case "ROOK":
                    result = new R(pawnToPromote.isWhitePiece());
                    ((R)result).setMoved(true);
                    break;
            }
            chessBoard[m.from.row, m.from.column] = result;
        }
        public bool isMovePawnPromotion(Move m)
        {
            return (chessBoard[m.from.row, m.from.column] is P && m.to.row == (m.isWhitesTurn ? 0 : 7));
        }
        public bool isThreefoldRepetition()
        {

            int count = 0;
            for (int i = 0; i < turn; i++)
            {
                if (this.ToString() + makeMovedPiecesTextReferenceForBoardToString() == getBoardHistoryForTurn(i))
                    count++;
                if (count > 1)
                {
                    textChessGraphics.displayText("threefold repetition");
                    return true;
                }
            }
            return false;
        }
        void addBoardToHistory()
        {
            if (boardHistoryCounter > 49)
                boardHistoryCounter = 0;
            boardHistory[boardHistoryCounter] = ToString();
            boardHistory[boardHistoryCounter] += makeMovedPiecesTextReferenceForBoardToString();
            boardHistoryCounter++;
        }
        string makeMovedPiecesTextReferenceForBoardToString()
        {
            string result = (chessBoard[0, 4] is K && !((K)chessBoard[0, 4]).hasMoved() ? "1" : "0") + (chessBoard[7, 4] is K && !((K)chessBoard[0, 4]).hasMoved() ? "1" : "0");
            for (int row = 0; row < chessBoard.GetLength(0); row += 7)
                for (int column = 0; column < chessBoard.GetLength(1); column += 7)
                    result += (chessBoard[row, column] is R && !((R)chessBoard[row, column]).hasMoved()) ? "1" : "0";
            result += isWhitesTurn;
            return result;
        }
        string getBoardHistoryForTurn(int turnNumber)
        {//board history's max length is 50
            return boardHistory[turnNumber % 50];
        }
        public int getCurrentTurn()
        {
            return turn;
        }
        public void setCurrentPlayerIsInCheck(bool isInCheck)
        {
            getPlayer().isInCheck = isInCheck;
        }
        public bool isCurrentPlayerInCheck()
        {
            return getPlayer().isInCheck;
        }
        public Piece getPieceToMove(Move m)
        {
            return chessBoard[m.from.row, m.from.column];
        }
        public bool doesMoveEndInCheck(Move m)
        {
            Piece[,] testBoard = copyBoard();
            makeTestMove(m, testBoard);
            return isKingInCheck(m.isWhitesTurn, testBoard);
        }
    }

    class chessUtils
    {
        public static string requestPieceTypeForPromotion(Player currentPlayer)
        {
            string legalTypes = "|QUEEN|BISHOP|KNIGHT|ROOK";
            string input;
            textChessGraphics.displayText("Congratulations! your pawn has been promoted");
            while (true)
            {
                textChessGraphics.displayText("Please enter the type of piece you would like to promote your pawn to");
                input = Console.ReadLine();
                input.Trim();
                input = input.ToUpper();
                if (legalTypes.Contains("|" + input + "|"))
                    return input;
            }
        }
        public static Move getMoveFromPlayerOrNullForExit(Player currentPlayer)
        {
            string input;
            while (true)
            {
                textChessGraphics.displayText(currentPlayer.name + " please enter your next move and press ENTER");
                textChessGraphics.displayText("Or enter END and hit ENTER to exit");
                input = Console.ReadLine();
                input.Trim();
                input = input.ToUpper();
                if (input == "END")
                    return null;//exit the game
                else if (isValidMoveInput(input))
                {
                    Move result = translatePlayersInputToMove(input, currentPlayer.isWhitePlayer);
                    return result;
                }
                else
                {
                    textChessGraphics.displayText("Incorrect input");
                    textChessGraphics.displayChessNotation();
                }
            }
        }
        public static bool isValidMoveInput(string input)
        {
            string legalChars = "aAbBcCdDeEfFgGhH";
            string legalNums = "12345678";
            if (input.Length == 4 &&
                    legalChars.Contains(input[0]) && legalChars.Contains(input[2]) &&
                    legalNums.Contains(input[1]) && legalNums.Contains(input[3]))
                return true;
            return false;
        }
        public static Move translatePlayersInputToMove(string input, bool isWhitePlayer)
        {
            int[] movment = new int[4];
            for (int i = 0; i < input.Length; i++)
            {
                switch (input[i])
                {
                    case 'A': movment[i] = 0; break;
                    case 'B': movment[i] = 1; break;
                    case 'C': movment[i] = 2; break;
                    case 'D': movment[i] = 3; break;
                    case 'E': movment[i] = 4; break;
                    case 'F': movment[i] = 5; break;
                    case 'G': movment[i] = 6; break;
                    case 'H': movment[i] = 7; break;
                    case '1': movment[i] = 7; break;
                    case '2': movment[i] = 6; break;
                    case '3': movment[i] = 5; break;
                    case '4': movment[i] = 4; break;
                    case '5': movment[i] = 3; break;
                    case '6': movment[i] = 2; break;
                    case '7': movment[i] = 1; break;
                    case '8': movment[i] = 0; break;
                }
            }
            return new Move(new pos(movment[1], movment[0]), new pos(movment[3], movment[2]), isWhitePlayer);
        }
    }
    class textChessGraphics
    {
        public static void displaySplashScreen()
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("||###################################################||");
            Console.WriteLine("||                                                   ||");
            Console.WriteLine("||                   TEXT    CHESS                   ||");
            Console.WriteLine("||                                                   ||");
            Console.WriteLine("||###################################################||");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine();
            Console.WriteLine("Hello and welcome to text chess");
        }
        public static void displayChessNotation()
        {
            Console.WriteLine("First enter the letter and number for the piece you want to move");
            Console.WriteLine("Than enter the letter and number for your piece's destination");
            Console.WriteLine("A correct chess move looks like this: c2c3");
        }
        public static void displayStaleMate(chessGame lastBoard)
        {
            Console.WriteLine("stalemate");
            Console.WriteLine("Game lasted " + (lastBoard.getCurrentTurn() + 1) + " turns and ended on " + lastBoard.getPlayer(lastBoard.IsWhitesTurn()).name + "'s turn");
        }
        public static void displayChackMate(chessGame lastBoard)
        {
            Console.WriteLine("checkmate");
            Console.WriteLine("Game lasted " + (lastBoard.getCurrentTurn() + 1) + " turns and the winner is " + lastBoard.getPlayer(!lastBoard.IsWhitesTurn()).name);
        }
        public static void displayExit()
        {
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine("||###################################################||");
            Console.WriteLine("||                                                   ||");
            Console.WriteLine("||                     GOOD   BYE                    ||");
            Console.WriteLine("||                                                   ||");
            Console.WriteLine("||###################################################||");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine();
        }
        public static void displayText(string text)
        {
            Console.WriteLine(text);
        }
        public static void PrintBoard(string boardToString)
        {

            string[] boxesToStringArray = boardToString.Split('_');

            


            Console.WriteLine(
                        "|###############################################################################|\n" +
                        "|#####  |       |       |       |       |       |       |       |       |  #####|\n" +
                        "|###    |   A   |   B   |   C   |   D   |   E   |   F   |   G   |   H   |    ###|\n" +
                        "|#      |       |       |       |       |       |       |       |       |      #|\n" +
                        "|~~~~~~~|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|~~~~~~~|\n" +
                        "|       |       |       |       |       |       |       |       |       |       |\n" +
                        "|   8   |  {0}  |  {1}  |  {2}  |  {3}  |  {4}  |  {5}  |  {6}  |  {7}  |   8   |\n" +
                        "|       |       |       |       |       |       |       |       |       |       |\n" +
                        "|~~~~~~~|===============================================================|~~~~~~~|\n" +
                        "|       |       |       |       |       |       |       |       |       |       |\n" +
                        "|   7   |  {8}  |  {9}  |  {10}  |  {11}  |  {12}  |  {13}  |  {14}  |  {15}  |   7   |\n" +
                        "|       |       |       |       |       |       |       |       |       |       |\n" +
                        "|~~~~~~~|===============================================================|~~~~~~~|\n" +
                        "|       |       |       |       |       |       |       |       |       |       |\n" +
                        "|   6   |  {16}  |  {17}  |  {18}  |  {19}  |  {20}  |  {21}  |  {22}  |  {23}  |   6   |\n" +
                        "|       |       |       |       |       |       |       |       |       |       |\n" +
                        "|~~~~~~~|===============================================================|~~~~~~~|\n" +
                        "|       |       |       |       |       |       |       |       |       |       |\n" +
                        "|   5   |  {24}  |  {25}  |  {26}  |  {27}  |  {28}  |  {29}  |  {30}  |  {31}  |   5   |\n" +
                        "|       |       |       |       |       |       |       |       |       |       |\n" +
                        "|~~~~~~~|===============================================================|~~~~~~~|\n" +
                        "|       |       |       |       |       |       |       |       |       |       |\n" +
                        "|   4   |  {32}  |  {33}  |  {34}  |  {35}  |  {36}  |  {37}  |  {38}  |  {39}  |   4   |\n" +
                        "|       |       |       |       |       |       |       |       |       |       |\n" +
                        "|~~~~~~~|===============================================================|~~~~~~~|\n" +
                        "|       |       |       |       |       |       |       |       |       |       |\n" +
                        "|   3   |  {40}  |  {41}  |  {42}  |  {43}  |  {44}  |  {45}  |  {46}  |  {47}  |   3   |\n" +
                        "|       |       |       |       |       |       |       |       |       |       |\n" +
                        "|~~~~~~~|===============================================================|~~~~~~~|\n" +
                        "|       |       |       |       |       |       |       |       |       |       |\n" +
                        "|   2   |  {48}  |  {49}  |  {50}  |  {51}  |  {52}  |  {53}  |  {54}  |  {55}  |   2   |\n" +
                        "|       |       |       |       |       |       |       |       |       |       |\n" +
                        "|~~~~~~~|===============================================================|~~~~~~~|\n" +
                        "|       |       |       |       |       |       |       |       |       |       |\n" +
                        "|   1   |  {56}  |  {57}  |  {58}  |  {59}  |  {60}  |  {61}  |  {62}  |  {63}  |   1   |\n" +
                        "|       |       |       |       |       |       |       |       |       |       |\n" +
                        "|~~~~~~~|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~|~~~~~~~|\n" +
                        "|#      |       |       |       |       |       |       |       |       |      #|\n" +
                        "|###    |   A   |   B   |   C   |   D   |   E   |   F   |   G   |   H   |    ###|\n" +
                        "|#####  |       |       |       |       |       |       |       |       |  #####|\n" +
                        "|###############################################################################|",
                        boxesToStringArray[0], boxesToStringArray[1], boxesToStringArray[2], boxesToStringArray[3], boxesToStringArray[4], boxesToStringArray[5], boxesToStringArray[6], boxesToStringArray[7],
                        boxesToStringArray[8], boxesToStringArray[9], boxesToStringArray[10], boxesToStringArray[11], boxesToStringArray[12], boxesToStringArray[13], boxesToStringArray[14], boxesToStringArray[15],
                        boxesToStringArray[16], boxesToStringArray[17], boxesToStringArray[18], boxesToStringArray[19], boxesToStringArray[20], boxesToStringArray[21], boxesToStringArray[22], boxesToStringArray[23],
                        boxesToStringArray[24], boxesToStringArray[25], boxesToStringArray[26], boxesToStringArray[27], boxesToStringArray[28], boxesToStringArray[29], boxesToStringArray[30], boxesToStringArray[31],
                        boxesToStringArray[32], boxesToStringArray[33], boxesToStringArray[34], boxesToStringArray[35], boxesToStringArray[36], boxesToStringArray[37], boxesToStringArray[38], boxesToStringArray[39],
                        boxesToStringArray[40], boxesToStringArray[41], boxesToStringArray[42], boxesToStringArray[43], boxesToStringArray[44], boxesToStringArray[45], boxesToStringArray[46], boxesToStringArray[47],
                        boxesToStringArray[48], boxesToStringArray[49], boxesToStringArray[50], boxesToStringArray[51], boxesToStringArray[52], boxesToStringArray[53], boxesToStringArray[54], boxesToStringArray[55],
                        boxesToStringArray[56], boxesToStringArray[57], boxesToStringArray[58], boxesToStringArray[59], boxesToStringArray[60], boxesToStringArray[61], boxesToStringArray[62], boxesToStringArray[63]);
        }
    }

    class Player
    {
        public Player (bool isWhitePlayer)
        {
            textChessGraphics.displayText("Please enter " + (isWhitePlayer ? "white" : "black") + " player's name and press ENTER");
            this.name = Console.ReadLine();
            this.isWhitePlayer = isWhitePlayer;
            this.isInCheck = false;
        }
        public string name;
        public bool isWhitePlayer;
        public bool isInCheck;
        public Player(string name, bool isWhitePlayer)
        {
            this.name = name;
            this.isWhitePlayer = isWhitePlayer;
            this.isInCheck = false;
        }
    }
    class Move
    {
        public pos from;
        public pos to;
        public bool isWhitesTurn;
        public Move()
        {

        }
        public Move(pos from, pos to, bool isWhitesTurn)
        {

            this.from = from;
            this.to = to;
            this.isWhitesTurn = isWhitesTurn;
        }
    }
    class pos
    {
        public int row;
        public int column;
        public pos(int row, int column)
        {
            this.row = row;
            this.column = column;
        }
    }
}
