using System;
using UnityEngine;

namespace Chess.Scripts.Core
{
    public class ChessPlayerPlacementHandler : MonoBehaviour
    {
        [SerializeField] public int row, column;

        private void Start()
        {
            transform.position = ChessBoardPlacementHandler.Instance.GetTile(row, column).transform.position;
        }

        private void OnMouseDown()
        {
            // Clear all previous highlights
            ChessBoardPlacementHandler.Instance.ClearHighlights();

            string enemyTag = gameObject.tag;
            if(enemyTag == "Enemy")
            {
                return;
            }
            // Highlight possible moves based on the piece's tag
            HighlightPossibleMoves();
        }

        private void HighlightPossibleMoves()
        {
            string pieceTag = gameObject.tag;
            switch (pieceTag)
            {
                case "Pawn":
                    HighlightPawnMoves(row, column);
                    break;
                case "Rook":
                    HighlightRookMoves(row, column);
                    break;
                case "Knight":
                    HighlightKnightMoves(row, column);
                    break;
                case "Bishop":
                    HighlightBishopMoves(row, column);
                    break;
                case "Queen":
                    HighlightQueenMoves(row, column);
                    break;
                case "King":
                    HighlightKingMoves(row, column);
                    break;
                default:
                    Debug.LogError("Unknown piece tag.");
                    break;
            }
        }
        #region Pawn Movement Logic
        private void HighlightPawnMoves(int row, int column)
        {
            // Implement highlighting logic for pawn moves

            // Highlight one square forward
            int forwardDirection = 1;
            if (IsValidTile(row + forwardDirection, column) && !IsPiecePresent(row + forwardDirection, column))
            {
                ChessBoardPlacementHandler.Instance.Highlight(row + forwardDirection, column);
            }

            // Highlight two squares forward if the pawn is in its starting position and the path is clear
            if (row == 1 && !IsPiecePresent(row + 1, column) && !IsPiecePresent(row + 2, column))
            {
                ChessBoardPlacementHandler.Instance.Highlight(row + 2, column);
            }

            // Check diagonally for enemy pieces and highlight them
            if (IsValidTile(row + forwardDirection, column + 1) && IsEnemyPiece(row + forwardDirection, column + 1))
            {
                ChessBoardPlacementHandler.Instance.HighlightEnemy(row + forwardDirection, column + 1);
            }
            if (IsValidTile(row + forwardDirection, column - 1) && IsEnemyPiece(row + forwardDirection, column - 1))
            {
                ChessBoardPlacementHandler.Instance.HighlightEnemy(row + forwardDirection, column - 1);
            }
        }


        #endregion

        #region Rook Movement Logic
        private void HighlightRookMoves(int row, int column)
        {
            // Highlight horizontal moves to the right
            for (int i = row + 1; i < 8; i++)
            {
                if (!HighlightAndCheckOccupancy(i, column))
                    break; // Stop highlighting if there's an obstacle
            }

            // Highlight horizontal moves to the left
            for (int i = row - 1; i >= 0; i--)
            {
                if (!HighlightAndCheckOccupancy(i, column))
                    break; // Stop highlighting if there's an obstacle
            }

            // Highlight vertical moves upwards
            for (int j = column + 1; j < 8; j++)
            {
                if (!HighlightAndCheckOccupancy(row, j))
                    break; // Stop highlighting if there's an obstacle
            }

            // Highlight vertical moves downwards
            for (int j = column - 1; j >= 0; j--)
            {
                if (!HighlightAndCheckOccupancy(row, j))
                    break; // Stop highlighting if there's an obstacle
            }
        }

        private bool HighlightAndCheckOccupancy(int row, int column)
        {
            // Check if the tile is valid and not obstructed by other pieces
            if (IsValidTile(row, column) && !IsPiecePresent(row, column))
            {
                ChessBoardPlacementHandler.Instance.Highlight(row, column);
                // Return true if the tile is unoccupied, false if occupied

                return true;
            }
            return false; // The tile is invalid (out of bounds)
        }


        #endregion

        #region Knight Movement Logic
        private void HighlightKnightMoves(int row, int column)
        {
            HighlightLShapeMoves(row, column, 2, 1); // Up 2, Right 1
            HighlightLShapeMoves(row, column, 2, -1); // Up 2, Left 1
            HighlightLShapeMoves(row, column, -2, 1); // Down 2, Right 1
            HighlightLShapeMoves(row, column, -2, -1); // Down 2, Left 1
            HighlightLShapeMoves(row, column, 1, 2); // Up 1, Right 2
            HighlightLShapeMoves(row, column, 1, -2); // Up 1, Left 2
            HighlightLShapeMoves(row, column, -1, 2); // Down 1, Right 2
            HighlightLShapeMoves(row, column, -1, -2); // Down 1, Left 2
        }

        private void HighlightLShapeMoves(int row, int column, int rowDelta, int colDelta)
        {
            int newRow = row + rowDelta;
            int newColumn = column + colDelta;

            // Check if the tile is valid and not obstructed by other pieces
            if (IsValidTile(newRow, newColumn) && !IsPiecePresent(newRow, newColumn))
            {
                ChessBoardPlacementHandler.Instance.Highlight(newRow, newColumn);
            }
        }

        #endregion

        #region Bishop Movement Logic
        private void HighlightBishopMoves(int row, int column)
        {
            // Check diagonally up and right
            HighlightDiagonalMoves(row, column, 1, 1);

            // Check diagonally up and left
            HighlightDiagonalMoves(row, column, 1, -1);

            // Check diagonally down and right
            HighlightDiagonalMoves(row, column, -1, 1);

            // Check diagonally down and left
            HighlightDiagonalMoves(row, column, -1, -1);
        }

        private void HighlightDiagonalMoves(int row, int column, int rowDirection, int columnDirection)
        {
            int newRow = row + rowDirection;
            int newColumn = column + columnDirection;

            // Loop until we reach the board boundary
            while (IsValidTile(newRow, newColumn))
            {
                
                // Check if there's an obstacle (another piece) on this tile
                if (IsPiecePresent(newRow, newColumn))
                {
                    break; // Stop highlighting in this direction if an obstacle is found
                }

                // Highlight the tile
                ChessBoardPlacementHandler.Instance.Highlight(newRow, newColumn);

                // Move to the next tile in the diagonal direction
                newRow += rowDirection;
                newColumn += columnDirection;
            }
        }

        #endregion

        #region Queen Movement Logic
        private void HighlightQueenMoves(int row, int column)
        {
            // Implement highlighting logic for queen moves
            // Example:
            // Highlight horizontal, vertical, and diagonal moves
            // Check for obstacles and boundaries
            HighlightBishopMoves(row, column);
            HighlightRookMoves(row, column);
             
        }
        #endregion

        #region King Movement Logic

        private void HighlightKingMoves(int row, int column)
        {
            // Highlight adjacent squares
            HighlightSingleMove(row + 1, column);
            HighlightSingleMove(row - 1, column);
            HighlightSingleMove(row, column + 1);
            HighlightSingleMove(row, column - 1);

            // Highlight diagonally adjacent squares
            HighlightSingleMove(row + 1, column + 1);
            HighlightSingleMove(row - 1, column - 1);
            HighlightSingleMove(row - 1, column + 1);
            HighlightSingleMove(row + 1, column - 1);
        }

        private void HighlightSingleMove(int row, int column)
        {
            // Check if the tile is valid and not obstructed by other pieces
            if (IsValidTile(row, column) && !IsPiecePresent(row, column))
            {
                ChessBoardPlacementHandler.Instance.Highlight(row, column);
            }
        }

        #endregion


        #region Helper Fucntions for Pieces Movement Logic

        private bool IsValidTile(int row, int column)
        {
            return row >= 0 && row < 8 &&
                   column >= 0 && column < 8;
        }

        private bool IsPiecePresent(int row, int column)
        {
            // Check if the tile at the specified position exists
            GameObject tile = ChessBoardPlacementHandler.Instance.GetTile(row, column);

            // If the tile is null, no piece is present
            if (tile == null)
            {
                return false;
            }

            // Check if any chess piece is present at the specified position
            ChessPlayerPlacementHandler[] allPieces = FindObjectsOfType<ChessPlayerPlacementHandler>();
            foreach (var piece in allPieces)
            {
                // Check if a piece is present at the specified position
                if (piece.row == row && piece.column == column)
                {
                    // If it's an enemy piece, highlight it and return true
                    if (piece.gameObject.CompareTag("Enemy"))
                    {
                        ChessBoardPlacementHandler.Instance.HighlightEnemy(row, column);
                    }
                    return true;
                }
            }

            // No piece found at the specified position
            return false;
        }

        private bool IsEnemyPiece(int row, int column)
        {
            // Check if any chess piece is present at the specified position
            ChessPlayerPlacementHandler[] allPieces = FindObjectsOfType<ChessPlayerPlacementHandler>();
            foreach (var piece in allPieces)
            {
                // Check if a piece is present at the specified position and is an enemy piece
                if (piece.row == row && piece.column == column && piece.CompareTag("Enemy"))
                {
                    return true;
                }
            }

            // No enemy piece found at the specified position
            return false;
        }
        #endregion
    }
}
