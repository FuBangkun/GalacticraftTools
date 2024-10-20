using System.Data;
using System.Text;

class Program
{
    static string ParentDir = System.Environment.CurrentDirectory;
    static readonly List<string> selectedOptions = [];
    static List<string[]> Options =
    [
        ["额外行星", "星空", "更多行星", "太阳系", "AsmodeusCore"],
        ["星系"],
        ["星空", "额外行星"],
        ["是", "否"],
    ];
    static readonly int[] selectedIndices = new int[8];
    static int Index;
    const string bold = "\x1b[1m";
    const string red = "\x1b[31m";
    const string green = "\x1b[32m";
    const string yellow = "\x1b[33m";
    const string reset = "\x1b[0m";

    static void Main()
    {
        Console.Title = "星系附属兼容问题解决工具";
        if (Path.GetFileNameWithoutExtension(ParentDir) != "config")
        {
            Console.WriteLine($"{bold}{red}程序不在config文件夹下\n请输入config文件夹所在路径{reset}");
            string? input = Console.ReadLine();
            while (input == null || Path.GetFileNameWithoutExtension(input) != "config")
            {
                Console.WriteLine($"{bold}{red}路径错误，请输入config文件夹的正确路径{reset}");
                input = Console.ReadLine();
            }
            ParentDir = input;
        }

        Console.CursorVisible = false;
        Console.Clear();
        Console.WriteLine($"欢迎使用星系附属兼容工具v1.0.2，本工具由付邦坤制作并发布于MC百科社群（https://bbs.mcmod.cn/thread-19017-1-1.html），旨在解决星系模组间的兼容问题以提升游戏体验。本工具完全免费，鼓励分享但严禁商业用途，误购请立即联系退款。目前支持星空、额外行星、更多行星、太阳系、AsmodeusCore等模组，其他模组基本兼容。需注意，太阳系模组可能导致游戏崩溃，本工具仅优化其星图配置。未来计划移除重复天体并增加对Minecraft 1.7.10的支持，敬请期待（临时修改可参考：https://www.mcmod.cn/post/2728.html）。如觉本工具有用，请在Github（https://github.com/FuBangkun/GalacticraftTools）给予Star支持，感谢！\n\n{green}按下任意键继续...{reset}");
        Console.ReadKey();
        SetIndexAndDrawMenu(1);

        while (true)
        {
            var key = Console.ReadKey(true);
            switch (Index)
            {
                case 1:
                    HandleArrowKeys();
                    switch (key.Key)
                    {
                        case ConsoleKey.Spacebar:
                            ToggleSelection(Options[0]);
                            break;
                        case ConsoleKey.Escape:
                            Console.Clear();
                            Console.WriteLine($"{bold}{red}按下回车键确认退出，按其他键返回...{reset}");
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
                                    string fullPath = Path.Combine(ParentDir + dirModifier, fileName);
                                    if (!File.Exists(fullPath))
                                    {
                                        Console.WriteLine($"未检测到{optionName}配置文件，请安装{optionName}并运行一次游戏以生成配置文件。");
                                        errorCount++;
                                    }
                                }
                                else
                                {
                                    options.Remove(optionName);
                                }
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
                            SetIndexAndDrawMenu(2);
                            break;
                    }
                    break;

                case 2:
                    HandleArrowKeys();
                    HandleEscapeKey();
                    if (key.Key == ConsoleKey.Enter)
                    {
                        if (selectedOptions.Contains("额外行星") && selectedOptions.Contains("星空"))
                        {
                            SetIndexAndDrawMenu(3);
                        }
                        else
                        {
                            if (selectedOptions.Contains("额外行星") && selectedOptions.Contains("AsmodeusCore")) SetIndexAndDrawMenu(7);
                            if (selectedOptions.Contains("星空")) SetIndexAndDrawMenu(5);
                        }
                    }
                    break;

                case 7:
                    HandleArrowKeys();
                    switch (key.Key)
                    {
                        case ConsoleKey.Escape:
                            SetIndexAndDrawMenu(selectedOptions.Contains("星空") ? 6 : 2);
                            break;
                        case ConsoleKey.Enter:
                            Console.Clear();
                            Modify();
                            break;
                    }
                    break;

                case 3:
                case 4:
                case 5:
                case 6:
                    HandleArrowKeys();
                    HandleKey();
                    break;
            }

            void HandleArrowKeys()
            {
                var options = GetOptions();
                if (key.Key == ConsoleKey.UpArrow)
                {
                    selectedIndices[Index] = (selectedIndices[Index] - 1 + options.Length) % options.Length;
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    selectedIndices[Index] = (selectedIndices[Index] + 1) % options.Length;
                }
            }

            void HandleEscapeKey()
            {
                if (key.Key == ConsoleKey.Escape)
                {
                    SetIndexAndDrawMenu(Index - 1);
                }
            }

            void HandleKey()
            {
                if (key.Key == ConsoleKey.Enter)
                {
                    SetIndexAndDrawMenu(Index + 1);
                }
                HandleEscapeKey();
            }

            DrawMenu(GetOptions(), Index);
        }
    }

    static void Modify()
    {
        Console.WriteLine($"{green}按下任意键确认更改，ESC键返回...{reset}");
        var key = Console.ReadKey(true);
        if (key.Key == ConsoleKey.Escape)
        {
            SetIndexAndDrawMenu(7);
        }
        else
        {
            Console.Clear();
            string[] lines;
            int error = 0;

            void ModifyBool(string configFilePath, string keyToFind, bool value)
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
                        Console.Write($"已修改{yellow}{keyToFind}{reset}为{red}{value.ToString().ToLower()}{reset}，修改前的值为{red}{currentValue}{reset}\n");
                        foundAndModified = true;
                        break;
                    }
                }
                if (!foundAndModified)
                {
                    Console.WriteLine($"{bold}{red}无法找到{keyToFind}{reset}");
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
                    else
                    {
                        sb.AppendLine(line);
                    }
                }
                if (!foundAndModified)
                {
                    Console.WriteLine($"{bold}{red}无法找到{keyToFind}{reset}");
                    error++;
                }
                File.WriteAllText(configFilePath, sb.ToString());
            }

            if (selectedOptions.Contains("额外行星")) ConfigureExtraPlanets();
            if (selectedOptions.Contains("AsmodeusCore")) ConfigureAsmodeusCore();
            if (selectedOptions.Contains("太阳系")) ConfigureSolarSystem();
            if (selectedOptions.Contains("星空")) ConfigureGalaxySpace();
            Console.WriteLine(error != 0 ? $"{bold}{red}{error}个错误{reset}" : $"{bold}{green}没有错误{reset}");

            void ConfigureExtraPlanets()
            {
                bool hasGalaxySpace = selectedOptions.Contains("星空");
                string extraPlanetsConfig = $"{ParentDir}\\ExtraPlanets.cfg";
                string galaxySpaceConfig = $"{ParentDir}\\GalaxySpace\\dimensions.conf";

                ModifyBool(extraPlanetsConfig, "B:\"Enable Galaxy Space Compatibility\"", hasGalaxySpace);

                if (hasGalaxySpace)
                {

                    ModifyBool(extraPlanetsConfig, "B:\"Mars SpaceStation\"", selectedIndices[3] != 0);
                    ModifyBool(galaxySpaceConfig, "B:enableMarsSpaceStation", selectedIndices[3] == 0);

                    ModifyBool(extraPlanetsConfig, "B:\"Venus SpaceStation\"", selectedIndices[4] != 0);
                    ModifyBool(galaxySpaceConfig, "B:enableVenusSpaceStation", selectedIndices[4] == 0);
                    List<string> galaxySpaceSuits =
                    [
                        "galaxyspace:space_suit_head:1",
                        "galaxyspace:space_suit_chest:1",
                        "galaxyspace:space_suit_legs:1",
                        "galaxyspace:space_suit_feet:1",
                        "galaxyspace:space_suit_light_head:1",
                        "galaxyspace:space_suit_light_chest:1",
                        "galaxyspace:space_suit_light_legs:1",
                        "galaxyspace:space_suit_light_feet:1"
                    ];
                    ModifyList($"{ParentDir}\\ExtraPlanets.cfg", "S:\"List of armour items to be considered as a space suit\" <", galaxySpaceSuits);

                    List<string> extraPlanetsSuits =
                    [
                        "extraplanets:tier1_space_suit_helmet",
                        "extraplanets:tier1_space_suit_chest",
                        "extraplanets:tier1_space_suit_jetpack_chest",
                        "extraplanets:tier1_space_suit_legings",
                        "extraplanets:tier1_space_suit_boots",
                        "extraplanets:tier1_space_suit_gravity_boots",
                        "extraplanets:tier2_space_suit_helmet",
                        "extraplanets:tier2_space_suit_chest",
                        "extraplanets:tier2_space_suit_jetpack_chest",
                        "extraplanets:tier2_space_suit_legings",
                        "extraplanets:tier2_space_suit_boots",
                        "extraplanets:tier2_space_suit_gravity_boots",
                        "extraplanets:tier3_space_suit_helmet",
                        "extraplanets:tier3_space_suit_chest",
                        "extraplanets:tier3_space_suit_jetpack_chest",
                        "extraplanets:tier3_space_suit_legings",
                        "extraplanets:tier3_space_suit_boots",
                        "extraplanets:tier3_space_suit_gravity_boots",
                        "extraplanets:tier4_space_suit_helmet",
                        "extraplanets:tier4_space_suit_chest",
                        "extraplanets:tier4_space_suit_jetpack_chest",
                        "extraplanets:tier4_space_suit_legings",
                        "extraplanets:tier4_space_suit_boots",
                        "extraplanets:tier4_space_suit_gravity_boots"
                    ];
                    ModifyList($"{ParentDir}\\GalaxySpace\\core.conf", "S:\"Radiation and Pressure Armor List\" <", extraPlanetsSuits);
                }
                else
                {
                    ModifyBool(extraPlanetsConfig, "B:\"Mars SpaceStation\"", true);
                    ModifyBool(extraPlanetsConfig, "B:\"Venus SpaceStation\"", true);
                }

                ModifyBool(extraPlanetsConfig, "B:\"Enable More Planets Compatibility\"", selectedOptions.Contains("更多行星"));
                ModifyBool(extraPlanetsConfig, "B:\"Use Custom Galaxy Map/Celestial Selection Screen\"", Options[1][selectedIndices[2]] == "额外行星");
            }

            void ConfigureAsmodeusCore()
            {
                string asmodeusCoreConfig = $"{ParentDir}\\AsmodeusCore\\core.conf";

                ModifyBool(asmodeusCoreConfig, "B:enableNewGalaxyMap", Options[1][selectedIndices[2]] == "AsmodeusCore（星空）");

                bool enableSkyFeatures = selectedIndices[7] != 0;
                ModifyBool(asmodeusCoreConfig, "B:enableSkyAsteroids", enableSkyFeatures);
                ModifyBool(asmodeusCoreConfig, "B:enableSkyMoon", enableSkyFeatures);
                ModifyBool(asmodeusCoreConfig, "B:enableSkyOverworld", enableSkyFeatures);
                ModifyBool(asmodeusCoreConfig, "B:enableSkyOverworldOrbit", enableSkyFeatures);
            }

            void ConfigureSolarSystem()
            {
                ModifyBool($"{ParentDir}\\Sol\\sol.conf", "B:\"Enable Custom Galaxymap?\"", Options[1][selectedIndices[2]] == "太阳系");
            }

            void ConfigureGalaxySpace()
            {
                string galaxySpaceConfig = $"{ParentDir}\\GalaxySpace\\core.conf";
                string galaxySpaceDimensionsConfig = $"{ParentDir}\\GalaxySpace\\dimensions.conf";

                if (!selectedOptions.Contains("额外行星"))
                {
                    ModifyBool(galaxySpaceDimensionsConfig, "B:enableMarsSpaceStation", true);
                    ModifyBool(galaxySpaceDimensionsConfig, "B:enableVenusSpaceStation", true);
                }

                ModifyBool(galaxySpaceConfig, "B:enableNewMenu", selectedIndices[5] == 0);
                ModifyBool(galaxySpaceConfig, "B:enableAdvancedRocketCraft", selectedIndices[6] == 0);
            }

            Console.WriteLine($"\n\n{bold}更多配置请手动修改配置文件，参考：https://www.mcmod.cn/post/2728.html的解决重复天体(1.8)和附属加载顺序(1.9)部分{reset}\n\n\n{green}按下任意键退出...{reset}");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }

    static string GetPrompt(int index)
    {
        string prompt = $"{reset}\nESC返回 ";
        return index switch
        {
            1 => $"{bold}选择安装的附属{reset}\nESC退出 空格选择/取消 ",
            2 => $"{bold}选择星图" + prompt,
            3 => $"{bold}选择火星空间站" + prompt,
            4 => $"{bold}选择金星空间站" + prompt,
            5 => $"{bold}是否启用星空新主菜单" + prompt,
            6 => $"{bold}是否启用星空2~6阶困难火箭配方" + prompt,
            7 => $"{bold}是否要安装光影（仅防止与光影存在冲突，不会下载光影）" + prompt,
            _ => $"{bold}加载错误{reset}",
        };
    }

    static string[] GetOptions()
    {
        return Index switch
        {
            1 => Options[0],
            2 => Options[1],
            3 => Options[2],
            4 => Options[2],
            5 => Options[3],
            6 => Options[3],
            7 => Options[3],
            _ => throw new ArgumentOutOfRangeException(nameof(Index), "无效的序号")
        };
    }
    static void SetIndexAndDrawMenu(int index)
    {
        Console.Clear();
        Index = index;
        DrawMenu(GetOptions(), index);
    }


    static void DrawMenu(string[] options, int index)
    {
        Console.Clear();
        Console.WriteLine(GetPrompt(Index) + "↑↓切换选项 回车确认\n");
        for (int i = 0; i < options.Length; i++)
        {
            string prefix = Index == 1 ? (selectedOptions.Contains(options[i]) ? "[X] " : "[ ] ") : (i == selectedIndices[index] ? "[X] " : "[ ] ");
            Console.WriteLine(i == selectedIndices[index] ? $"\x1b[44m{prefix}{options[i]}\x1b[0m" : prefix + options[i]);
        }
    }

    static void ToggleSelection(string[] options)
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
