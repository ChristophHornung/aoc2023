namespace Day15;

using Base;

internal class Program
{
	private static void Main(string[] args)
	{
		new Day15().Run();
	}
}

internal class Day15 : DayX
{
	private List<Lens>[] boxes = new List<Lens>[256];

	public Day15()
	{
		for (int i = 0; i < this.boxes.Length; i++)
		{
			this.boxes[i] = new List<Lens>();
		}
	}

	public static int GetHash(string line)
	{
		int hash = 0;
		foreach (char c in line)
		{
			hash += c;
			hash *= 17;
			hash %= 256;
		}

		return hash;
	}

	public void Run()
	{
		StreamReader stream = this.GetInput();
		string line = stream.ReadLine()!;
		string[] split = line.Split(',');

		this.ReportResult(split.Select(Day15.GetHash).Sum());

		foreach (string s in split)
		{
			this.Process(s);
		}

		long focusingPower = 0;
		for (var index = 0; index < this.boxes.Length; index++)
		{
			List<Lens> lensList = this.boxes[index];
			for (var i = 0; i < lensList.Count; i++)
			{
				Lens lens = lensList[i];
				focusingPower += (i + 1) * (index + 1) * lens.Value;
			}
		}

		this.ReportResult(focusingPower);
	}

	public void Process(string s)
	{
		string[] split = s.Split(new[] { '=', '-' }, StringSplitOptions.RemoveEmptyEntries);
		int hash = Day15.GetHash(split[0]);
		Lens? lens = this.boxes[hash].FirstOrDefault(h => h.Label == split[0]);

		if (split.Length == 2)
		{
			int focalLength = int.Parse(split[1]);
			Lens newLens = new(split[0], focalLength);
			if (lens != null)
			{
				this.boxes[hash][this.boxes[hash].IndexOf(lens)] = newLens;
			}
			else
			{
				this.boxes[hash].Add(newLens);
			}
		}
		else
		{
			if (lens != null)
			{
				this.boxes[hash].Remove(lens);
			}
		}
	}

	private void WriteBoxes()
	{
		for (var index = 0; index < this.boxes.Length; index++)
		{
			List<Lens> lensList = this.boxes[index];
			if (lensList.Count > 0)
			{
				Console.WriteLine($"{index}: {string.Join(", ", lensList.Select(l => $"{l.Label}({l.Value})"))}");
			}
		}

		Console.WriteLine("----------");
	}
}

internal record class Lens(string Label, int Value);