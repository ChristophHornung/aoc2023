namespace Day8;

using Base;

internal class Program
{
	private static void Main(string[] args)
	{
		//new Day8().Run(false);
		new Day8().Run(true);
	}

	internal class Day8 : DayX
	{
		public void Run(bool isPart2)
		{
			using StreamReader input = this.GetInput();
			string? line = input.ReadLine();
			List<Directions> directions = line.Select(c => c == 'R' ? Directions.Right : Directions.Left).ToList();
			line = input.ReadLine();
			line = input.ReadLine();
			Dictionary<string, Node> nodes = new();
			while (line != null)
			{
				string code = line[..3];
				string left = line[7..10];
				nodes.TryGetValue(left, out Node? leftNode);
				leftNode ??= new Node(left, null, null);
				nodes[left] = leftNode;
				string right = line[12..15];
				nodes.TryGetValue(right, out Node? rightNode);
				rightNode ??= new Node(right, null, null);
				nodes[right] = rightNode;
				nodes.TryGetValue(code, out Node? node);
				node ??= new Node(code, leftNode, rightNode);
				node.Left = leftNode;
				node.Right = rightNode;
				nodes[code] = node;
				line = input.ReadLine();
			}

			if (!isPart2)
			{
				Node currentNode = nodes["AAA"];
				long count = 0;
				foreach (Directions direction in this.RepeatSequence(directions))
				{
					count++;
					currentNode = direction switch
					{
						Directions.Left => currentNode.Left!,
						Directions.Right => currentNode.Right!,
						_ => throw new ArgumentOutOfRangeException()
					};
					if (currentNode.Code == "ZZZ")
					{
						break;
					}
				}

				this.ReportResult(count);
			}
			else
			{
				List<Node> starts = nodes.Values.Where(c => c.Code.EndsWith('A')).ToList();

				List<(long loopStart, long loopLength, List<long> loopZs)> loops = [];
				foreach (Node start in starts)
				{
					Dictionary<(Node node, int dirInxed), int> visited = [];
					visited.Add((start, 0), 0);
					var directionIndex = 0;
					var steps = 0;
					List<long> loopZs = new();
					var looped = false;
					Node currentNode = start;
					while (!looped)
					{
						directionIndex %= directions.Count;
						Directions direction = directions[directionIndex];
						currentNode = direction switch
						{
							Directions.Left => currentNode.Left!,
							Directions.Right => currentNode.Right!,
							_ => throw new ArgumentOutOfRangeException()
						};
						steps++;
						(Node currentNode, int directionIndex) possibleLoopEnd = (currentNode, directionIndex);
						directionIndex++;
						if (visited.ContainsKey(possibleLoopEnd))
						{
							looped = true;
							long loopStart = visited[possibleLoopEnd];
							long loopLength = steps - loopStart;
							var stepsInLoop = 0;
							while (true)
							{
								directionIndex %= directions.Count;
								direction = directions[directionIndex];
								currentNode = direction switch
								{
									Directions.Left => currentNode.Left!,
									Directions.Right => currentNode.Right!,
									_ => throw new ArgumentOutOfRangeException()
								};
								stepsInLoop++;
								if (possibleLoopEnd == (currentNode, directionIndex))
								{
									loops.Add((loopStart, loopLength, loopZs));
									break;
								}

								if (currentNode.Code.EndsWith('Z'))
								{
									loopZs.Add(stepsInLoop);
								}

								directionIndex++;
							}
						}
						else
						{
							visited.Add(possibleLoopEnd, steps);
						}
					}
				}

				long[] pos = loops.Select(l => l.loopStart + l.loopZs.First()).ToArray();
				long[] loopLengths = loops.Select(l => l.loopLength).ToArray();
				var identical = false;
				while (!identical)
				{
					(identical, int minIndex) = this.GetMinIndex(pos, loopLengths);
					if (!identical)
					{
						pos[minIndex] += loops[minIndex].loopLength;
					}
				}

				this.ReportResult(pos.Distinct().Single());
			}
		}

		private (bool identical, int minIndex) GetMinIndex(long[] pos, long[] loopLengths)
		{
			long min = pos[0];
			var minIndex = 0;
			var identical = 1;
			for (var i = 1; i < pos.Length; i++)
			{
				if (pos[i] < min)
				{
					minIndex = i;
				}
				else if (pos[i] == min)
				{
					if (loopLengths[i] < loopLengths[minIndex])
					{
						minIndex = i;
					}

					identical++;
				}
			}

			return (identical == pos.Length, minIndex);
		}

		private IEnumerable<Directions> RepeatSequence(List<Directions> directions)
		{
			for (var index = 0; index < directions.Count; index++)
			{
				Directions direction = directions[index];
				yield return direction;
				if (index == directions.Count - 1)
				{
					index = -1;
				}
			}
		}
	}

	internal class Node
	{
		public Node(string code, Node? left, Node? right)
		{
			this.Code = code;
			this.Left = left;
			this.Right = right;
		}

		public string Code { get; }
		public Node? Left { get; set; }
		public Node? Right { get; set; }

		public override string ToString()
		{
			return $"{this.Code} ({this.Left?.Code} {this.Right?.Code})";
		}
	}

	internal enum Directions
	{
		Left,
		Right
	}
}