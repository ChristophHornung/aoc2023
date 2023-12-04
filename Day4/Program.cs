namespace Day4;

using Base;

internal class Program
{
	private static void Main(string[] args)
	{
		new Day4().Run(false);
	}

	internal class Day4 : DayX
	{
		//private const int split = 6;

		const int split = 11;
		public void Run(bool isPart2)
		{
			using StreamReader input = this.GetInput();
			string? line = input.ReadLine();
			long result = 0;
			long result2 = 0;
			Dictionary<int, int> wonExtra = new();
			while (line != null)
			{
				string clean = new string(line.Where(c => char.IsNumber(c) || c == ' ').ToArray());
				List<int> numbers = clean.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
				int id = numbers[0];
				result += this.GetWinnings(numbers[1..Day4.split], numbers[Day4.split..]);
				int winningsCount = this.GetWinningsCount(numbers[1..Day4.split], numbers[Day4.split..]);
				wonExtra.TryGetValue(id, out int extraCards);
				int ownedCards = 1 + extraCards;
				result2 += ownedCards;
				for (int i = 0; i < winningsCount; i++)
				{
					if (wonExtra.ContainsKey(id + i + 1))
					{
						wonExtra[id + i + 1] += ownedCards;
					}
					else
					{
						wonExtra[id + i + 1] = ownedCards;
					}
				}

				line = input.ReadLine();
			}

			this.ReportResult(result);
			this.ReportResult(result2);
		}

		private long GetWinnings(List<int> winningNumbers, List<int> myNumbers)
		{
			return (long)Math.Floor(Math.Pow(2, myNumbers.Where(winningNumbers.Contains).Count() - 1));
		}

		private int GetWinningsCount(List<int> winningNumbers, List<int> myNumbers)
		{
			return myNumbers.Where(winningNumbers.Contains).Count();
		}
	}
}