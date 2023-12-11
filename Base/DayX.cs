namespace Base;

using System.Reflection;

public class DayX
{
	protected StreamReader GetInput()
	{
		var assembly = Assembly.GetEntryAssembly();

		int day = this.GetDay();

		Stream stream = assembly.GetManifestResourceStream($"Day{day}.input.txt")!;
		StreamReader reader = new StreamReader(stream);
		return reader;
	}

	protected void ReportResult(string result)
	{
		Console.WriteLine($"Day {this.GetDay()} {result}");
	}

	protected void ReportResult(int result)
	{
		this.ReportResult(result.ToString());
	}

	protected void ReportResult(long result)
	{
		this.ReportResult(result.ToString());
	}

	private int GetDay()
	{
		string typeName = this.GetType().Name;
		int day = int.Parse(typeName[(typeName.IndexOf('y') + 1)..]);
		return day;
	}

	protected void Print(List<List<char>> map)
	{
		foreach (List<char> chars in map)
		{
			Console.WriteLine(new string(chars.ToArray()));
		}
	}
}