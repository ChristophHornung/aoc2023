namespace Day10;

using System.IO;
using Base;

internal class Program
{
	static void Main(string[] args)
	{
		new Day10().Run(false);
	}

	internal class Day10 : DayX
	{
		public void Run(bool isPart2)
		{
			using StreamReader input = this.GetInput();
			string? line = input.ReadLine();
			Dictionary<(int x, int y), Tile> tiles = new();
			int y = 0;
			Tile startTile = null!;
			while (line != null)
			{
				for (int x = 0; x < line.Length; x++)
				{
					PipeShape pipeShape = line[x] switch
					{
						'.' => PipeShape.Empty,
						'S' => PipeShape.Empty,
						'|' => PipeShape.Vertical,
						'-' => PipeShape.Horizontal,
						'L' => PipeShape.TopRight,
						'J' => PipeShape.TopLeft,
						'7' => PipeShape.BottomLeft,
						'F' => PipeShape.BottomRight,
					};
					Tile tile = new Tile(x, y, pipeShape);
					tiles.Add((x, y), tile);

					if (line[x] == 'S')
					{
						startTile = tile;
					}
				}

				line = input.ReadLine();
				y++;
			}

			// Our path starts downwards and ends upwards
			List<Tile>? path = this.TracePath(startTile, Direction.Down, tiles)!;
			Direction startDirection = Direction.Down;
			if (path == null)
			{
				path = this.TracePath(startTile, Direction.Up, tiles)!;
				startDirection = Direction.Up;
			}

			if (path == null)
			{
				path = this.TracePath(startTile, Direction.Right, tiles)!;
				startDirection = Direction.Right;
			}

			var pathTiles = path.Concat([startTile]).ToHashSet();
			(HashSet<Tile>? leftOfPath, HashSet<Tile>? rightOfPath) = this.TraceOutsides(startTile, startDirection,
				tiles, pathTiles);

			leftOfPath = Fill(leftOfPath, pathTiles, tiles);
			rightOfPath = Fill(rightOfPath, pathTiles, tiles);

			this.ReportResult(path.Count / 2);
			Console.WriteLine("Path Tiles");
			Console.WriteLine(path.Count);
			Console.WriteLine("Left Of Path");
			this.ReportResult(leftOfPath.Count);
			Console.WriteLine("Right Of Path");
			this.ReportResult(rightOfPath.Count);
			Console.WriteLine("Total Count 1");
			this.ReportResult(path.Count + leftOfPath.Count + rightOfPath.Count);
			Console.WriteLine("Total Count 2");
			this.ReportResult(tiles.Count);
			Print(path, tiles, leftOfPath, rightOfPath);
		}

		private void Print(List<Tile> path, Dictionary<(int x, int y), Tile> tiles, HashSet<Tile> leftOfPath,
			HashSet<Tile> rightOfPath)
		{
			// Set console to unicode
			Console.OutputEncoding = System.Text.Encoding.UTF8;
			for (int y = 0; y <= tiles.Max(t => t.Key.y); y++)
			{
				for (int x = 0; x <= tiles.Max(t => t.Key.x); x++)
				{
					Tile tile = tiles[(x, y)];
					if (path.Contains(tile))
					{
						char write = tile.PipeShape switch
						{
							PipeShape.Empty => 'S',
							PipeShape.Vertical => '\u2503',
							PipeShape.Horizontal => '\u2501',
							PipeShape.TopLeft => '\u251b',
							PipeShape.TopRight => '\u2517',
							PipeShape.BottomLeft => '\u2513',
							PipeShape.BottomRight => '\u250f',
							_ => throw new ArgumentOutOfRangeException(nameof(tile.PipeShape), tile.PipeShape, null)
						};
						Console.Write(write);
					}
					else if (leftOfPath.Contains(tile))
					{
						Console.Write("1");
					}
					else if (rightOfPath.Contains(tile))
					{
						Console.Write("0");
					}
					else
					{
						Console.Write("X");
					}
				}

				Console.WriteLine();
			}
		}

		private HashSet<Tile> Fill(HashSet<Tile> toFill, HashSet<Tile> pathTiles,
			Dictionary<(int x, int y), Tile> tiles)
		{
			HashSet<Tile> visited = toFill.ToHashSet();
			Queue<Tile> queue = new(toFill);
			while (queue.TryDequeue(out Tile? next))
			{
				foreach (Direction direction in Enum.GetValuesAsUnderlyingType<Direction>())
				{
					try
					{
						(Tile? check, _) = this.GetNext(next, direction, tiles);
						if (!pathTiles.Contains(check) && visited.Add(check))
						{
							queue.Enqueue(check);
						}
					}
					catch (KeyNotFoundException)
					{
						// Just ignore out of bounds
					}
				}
			}

			return visited;
		}

		int ModThatMakesSense(int x, int m)
		{
			// Why oh wy do you always have to be so difficult, C#?
			int r = x % m;
			return r < 0 ? r + m : r;
		}

		private (HashSet<Tile> leftOfPath, HashSet<Tile> rightOfPath) TraceOutsides(Tile startTile,
			Direction? direction, Dictionary<(int x, int y), Tile> tiles, HashSet<Tile> pathTiles)
		{
			HashSet<Tile> leftOfPath = [];
			HashSet<Tile> rightOfPath = [];
			Tile? currentTile = startTile;
			do
			{
				Direction priorDirection = direction!.Value;
				(currentTile, direction) = this.GetNext(currentTile, direction.Value, tiles);
				if (direction != null)
				{
					Direction left = (Direction)(this.ModThatMakesSense((int)direction.Value - 1, 4));
					Direction right = (Direction)(this.ModThatMakesSense((int)direction.Value + 1, 4));
					this.AddIfNotOnPath(tiles, pathTiles, currentTile, left, leftOfPath);
					this.AddIfNotOnPath(tiles, pathTiles, currentTile, right, rightOfPath);

					if (priorDirection != direction.Value)
					{
						// We're turning
						left = (Direction)(this.ModThatMakesSense((int)priorDirection - 1, 4));
						right = (Direction)(this.ModThatMakesSense((int)priorDirection + 1, 4));
						this.AddIfNotOnPath(tiles, pathTiles, currentTile, left, leftOfPath);
						this.AddIfNotOnPath(tiles, pathTiles, currentTile, right, rightOfPath);
					}

					
				}
			} while (direction != null && currentTile != startTile);

			return (leftOfPath, rightOfPath);
		}

		private void AddIfNotOnPath(Dictionary<(int x, int y), Tile> tiles, HashSet<Tile> pathTiles, Tile currentTile, Direction direction, HashSet<Tile> addTo)
		{
			try
			{
				Tile leftTile = this.GetNext(currentTile, direction, tiles).Item1;
				if (!pathTiles.Contains(leftTile))
				{
					addTo.Add(leftTile);
				}
			}
			catch (KeyNotFoundException)
			{
				// Just ignore out of bounds
			}
		}

		private List<Tile>? TracePath(Tile startTile, Direction? direction, Dictionary<(int x, int y), Tile> tiles)
		{
			List<Tile> path = [];
			Tile? currentTile = startTile;
			do
			{
				path.Add(currentTile);
				(currentTile, direction) = this.GetNext(currentTile, direction.Value, tiles);
			} while (direction != null && currentTile != startTile);

			return currentTile != startTile ? null : path;
		}

		private (Tile, Direction?) GetNext(Tile currentTile, Direction direction,
			Dictionary<(int x, int y), Tile> tiles)
		{
			Tile nextTile = direction switch
			{
				Direction.Up => tiles[(currentTile.X, currentTile.Y - 1)],
				Direction.Down => tiles[(currentTile.X, currentTile.Y + 1)],
				Direction.Left => tiles[(currentTile.X - 1, currentTile.Y)],
				Direction.Right => tiles[(currentTile.X + 1, currentTile.Y)],
				_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
			};

			return nextTile.PipeShape switch
			{
				PipeShape.Horizontal when direction is Direction.Left or Direction.Right => (nextTile, direction),

				PipeShape.Vertical when direction is Direction.Up or Direction.Down => (nextTile, direction),

				PipeShape.TopLeft when direction == Direction.Down => (nextTile, Direction.Left),
				PipeShape.TopLeft when direction == Direction.Right => (nextTile, Direction.Up),

				PipeShape.TopRight when direction == Direction.Down => (nextTile, Direction.Right),
				PipeShape.TopRight when direction == Direction.Left => (nextTile, Direction.Up),

				PipeShape.BottomLeft when direction == Direction.Up => (nextTile, Direction.Left),
				PipeShape.BottomLeft when direction == Direction.Right => (nextTile, Direction.Down),

				PipeShape.BottomRight when direction == Direction.Up => (nextTile, Direction.Right),
				PipeShape.BottomRight when direction == Direction.Left => (nextTile, Direction.Down),
				_ => (nextTile, null)
			};
		}
	}

	internal enum Direction
	{
		Up = 0,
		Right = 1,
		Down = 2,
		Left = 3
	}

	internal class Tile
	{
		public int X { get; }
		public int Y { get; }
		public PipeShape PipeShape { get; }

		public Tile(int x, int y, PipeShape pipeShape)
		{
			this.X = x;
			this.Y = y;
			this.PipeShape = pipeShape;
		}

		public override string ToString()
		{
			return $"({this.X},{this.Y})";
		}
	}

	internal enum PipeShape
	{
		Empty,
		Vertical,
		Horizontal,
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}
}