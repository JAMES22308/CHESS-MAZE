
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BCIT615.Assessment1.GamePlayer;
using BCIT615.Assessment1.GamePlayer.Model;

namespace Wpf;

public partial class MainWindow : Window
{
    private readonly GamePlayer game = new GamePlayer();

    public MainWindow()
    {
        InitializeComponent();
        CreateBoard();
        RefreshBoard();

        StatusText.Text = "Select a destination square.";
    }

    private void CreateBoard()
    {
        Board.Children.Clear();

        for (int row = 0; row < ReferenceBoardData.Rows; row++)
        {
            for (int column = 0;
                 column < ReferenceBoardData.Columns;
                 column++)
            {
                var position = new Position(row, column);

                var button = new Button
                {
                    Tag = position,
                    FontSize = 36,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(
                        Color.FromRgb(75, 85, 99))
                };

                button.Click += Square_Click;
                Board.Children.Add(button);
            }
        }
    }

    private void RefreshBoard()
    {
        foreach (Button button in Board.Children)
        {
            var position = (Position)button.Tag;
            var piece = game.GetPieceAt(position);

            string symbol = piece switch
            {
                PieceType.Rook => "♖",
                PieceType.Bishop => "♗",
                PieceType.Knight => "♘",
                PieceType.King => "♔",
                _ => ""
            };

            var content = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            content.Children.Add(new TextBlock
            {
                Text = symbol,
                FontSize = 36,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            if (position == game.TargetPosition)
            {
                content.Children.Add(new TextBlock
                {
                    Text = "TARGET",
                    FontSize = 9,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                });
            }

            button.Content = content;

            button.Background = (position.Row + position.Column) % 2 == 0
                ? new SolidColorBrush(Color.FromRgb(55, 65, 81))
                : new SolidColorBrush(Color.FromRgb(31, 41, 55));

            // Highlight the player's current square.
            if (position == game.CurrentPosition)
            {
                button.BorderBrush = Brushes.Gold;
                button.BorderThickness = new Thickness(4);
                button.ToolTip = "Current position";
            }
            else if (position == game.TargetPosition)
            {
                button.BorderBrush = Brushes.LightGreen;
                button.BorderThickness = new Thickness(3);
                button.ToolTip = "Target";
            }
            else
            {
                button.BorderBrush = new SolidColorBrush(
                    Color.FromRgb(75, 85, 99));
                button.BorderThickness = new Thickness(1);
                button.ToolTip = position.ToString();
            }
        }

        MoveCountText.Text = $"Moves: {game.MoveHistory.Count}";

        MoveHistoryList.Items.Clear();

        foreach (var move in game.MoveHistory)
        {
            MoveHistoryList.Items.Add(
                $"{move.SequenceNumber}. " +
                $"{FormatPosition(move.From)} → " +
                $"{FormatPosition(move.To)} " +
                $"({move.MovementPiece})");
        }
    }

    private void Square_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button ||
            button.Tag is not Position destination)
        {
            return;
        }

        MoveResult result = game.TryMove(destination);

        StatusText.Text = result switch
        {
            MoveResult.Success => "Move successful.",
            MoveResult.GameCompleted => "Congratulations! You reached the target!",
            MoveResult.GameAlreadyCompleted => "The game is already complete. Restart to play again.",
            MoveResult.OutOfBounds => "That position is outside the board.",
            MoveResult.InvalidMovement => "That move is not allowed for this piece.",
            MoveResult.PathBlocked => "Another piece blocks the path.",
            MoveResult.InvalidDestination => "You must move to a valid destination.",
            _ => result.ToString()
        };

        RefreshBoard();
    }

    private void RestartButton_Click(object sender, RoutedEventArgs e)
    {
        game.Restart();
        RefreshBoard();
        StatusText.Text = "Game restarted. Select a destination.";
    }

    private string FormatPosition(Position position)
    {
        char column = (char)('A' + position.Column);
        int row = ReferenceBoardData.Rows - position.Row;

        return $"{column}{row}";
    }
}
