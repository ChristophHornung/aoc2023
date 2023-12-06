namespace Day6;

using Base;

internal class Program
{
	private static void Main(string[] args)
	{
		new Day6().Run(false);
		new Day6().Run(true);
	}
}

internal class Day6 : DayX
{
	public void Run(bool isPart2)
	{
		using StreamReader input = this.GetInput();
		string? line = input.ReadLine();
		long[] times;
		long[] distances;
		if (!isPart2)
		{
			times = new string(line.Where(c => char.IsNumber(c) || c == ' ').ToArray())
				.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
			line = input.ReadLine();
			distances = new string(line.Where(c => char.IsNumber(c) || c == ' ').ToArray())
				.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
		}
		else
		{
			times = new string(line.Where(char.IsNumber).ToArray())
				.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
			line = input.ReadLine();
			distances = new string(line.Where(char.IsNumber).ToArray())
				.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
		}

		long result = 1;
		for (int i = 0; i < times.Length; i++)
		{
			result *= this.GetWaysToWin(times[i], distances[i]);
		}

		this.ReportResult(result);
	}

	private long GetWaysToWin(long time, long distance)
	{
		long result = 0;
		for (int i = 1; i < time; i++)
		{
			if ((time - i) * i > distance)
			{
				result++;
			}
		}

		return result;
	}
}