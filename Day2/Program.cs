namespace Day2;

using Base;

internal class Program
{
	private static void Main(string[] args)
	{
		new Day2().Run(false);
	}
}

internal class Day2 : DayX
{
	public void Run(bool isPart2)
	{
		using StreamReader input = this.GetInput();
		string? line = input.ReadLine();
		List<Game> games = [];
		while (line != null)
		{
			games.Add(this.ParseLine(line));
			line = input.ReadLine();
		}


		this.ReportResult(games.Where(this.IsGamePossible).Sum(r => r.Id));
		this.ReportResult(games.Sum(this.GetMinimalPower));
	}

	private bool IsGamePossible(Game g)
	{
		return g.Reveals.All(r => r is {Red: <= 12, Green: <= 13, Blue: <= 14});
	}

	private long GetMinimalPower(Game g)
	{
		return g.Reveals.Max(r => r.Blue) * g.Reveals.Max(r => r.Green) * g.Reveals.Max(r => r.Red);
	}

	private Game ParseLine(string line)
	{
		string[] gameSplit = line.Split(':');
		int id = int.Parse(gameSplit[0][5..]);
		string[] revealsSplit = gameSplit[1].Split(';');
		List<Reveal> reveals = [];
		foreach (string revealString in revealsSplit)
		{
			int red = 0;
			int green = 0;
			int blue = 0;
			string[] revealSplit = revealString.Split(',', StringSplitOptions.TrimEntries);
			foreach (string item in revealSplit)
			{
				string[] itemSplit = item.Split(' ');
				int count = int.Parse(itemSplit[0]);
				string color = itemSplit[1];
				switch (color)
				{
					case "red":
						red = count;
						break;
					case "green":
						green = count;
						break;
					case "blue":
						blue = count;
						break;
				}
			}

			reveals.Add(new Reveal(red, green, blue));
		}

		return new Game(id, reveals);
	}
}

internal record struct Game(int Id, List<Reveal> Reveals);

internal record struct Reveal(int Red, int Green, int Blue);