﻿/*
- Copyright(C) 2022 Prevter
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

using DiscordRPC;
using FloatTool.Common;
using System.Collections.Generic;
using System.IO;

namespace FloatTool
{
	internal static class AppHelpers
    {
        public static FileSystemWatcher Watcher;
        public static DiscordRpcClient DiscordClient;
        public static string VersionCode;
        public static string AppDirectory;
        public static Settings Settings;
        public static List<string> ThemesFound { get; set; }
    }
}