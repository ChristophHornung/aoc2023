namespace Day9;

using Base;
using MathNet.Numerics;

internal class Program
{
	private static void Main(string[] args)
	{
		new Day9().Run(false);
	}

	internal class Day9 : DayX
	{
		public void Run(bool isPart2)
		{
			using StreamReader input = this.GetInput();
			string? line = input.ReadLine();

			long totalResult = 0;
			while (line != null)
			{
				long[] seeds = new string(line.Where(c => char.IsNumber(c) || c == ' ' || c == '-').ToArray())
					.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
				long result = seeds[^1];
				var i = 1;
				long diff = seeds[^i] - seeds[^(i + 1)];
				while (i < seeds.Length - 1)
				{
					Console.WriteLine(diff);
					result += diff;
					i++;
					diff = seeds[^1];
					int add = -1;
					for (var j = 1; j <= i; j++)
					{
						var leftTurnCombinations = (long)SpecialFunctions.Binomial(i, j);
						diff += add * leftTurnCombinations * seeds[^(j + 1)];
						add *= -1;
					}
				}

				totalResult += result;
				line = input.ReadLine();
			}

			this.ReportResult(totalResult);
		}
	}
}