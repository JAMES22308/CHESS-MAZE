namespace ChessMaze.Model;

public sealed class GamePlayer
{
    readonly int rows = 6,
        cols = 6;
    readonly Dictionary<Position, PieceType> board;
    readonly List<MoveRecord> history = [];
    public Position StartPosition { get; } = new(5, 0);
    public Position TargetPosition { get; } = new(0, 5);
    public Position CurrentPosition { get; private set; } = new(5, 0);
    public bool IsComplete { get; private set; }
    public IReadOnlyList<MoveRecord> MoveHistory => history.AsReadOnly();
    public IReadOnlyDictionary<Position, PieceType> Board => board;

    public GamePlayer()
    {
        board = new()
        {
            [new(5, 0)] = PieceType.Rook,
            [new(5, 3)] = PieceType.Bishop,
            [new(3, 5)] = PieceType.Knight,
            [new(1, 4)] = PieceType.King,
        };
    }

    public PieceType? GetPieceAt(Position p) => board.TryGetValue(p, out var x) ? x : null;

    public MoveResult TryMove(Position d)
    {
        if (IsComplete)
            return MoveResult.GameAlreadyCompleted;
        if (!Inside(d))
            return MoveResult.OutOfBounds;
        var f = CurrentPosition;
        var piece = board[f];
        int dr = d.Row - f.Row,
            dc = d.Column - f.Column;
        if (!Shape(piece, dr, dc))
            return MoveResult.InvalidMovement;
        if ((piece is PieceType.Rook or PieceType.Bishop) && Blocked(f, d))
            return MoveResult.PathBlocked;
        if (d != TargetPosition && !board.ContainsKey(d))
            return MoveResult.InvalidDestination;
        CurrentPosition = d;
        history.Add(new(history.Count + 1, f, d, piece));
        if (d == TargetPosition)
        {
            IsComplete = true;
            return MoveResult.GameCompleted;
        }

        return MoveResult.Success;
    }

    public void Restart()
    {
        CurrentPosition = StartPosition;
        history.Clear();
        IsComplete = false;
    }

    public GameSnapshot Capture() =>
        new("Reference-v1.6", CurrentPosition, history.ToList(), IsComplete);

    public void Restore(GameSnapshot s)
    {
        if (s.BoardId != "Reference-v1.6")
            throw new InvalidOperationException("Unknown board.");
        if (!Inside(s.CurrentPosition))
            throw new InvalidOperationException("Invalid position.");
        if (s.History.Select((m, i) => m.SequenceNumber == i + 1).Any(ok => !ok))
            throw new InvalidOperationException("Invalid history ordering.");
        if (s.IsComplete != (s.CurrentPosition == TargetPosition))
            throw new InvalidOperationException("Incoherent completion state.");
        CurrentPosition = s.CurrentPosition;
        history.Clear();
        history.AddRange(s.History);
        IsComplete = s.IsComplete;
    }

    bool Inside(Position p) => p.Row >= 0 && p.Row < rows && p.Column >= 0 && p.Column < cols;

    static bool Shape(PieceType p, int dr, int dc) =>
        p switch
        {
            PieceType.Rook => (dr == 0) ^ (dc == 0),
            PieceType.Bishop => dr != 0 && Math.Abs(dr) == Math.Abs(dc),
            PieceType.Knight => (Math.Abs(dr), Math.Abs(dc)) is (2, 1) or (1, 2),
            PieceType.King => Math.Max(Math.Abs(dr), Math.Abs(dc)) == 1,
            _ => false,
        };

    bool Blocked(Position a, Position b)
    {
        int sr = Math.Sign(b.Row - a.Row),
            sc = Math.Sign(b.Column - a.Column);
        for (
            var q = new Position(a.Row + sr, a.Column + sc);
            q != b;
            q = new(q.Row + sr, q.Column + sc)
        )
            if (board.ContainsKey(q))
                return true;
        return false;
    }
}
