namespace Day17;

using Base;

internal class Program
{
	private static void Main(string[] args)
	{
		new Day17(true).Run();
	}
}

internal class Day17 : DayX
{
	private readonly bool isPart2;

	public Day17(bool isPart2)
	{
		this.isPart2 = isPart2;
	}

	public void Run()
	{
		Dictionary<(int x, int y), int> map = new();
		StreamReader input = this.GetInput();
		string? line = input.ReadLine();
		int y = 0;
		while (line != null)
		{
			for (int x = 0; x < line.Length; x++)
			{
				map[(x, y)] = int.Parse(line[x].ToString());
			}

			line = input.ReadLine();
			y++;
		}

		// Dictionary to array
		int maxX = map.Keys.Max(k => k.x);
		int maxY = map.Keys.Max(k => k.y);
		int[,] mapArray = new int[maxX + 1, maxY + 1];
		for (int x = 0; x <= maxX; x++)
		for (int yI = 0; yI <= maxY; yI++)
		{
			mapArray[x, yI] = map[(x, yI)];
		}

		long minimalHeatLoss = this.GetMinimalHeatLoss(mapArray, (0, 0), maxX, maxY);
		this.ReportResult(minimalHeatLoss);
	}

	private long GetMinimalHeatLoss(int[,] map, (int, int) position, int maxX, int maxY)
	{
		var stack = new Stack<((int x, int y) position, int stepCount, Direction direction, long heatLoss)>();
		stack.Push((position, 0, Direction.Down, -map[0, 0]));
		stack.Push((position, 0, Direction.Right, -map[0, 0]));

		long minimalHeatLoss = long.MaxValue;
		Dictionary<(int x, int y, Direction direction, int stepCount), long> visited = new();
		while (stack.Count > 0)
		{
			var (currentPosition, stepCount, direction, heatLoss) = stack.Pop();

			if (currentPosition.x < 0 || currentPosition.x > maxX ||
			    currentPosition.y < 0 || currentPosition.y > maxY)
			{
				continue;
			}

			heatLoss += map[currentPosition.x, currentPosition.y];
			if (visited.ContainsKey((currentPosition.x, currentPosition.y, direction, stepCount)) &&
			    visited[(currentPosition.x, currentPosition.y, direction, stepCount)] <= heatLoss)
			{
				continue;
			}

			if (heatLoss >= minimalHeatLoss)
			{
				continue;
			}

			if (currentPosition == (maxX, maxY) && stepCount >= 4)
			{
				minimalHeatLoss = Math.Min(minimalHeatLoss, heatLoss);
				Console.WriteLine(minimalHeatLoss);
				continue;
			}

			visited[(currentPosition.x, currentPosition.y, direction, stepCount)] = heatLoss;

			foreach (Direction newDirection in Enum.GetValues<Direction>())
			{
				if (this.IsValidNextPosition(newDirection, direction, stepCount))
				{
					(int x, int y) nextPosition = this.GetNext(currentPosition, newDirection);
					stack.Push((nextPosition,
						direction == newDirection ? stepCount + 1 : 1,
						newDirection, heatLoss));
				}
			}
		}

		return minimalHeatLoss;
	}

	private bool IsValidNextPosition(Direction newDirection, Direction direction, int stepCount)
	{
		bool fullRotate = (Direction)this.ModThatMakesSense((int)(newDirection - 2), 4) == direction;
		if (fullRotate)
		{
			return false;
		}

		if (this.isPart2)
		{
			if (stepCount < 4 && newDirection != direction)
			{
				return false;
			}

			if (stepCount == 10 && newDirection == direction)
			{
				return false;
			}
		}
		else
		{
			return (newDirection != direction || stepCount < 3);
		}

		return true;
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

	private enum Direction
	{
		Left = 0,
		Up = 1,
		Right = 2,
		Down = 3,
	}
}