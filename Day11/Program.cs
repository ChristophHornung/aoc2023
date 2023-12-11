namespace Day11;

using Base;
// 406726138763
internal class Program
{
	static void Main(string[] args)
	{
		new Day11().Run(1);
		new Day11().Run(1_000_000-1);
	}
}

internal class Day11 : DayX
{
	public void Run(int expansion)
	{
		StreamReader input = this.GetInput();
		string? line = input.ReadLine();
		List<List<char>> map = [];
		HashSet<int> expandingRows = [];
		HashSet<int> expandingColumns = [];
		int r = 0;
		while (line != null)
		{
			map.Add(line.ToList());
			if (!line.Contains('#'))
			{
				expandingRows.Add(r);
			}

			line = input.ReadLine();
			r++;
		}

		for (int i = 0; i < map[0].Count; i++)
		{
			if (map.All(l => l[i] == '.'))
			{
				expandingColumns.Add(i);
			}
		}

		char[][] starMap = map.Select(l => l.ToArray()).ToArray();
		List<(int x, int y)> galaxies = [];
		for (int y = 0; y < starMap.Length; y++)
		for (int x = 0; x < starMap[0].Length; x++)
		{
			if (starMap[y][x] == '#')
			{
				galaxies.Add((x, y));
			}
		}

		long distance = 0;
		for (int i = 0; i < galaxies.Count - 1; i++)
		for (int j = i + 1; j < galaxies.Count; j++)
		{
			distance += GetDistance(galaxies[i], galaxies[j], expandingRows, expandingColumns, expansion);
		}

		ReportResult(distance);
	}

	private long GetDistance((int x, int y) a, (int x, int y) b, HashSet<int> expandingRows,
		HashSet<int> expandingColumns, int expansion)
	{
		(int x1, int y1) = a;
		(int x2, int y2) = b;
		long unexpandedDistance = Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
		for (int x = Math.Min(x1, x2); x < Math.Max(x1, x2); x++)
		{
			if (expandingColumns.Contains(x))
			{
				unexpandedDistance += expansion;
			}
		}

		for (int y = y1; y < y2; y++)
		{
			if (expandingRows.Contains(y))
			{
				unexpandedDistance += expansion;
			}
		}

		return unexpandedDistance;
	}
}