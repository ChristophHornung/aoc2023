namespace Day1
{
	using Base;

	internal class Program
	{
		private static void Main(string[] args)
		{
			new Day1().Run(false);
			new Day1().Run(true);
		}
	}

	internal class Day1 : DayX
	{
		public void Run(bool partTwo)
		{
			using StreamReader input = this.GetInput();
			int result = 0;
			string? line = input.ReadLine();
			while (line != null)
			{
				List<int> numbers = this.ReplaceNumbers(line, partTwo).ToList();
				int lineResult = numbers.First() * 10;
				lineResult += numbers.Last();
				Console.WriteLine($"{line} = {lineResult}");
				result += lineResult;

				line = input.ReadLine();
			}

			this.ReportResult(result);
		}

		private IEnumerable<int> ReplaceNumbers(IEnumerable<char> line, bool partTwo)
		{
			string[] numbers =
			[
				"one",
				"two",
				"three",
				"four",
				"five",
				"six",
				"seven",
				"eight",
				"nine"
			];

			int[] matches = new int[numbers.Length];

			void ResetMatches()
			{
				for (int i = 0; i < matches.Length; i++)
				{
					matches[i] = 0;
				}
			}

			foreach (char c in line)
			{
				if (char.IsNumber(c))
				{
					ResetMatches();
					yield return c - 48;
					continue;
				}

				if (partTwo)
				{
					for (int index = 0; index < matches.Length; index++)
					{
						if (numbers[index][matches[index]] == c)
						{
							matches[index]++;
							if (matches[index] == numbers[index].Length)
							{
								matches[index] = 0;
								yield return index + 1;
							}
						}
						else
						{
							matches[index] = numbers[index][0] == c ? 1 : 0;
						}
					}
				}
			}
		}
	}
}