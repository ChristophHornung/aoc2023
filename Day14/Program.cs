namespace Day14;

using Base;

internal class Program
{
	private static void Main(string[] args)
	{
		//new Day14(false).Run();
		new Day14(true).Run();
	}
}

internal class Day14 : DayX
{
	private readonly bool isPart2;

	public Day14(bool isPart2)
	{
		this.isPart2 = isPart2;
	}

	private static void ShiftNorth(Map map)
	{
		int maxY = map.maxY;
		int maxX = map.maxX;
		for (int yI = 0; yI <= maxY; yI++)
		for (int xI = 0; xI <= maxX; xI++)
		{
			if (map[(xI, yI)] == Element.Rock)
			{
				for (int newY = yI - 1; newY >= -1; newY--)
				{
					if (newY == -1 || map[(xI, newY)] != Element.Empty)
					{
						map[(xI, yI)] = Element.Empty;
						map[(xI, newY + 1)] = Element.Rock;
						break;
					}
				}
			}
		}
	}

	private static void ShiftSouth(Map map)
	{
		int maxY = map.maxY;
		int maxX = map.maxX;
		for (int yI = maxY; yI >= 0; yI--)
		for (int xI = maxX; xI >= 0; xI--)
		{
			if (map[(xI, yI)] == Element.Rock)
			{
				for (int newY = yI + 1; newY <= maxY + 1; newY++)
				{
					if (newY == maxY + 1 || map[(xI, newY)] != Element.Empty)
					{
						map[(xI, yI)] = Element.Empty;
						map[(xI, newY - 1)] = Element.Rock;
						break;
					}
				}
			}
		}
	}

	private static void ShiftEast(Map map)
	{
		int maxY = map.maxY;
		int maxX = map.maxX;
		for (int xI = maxX; xI >= 0; xI--)
		for (int yI = maxY; yI >= 0; yI--)
		{
			if (map[(xI, yI)] == Element.Rock)
			{
				for (int newX = xI + 1; newX <= maxX + 1; newX++)
				{
					if (newX == maxX + 1 || map[(newX, yI)] != Element.Empty)
					{
						map[(xI, yI)] = Element.Empty;
						map[(newX - 1, yI)] = Element.Rock;
						break;
					}
				}
			}
		}
	}

	private static void ShiftWest(Map map)
	{
		int maxY = map.maxY;
		int maxX = map.maxX;
		for (int xI = 0; xI <= maxX; xI++)
		for (int yI = 0; yI <= maxY; yI++)
		{
			if (map[(xI, yI)] == Element.Rock)
			{
				for (int newX = xI - 1; newX >= -1; newX--)
				{
					if (newX == -1 || map[(newX, yI)] != Element.Empty)
					{
						map[(xI, yI)] = Element.Empty;
						map[(newX + 1, yI)] = Element.Rock;
						break;
					}
				}
			}
		}
	}

	private static long GetLoad(Map map)
	{
		int maxY = map.maxY;
		int maxX = map.maxX;
		long load = 0;
		for (int yI = 0; yI <= maxY; yI++)
		for (int xI = 0; xI <= maxX; xI++)
		{
			if (map[(xI, yI)] == Element.Rock)
			{
				load += maxY - yI + 1;
			}
		}

		return load;
	}

	public void Run()
	{
		Dictionary<(int x, int y), Element> map = new();
		StreamReader stream = this.GetInput();
		string? line = stream.ReadLine();
		int maxX = line.Length - 1;
		int y = 0;
		while (line != null)
		{
			for (var x = 0; x < line.Length; x++)
			{
				char c = line[x];
				map[(x, y)] = c switch
				{
					'.' => Element.Empty,
					'#' => Element.Cube,
					'O' => Element.Rock,
					_ => throw new Exception("Unknown element")
				};
			}

			line = stream.ReadLine();
			y++;
		}

		int maxY = y - 1;
		Dictionary<Map, Map> cache = new();
		Map currentMap = new(map, maxX, maxY);
		if (!this.isPart2)
		{
			Day14.ShiftNorth(currentMap);
		}
		else
		{
			for (int i = 0; i < 1_000_000_000; i++)
			{
				if (cache.TryGetValue(currentMap, out Map? value))
				{
					currentMap = value;
				}
				else
				{
					Map prior = currentMap.Clone();
					Day14.ShiftNorth(currentMap);
					Day14.ShiftWest(currentMap);
					Day14.ShiftSouth(currentMap);
					Day14.ShiftEast(currentMap);
					cache[prior] = currentMap.Clone();
					if (cache.ContainsKey(currentMap))
					{
						// Found a loop
						var start = currentMap.Clone();
						int loopSize = 0;
						bool loopFinished = false;
						while (!loopFinished)
						{
							currentMap = cache[currentMap];
							loopSize++;
							if (currentMap.Equals(start))
							{
								loopFinished = true;
							}
						}

						int remaining = (1_000_000_000 - i - 1) % loopSize;
						i = 1_000_000_000 - remaining - 1;
					}
				}
			}
		}

		this.ReportResult(Day14.GetLoad(currentMap));
	}

	private void Output(Dictionary<(int x, int y), Element> map)
	{
		for (int yI = 0; yI <= map.Keys.Max(v => v.y); yI++)
		{
			for (int xI = 0; xI <= map.Keys.Max(v => v.x); xI++)
			{
				Console.Write(map[(xI, yI)] switch
				{
					Element.Empty => '.',
					Element.Cube => '#',
					Element.Rock => 'O',
					_ => throw new Exception("Unknown element")
				});
			}

			Console.WriteLine();
		}

		Console.WriteLine();
	}

	private class Map
	{
		public Dictionary<(int x, int y), Element> map;
		public readonly int maxX;
		public readonly int maxY;

		public Map(Dictionary<(int x, int y), Element> map, int maxX, int maxY)
		{
			this.map = map;
			this.maxX = maxX;
			this.maxY = maxY;
		}

		public Element this[(int xI, int yI) valueTuple]
		{
			get => this.map[valueTuple];
			set => this.map[valueTuple] = value;
		}

		public Map Clone()
		{
			return new Map(this.map.ToDictionary(v => v.Key, v => v.Value), this.maxX, this.maxY);
		}

		public override bool Equals(object? obj)
		{
			if (obj is not Map other)
			{
				return false;
			}

			for (int yI = 0; yI <= this.maxY; yI++)
			for (int xI = 0; xI <= this.maxX; xI++)
			{
				if (this[(xI, yI)] != other[(xI, yI)])
				{
					return false;
				}
			}

			return true;
		}

		public override int GetHashCode()
		{
			int hash = 0;
			for (int yI = 0; yI <= this.maxY; yI++)
			for (int xI = 0; xI <= this.maxX; xI++)
			{
				hash = HashCode.Combine(hash, this[(xI, yI)] switch
				{
					Element.Empty => 0,
					Element.Cube => 1,
					Element.Rock => 2,
					_ => throw new Exception("Unknown element")
				});
			}

			return hash;
		}
	}
}

internal enum Element
{
	Empty,
	Cube,
	Rock
}