namespace Day13;

using System.Collections;
using System.Diagnostics;
using Base;

internal class Program
{
	private static void Main(string[] args)
	{
		//new Day13(false).Run();
		new Day13(true).Run();
	}
}

public class Day13 : DayX
{
	private readonly bool isPart2;

	public Day13(bool isPart2)
	{
		this.isPart2 = isPart2;
	}

	private static void Write(BitArray bitArray)
	{
		foreach (bool b in bitArray)
		{
			Console.Write(b ? '#' : '.');
		}

		Console.WriteLine();
	}


	public void Run()
	{
		StreamReader stream = this.GetInput();
		string? line = stream.ReadLine();
		List<string> pattern = [];
		long result = 0;
		while (line != null)
		{
			if (string.IsNullOrEmpty(line))
			{
				result += this.Reflection(pattern);
				pattern = [];
			}
			else
			{
				pattern.Add(line);
			}

			line = stream.ReadLine();
		}

		result += this.Reflection(pattern);

		this.ReportResult(result);
	}

	private long Reflection(List<string> pattern)
	{
		List<int> rowsAsInt = pattern.Select(p => new BitArray(p.Select(c => c != '#').ToArray()))
			.Select(this.GetIntFromBitArray).ToList();

		// also convert the columns to int
		List<int> columnsAsInt = [];
		for (int i = 0; i < pattern[0].Length; i++)
		{
			BitArray column = new BitArray(pattern.Count);
			for (int j = 0; j < pattern.Count; j++)
			{
				column[j] = pattern[j][i] != '#';
			}

			columnsAsInt.Add(this.GetIntFromBitArray(column));
		}

		long rowReflection = this.Reflection(rowsAsInt);
		long columnReflection = this.Reflection(columnsAsInt);

		if (this.isPart2)
		{
			rowReflection = this.ReflectionWithSmudge(rowsAsInt, rowReflection);
			columnReflection = this.ReflectionWithSmudge(columnsAsInt, columnReflection);
			if (columnReflection != 0 && rowReflection != 0)
			{
				Debugger.Break();
			}
		}

		return 100 * rowReflection + columnReflection;
	}

	private long ReflectionWithSmudge(List<int> pattern, long originalReflection)
	{
		for (int i = 0; i < pattern.Count; i++)
		{
			int old = pattern[i];
			var bits = new BitArray(new[] { old });
			for (var index = 0; index < bits.Count; index++)
			{
				bits[index] = !bits[index];
				pattern[i] = this.GetIntFromBitArray(bits);
				bits[index] = !bits[index];

				long reflectionNew = this.Reflection(pattern, originalReflection);

				if (reflectionNew != 0)
				{
					return reflectionNew;
				}
			}

			pattern[i] = old;
		}

		return 0;
	}

	private long Reflection(List<int> pattern, long? originalReflection = null)
	{
		for (int i = 0; i < pattern.Count - 1; i++)
		{
			bool isReflection = true;
			int j = i + 1;
			while (isReflection)
			{
				int mirrorIndex = 2 * i - j + 1;
				if (mirrorIndex < 0 || j >= pattern.Count)
				{
					if (originalReflection != i + 1)
					{
						return i + 1;
					}
					else
					{
						isReflection = false;
					}
				}
				else if (pattern[mirrorIndex] != pattern[j])
				{
					isReflection = false;
				}

				j++;
			}
		}

		return 0;
	}

	private int GetIntFromBitArray(BitArray bitArray)
	{
		if (bitArray.Length > 32)
		{
			throw new ArgumentException("Argument length shall be at most 32 bits.");
		}

		int[] array = new int[1];
		bitArray.CopyTo(array, 0);
		return array[0];
	}
}