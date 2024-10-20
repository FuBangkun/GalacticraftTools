using Newtonsoft.Json.Linq;
using System.Data;
using System.IO.Compression;
using System.Text;

class Program
{
    static string ConfigPath = System.Environment.CurrentDirectory;
    static readonly List<string> selectedOptions = [];
    static readonly List<string[]> Options =
    [
        ["额外行星", "星空", "更多行星", "太阳系", "AsmodeusCore"],
        ["星系"],
        ["星空", "额外行星"],
        ["是", "否"],
    ];
    static readonly int[] selectedIndices = new int[8];
    static int Index;
    const string bold = "\x1b[1m";
    const string red = $"{bold}\x1b[31m";
    const string green = $"{bold}\x1b[32m";
    const string yellow = $"{bold}\x1b[33m";
    const string reset = "\x1b[0m";

    static void Main()
    {
        Console.Title = "星系附属兼容问题解决工具";
        if (Path.GetFileNameWithoutExtension(ConfigPath) != "config")
        {
            Console.WriteLine($"{red}程序不在config文件夹下\n请输入config文件夹所在路径{reset}");
            string? input = Console.ReadLine();
            while (input == null || Path.GetFileNameWithoutExtension(input) != "config")
            {
                Console.WriteLine($"{red}路径错误，请输入config文件夹的正确路径{reset}");
                input = Console.ReadLine();
            }
            ConfigPath = input;
        }

        Console.CursorVisible = false;
        Console.Clear();
        Console.WriteLine($"欢迎使用星系附属兼容工具v1.0.2，本工具由付邦坤制作并发布于MC百科社群（https://bbs.mcmod.cn/thread-19017-1-1.html），旨在解决星系模组间的兼容问题以提升游戏体验。本工具完全免费，鼓励分享但严禁商业用途，误购请立即联系退款。目前支持星空、额外行星、更多行星、太阳系、AsmodeusCore等模组，其他模组基本兼容。需注意，太阳系模组可能导致游戏崩溃，本工具仅优化其星图配置。未来计划移除重复天体并增加对Minecraft 1.7.10的支持，敬请期待（临时修改可参考：https://www.mcmod.cn/post/2728.html）。如觉本工具有用，请在Github（https://github.com/FuBangkun/GalacticraftTools）给予Star支持，感谢！\n\n{green}按下任意键继续...{reset}");
        Console.ReadKey();
        UpdateMenuIndex(1);

        while (true)
        {
            var key = Console.ReadKey(true);
            switch (Index)
            {
                case 3:
                case 4:
                case 5:
                case 6:
                    NavigateOptions();
                    ProcessKeyInput();
                    break;

                case 1:
                    NavigateOptions();
                    switch (key.Key)
                    {
                        case ConsoleKey.Spacebar:
                            UpdateSelection(Options[0]);
                            break;
                        case ConsoleKey.Escape:
                            Console.Clear();
                            Console.WriteLine($"{red}按下回车键确认退出，按其他键返回...{reset}");
                            if (Console.ReadKey(true).Key == ConsoleKey.Enter) Environment.Exit(0);
                            break;
                        case ConsoleKey.Enter:
                            Console.Clear();
                            if (selectedOptions.Count < 2)
                            {
                                Console.WriteLine((selectedOptions.Count == 0 ? $"{red}你没有选择任何模组" : $"{red}请选择至少两个模组") + $"\n\n{green}按下任意键返回...{reset}");
                                continue;
                            }
                            HashSet<string> options = [.. Options[1]];
                            int errorCount = 0;

                            void CheckOptionAndFile(string optionName, string fileName, bool addToOptions = true, string dirModifier = "")
                            {
                                if (selectedOptions.Contains(optionName))
                                {
                                    if (addToOptions) options.Add(optionName == "AsmodeusCore" ? "AsmodeusCore（星空）" : optionName);
                                    string fullPath = Path.Combine(ConfigPath + dirModifier, fileName);
                                    if (!File.Exists(fullPath))
                                    {
                                        Console.WriteLine($"未检测到{optionName}配置文件，请安装{optionName}并运行一次游戏以生成配置文件。");
                                        errorCount++;
                                    }
                                }
                                else options.Remove(optionName);
                            }
                            CheckOptionAndFile("额外行星", "ExtraPlanets.cfg");
                            CheckOptionAndFile("星空", "core.conf", false, "\\GalaxySpace");
                            CheckOptionAndFile("更多行星", "moreplanets.cfg", false);
                            CheckOptionAndFile("太阳系", "sol.conf", true, "\\sol");
                            CheckOptionAndFile("AsmodeusCore", "core.conf", true, "\\AsmodeusCore");
                            Options[1] = [.. options];
                            if (errorCount > 0)
                            {
                                Console.WriteLine($"{errorCount}个错误\n{green}按下任意键退出...{reset}");
                                Console.ReadKey();
                                return;
                            }
                            UpdateMenuIndex(2);
                            break;
                    }
                    break;

                case 2:
                    NavigateOptions();
                    if (key.Key == ConsoleKey.Escape) UpdateMenuIndex(Index - 1);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        if (selectedOptions.Contains("额外行星") && selectedOptions.Contains("星空")) UpdateMenuIndex(3);
                        else
                        {
                            if (selectedOptions.Contains("额外行星") && selectedOptions.Contains("AsmodeusCore")) UpdateMenuIndex(7);
                            if (selectedOptions.Contains("星空")) UpdateMenuIndex(5);
                        }
                    }
                    break;

                case 7:
                    NavigateOptions();
                    switch (key.Key)
                    {
                        case ConsoleKey.Escape:
                            UpdateMenuIndex(selectedOptions.Contains("星空") ? 6 : 2);
                            break;
                        case ConsoleKey.Enter:
                            Console.Clear();
                            ApplyChanges();
                            break;
                    }
                    break;
            }

            void NavigateOptions()
            {
                var options = GetOptions();
                if (key.Key == ConsoleKey.UpArrow) selectedIndices[Index] = (selectedIndices[Index] - 1 + options.Length) % options.Length;
                else if (key.Key == ConsoleKey.DownArrow) selectedIndices[Index] = (selectedIndices[Index] + 1) % options.Length;
            }

            void ProcessKeyInput()
            {
                if (key.Key == ConsoleKey.Enter) UpdateMenuIndex(Index + 1);
                if (key.Key == ConsoleKey.Escape) UpdateMenuIndex(Index - 1);
            }

            RenderMenu(GetOptions(), Index);
        }
    }

    static void ApplyChanges()
    {
        Console.WriteLine($"{green}按下任意键确认更改，ESC键返回...{reset}");
        var key = Console.ReadKey(true);
        if (key.Key == ConsoleKey.Escape) UpdateMenuIndex(7);
        else
        {
            Console.Clear();
            string[] lines;
            int error = 0;

            void ModifyBool(string configFilePath, string keyToFind, bool value, bool defaultValue)
            {
                lines = File.ReadAllLines(configFilePath);
                bool foundAndModified = false;
                for (int i = 0; i < lines.Length; i++)
                {
                    string trimmedLine = lines[i].TrimStart();
                    if (trimmedLine.StartsWith(keyToFind + "="))
                    {
                        string currentValue = trimmedLine[(keyToFind + "=").Length..].Trim();
                        lines[i] = "    " + keyToFind + $"={value.ToString().ToLower()}";
                        Console.Write($"已修改{yellow}{keyToFind}{reset}为{red}{value.ToString().ToLower()}{reset}，修改前为{red}{currentValue}{reset}，默认为{red}{defaultValue.ToString().ToLower()}{reset}\n");
                        foundAndModified = true;
                        break;
                    }
                }
                if (!foundAndModified)
                {
                    Console.WriteLine($"{red}无法找到{keyToFind}{reset}");
                    error++;
                }
                File.WriteAllLines(configFilePath, lines);
            }

            void ModifyList(string configFilePath, string keyToFind, List<string> itemsToAdd)
            {
                string[] lines = File.ReadAllLines(configFilePath);
                StringBuilder sb = new();
                bool inSection = false;
                bool foundAndModified = false;
                List<string> existingItems = [];

                foreach (string line in lines)
                {
                    if (line.Trim() == keyToFind)
                    {
                        inSection = true;
                        sb.AppendLine(line);
                        continue;
                    }
                    if (inSection && line.Trim() == ">")
                    {
                        inSection = false;
                        foreach (var item in itemsToAdd.Where(item => !existingItems.Contains(item))) sb.AppendLine($"        {item}");
                        sb.AppendLine(line);
                        foundAndModified = true;
                        continue;
                    }
                    if (inSection)
                    {
                        sb.AppendLine(line);
                        if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("        ")) continue;
                        string item = line.TrimStart(' ', '\t').Trim();
                        if (!string.IsNullOrWhiteSpace(item) && !item.StartsWith("//")) existingItems.Add(item);
                    }
                    else sb.AppendLine(line);
                }
                if (!foundAndModified)
                {
                    Console.WriteLine($"{red}无法找到{keyToFind[..^2]}{reset}");
                    error++;
                }
                else Console.WriteLine($"已修改{yellow}{keyToFind[..^2]}{reset}");
                File.WriteAllText(configFilePath, sb.ToString());
            }

            if (selectedOptions.Contains("额外行星")) ConfigureExtraPlanets();
            if (selectedOptions.Contains("AsmodeusCore")) ConfigureAsmodeusCore();
            if (selectedOptions.Contains("太阳系")) ConfigureSolarSystem();
            if (selectedOptions.Contains("星空")) ConfigureGalaxySpace();
            Console.WriteLine(error != 0 ? $"{red}{error}个错误{reset}" : $"{green}没有错误{reset}");

            void ConfigureExtraPlanets()
            {
                bool hasGalaxySpace = selectedOptions.Contains("星空");
                string extraPlanetsConfig = $"{ConfigPath}\\ExtraPlanets.cfg";
                string galaxySpaceConfig = $"{ConfigPath}\\GalaxySpace\\dimensions.conf";

                ModifyBool(extraPlanetsConfig, "B:\"Enable Galaxy Space Compatibility\"", hasGalaxySpace, false);

                if (hasGalaxySpace)
                {
                    ModifyBool(extraPlanetsConfig, "B:\"Mars SpaceStation\"", selectedIndices[3] != 0, true);
                    ModifyBool(galaxySpaceConfig, "B:enableMarsSpaceStation", selectedIndices[3] == 0, true);

                    ModifyBool(extraPlanetsConfig, "B:\"Venus SpaceStation\"", selectedIndices[4] != 0, true);
                    ModifyBool(galaxySpaceConfig, "B:enableVenusSpaceStation", selectedIndices[4] == 0, true);
                    var front = "galaxyspace:";
                    var back = "space_suit_";
                    var galaxySpaceSuits = new List<string> {
                        $"{front}{back}head:1", $"{front}{back}chest:1", $"{front}{back}legs:1", $"{front}{back}feet:1",
                        $"{front}{back}light_head:1", $"{front}{back}light_chest:1", $"{front}{back}light_legs:1", $"{front}{back}light_feet:1"
                    };
                    front = "extraplanets:tier";
                    var extraPlanetsSuits = new List<string>();
                    for (int tier = 1; tier <= 4; tier++)
                    {
                        extraPlanetsSuits.AddRange([
                            $"{front}{tier}_{back}helmet", $"{front}{tier}_{back}chest", $"{front}{tier}_{back}jetpack_chest",
                            $"{front}{tier}_{back}legings", $"{front}{tier}_{back}boots", $"{front}{tier}_{back}gravity_boots"
                        ]);
                    }
                    ModifyList($"{ConfigPath}\\ExtraPlanets.cfg", "S:\"List of armour items to be considered as a space suit\" <", galaxySpaceSuits);
                    ModifyList($"{ConfigPath}\\GalaxySpace\\core.conf", "S:\"Radiation and Pressure Armor List\" <", extraPlanetsSuits);
                }
                else
                {
                    ModifyBool(extraPlanetsConfig, "B:\"Mars SpaceStation\"", true, true);
                    ModifyBool(extraPlanetsConfig, "B:\"Venus SpaceStation\"", true, true);
                }
                ModifyBool(extraPlanetsConfig, "B:\"Enable More Planets Compatibility\"", selectedOptions.Contains("更多行星"), false);
                ModifyBool(extraPlanetsConfig, "B:\"Use Custom Galaxy Map/Celestial Selection Screen\"", Options[1][selectedIndices[2]] == "额外行星", true);
            }

            void ConfigureAsmodeusCore()
            {
                string asmodeusCoreConfig = $"{ConfigPath}\\AsmodeusCore\\core.conf";

                ModifyBool(asmodeusCoreConfig, "B:enableNewGalaxyMap", Options[1][selectedIndices[2]] == "AsmodeusCore（星空）", true);

                bool enableSkyFeatures = selectedIndices[7] != 0;
                ModifyBool(asmodeusCoreConfig, "B:enableSkyAsteroids", enableSkyFeatures, true);
                ModifyBool(asmodeusCoreConfig, "B:enableSkyMoon", enableSkyFeatures, true);
                ModifyBool(asmodeusCoreConfig, "B:enableSkyOverworld", enableSkyFeatures, true);
                ModifyBool(asmodeusCoreConfig, "B:enableSkyOverworldOrbit", enableSkyFeatures, true);
            }

            void ConfigureSolarSystem()
            {
                ModifyBool($"{ConfigPath}\\Sol\\sol.conf", "B:\"Enable Custom Galaxymap?\"", Options[1][selectedIndices[2]] == "太阳系", true);
            }

            void ConfigureGalaxySpace()
            {
                string galaxySpaceConfig = $"{ConfigPath}\\GalaxySpace\\core.conf";
                string galaxySpaceDimensionsConfig = $"{ConfigPath}\\GalaxySpace\\dimensions.conf";

                if (!selectedOptions.Contains("额外行星"))
                {
                    ModifyBool(galaxySpaceDimensionsConfig, "B:enableMarsSpaceStation", true, true);
                    ModifyBool(galaxySpaceDimensionsConfig, "B:enableVenusSpaceStation", true, true);
                }

                ModifyBool(galaxySpaceConfig, "B:enableNewMenu", selectedIndices[5] == 0, true);
                ModifyBool(galaxySpaceConfig, "B:enableAdvancedRocketCraft", selectedIndices[6] == 0, true);
            }
            bool planetprogression = false;
            bool galacticresearch = false;
            foreach (string jarFilePath in Directory.GetFiles(ConfigPath.Replace("\\config", "\\mods"), "*.jar"))
            {
                using ZipArchive archive = ZipFile.OpenRead(jarFilePath);
                ZipArchiveEntry? mcmodInfoEntry = archive.Entries.FirstOrDefault(e => e.FullName.EndsWith("mcmod.info", StringComparison.OrdinalIgnoreCase));
                if (mcmodInfoEntry != null)
                {
                    using StreamReader reader = new(mcmodInfoEntry.Open());
                    string jsonContent = reader.ReadToEnd();
                    JArray modArray = JArray.Parse(jsonContent);
                    if (modArray.Count > 0 && modArray[0] is JObject modObject)
                    {
                        string? modId = modObject["modid"]?.ToString();
                        switch (modId)
                        {
                            case "planetprogression":
                                planetprogression = true;
                                break;
                            case "galacticresearch":
                                galacticresearch = true;
                                break;
                        }
                    }
                }
            }
            if (planetprogression && galacticresearch) Console.WriteLine($"\n{red}检测到你安装了PlanetProgression和GalacticResearch，这两个模组都会让你的星图只有一个星球，请删除一个。其他的星球需要你自行研究，如不想研究请全部删除{reset}");
            else if (planetprogression || galacticresearch)
            {
                string modName = planetprogression ? "PlanetProgression" : "GalacticResearch";
                Console.WriteLine($"\n{red}检测到你安装了{modName}，这个模组会让你的星图只有一个星球。其他的星球需要你自行研究，如不想研究请删除{reset}");
            }
            Console.WriteLine($"\n\n{bold}更多配置请手动修改配置文件，参考：https://www.mcmod.cn/post/2728.html的解决重复天体(1.8)和附属加载顺序(1.9)部分{reset}\n\n\n{green}按下任意键退出...{reset}");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }

    static string GetPrompt(int index)
    {
        string specificPrompt = index switch
        {
            1 => "选择安装的附属",
            2 => "选择星图",
            3 => "选择火星空间站",
            4 => "选择金星空间站",
            5 => "是否启用星空新主菜单",
            6 => "是否启用星空2~6阶困难火箭配方",
            7 => "是否要安装光影（仅防止与光影存在冲突，不会下载光影）",
            _ => $"{red}加载失败"
        };
        return $"{bold}{index}." + (index == 1 ? specificPrompt + $"{reset}\nESC退出 空格选择/取消 " : specificPrompt + $"{reset}\nESC返回 ") + "↑↓切换选项 回车确认\n";
    }

    static string[] GetOptions()
    {
        return Index switch
        {
            1 => Options[0],
            2 => Options[1],
            3 or 4 => Options[2],
            5 or 6 or 7 => Options[3],
            _ => [$"{red}加载失败"]
        };
    }

    static void UpdateMenuIndex(int index)
    {
        Console.Clear();
        Index = index;
        RenderMenu(GetOptions(), index);
    }

    static void RenderMenu(string[] options, int index)
    {
        Console.Clear();
        Console.WriteLine(GetPrompt(Index));
        for (int i = 0; i < options.Length; i++)
        {
            string prefix = (Index == 1 && selectedOptions.Contains(options[i])) || (Index != 1 && i == selectedIndices[index]) ? "[X] " : "[ ] ";
            Console.WriteLine(i == selectedIndices[index] ? $"\x1b[44m{prefix}{options[i]}{reset}" : prefix + options[i]);
        }
    }

    static void UpdateSelection(string[] options)
    {
        if (selectedOptions.Contains(options[selectedIndices[1]]))
        {
            selectedOptions.Remove(options[selectedIndices[1]]);
            if (options[selectedIndices[1]] == "AsmodeusCore") selectedOptions.Remove("星空");
        }
        else
        {
            selectedOptions.Add(options[selectedIndices[1]]);
            if (options[selectedIndices[1]] == "星空") selectedOptions.Add("AsmodeusCore");
        }
    }
}
