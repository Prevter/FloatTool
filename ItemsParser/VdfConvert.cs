/*
- Copyright(C) 2023 Prevter
-
- This program is free software: you can redistribute it and/or modify
- it under the terms of the GNU General Public License as published by
- the Free Software Foundation, either version 3 of the License, or
- (at your option) any later version.
-
- This program is distributed in the hope that it will be useful,
- but WITHOUT ANY WARRANTY; without even the implied warranty of
- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
- GNU General Public License for more details.
-
- You should have received a copy of the GNU General Public License
- along with this program. If not, see <https://www.gnu.org/licenses/>.
*/
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace ItemsParser
{
	public static class VdfConvert
	{
		public static dynamic Parse(string text)
		{
			string[] lines = text.Split("\n");

			Dictionary<string, object> obj = new();
			Stack<Dictionary<string, object>> stack = new();
			stack.Push(obj);

			Regex regex = new(@"^(""((?:\\.|[^\\""])+)""|([a-z0-9\-_]+))([ \t]*(""((?:\\.|[^\\""])*)("")?|([a-z0-9\-_]+)))?");

			bool comment = false;
			bool expect_bracket = false;

			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i].Trim();

				if (line.StartsWith("/*") && line.EndsWith("*/"))
					continue;

				if (line.StartsWith("/*"))
				{
					comment = true;
					continue;
				}

				if (line.EndsWith("*/"))
				{
					comment = false;
					continue;
				}

				if (comment) continue;
				if (string.IsNullOrWhiteSpace(line) || line[0] == '/') continue;

				if (line[0] == '{')
				{
					expect_bracket = false;
					continue;
				}

				if (expect_bracket)
					throw new SyntaxError($"Invalid syntax on line {i + 1}");

				if (line[0] == '}')
				{
					if (stack.Count == 0)
						throw new SyntaxError($"Unexpected closing bracket on line {i + 1}");

					stack.Pop();
					continue;
				}

				while (true)
				{
					var match = regex.Match(line);

					if (!match.Success)
						throw new SyntaxError($"Invalid syntax on line {i + 1}");

					var key = match.Groups[2].Success ? match.Groups[2].Value : (match.Groups[3].Success ? match.Groups[3].Value : null);
					var val = match.Groups[6].Success ? match.Groups[6].Value : (match.Groups[8].Success ? match.Groups[8].Value : null);

					if (key is null)
						throw new SyntaxError($"Expected valid key name on line {i + 1}");

					if (val is null)
					{
						if (!stack.Peek().ContainsKey(key))
							stack.Peek()[key] = new Dictionary<string, object>();

						stack.Push((Dictionary<string, object>)stack.Peek()[key]);
						expect_bracket = true;
					}
					else
					{
						if (!match.Groups[7].Success && !match.Groups[8].Success)
						{
							line += '\n' + lines[++i];
							continue;
						}

						stack.Peek()[key] = val;
					}

					break;
				}

			}

			if (stack.Count != 1)
				throw new SyntaxError("Expected '}' at EOF");

			return obj;
		}
	}

	[Serializable]
	internal class SyntaxError : Exception
	{
		public SyntaxError()
		{
		}

		public SyntaxError(string? message) : base(message)
		{
		}

		public SyntaxError(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		protected SyntaxError(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
