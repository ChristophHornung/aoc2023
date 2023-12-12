namespace Day12;

using Base;

internal class Program
{
	static void Main(string[] args)
	{
		new Day12().Run(false);
		new Day12().Run(true);
	}
}

internal class Day12 : DayX
{
	public void Run(bool isPart2)
	{
		StreamReader stream = this.GetInput();
		string? line = stream.ReadLine();
		long result = 0;
		while (line != null)
		{
			var lineParts = line.Split(' ');
			List<int> groups = lineParts[1].Split(',').Select(int.Parse).ToList();
			string springs = lineParts[0];
			if (isPart2)
			{
				springs = springs + "?" + springs + "?" + springs + "?" + springs + "?" + springs;
				groups = groups.Concat(groups).Concat(groups).Concat(groups).Concat(groups).ToList();
			}

			//result += GetArrangements(springs, groups);
			result += GetArrangements2(springs, groups, 0);
			Console.Write("+");
			line = stream.ReadLine();
		}

		ReportResult(result);
	}

	private long GetArrangements(string line, List<int> groups)
	{
		int unknown = line.IndexOf('?');
		if (unknown == -1)
		{
			return IsValid(line, groups) ? 1 : 0;
		}

		string line1 = line[..unknown] + '.' + line[(unknown + 1)..];
		string line2 = line[..unknown] + '#' + line[(unknown + 1)..];
		long cLine1 = IsValid(line1, groups) ? GetArrangements(line1, groups) : 0;
		long cLine2 = IsValid(line2, groups) ? GetArrangements(line2, groups) : 0;
		return cLine1 + cLine2;
	}

	private long GetArrangements2(string line, List<int> groups, int groupIndex)
	{
		if (line.Length == 0)
		{
			return groupIndex == groups.Count ? 1 : 0;
		}

		long arrangements = 0;

		if (line[0] is '#' or '?')
		{
			//Start a new group
			if (groupIndex < groups.Count)
			{
				int expectedGroup = groups[groupIndex];
				bool canStart = true;
				for (int i = 0; i < expectedGroup; i++)
				{
					if (i >= line.Length || line[i] == '.')
					{
						canStart = false;
						break;
					}
				}

				bool endsLine = expectedGroup >= line.Length;
				if (!endsLine && line[expectedGroup] == '#')
				{
					canStart = false;
				}

				if (canStart)
				{
					int cut = endsLine ? expectedGroup : expectedGroup + 1;
					arrangements += GetArrangements2(line[cut..], groups, groupIndex + 1);
				}
			}
		}
		if (line[0] == '.' || line[0] == '?')
		{
			arrangements += GetArrangements2(line[1..], groups, groupIndex);
		}

		return arrangements;
	}

	private bool IsValid(string line, List<int> groups)
	{
		int unknown = line.IndexOf('?');
		if (unknown != -1)
		{
			// We also check for partially valid lines.
			string[] split = line[..unknown].Split('.', StringSplitOptions.RemoveEmptyEntries);
			if (split.Length == 0)
			{
				return true;
			}

			if (split.Length > groups.Count)
			{
				return false;
			}

			for (int i = 0; i < split.Length - 1; i++)
			{
				if (split[i].Length != groups[i])
				{
					return false;
				}
			}

			if (split[^1].Length > groups[split.Length - 1])
			{
				return false;
			}
		}
		else
		{
			string[] split = line.Split('.', StringSplitOptions.RemoveEmptyEntries);
			if (split.Length != groups.Count)
			{
				return false;
			}

			for (int i = 0; i < split.Length; i++)
			{
				if (split[i].Length != groups[i])
				{
					return false;
				}
			}
		}

		return true;
	}
}