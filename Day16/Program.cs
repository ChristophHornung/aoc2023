namespace Day16;

using Base;

internal class Program
{
	private static void Main(string[] args)
	{
		new Day16().Run();
	}
}

internal class Day16 : DayX
{
	public Day16()
	{
	}

	private static void WriteEnergized(Dictionary<(int x, int y), Direction> existing)
	{
		int y;
		for (y = 0; y <= existing.Keys.Max(k => k.y); y++)
		{
			for (int x = 0; x <= existing.Keys.Max(k => k.x); x++)
			{
				Console.Write(existing[(x, y)] != Direction.None ? '#' : ' ');
			}

			Console.WriteLine();
		}
	}

	public void Run()
	{
		StreamReader stream = this.GetInput();
		Dictionary<(int x, int y), Mirror> map = new();
		string line = stream.ReadLine()!;
		int y = 0;
		while (line != null)
		{
			for (int x = 0; x < line.Length; x++)
			{
				map[(x, y)] = line[x] switch
				{
					'/' => Mirror.Slash,
					'\\' => Mirror.Backslash,
					'-' => Mirror.Horizontal,
					'|' => Mirror.Vertical,
					'.' => Mirror.Empty
				};
			}

			y++;
			line = stream.ReadLine();
		}

		int maxY = y - 1;
		int maxX = map.Keys.Max(k => k.x);

		Dictionary<(int x, int y), Direction> existing = Day16.Empty(map);

		this.TraceBeam(map, (0, 0), Direction.Right, existing);

		this.ReportResult(existing.Count(c => c.Value != Direction.None));

		long energized = 0;
		for (int yI = 0; yI <= maxY; yI++)
		{
			existing = Day16.Empty(map);
			this.TraceBeam(map, (0, yI), Direction.Right, existing);
			energized = Math.Max(energized, existing.Count(c => c.Value != Direction.None));
			existing = Day16.Empty(map);
			this.TraceBeam(map, (maxX, yI), Direction.Left, existing);
			energized = Math.Max(energized, existing.Count(c => c.Value != Direction.None));
		}

		for (int xI = 0; xI <= maxX; xI++)
		{
			existing = Day16.Empty(map);
			this.TraceBeam(map, (xI, 0), Direction.Down, existing);
			energized = Math.Max(energized, existing.Count(c => c.Value != Direction.None));
			existing = Day16.Empty(map);
			this.TraceBeam(map, (xI, maxY), Direction.Up, existing);
			energized = Math.Max(energized, existing.Count(c => c.Value != Direction.None));
		}

		this.ReportResult(energized);
	}

	private static Dictionary<(int x, int y), Direction> Empty(Dictionary<(int x, int y), Mirror> map)
	{
		Dictionary<(int x, int y), Direction> existing = new();
		foreach ((int xK, int yK) in map.Keys)
		{
			existing[(xK, yK)] = Direction.None;
		}

		return existing;
	}

	private void TraceBeam(Dictionary<(int x, int y), Mirror> map, (int x, int y) position, Direction direction,
		Dictionary<(int x, int y), Direction> existing)
	{
		while (true)
		{
			if (position.x < 0 || position.y < 0)
			{
				return;
			}

			if (position.x > map.Keys.Max(k => k.x) || position.y > map.Keys.Max(k => k.y))
			{
				return;
			}

			if (existing[position].HasFlag(direction))
			{
				return;
			}

			existing[position] |= direction;
			Mirror mirror = map[position];

			switch (mirror)
			{
				case Mirror.Empty:
					position = this.GetNext(position, direction);
					break;
				case Mirror.Slash:
					direction = direction switch
					{
						Direction.Right => Direction.Up,
						Direction.Up => Direction.Right,
						Direction.Left => Direction.Down,
						Direction.Down => Direction.Left,
						_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
					};
					position = this.GetNext(position, direction);
					break;
				case Mirror.Backslash:
					direction = direction switch
					{
						Direction.Left => Direction.Up,
						Direction.Up => Direction.Left,
						Direction.Right => Direction.Down,
						Direction.Down => Direction.Right,
						_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
					};
					position = this.GetNext(position, direction);
					break;
				case Mirror.Horizontal:
					switch (direction)
					{
						case Direction.Left:
						case Direction.Right:
							position = this.GetNext(position, direction);
							break;
						case Direction.Up:
						case Direction.Down:
							this.TraceBeam(map, this.GetNext(position, Direction.Left), Direction.Left, existing);
							this.TraceBeam(map, this.GetNext(position, Direction.Right), Direction.Right, existing);
							return;
						default:
							throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
					}

					break;
				case Mirror.Vertical:
					switch (direction)
					{
						case Direction.Left:
						case Direction.Right:
							this.TraceBeam(map, this.GetNext(position, Direction.Up), Direction.Up, existing);
							this.TraceBeam(map, this.GetNext(position, Direction.Down), Direction.Down, existing);
							return;
						case Direction.Up:
						case Direction.Down:
							position = this.GetNext(position, direction);
							break;
						default:
							throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
					}

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}


	private (int x, int y) GetNext((int x, int y) position, Direction direction)
	{
		return this.GetNext(position.x, position.y, direction);
	}

	private (int x, int y) GetNext(int x, int y, Direction direction)
	{
		return direction switch
		{
			Direction.Left => (x - 1, y),
			Direction.Up => (x, y - 1),
			Direction.Right => (x + 1, y),
			Direction.Down => (x, y + 1),
			_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
		};
	}
}

[Flags]
internal enum Direction
{
	None = 0,
	Left = 1,
	Up = 1 << 1,
	Right = 1 << 2,
	Down = 1 << 3
}

internal enum Mirror
{
	Empty,
	Slash,
	Backslash,
	Horizontal,
	Vertical
}