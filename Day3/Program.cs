namespace Day3;

using Base;

internal class Program
{
	private static void Main(string[] args)
	{
		new Day3().Run(false);
		new Day3().Run(true);
	}
}

internal class Day3 : DayX
{
	public void Run(bool isPart2)
	{
		if (isPart2)
		{
			this.RunPart2();
		}
		else
		{
			this.RunPart1();
		}
	}

	private void RunPart2()
	{
		using StreamReader input = this.GetInput();
		string? line = input.ReadLine();
		List<char[]> map = [];
		while (line != null)
		{
			map.Add(line.ToCharArray());
			line = input.ReadLine();
		}

		List<long> gears = new();
		for (int y = 0; y < map.Count; y++)
		for (int x = 0; x < map[y].Length; x++)
		{
			if (map[y][x] == '*')
			{
				gears.Add(this.GetGear(y, x, map));
			}
		}

		this.ReportResult(gears.Sum());
	}

	private long GetGear(int y, int x, List<char[]> map)
	{
		List<long> gears = [];

		List<(int x, int y)> checks =
		[
			(x, y - 1), (x, y + 1), (x - 1, y), (x + 1, y)
		];

		if (!this.IsNumber(y - 1, x, map))
		{
			// Have to check diagonals
			checks.Add((x - 1, y - 1));
			checks.Add((x + 1, y - 1));
		}

		if (!this.IsNumber(y + 1, x, map))
		{
			// Have to check diagonals
			checks.Add((x - 1, y + 1));
			checks.Add((x + 1, y + 1));
		}

		foreach ((int xCheck, int yCheck) in checks)
		{
			if (this.IsNumber(yCheck, xCheck, map))
			{
				gears.Add(this.GetNumber(yCheck, xCheck, map));
			}
		}

		if (gears.Count == 2)
		{
			return gears[0] * gears[1];
		}

		return 0;
	}

	private long GetNumber(int yCheck, int xCheck, List<char[]> map)
	{
		string number = "" + map[yCheck][xCheck];
		int x = xCheck - 1;
		while (this.IsNumber(yCheck, x, map))
		{
			number = map[yCheck][x] + number;
			x--;
		}

		x = xCheck + 1;
		while (this.IsNumber(yCheck, x, map))
		{
			number += map[yCheck][x];
			x++;
		}

		return int.Parse(number);
	}

	private void RunPart1()
	{
		using StreamReader input = this.GetInput();
		string? line = input.ReadLine();
		List<char[]> map = [];
		while (line != null)
		{
			map.Add(line.ToCharArray());
			line = input.ReadLine();
		}

		string currentNumber = string.Empty;
		List<int> partNumber = [];
		for (int y = 0; y < map.Count; y++)
		for (int x = 0; x < map[y].Length; x++)
		{
			if (char.IsNumber(map[y][x]))
			{
				currentNumber += map[y][x];
				if (x == map[y].Length - 1 || !char.IsNumber(map[y][x + 1]))
				{
					if (this.CheckIsPartNumber(currentNumber.Length, x, y, map))
					{
						partNumber.Add(int.Parse(currentNumber));
					}

					currentNumber = string.Empty;
				}
			}
		}

		this.ReportResult(partNumber.Sum());
	}

	private bool CheckIsPartNumber(int currentNumberLength, int x, int y, List<char[]> map)
	{
		// Left
		if (this.IsSymbol(y, x - currentNumberLength, map))
		{
			return true;
		}

		// Right
		if (this.IsSymbol(y, x + 1, map))
		{
			return true;
		}

		// Top and Bottom
		for (int i = -currentNumberLength; i <= 1; i++)
		{
			if (this.IsSymbol(y - 1, x + i, map) || this.IsSymbol(y + 1, x + i, map))
			{
				return true;
			}
		}

		return false;
	}

	private bool IsSymbol(int y, int x, List<char[]> map)
	{
		if (y < 0 || y > map.Count - 1)
		{
			return false;
		}

		if (x < 0 || x > map[y].Length - 1)
		{
			return false;
		}

		return map[y][x] != '.';
	}

	private bool IsNumber(int y, int x, List<char[]> map)
	{
		if (y < 0 || y > map.Count - 1)
		{
			return false;
		}

		if (x < 0 || x > map[y].Length - 1)
		{
			return false;
		}

		return char.IsNumber(map[y][x]);
	}
}