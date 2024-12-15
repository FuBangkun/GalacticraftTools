using Newtonsoft.Json.Linq;
using System.Data;
using System.IO.Compression;
using System.Text;

class Program
{
    static string ConfigPath = System.Environment.CurrentDirectory;
    static readonly HashSet<string> selectedOptions1 = [];
    static readonly HashSet<string> selectedOptions2 = [];
    static readonly List<string[]> Options =
    [
        ["1.12.2", "1.7.10"],
        ["额外行星", "星空", "更多行星", "太阳系", "AsmodeusCore"],
        ["星系"],
        ["星空", "额外行星"],
        ["是", "否"],
        ["额外行星", "星空", "更多行星", "扩展行星", "Amun-Ra"],
        []
    ];
    static readonly int[] selectedIndices = Enumerable.Repeat(-1, 10).ToArray();
    static readonly int[] selectedIndicesSet = new int[10];
    static int Index;
    static int Number = 1;

    static void Main()
    {
        Console.Title = "星系附属兼容问题解决工具";
        if (Path.GetFileNameWithoutExtension(ConfigPath) != "config")
        {
            WriteLine("程序不在config文件夹下\r\n请输入config文件夹所在路径", ConsoleColor.Red);
            string? input = Console.ReadLine();
            while (input == null || Path.GetFileNameWithoutExtension(input) != "config")
            {
                WriteLine("路径错误，请输入config文件夹的正确路径", ConsoleColor.Red);
                input = Console.ReadLine();
            }
            ConfigPath = input;
        }

        Console.CursorVisible = false;
        Console.Clear();
        Console.WriteLine($"本工具已暂停更新，请移步至星系兼容（https://www.mcmod.cn/class/17145.html）以获得更好的体验。\r\n");
        WriteLine("按下任意键继续...", ConsoleColor.Green);
        Console.ReadKey();
        UpdateMenuIndex(0, 0);

        while (true)
        {
            var key = Console.ReadKey(true);
            switch (Index)
            {
                case 3:
                case 4:
                case 6:
                    NavigateOptions();
                    ProcessKeyInput();
                    break;

                case 0:
                    NavigateOptions();
                    switch (key.Key)
                    {
                        case ConsoleKey.Spacebar:
                            selectedIndices[Index] = selectedIndicesSet[Index];
                            break;
                        case ConsoleKey.Escape:
                            Console.Clear();
                            WriteLine("按下回车键确认退出，按其他键返回...", ConsoleColor.Red);
                            if (Console.ReadKey(true).Key == ConsoleKey.Enter) Environment.Exit(0);
                            break;
                        case ConsoleKey.Enter:
                            if (selectedIndices[Index] > -1) UpdateMenuIndex(selectedIndices[0] == 0 ? 1 : 8, 1);
                            break;
                    }
                    break;

                case 1:
                    NavigateOptions();
                    if (key.Key == ConsoleKey.Escape) UpdateMenuIndex(Index - 1, -1);
                    switch (key.Key)
                    {
                        case ConsoleKey.Spacebar:
                            UpdateSelection(Options[1]);
                            break;
                        case ConsoleKey.Enter:
                            Console.Clear();
                            if (selectedOptions1.Count < 2)
                            {
                                WriteLine(selectedOptions1.Count == 0 ? "你没有选择任何模组" : "请选择至少两个模组", ConsoleColor.Red);
                                WriteLine("\r\n按下任意键返回...", ConsoleColor.Green);
                                continue;
                            }
                            HashSet<string> options = [.. Options[2]];
                            int errorCount = 0;

                            void CheckFile(string optionName, string fileName, bool addToOptions = true, string dirModifier = "")
                            {
                                if (selectedOptions1.Contains(optionName))
                                {
                                    if (addToOptions) options.Add(optionName == "AsmodeusCore" ? "AsmodeusCore（星空）" : optionName);
                                    if (!File.Exists(Path.Combine(ConfigPath + dirModifier, fileName)))
                                    {
                                        WriteLine($"未检测到{optionName}配置文件，请安装{optionName}并运行一次游戏以生成配置文件。", ConsoleColor.Red);
                                        errorCount++;
                                    }
                                }
                                else options.Remove(optionName);
                            }
                            CheckFile("额外行星", "ExtraPlanets.cfg");
                            CheckFile("星空", "core.conf", false, "\\GalaxySpace");
                            CheckFile("更多行星", "moreplanets.cfg", false);
                            CheckFile("太阳系", "sol.conf", true, "\\sol");
                            CheckFile("AsmodeusCore", "core.conf", true, "\\AsmodeusCore");
                            Options[2] = [.. options];
                            Array.Sort(Options[2]);
                            if (errorCount > 0)
                            {
                                WriteLine($"{errorCount}个错误", ConsoleColor.Red);
                                WriteLine("按下任意键退出...", ConsoleColor.Red);
                                Console.ReadKey();
                                return;
                            }
                            UpdateMenuIndex(2, 1);
                            break;
                    }
                    break;

                case 2:
                    NavigateOptions();
                    switch (key.Key)
                    {
                        case ConsoleKey.Spacebar:
                            selectedIndices[Index] = selectedIndicesSet[Index];
                            break;
                        case ConsoleKey.Escape:
                            UpdateMenuIndex(Index - 1, -1);
                            break;
                        case ConsoleKey.Enter:
                            if (selectedIndices[Index] > -1)
                            {
                                if (selectedOptions1.Contains("额外行星") && selectedOptions1.Contains("星空")) UpdateMenuIndex(3, 1);
                                else
                                {
                                    if (selectedOptions1.Contains("额外行星") && selectedOptions1.Contains("AsmodeusCore")) UpdateMenuIndex(7, 1);
                                    if (selectedOptions1.Contains("星空")) UpdateMenuIndex(5, 1);
                                }
                            }
                            break;
                    }
                    break;

                case 5:
                    NavigateOptions();
                    switch (key.Key)
                    {
                        case ConsoleKey.Spacebar:
                            selectedIndices[Index] = selectedIndicesSet[Index];
                            break;
                        case ConsoleKey.Escape:
                            UpdateMenuIndex(selectedIndices[0] == 0 ? 4 : 9, -1);
                            break;
                        case ConsoleKey.Enter:
                            if (selectedIndices[Index] > -1) UpdateMenuIndex(Index + 1, 1);
                            break;
                    }
                    break;

                case 7:
                    NavigateOptions();
                    switch (key.Key)
                    {
                        case ConsoleKey.Spacebar:
                            selectedIndices[Index] = selectedIndicesSet[Index];
                            break;
                        case ConsoleKey.Escape:
                            UpdateMenuIndex(selectedIndices[0] == 0 ? (selectedOptions1.Contains("星空") ? 6 : 2) : 6, -1);
                            break;
                        case ConsoleKey.Enter:
                            if (selectedIndices[Index] > -1)
                            {
                                Console.Clear();
                                ApplyChanges();
                            }
                            break;
                    }
                    break;

                case 8:
                    NavigateOptions();
                    if (key.Key == ConsoleKey.Escape) UpdateMenuIndex(0, -1);
                    switch (key.Key)
                    {
                        case ConsoleKey.Spacebar:
                            UpdateSelection(Options[5]);
                            break;
                        case ConsoleKey.Enter:
                            Console.Clear();
                            if (selectedOptions2.Count < 2)
                            {
                                WriteLine(selectedOptions1.Count == 0 ? "你没有选择任何模组" : "请选择至少两个模组", ConsoleColor.Red);
                                WriteLine("\r\n按下任意键返回...", ConsoleColor.Green);
                                continue;
                            }
                            HashSet<string> options = [.. Options[6]];
                            int errorCount = 0;

                            void CheckFile(string optionName, string fileName, string dirModifier = "")
                            {
                                if (!File.Exists(Path.Combine(ConfigPath + dirModifier, fileName)))
                                {
                                    Console.WriteLine($"未检测到{optionName}配置文件，请安装{optionName}并运行一次游戏以生成配置文件。");
                                    errorCount++;
                                }
                            }
                            CheckFile("额外行星", "ExtraPlanets.cfg");
                            CheckFile("星空", "core.conf", "\\GalaxySpace");
                            CheckFile("更多行星", "MorePlanets.cfg");
                            if (selectedOptions2.Contains("额外行星") && !Options[6].Contains("额外行星")) options.Add("额外行星");
                            if (selectedOptions2.Contains("星空") && !Options[6].Contains("星空")) options.Add("星空");
                            if (selectedOptions2.Contains("更多行星") && !Options[6].Contains("更多行星")) options.Add("更多行星");
                            Options[6] = [.. options];
                            Array.Sort(Options[6]);
                            if (errorCount > 0)
                            {
                                Write($"{errorCount}个错误");
                                WriteLine("按下任意键退出...", ConsoleColor.Green);
                                Console.ReadKey();
                                return;
                            }
                            UpdateMenuIndex(9, 1);
                            break;
                    }
                    break;

                case 9:
                    NavigateOptions();
                    switch (key.Key)
                    {
                        case ConsoleKey.Spacebar:
                            selectedIndices[Index] = selectedIndicesSet[Index];
                            break;
                        case ConsoleKey.Escape:
                            UpdateMenuIndex(Index - 1, -1);
                            Options[6] = [];
                            break;
                        case ConsoleKey.Enter:
                            if (selectedIndices[Index] > -1) UpdateMenuIndex(selectedOptions2.Contains("星空") ? 5 : 7, 1);
                            break;
                    }
                    break;
            }

            void NavigateOptions()
            {
                var options = GetOptions();
                if (key.Key == ConsoleKey.UpArrow) selectedIndicesSet[Index] = (selectedIndicesSet[Index] - 1 + options.Length) % options.Length;
                else if (key.Key == ConsoleKey.DownArrow) selectedIndicesSet[Index] = (selectedIndicesSet[Index] + 1) % options.Length;
            }

            void ProcessKeyInput()
            {
                switch (key.Key)
                {
                    case ConsoleKey.Spacebar:
                        selectedIndices[Index] = selectedIndicesSet[Index];
                        break;
                    case ConsoleKey.Enter:
                        if (selectedIndices[Index] > -1) UpdateMenuIndex(Index + 1, 1);
                        break;
                    case ConsoleKey.Escape:
                        UpdateMenuIndex(Index - 1, -1);
                        break;
                }
            }

            RenderMenu(GetOptions(), Index);
        }
    }

    static void ApplyChanges()
    {
        WriteLine($"按下任意键确认更改，ESC键返回...", ConsoleColor.Green);
        var key = Console.ReadKey(true);
        if (key.Key == ConsoleKey.Escape) UpdateMenuIndex(7, 0);
        else
        {
            Console.Clear();
            string[] lines;
            int error = 0;

            string modsFloder = ConfigPath.Replace("\\config", "\\mods");
            bool planetprogression = false;
            bool galacticresearch = false;
            bool thaumcraft = false;
            bool beyondspace = false;
            if (File.Exists(modsFloder))
            {
                foreach (string jarFilePath in Directory.GetFiles(modsFloder, "*.jar"))
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
                                case "Thaumcraft":
                                    thaumcraft = true;
                                    break;
                                case "beyondspace":
                                    beyondspace = true;
                                    break;
                            }
                        }
                    }
                }
            }

            void Modify(string configFilePath, string keyToFind, string value, string defaultValue)
            {
                lines = File.ReadAllLines(configFilePath);
                bool foundAndModified = false;
                for (int i = 0; i < lines.Length; i++)
                {
                    string trimmedLine = lines[i].TrimStart();
                    if (trimmedLine.StartsWith(keyToFind + "="))
                    {
                        string currentValue = trimmedLine[(keyToFind + "=").Length..].Trim();
                        lines[i] = "    " + keyToFind + $"={value}";
                        Write("已修改");
                        Write(keyToFind, ConsoleColor.Yellow);
                        Write("为");
                        Write(value, ConsoleColor.Red);
                        Write("，修改前为");
                        Write(currentValue, ConsoleColor.Red);
                        Write("，默认为");
                        WriteLine(defaultValue.ToString().ToLower(), ConsoleColor.Red);
                        foundAndModified = true;
                        break;
                    }
                }
                if (!foundAndModified)
                {
                    WriteLine($"无法找到{keyToFind}", ConsoleColor.Red);
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
                    WriteLine($"无法找到{keyToFind[..^2]}", ConsoleColor.Red);
                    error++;
                }
                else
                {
                    Write("已修改");
                    WriteLine(keyToFind[..^2], ConsoleColor.Yellow);
                }
                File.WriteAllText(configFilePath, sb.ToString());
            }

            if (selectedIndices[0] == 0)
            {
                if (selectedOptions1.Contains("额外行星")) ConfigureExtraPlanets1();
                if (selectedOptions1.Contains("AsmodeusCore")) ConfigureAsmodeusCore();
                if (selectedOptions1.Contains("太阳系")) ConfigureSolarSystem();
                if (selectedOptions1.Contains("星空")) ConfigureGalaxySpace1();
            }
            else
            {
                if (selectedOptions2.Contains("额外行星")) ConfigureExtraPlanets2();
                if (selectedOptions2.Contains("星空")) ConfigureGalaxySpace2();
                if (selectedOptions2.Contains("更多行星")) ConfigureMorePlanets();
            }
            if (error != 0) WriteLine($"{error}个错误", ConsoleColor.Red);
            else WriteLine($"没有错误", ConsoleColor.Green);

            void ConfigureExtraPlanets1()
            {
                string hasGS = selectedOptions1.Contains("星空").ToString().ToLower();
                string Config = $"{ConfigPath}\\ExtraPlanets.cfg";
                Modify(Config, "B:\"Enable Galaxy Space Compatibility\"", hasGS, "false");

                if (selectedOptions1.Contains("星空"))
                {
                    Modify(Config, "B:\"Mars SpaceStation\"", (selectedIndices[3] != 0).ToString().ToLower(), "true");
                    Modify($"{ConfigPath}\\GalaxySpace\\dimensions.conf", "B:enableMarsSpaceStation", (selectedIndices[3] == 0).ToString().ToLower(), "true");

                    Modify(Config, "B:\"Venus SpaceStation\"", (selectedIndices[4] != 0).ToString().ToLower(), "true");
                    Modify($"{ConfigPath}\\GalaxySpace\\dimensions.conf", "B:enableVenusSpaceStation", (selectedIndices[4] == 0).ToString().ToLower(), "true");
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
                    Modify(Config, "B:\"Mars SpaceStation\"", "true", "true");
                    Modify(Config, "B:\"Venus SpaceStation\"", "true", "true");
                }
                Modify(Config, "B:\"Enable More Planets Compatibility\"", selectedOptions1.Contains("更多行星").ToString().ToLower(), "false");
                Modify(Config, "B:\"Use Custom Galaxy Map/Celestial Selection Screen\"", (Options[2][selectedIndices[2]] == "额外行星").ToString().ToLower(), "true");
            }

            void ConfigureExtraPlanets2()
            {
                string Config = $"{ConfigPath}\\ExtraPlanets.cfg";
                Modify(Config, "B:\"Enable New Galaxy Space Compatibility (v1.2.3 and above)\"", selectedOptions2.Contains("星空").ToString().ToLower(), "false");
                Modify(Config, "B:\"Enable New More Planets Compatibility\"", selectedOptions2.Contains("更多行星").ToString().ToLower(), "false");
                Modify(Config, "B:\"Enable AmunRa Compatibility\"", selectedOptions2.Contains("Amun-Ra").ToString().ToLower(), "false");
                Modify(Config, "B:\"Enable New Extended Planets Compatibility (v1.4 Alpha)\"", selectedOptions2.Contains("扩展行星").ToString().ToLower(), "false");
                Modify(Config, "B:\"Enable New Extended Planets Compatibility (v1.3.1 Alpha)\"", selectedOptions2.Contains("扩展行星").ToString().ToLower(), "false");
                if (thaumcraft) Modify(Config, "I:\"Titania Dimension ID\"", "-44", "-42");
                if (selectedOptions2.Contains("星空") || selectedOptions2.Contains("更多行星")) Modify(Config, "B:\"Mars SpaceStation\"", (selectedIndices[9] == 0).ToString().ToLower(), "true");
            }

            void ConfigureAsmodeusCore()
            {
                string Config = $"{ConfigPath}\\AsmodeusCore\\core.conf";

                Modify(Config, "B:enableNewGalaxyMap", (Options[2][selectedIndices[2]] == "AsmodeusCore（星空）").ToString().ToLower(), "true");

                string enableSkyFeatures = (selectedIndices[7] != 0).ToString().ToLower();
                Modify(Config, "B:enableSkyAsteroids", enableSkyFeatures, "true");
                Modify(Config, "B:enableSkyMoon", enableSkyFeatures, "true");
                Modify(Config, "B:enableSkyOverworld", enableSkyFeatures, "true");
                Modify(Config, "B:enableSkyOverworldOrbit", enableSkyFeatures, "true");
            }

            void ConfigureSolarSystem()
            {
                Modify($"{ConfigPath}\\Sol\\sol.conf", "B:\"Enable Custom Galaxymap?\"", (Options[2][selectedIndices[2]] == "太阳系").ToString().ToLower(), "true");
            }

            void ConfigureGalaxySpace1()
            {
                string CoreConfig = $"{ConfigPath}\\GalaxySpace\\core.conf";
                string DimensionsConfig = $"{ConfigPath}\\GalaxySpace\\dimensions.conf";

                if (!selectedOptions1.Contains("额外行星"))
                {
                    Modify(DimensionsConfig, "B:enableMarsSpaceStation", "true", "true");
                    Modify(DimensionsConfig, "B:enableVenusSpaceStation", "true", "true");
                }

                Modify(CoreConfig, "B:enableNewMenu", (selectedIndices[5] == 0).ToString().ToLower(), "true");
                Modify(CoreConfig, "B:enableAdvancedRocketCraft", (selectedIndices[6] == 0).ToString().ToLower(), "true");
            }

            void ConfigureGalaxySpace2()
            {
                string CoreConfig = ConfigPath + "\\GalaxySpace\\core.conf";
                string BiomesConfig = ConfigPath + "\\GalaxySpace\\biomes.conf";

                if (selectedOptions2.Contains("更多行星") || selectedOptions2.Contains("额外行星")) Modify(ConfigPath + "\\GalaxySpace\\dimensions.conf", "B:enableMarsSS", (selectedIndices[9] == 2).ToString().ToLower(), "true");
                if (File.Exists(BiomesConfig))
                {
                    File.WriteAllText(BiomesConfig, "# Configuration file\r\n\r\ngeneral {\r\n    # Global ID Biome for Planets/Moons\r\n    I:IDSpaceBiome=214\r\n     \r\n    # Global ID Biome for Planets/Moons (Shallow Waters Biome)\r\n    I:IDSpaceShallowWatersBiome=215\r\n     \r\n    # Global ID Biome for Planets/Moons (Oceans Biome)\r\n    I:IDSpaceOceansBiome=216\r\n     \r\n    # Global ID Biome for Planets/Moons (Deep Oceans Biome)\r\n    I:IDSpaceDeepOceansBiome=217\r\n     \r\n    # Global ID Biome for Planets/Moons (Low Plains Biome)\r\n    I:IDSpaceLowPlainsBiome=218\r\n     \r\n    # Global ID Biome for Planets/Moons (Mid Plains Biome)\r\n    I:IDSpaceMidPlainsBiome=219\r\n     \r\n    # Global ID Biome for Planets/Moons (Low Hills Biome)\r\n    I:IDSpaceLowHillsBiome=220\r\n     \r\n    # Global ID Biome for Planets/Moons (High Plateaus Biome)\r\n    I:IDSpaceHighPlateausBiome=221\r\n     \r\n    # Global ID Biome for Planets/Moons (Mid Hills Biome)\r\n    I:IDSpaceMidHillsBiome=222\r\n     \r\n    # Global ID Biome for Planets/Moons (Rocky Waters Biome)\r\n    I:IDSpaceRockyWatersBiome=223\r\n     \r\n    # Global ID Biome for Planets/Moons (Low Islands Biome)\r\n    I:IDSpaceLowIslandsBiome=224\r\n     \r\n    # Global ID Biome for Planets/Moons (Partially Submerged Biome)\r\n    I:IDSpacePartiallySubmergedBiome=225\r\n     \r\n    # Global ID Biome for Planets/Moons (Beach Biome)\r\n    I:IDSpaceBeachBiome=226\r\n     \r\n    # Biome ID for World Engine\r\n    I:IDWorldEngineBiome=227\r\n}");
                    Write($"已修改");
                    WriteLine("GalaxySpace\\biomes.conf", ConsoleColor.Yellow);
                }
                else WriteLine($"无法找到GalaxySpace\\biomes.conf", ConsoleColor.Red);

                Modify(CoreConfig, "B:enableNewMenu", (selectedIndices[5] == 0).ToString().ToLower(), "true");
                Modify(CoreConfig, "B:enableSkyOverworld", (selectedIndices[7] != 0).ToString().ToLower(), "true");
            }

            void ConfigureMorePlanets()
            {
                if (selectedOptions2.Contains("星空") || selectedOptions2.Contains("额外行星")) Modify(ConfigPath + "\\MorePlanets.cfg", "B:\"Enable Mars Space Station\"", (selectedIndices[9] == 1).ToString().ToLower(), "true");
            }

            if (File.Exists(modsFloder))
            {
                if (planetprogression && galacticresearch) WriteLine($"\r\n检测到你安装了PlanetProgression和GalacticResearch，这两个模组都会让你的星图只有一个星球，请删除一个。其他的星球需要你自行研究，如不想研究请全部删除。", ConsoleColor.Red);
                else if (planetprogression || galacticresearch) WriteLine($"\r\n检测到你安装了" + (planetprogression ? "PlanetProgression" : "GalacticResearch") + "，这个模组会让你的星图只有一个星球。其他的星球需要你自行研究，如不想研究请删除。", ConsoleColor.Red);
                if (beyondspace) WriteLine($"\r\n检测到你安装了Beyond Space(Galaxy Additions)，请确保星空的版本为1.2.14，否则游戏会崩溃。", ConsoleColor.Red);
            }
            Console.WriteLine("\r\n\r\n更多配置请手动修改配置文件，参考：https://www.mcmod.cn/post/2728.html的解决重复天体" + (selectedIndices[0] == 0 ? "(1.8)和附属加载顺序(1.9)" : "(2.4)和附属加载顺序(2.5)") + "部分");
            WriteLine("\r\n\r\n按下任意键退出...", ConsoleColor.Green);
            Console.ReadKey();
            Environment.Exit(0);
        }
    }

    static string[] GetOptions()
    {
        return Index switch
        {
            0 => Options[0],
            1 => Options[1],
            2 => Options[2],
            3 or 4 => Options[3],
            5 or 6 or 7 => Options[4],
            8 => Options[5],
            9 => Options[6],
            _ => []
        };
    }

    static void UpdateMenuIndex(int index, int number)
    {
        Console.Clear();
        Index = index;
        Number += number;
        RenderMenu(GetOptions(), index);
    }

    static void RenderMenu(string[] options, int index)
    {
        Console.Clear();
        string specificPrompt = index switch
        {
            0 => "选择游戏版本",
            1 or 8 => "选择安装的附属",
            2 => "选择星图",
            3 or 9 => "选择火星空间站",
            4 => "选择金星空间站",
            5 => "是否启用星空新主菜单",
            6 => "是否启用星空2~6阶困难火箭配方",
            7 => "是否要安装光影（仅防止与光影存在冲突，不会下载光影）",
            _ => "加载失败"
        };
        if (specificPrompt == "加载失败") { WriteLine(specificPrompt, ConsoleColor.Red); return; }
        WriteLine($"{Number}." + (index == 0 ? specificPrompt + "\r\nESC退出" : specificPrompt + "\r\nESC返回") + " 空格选择/取消 ↑↓切换选项 回车确认\r\n");
        for (int i = 0; i < options.Length; i++)
        {
            string prefix = ((Index == 1) && selectedOptions1.Contains(options[i])) || ((Index == 8) && selectedOptions2.Contains(options[i])) || ((Index != 1 || Index != 8) && i == selectedIndices[index]) ? "[X] " : "[ ] ";
            if (i == selectedIndicesSet[index])
            {
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.WriteLine(prefix + options[i]);
                Console.ResetColor();
            }
            else Console.WriteLine(prefix + options[i]);
        }
    }

    static void UpdateSelection(string[] options)
    {
        if (Index == 1)
        {
            if (selectedOptions1.Contains(options[selectedIndicesSet[1]]))
            {
                selectedOptions1.Remove(options[selectedIndicesSet[1]]);
                if (options[selectedIndicesSet[1]] == "AsmodeusCore") selectedOptions1.Remove("星空");
            }
            else
            {
                selectedOptions1.Add(options[selectedIndicesSet[1]]);
                if (options[selectedIndicesSet[1]] == "星空") selectedOptions1.Add("AsmodeusCore");
            }
        }
        if (Index == 8)
        {
            if (selectedOptions2.Contains(options[selectedIndicesSet[8]])) selectedOptions2.Remove(options[selectedIndicesSet[8]]);
            else selectedOptions2.Add(options[selectedIndicesSet[8]]);
        }
    }

    static void WriteLine(string message, ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }
    static void Write(string message, ConsoleColor color = ConsoleColor.White)
    {
        Console.ForegroundColor = color;
        Console.Write(message);
        Console.ResetColor();
    }
}
