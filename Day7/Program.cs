namespace Day7;

using Base;

internal class Program
{
	private static void Main(string[] args)
	{
		new Day7().Run(false);
		new Day7().Run(true);
	}

	internal class Day7 : DayX
	{
		public void Run(bool isPart2)
		{
			using StreamReader input = this.GetInput();
			string? line = input.ReadLine();
			List<(int[], int)> hands = new();
			while (line != null)
			{
				var hand = new int[5];
				for (var i = 0; i < 5; i++)
				{
					hand[i] = this.GetValue(line[i], isPart2);
				}

				int handBid = int.Parse(line[6..]);
				hands.Add((hand, handBid));
				line = input.ReadLine();
			}

			hands.Sort(new HandComparer());

			long result = hands.Select((h, i) => (i + 1) * h.Item2).Sum();
			this.ReportResult(result);
		}

		private int GetValue(char c, bool isPart2)
		{
			if (char.IsNumber(c))
			{
				return c - 48;
			}

			return c switch
			{
				'T' => 10,
				'J' => isPart2 ? 1 : 11,
				'Q' => 12,
				'K' => 13,
				'A' => 14,
				_ => throw new ArgumentOutOfRangeException()
			};
		}
	}
}

internal class HandComparer : IComparer<(int[], int)>
{
	public int Compare((int[], int) x, (int[], int) y)
	{
		int rankX = this.GetRankWithJ(x.Item1);
		int rankY = this.GetRankWithJ(y.Item1);
		if (rankX != rankY)
		{
			return rankX.CompareTo(rankY);
		}

		for (var i = 0; i < x.Item1.Length; i++)
		{
			if (x.Item1[i] != y.Item1[i])
			{
				return x.Item1[i].CompareTo(y.Item1[i]);
			}
		}

		return 0;
	}

	private int GetRankWithJ(int[] cards)
	{
		int joker = cards.Count(c => c == 1);
		if (joker is 0 or 5)
		{
			return this.GetRank(cards);
		}

		int maxCard = cards.Where(c => c != 1).GroupBy(c => c).MaxBy(c => c.Count())!.Key;
		int[] newCards = cards.Select(c => c == 1 ? maxCard : c).ToArray();
		return this.GetRank(newCards);
	}

	private int GetRank(int[] cards)
	{
		if (cards.Distinct().Count() == 1)
		{
			return 7;
		}

		if (cards.Distinct().Count() == 2)
		{
			if (cards.GroupBy(c => c).Any(g => g.Count() == 4))
			{
				return 6;
			}

			return 5;
		}

		if (cards.GroupBy(c => c).Any(g => g.Count() == 3))
		{
			return 4;
		}

		int pairs = cards.GroupBy(c => c).Count(g => g.Count() == 2);
		if (pairs == 2)
		{
			return 3;
		}

		if (pairs == 1)
		{
			return 2;
		}

		return 1;
	}
}