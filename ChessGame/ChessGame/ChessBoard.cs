﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChessGame.Taslar;

namespace ChessGame
{
    public class ChessBoard : Form
    {
        private int tileSize = 60;
        private const int ROWS = 8;
        private const int COLUMNS = 8;
        private Button[,] buttons = new Button[ROWS, COLUMNS];
        private Label[] columnLabels = new Label[COLUMNS];
        private Label[] rowLabels = new Label[ROWS];
        private Panel panel1 = new Panel();
        private InputHandler _inputHandler;
        private List<ChessPiece> pieces = new List<ChessPiece>();

        public ChessBoard()
        {
            _inputHandler = new InputHandler(this);
            
            // Tahta butonlarının oluşturulması
            for (int i = 0; i < COLUMNS; i++)
            {
                for (int j = 0; j < ROWS; j++)
                {
                    Button button = new Button();
                    button.Size = new Size(tileSize, tileSize);
                    if ((i + j) % 2 == 0) // Karelerin siyah ve beyaz olmasını sağlamak için
                        button.BackColor = System.Drawing.Color.Black;
                    else
                        button.BackColor = System.Drawing.Color.White;
                    panel1.Controls.Add(button);
                    button.Location = new Point(j * tileSize, i * tileSize);
                    button.Tag = new Tuple<int, int>(i, j);
                    buttons[i,j] = button;
                    
                    // Taşların yerleştirilmesi
                    ChessPiece piece = null;
                    if (i == 0 || i == 7) // Siyah veya beyaz taşların sıraları
                    {
                        if (j == 0 || j == 7) // Kale
                            pieces.Add(piece = new Kale(i == 0 ? PieceColor.Black : PieceColor.White,
                                ChessPieceType.Kale, i, j));
                        else if (j == 1 || j == 6) // At
                            pieces.Add(piece = new At(i == 0 ? PieceColor.Black : PieceColor.White,
                                ChessPieceType.At, i, j));
                        else if (j == 2 || j == 5) // Fil
                            pieces.Add(piece = new Fil(i == 0 ? PieceColor.Black : PieceColor.White,
                                ChessPieceType.Fil, i, j));
                        else if (j == 3) // Vezir
                            pieces.Add(piece = new Vezir(i == 0 ? PieceColor.Black : PieceColor.White,
                                ChessPieceType.Vezir, i, j));
                        else if (j == 4) // Şah
                            pieces.Add(piece = new Kral(i == 0 ? PieceColor.Black : PieceColor.White,
                                ChessPieceType.Kral, i, j));

                        if (piece != null)
                        {
                            button.BackgroundImage = piece.Image;
                            button.Tag = new Tuple<int, int>(i, j);
                            button.BackgroundImageLayout = ImageLayout.Stretch;
                        }
                    }
                    else if (i == 1 || i == 6) // Piyon
                    {
                        pieces.Add(piece = new Piyon(i == 1 ? PieceColor.Black : PieceColor.White,
                            ChessPieceType.Piyon, i, j));
                        button.BackgroundImage = piece.Image;
                        button.BackgroundImageLayout = ImageLayout.Stretch;
                        button.Tag = new Tuple<int, int>(i, j);
                    }
                }
            }
            InitializeComponent();

            // satir etiketlerinin oluşturulması
            for (int i = 0; i < COLUMNS; i++)
            {
                columnLabels[i] = new Label();
                columnLabels[i].Text = ((char)(65 + i)).ToString();
                columnLabels[i].Text = (i + 1).ToString();
                columnLabels[i].Size = new Size(20, 20);
                columnLabels[i].Location = new Point((i * tileSize) + tileSize + 100, 110);
                columnLabels[i].TextAlign = ContentAlignment.MiddleCenter;
                columnLabels[i].Font = new Font("Arial", 12, FontStyle.Bold);
                columnLabels[i].Text = ((char)(65 + i)).ToString();
                this.Controls.Add(columnLabels[i]);
            }


            // Sütun etiketlerinin oluşturulması
            for (int i = 0; i < ROWS; i++)
            {
                rowLabels[i] = new Label();
                rowLabels[i].Text = (ROWS - i).ToString();
                rowLabels[i].Size = new Size(20, 20);
                rowLabels[i].Location = new Point(100, (i * tileSize) + tileSize + 100);
                rowLabels[i].TextAlign = ContentAlignment.MiddleCenter;
                rowLabels[i].Font = new Font("Arial", 12, FontStyle.Bold);
                this.Controls.Add(rowLabels[i]);
            }


            // Panelin boyutunu ayarla
            panel1.Size = new Size(tileSize * 9, tileSize * 9);

            // Panelin konumunu ayarla
            panel1.Location = new Point((this.Width - panel1.Width) / 2, (this.Height - panel1.Height) / 2);
            this.Load += new System.EventHandler(this.ChessBoard_Load);
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            foreach (var button in buttons)
            {
                button.Click += new EventHandler(_inputHandler.Tile_Click);
                Console.WriteLine(button.Tag);
            }
            this.ClientSize = new System.Drawing.Size(800,800);
            this.Name = "ChessBoard";
            this.ResumeLayout(false);

        }

        private void ChessBoard_Load(object sender, EventArgs e)
        {
            // Panelin Form üzerine eklenmesi
            this.Controls.Add(panel1);
        }
        
        public ChessPiece GetPieceAtPosition(int row, int col)
        {
            foreach (ChessPiece piece in pieces)
            {
                if (piece.CurrentRow == row && piece.CurrentColumn == col)
                {
                    return piece;
                }
            }

            return null;
        }
        public void MovePiece(int fromRow, int fromCol, int toRow, int toCol)
        {
            // Başlangıç pozisyonundaki taşın alınması
            ChessPiece movingPiece = GetPieceAtPosition(fromRow, fromCol);
            if (movingPiece == null)
            {
                // Başlangıç pozisyonunda taş yoksa hata mesajı yazdırıp metodun sonlandırılması
                Console.WriteLine("Error: No piece found at starting position.");
                return;
            }

            // Taşın hedef pozisyona taşınması
            if (movingPiece.CanMove(toRow, toCol, this))
            {
                // Hedef pozisyonda taş yoksa taşın yerleştirilmesi
                ChessPiece targetPiece = GetPieceAtPosition(toRow, toCol);
                if (targetPiece == null)
                {
                    movingPiece.CurrentRow = toRow;
                    movingPiece.CurrentColumn = toCol;
                }
                // Hedef pozisyonda taş varsa taşın alınması ve yerine hareket eden taşın yerleştirilmesi
                else
                {
                    pieces.Remove(targetPiece);
                    movingPiece.CurrentRow = toRow;
                    movingPiece.CurrentColumn = toCol;
                }
            }
            else
            {
                // Hedef pozisyon geçersizse hata mesajı yazdırılır.
                Console.WriteLine("Error: Invalid move.");
                return;
            }
        }
        public bool IsOccupied(int row, int column)
        {
            if (row < 0 || row >= ROWS || column < 0 || column >= COLUMNS)
            {
                // Geçersiz koordinatlar
                return false;
            }

            foreach (var piece in pieces)
            {
                if (piece.CurrentColumn == column && piece.CurrentRow == row)
                {
                    return true;
                }
            }
            return false;
        }

        public void UpdateBoard()
        {
            foreach (Control control in panel1.Controls)
            {
                if (control is Button button)
                {
                    Tuple<int, int> position = (Tuple<int,int>)button.Tag;
                    ChessPiece piece = GetPieceAtPosition(position.Item1, position.Item2);

                    if (piece != null)
                    {
                        button.BackgroundImage = piece.Image;
                        button.BackgroundImageLayout = ImageLayout.Stretch;
                    }
                    else
                    {
                        button.BackgroundImage = null;
                    }
                    button.BackColor = (position.Item1 + position.Item2) % 2 == 0 ? Color.Black : Color.White; // siyah ve beyaz karelerin arka plan rengi
                }
            }
        }



    }
}
