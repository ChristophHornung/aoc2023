namespace Day5;

using Base;

internal class Program
{
	private static void Main(string[] args)
	{
		new Day5().Run(false);
	}
}

internal class Day5 : DayX
{
	public void Run(bool isPart2)
	{
		using StreamReader input = this.GetInput();
		string? line = input.ReadLine();
		long[] seeds = new string(line.Where(c => char.IsNumber(c) || c == ' ').ToArray())
			.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();

		line = input.ReadLine();
		line = input.ReadLine();
		line = input.ReadLine();
		List<List<Mapping>> maps = new();
		List<Mapping> current = new();
		while (line != null)
		{
			if (string.IsNullOrEmpty(line))
			{
				maps.Add(current);
				current = [];
				line = input.ReadLine();
			}
			else
			{
				long[] values = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
				current.Add(new Mapping(values[0], values[1], values[2]));
			}

			line = input.ReadLine();
		}

		maps.Add(current);
		this.ReportResult(this.CalculateClosest(seeds, maps));
		this.ReportResult(this.CalculateClosestPart2(seeds, maps));
	}

	private long CalculateClosest(long[] seeds, List<List<Mapping>> maps)
	{
		return seeds.Select(s => this.ToLocation(s, maps)).Min();
	}

	private long CalculateClosestPart2(long[] seeds, List<List<Mapping>> maps)
	{
		List<(long start, long length)> seedRanges = new();
		for (var i = 0; i < seeds.Length; i += 2)
		{
			seedRanges.Add((seeds[i], seeds[i + 1]));
		}

		maps.Reverse();
		maps = maps.Select(m => m.Select(mp => new Mapping(mp.Source, mp.Destination, mp.Length)).ToList()).ToList();
		long pos = 0;
		while (true)
		{
			long seed = this.ToLocation(pos, maps);
			if (seedRanges.Any(sr => sr.start <= seed && seed < sr.start + sr.length))
			{
				return pos;
			}

			pos++;
		}
	}

	private long ToLocation(long seed, List<List<Mapping>> maps)
	{
		long current = seed;
		foreach (List<Mapping> mappings in maps)
		foreach (Mapping mapping in mappings)
		{
			if (mapping.Source <= current && current < mapping.Source + mapping.Length)
			{
				current = mapping.Destination + (current - mapping.Source);
				break;
			}
		}

		return current;
	}
}

internal record struct Mapping(long Destination, long Source, long Length);