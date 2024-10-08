﻿using System.Data;
using System.Linq;
using System.Text;

class Program
{
    static string ParentDir = System.Environment.CurrentDirectory;
    static List<string> selectedOptions = new List<string>();
    static string[] Options1 = { "额外行星", "星空", "更多行星", "太阳系", "AsmodeusCore" };
    static string[] Options2 = { "星系" };
    static string[] Options3 = { "星空", "额外行星" };
    static string[] Options4 = { "是", "否" };
    static int selectedIndex1 = 0;
    static int selectedIndex2 = 0;
    static int selectedIndex3 = 0;
    static int selectedIndex4 = 0;
    static int selectedIndex5 = 0;
    static int selectedIndex6 = 0;
    static int selectedIndex7 = 0;
    static int Index = 1;

    static void Main(string[] args)
    {
        Console.Title = "星系附属兼容问题解决工具";
        if (Path.GetFileNameWithoutExtension(ParentDir) != "config")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\x1b[1m程序不在config文件夹下\x1b[0m");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\x1b[1m请输入config文件夹所在路径\x1b[0m");
            Console.ResetColor();
            string? input = Console.ReadLine();
            while (input == null || Path.GetFileNameWithoutExtension(input) != "config")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\x1b[1m路径错误，请输入config文件夹的正确路径\x1b[0m");
                Console.ResetColor();
                input = Console.ReadLine();
            }
            ParentDir = input;
        }
        Console.CursorVisible = false;
        Console.Clear();
        Console.WriteLine("欢迎使用星系附属兼容问题解决工具 v1.0.1\n\n工具简介：\n本工具由付邦坤（MC百科主页：https://center.mcmod.cn/157477/）制作，并在MC百科社群（https://bbs.mcmod.cn/thread-19017-1-1.html）发布。该工具旨在解决星系附属模组间的兼容问题，提升游戏体验。\n\n免费声明：\n本工具完全免费，我们鼓励大家自由使用并分享给更多需要的玩家。但请注意，本工具严禁用于任何商业用途。若您不慎通过购买方式获取，请立即联系退款。\n\n支持列表：\n本工具目前支持以下星系附属模组：星空、额外行星、更多行星、太阳系、AsmodeusCore。对于其他附属模组，基本不会造成兼容问题，因此无需特别处理。\n\n已知问题：\n由于太阳系模组本身的注册系统特性，可能导致游戏崩溃。本工具仅负责修改太阳系的星图配置，无法从根本上解决崩溃问题。\n\n未来计划：\n我们计划在未来版本中移除重复天体，并增加对Minecraft 1.7.10版本的支持，敬请期待。暂时可以参考此文进行修改：https://www.mcmod.cn/post/2728.html\n\n感谢与支持：\n如果您觉得本工具对您有所帮助，请在本工具Github仓库（https://github.com/FuBangkun/GalacticraftTools）上给予本工具一个Star以表示支持，这将是我们继续前进的动力。感谢您的理解与支持！");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n按下任意键继续...");
        Console.ResetColor();
        Console.ReadKey();
        DrawMenu(Options1, selectedIndex1);

        while (true)
        {
            var key = Console.ReadKey(true);
            switch (Index)
            {
                case 1:
                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            selectedIndex1 = (selectedIndex1 - 1 + Options1.Length) % Options1.Length;
                            break;
                        case ConsoleKey.DownArrow:
                            selectedIndex1 = (selectedIndex1 + 1) % Options1.Length;
                            break;
                        case ConsoleKey.Spacebar:
                            ToggleSelection(Options1);
                            break;
                        case ConsoleKey.Escape:
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\x1b[1m按下任意键确定退出\x1b[0m");
                            Console.ResetColor();
                            Console.ReadKey();
                            Environment.Exit(0);
                            return;
                        case ConsoleKey.Enter:
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Red;
                            if (selectedOptions.Count < 2)
                            {
                                if (selectedOptions.Count == 0)
                                {
                                    Console.WriteLine("\x1b[1m你没有选择任何东西\x1b[0m");
                                }
                                else
                                {
                                    Console.WriteLine("\x1b[1m请选择至少两个模组\x1b[0m");
                                }
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("\n按下任意键返回");
                                Console.ResetColor();
                                continue;
                            }
                            HashSet<string> options = Options2.ToHashSet();
                            int errorCount = 0;
                            void CheckOptionAndFile(string optionName, string fileName, bool addToOptions = true, string dirModifier = "")
                            {
                                if (selectedOptions.Contains(optionName))
                                {
                                    if (addToOptions)
                                    {
                                        if (optionName == "AsmodeusCore")
                                        {
                                            options.Add("AsmodeusCore（星空）");
                                        }
                                        else
                                        {
                                            options.Add(optionName);
                                        }
                                    }
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
                            Options2 = options.ToArray();
                            if (errorCount > 0)
                            {
                                Console.WriteLine($"{errorCount}个错误");
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("按下任意键退出");
                                Console.ResetColor();
                                Console.ReadKey();
                                return;
                            }
                            Index = 2;
                            DrawMenu(Options2, selectedIndex2);
                            break;
                    }
                    break;
                case 2:
                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            selectedIndex2 = (selectedIndex2 - 1 + Options2.Length) % Options2.Length;
                            break;
                        case ConsoleKey.DownArrow:
                            selectedIndex2 = (selectedIndex2 + 1) % Options2.Length;
                            break;
                        case ConsoleKey.Escape:
                            Console.ResetColor();
                            Index = 1;
                            DrawMenu(Options1, selectedIndex1);
                            break;
                        case ConsoleKey.Enter:
                            Console.Clear();
                            if (selectedOptions.Contains("额外行星") && selectedOptions.Contains("星空"))
                            {
                                Index = 3;
                                DrawMenu(Options3, selectedIndex3);
                                break;
                            }
                            else
                            {
                                if (selectedOptions.Contains("额外行星"))
                                {
                                    if (selectedOptions.Contains("AsmodeusCore"))
                                    {
                                        Index = 7;
                                        DrawMenu(Options4, selectedIndex7);
                                    }
                                }
                                if (selectedOptions.Contains("星空"))
                                {
                                    Index = 5;
                                    DrawMenu(Options4, selectedIndex5);
                                }
                            }
                            break;
                    }
                    break;
                case 3:
                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            selectedIndex3 = (selectedIndex3 - 1 + Options3.Length) % Options3.Length;
                            break;
                        case ConsoleKey.DownArrow:
                            selectedIndex3 = (selectedIndex3 + 1) % Options3.Length;
                            break;
                        case ConsoleKey.Escape:
                            Console.ResetColor();
                            Index = 2;
                            DrawMenu(Options2, selectedIndex2);
                            break;
                        case ConsoleKey.Enter:
                            Console.Clear();
                            Index = 4;
                            DrawMenu(Options3, selectedIndex4);
                            break;
                    }
                    break;
                case 4:
                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            selectedIndex4 = (selectedIndex4 - 1 + Options3.Length) % Options3.Length;
                            break;
                        case ConsoleKey.DownArrow:
                            selectedIndex4 = (selectedIndex4 + 1) % Options3.Length;
                            break;
                        case ConsoleKey.Escape:
                            Console.ResetColor();
                            Index = 3;
                            DrawMenu(Options3, selectedIndex3);
                            break;
                        case ConsoleKey.Enter:
                            Console.Clear();
                            Index = 5;
                            DrawMenu(Options4, selectedIndex5);
                            break;
                    }
                    break;
                case 5:
                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            selectedIndex5 = (selectedIndex5 - 1 + Options4.Length) % Options4.Length;
                            break;
                        case ConsoleKey.DownArrow:
                            selectedIndex5 = (selectedIndex5 + 1) % Options4.Length;
                            break;
                        case ConsoleKey.Escape:
                            Console.ResetColor();
                            Index = 4;
                            DrawMenu(Options3, selectedIndex4);
                            break;
                        case ConsoleKey.Enter:
                            Console.Clear();
                            Index = 6;
                            DrawMenu(Options4, selectedIndex6);
                            break;
                    }
                    break;
                case 6:
                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            selectedIndex6 = (selectedIndex6 - 1 + Options4.Length) % Options4.Length;
                            break;
                        case ConsoleKey.DownArrow:
                            selectedIndex6 = (selectedIndex6 + 1) % Options4.Length;
                            break;
                        case ConsoleKey.Escape:
                            Console.ResetColor();
                            Index = 5;
                            DrawMenu(Options4, selectedIndex5);
                            break;
                        case ConsoleKey.Enter:
                            Console.Clear();
                            Index = 7;
                            DrawMenu(Options4, selectedIndex7);
                            break;
                    }
                    break;
                case 7:
                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            selectedIndex7 = (selectedIndex7 - 1 + Options4.Length) % Options4.Length;
                            break;
                        case ConsoleKey.DownArrow:
                            selectedIndex7 = (selectedIndex7 + 1) % Options4.Length;
                            break;
                        case ConsoleKey.Escape:
                            Console.ResetColor();
                            if (selectedOptions.Contains("星空"))
                            {
                                Index = 6;
                                DrawMenu(Options4, selectedIndex6);
                            }
                            else
                            {
                                Index = 2;
                                DrawMenu(Options2, selectedIndex2);
                            }
                            break;
                        case ConsoleKey.Enter:
                            Console.Clear();
                            Modify(7);
                            break;
                    }
                    break;
            }

            switch (Index)
            {
                case 1:
                    DrawMenu(Options1, selectedIndex1);
                    break;
                case 2:
                    DrawMenu(Options2, selectedIndex2); 
                    break;
                case 3:
                    DrawMenu(Options3, selectedIndex3);
                    break;
                case 4:
                    DrawMenu(Options3, selectedIndex4);
                    break;
                case 5:
                    DrawMenu(Options4, selectedIndex5);
                    break;
                case 6:
                    DrawMenu(Options4, selectedIndex6);
                    break;
                case 7:
                    DrawMenu(Options4, selectedIndex7);
                    break;
            }
        }
    }

    static void Modify(int index)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("按下任意键确认更改，Escape键返回");
        Console.ResetColor();
        var key = Console.ReadKey(true);
        if (key.Key == ConsoleKey.Escape)
        {
            Console.ResetColor();
            selectedIndex7 = 0;
            DrawMenu(Options4, selectedIndex7);
            return;
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
                        string currentValue = trimmedLine.Substring((keyToFind + "=").Length).Trim();
                        lines[i] = "    " + keyToFind + $"={value.ToString().ToLower()}";

                        Console.Write("已修改");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write($"{keyToFind}");
                        Console.ResetColor();
                        Console.Write("为");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"{value.ToString().ToLower()}");
                        Console.ResetColor();
                        Console.Write("，修改前的值为");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($"{currentValue}\n");
                        Console.ResetColor();

                        foundAndModified = true;
                        break;
                    }
                }
                if (!foundAndModified)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\x1b[1m无法找到{keyToFind}\x1b[0m");
                    Console.ResetColor();
                    error++;
                }
                File.WriteAllLines(configFilePath, lines);
            }
            void ModifyList(string configFilePath, string keyToFind, List<string> itemsToAdd)
            {
                string[] lines = File.ReadAllLines(configFilePath);
                StringBuilder sb = new StringBuilder();
                bool inSection = false;
                bool foundAndModified = false;
                List<string> existingItems = new List<string>();
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
                        foreach (var item in itemsToAdd.Where(item => !existingItems.Contains(item)))
                        {
                            sb.AppendLine($"        {item}");
                        }

                        sb.AppendLine(line);
                        foundAndModified = true;
                        continue;
                    }

                    if (inSection)
                    {
                        sb.AppendLine(line);
                        if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("        "))
                            continue;

                        string item = line.TrimStart(' ', '\t').Trim();
                        if (!string.IsNullOrWhiteSpace(item) && !item.StartsWith("//"))
                        {
                            existingItems.Add(item);
                        }
                    }
                    else
                    {
                        sb.AppendLine(line);
                    }
                }
                if (!foundAndModified)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\x1b[1m无法找到{keyToFind}\x1b[0m");
                    Console.ResetColor();
                    error++;
                }
                File.WriteAllText(configFilePath, sb.ToString());
            }
            if (selectedOptions.Contains("额外行星"))
            {
                if (selectedOptions.Contains("星空"))
                {
                    if (selectedIndex3 == 0)
                    {
                        ModifyBool(ParentDir + "\\ExtraPlanets.cfg", "B:\"Mars SpaceStation\"", false);
                        ModifyBool(ParentDir + "\\GalaxySpace\\dimensions.conf", "B:enableMarsSpaceStation", true);
                    }
                    else
                    {
                        ModifyBool(ParentDir + "\\ExtraPlanets.cfg", "B:\"Mars SpaceStation\"", true);
                        ModifyBool(ParentDir + "\\GalaxySpace\\dimensions.conf", "B:enableMarsSpaceStation", false);
                    }
                    if (selectedIndex4 == 0)
                    {
                        ModifyBool(ParentDir + "\\ExtraPlanets.cfg", "B:\"Venus SpaceStation\"", false);
                        ModifyBool(ParentDir + "\\GalaxySpace\\dimensions.conf", "B:enableVenusSpaceStation", true);
                    }
                    else
                    {
                        ModifyBool(ParentDir + "\\ExtraPlanets.cfg", "B:\"Venus SpaceStation\"", true);
                        ModifyBool(ParentDir + "\\GalaxySpace\\dimensions.conf", "B:enableVenusSpaceStation", false);
                    }
                    ModifyBool(ParentDir + "\\ExtraPlanets.cfg", "B:\"Enable Galaxy Space Compatibility\"", true);
                    List<string> GS = new List<string>
                    {
                        "galaxyspace:space_suit_head:1",
                        "galaxyspace:space_suit_chest:1",
                        "galaxyspace:space_suit_legs:1",
                        "galaxyspace:space_suit_feet:1",
                        "galaxyspace:space_suit_light_head:1",
                        "galaxyspace:space_suit_light_chest:1",
                        "galaxyspace:space_suit_light_legs:1",
                        "galaxyspace:space_suit_light_feet:1"
                    };
                    ModifyList(ParentDir + "\\ExtraPlanets.cfg", "S:\"List of armour items to be considered as a space suit\" <", GS);
                    List<string> EP = new List<string>
                    {
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
                    };
                    ModifyList(ParentDir + "\\GalaxySpace\\core.conf", "S:\"Radiation and Pressure Armor List\" <", EP);
                }
                if (!selectedOptions.Contains("星空"))
                {
                    ModifyBool(ParentDir + "\\ExtraPlanets.cfg", "B:\"Enable Galaxy Space Compatibility\"", false);
                    ModifyBool(ParentDir + "\\ExtraPlanets.cfg", "B:\"Mars SpaceStation\"", true);
                    ModifyBool(ParentDir + "\\ExtraPlanets.cfg", "B:\"Venus SpaceStation\"", true);
                }
                if (selectedOptions.Contains("更多行星"))
                {
                    ModifyBool(ParentDir + "\\ExtraPlanets.cfg", "B:\"Enable More Planets Compatibility\"", true);
                }
                else
                {
                    ModifyBool(ParentDir + "\\ExtraPlanets.cfg", "B:\"Enable More Planets Compatibility\"", false);
                }
                if (Options2[selectedIndex2] == "额外行星")
                {
                    ModifyBool(ParentDir + "\\ExtraPlanets.cfg", "B:\"Use Custom Galaxy Map/Celestial Selection Screen\"", true);
                }
                else
                {
                    ModifyBool(ParentDir + "\\ExtraPlanets.cfg", "B:\"Use Custom Galaxy Map/Celestial Selection Screen\"", false);
                }
            }
            if (selectedOptions.Contains("AsmodeusCore"))
            {
                if (Options2[selectedIndex2] == "AsmodeusCore（星空）")
                {
                    ModifyBool(ParentDir + "\\AsmodeusCore\\core.conf", "B:enableNewGalaxyMap", true);
                }
                else
                {
                    ModifyBool(ParentDir + "\\AsmodeusCore\\core.conf", "B:enableNewGalaxyMap", false);
                }
                if (selectedIndex7 == 0)
                {
                    ModifyBool(ParentDir + "\\AsmodeusCore\\core.conf", "B:enableSkyAsteroids", false);
                    ModifyBool(ParentDir + "\\AsmodeusCore\\core.conf", "B:enableSkyMoon", false);
                    ModifyBool(ParentDir + "\\AsmodeusCore\\core.conf", "B:enableSkyOverworld", false);
                    ModifyBool(ParentDir + "\\AsmodeusCore\\core.conf", "B:enableSkyOverworldOrbit", false);
                }
                else
                {
                    ModifyBool(ParentDir + "\\AsmodeusCore\\core.conf", "B:enableSkyAsteroids", true);
                    ModifyBool(ParentDir + "\\AsmodeusCore\\core.conf", "B:enableSkyMoon", true);
                    ModifyBool(ParentDir + "\\AsmodeusCore\\core.conf", "B:enableSkyOverworld", true);
                    ModifyBool(ParentDir + "\\AsmodeusCore\\core.conf", "B:enableSkyOverworldOrbit", true);
                }
            }
            if (selectedOptions.Contains("太阳系"))
            {
                if (Options2[selectedIndex2] == "太阳系")
                {
                    ModifyBool(ParentDir + "\\Sol\\sol.conf", "B:\"Enable Custom Galaxymap?\"", true);
                }
                else
                {
                    ModifyBool(ParentDir + "\\Sol\\sol.conf", "B:\"Enable Custom Galaxymap?\"", false);
                }
            }
            if (selectedOptions.Contains("星空"))
            {
                if (!selectedOptions.Contains("额外行星"))
                {
                    ModifyBool(ParentDir + "\\GalaxySpace\\dimensions.conf", "B:enableMarsSpaceStation", true);
                    ModifyBool(ParentDir + "\\GalaxySpace\\dimensions.conf", "B:enableVenusSpaceStation", true);
                }
                if (selectedIndex5 == 0)
                {
                    ModifyBool(ParentDir + "\\GalaxySpace\\core.conf", "B:enableNewMenu", true);
                }
                else
                {
                    ModifyBool(ParentDir + "\\GalaxySpace\\core.conf", "B:enableNewMenu", false);
                }
                if (selectedIndex6 == 0)
                {
                    ModifyBool(ParentDir + "\\GalaxySpace\\core.conf", "B:enableAdvancedRocketCraft", true);
                }
                else
                {
                    ModifyBool(ParentDir + "\\GalaxySpace\\core.conf", "B:enableAdvancedRocketCraft", false);
                }
            }
            if (error != 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\x1b[1m{error}个错误\x1b[0m");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\x1b[1m没有错误\x1b[0m");
                Console.ResetColor();
            }
            Console.WriteLine("\n\n\x1b[1m更多配置请手动修改配置文件，参考：https://www.mcmod.cn/post/2728.html的解决重复天体(1.8)和附属加载顺序(1.9)部分\x1b[0m");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\n按下任意键退出，记得给个Star哦");
            Console.ResetColor();
            Console.ReadKey();
            Environment.Exit(0);
            
        }
    }

    static string GetPrompt(int index)
    {
        string prompt = "\x1b[0m\nEscape返回 ↑↓切换选项 Enter确定\n";
        switch (index)
        {
            case 1:
                return "\x1b[1m选择安装的附属\x1b[0m\nEscape退出 Space选择/取消 ↑↓切换选项 Enter确定\n";
            case 2:
                return "\x1b[1m选择星图" + prompt;
            case 3:
                return "\x1b[1m选择火星空间站" + prompt;
            case 4:
                return "\x1b[1m选择金星空间站" + prompt;
            case 5:
                return "\x1b[1m是否启用星空新主菜单" + prompt;
            case 6:
                return "\x1b[1m是否启用星空2~6阶困难火箭配方" + prompt;
            case 7:
                return "\x1b[1m是否要安装光影（仅防止与光影存在冲突，不会下载光影）" + prompt;
            default:
                return "\x1b[1m加载错误";
        }
    }

    static void DrawMenu(string[] options, int selectedIndex)
    {
        string prefix = string.Empty;
        Console.Clear();
        Console.WriteLine(GetPrompt(Index));
        for (int i = 0; i < options.Length; i++)
        {
            if (Index == 1)
            {
                prefix = (selectedOptions.Contains(options[i]) ? "[X] " : "[ ] ");
            }
            else
            {
                prefix = (i == selectedIndex ? "[X] " : "[ ] ");
            }
            if (i == selectedIndex)
            {
                Console.BackgroundColor = ConsoleColor.Blue;
            }
            else
            {
                Console.ResetColor();
            }
            Console.WriteLine(prefix + options[i]);
        }
        Console.ResetColor();
    }

    static void ToggleSelection(string[] options)
    {
        if (selectedOptions.Contains(options[selectedIndex1]))
        {
            selectedOptions.Remove(options[selectedIndex1]);
            if (options[selectedIndex1] == "AsmodeusCore")
            {
                selectedOptions.Remove("星空");
            }
        }
        else
        {
            selectedOptions.Add(options[selectedIndex1]);
            if (options[selectedIndex1] == "星空")
            {
                selectedOptions.Add("AsmodeusCore");
            }
        }
    }
}