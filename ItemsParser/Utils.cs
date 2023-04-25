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
static class Utils
{
	static readonly HttpClient Client = new();

	public static async Task DownloadFile(string url, string filename)
	{
		using var s = await Client.GetStreamAsync(url);
		using var fs = new FileStream(filename, FileMode.OpenOrCreate);
		await s.CopyToAsync(fs);
	}

	public static string ReplaceInvalidChars(string filename)
	{
		return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
	}
}
