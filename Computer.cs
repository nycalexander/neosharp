using Microsoft.Win32;
using System.Management;
using System.Runtime.InteropServices;

namespace neosharp;

public static partial class Computer
{
    public const string LogoWin11 = @"
lllllllllllllll   lllllllllllllll
lllllllllllllll   lllllllllllllll
lllllllllllllll   lllllllllllllll
lllllllllllllll   lllllllllllllll
lllllllllllllll   lllllllllllllll
lllllllllllllll   lllllllllllllll
lllllllllllllll   lllllllllllllll
 
lllllllllllllll   lllllllllllllll
lllllllllllllll   lllllllllllllll
lllllllllllllll   lllllllllllllll
lllllllllllllll   lllllllllllllll
lllllllllllllll   lllllllllllllll
lllllllllllllll   lllllllllllllll
lllllllllllllll   lllllllllllllll";

    public const string LogoWin10 = @"
                            .oodMMMM
                   .oodMMMMMMMMMMMMM
       ..oodMMM  MMMMMMMMMMMMMMMMMMM
 oodMMMMMMMMMMM  MMMMMMMMMMMMMMMMMMM
 MMMMMMMMMMMMMM  MMMMMMMMMMMMMMMMMMM
 MMMMMMMMMMMMMM  MMMMMMMMMMMMMMMMMMM
 MMMMMMMMMMMMMM  MMMMMMMMMMMMMMMMMMM
 MMMMMMMMMMMMMM  MMMMMMMMMMMMMMMMMMM
 MMMMMMMMMMMMMM  MMMMMMMMMMMMMMMMMMM
 
 MMMMMMMMMMMMMM  MMMMMMMMMMMMMMMMMMM
 MMMMMMMMMMMMMM  MMMMMMMMMMMMMMMMMMM
 MMMMMMMMMMMMMM  MMMMMMMMMMMMMMMMMMM
 MMMMMMMMMMMMMM  MMMMMMMMMMMMMMMMMMM
 MMMMMMMMMMMMMM  MMMMMMMMMMMMMMMMMMM
 `^^^^^^MMMMMMM  MMMMMMMMMMMMMMMMMMM
       ````^^^^  ^^MMMMMMMMMMMMMMMMM
                      ````^^^^^^MMMM";

    public static string GetMotherboard()
    {
        ManagementObject obj = new ManagementObjectSearcher("select Product from Win32_BaseBoard").Get().Cast<ManagementObject>().First();
        return obj["Product"]?.ToString() ?? "Unknown";
    }

    public static string GetOperatingSystem()
    {
        RegistryKey? reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
        string edition = reg?.GetValue("EditionID") as string ?? "Edition Unknown";
        string version = reg?.GetValue("DisplayVersion") as string ?? "Unknown";

        return $"Windows {Environment.OSVersion.Version.Major} {edition} [{version}, {(Environment.Is64BitOperatingSystem ? "x64" : "x86")}]";
    }

    public static string GetBuild() => Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "CurrentBuild", "0")?.ToString() ?? "Unknown";

    public static DateTime GetOSInstallDate() =>  DateTime.UnixEpoch + TimeSpan.FromSeconds(double.Parse(Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "InstallDate", "0")?.ToString() ?? "0"));

    public static TimeSpan GetUptime() => TimeSpan.FromMilliseconds(GetTickCount64());

    public static string GetResolution()
    {
        ManagementObject obj = new ManagementObjectSearcher($"select CurrentHorizontalResolution, CurrentVerticalResolution from Win32_VideoController where Name = '{GetPrimaryGPU()}'").Get().Cast<ManagementObject>().First();
        string horizontalResolution = obj["CurrentHorizontalResolution"]?.ToString() ?? "0";
        string verticalResolution = obj["CurrentVerticalResolution"]?.ToString() ?? "0";
        return horizontalResolution + "x" + verticalResolution;
    }

    public static string GetCPU()
    {
        ManagementObject obj = new ManagementObjectSearcher("select Name from Win32_Processor").Get().Cast<ManagementObject>().First();
        return obj["Name"]?.ToString() ?? "Unknown";
    }

    public static string GetPrimaryGPU()
    {
        ManagementObject obj = new ManagementObjectSearcher("select Name from Win32_VideoController where MaxRefreshRate != NULL and MaxRefreshRate != \"\"").Get().Cast<ManagementObject>().First();
        return obj["Name"]?.ToString() ?? "Unknown";
    }

    public static string GetMemory()
    {
        ManagementObject obj = new ManagementObjectSearcher("select TotalVisibleMemorySize, FreePhysicalMemory from Win32_OperatingSystem").Get().Cast<ManagementObject>().First();
        string totalMemoryStr = obj["TotalVisibleMemorySize"].ToString() ?? "";
        string freeMemoryStr = obj["FreePhysicalMemory"].ToString() ?? "";

        if (string.IsNullOrEmpty(totalMemoryStr) || string.IsNullOrEmpty(freeMemoryStr))
            return "Unknown";
        
        double total = double.Parse(totalMemoryStr);
        double free = double.Parse(freeMemoryStr);
        double used = total - free;
        double usingPrec = Math.Floor(used / total * 100d);

        return $"{used / 1074000:N2} GiB / {total / 1074000:N2} GiB ({usingPrec}%)";
    }

    public static string GetTheme()
    {
        RegistryKey? reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes");
        string theme = reg?.GetValue("CurrentTheme") as string ?? "Unknown";
        return theme.Split('\\').Last().Split('.').First();
    }

    public static string GetUsername() => Environment.UserName + "@" + Environment.MachineName;

    
    [LibraryImport("kernel32")]
    private static partial ulong GetTickCount64();
}