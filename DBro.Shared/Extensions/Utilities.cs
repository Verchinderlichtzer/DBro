using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DBro.Shared.Extensions;

public static partial class Utilities
{

    #region Current Project Only

    public static List<string> GetColorSVG => [
        "/images/colors/putih.svg",
        "/images/colors/abu-abu.svg",
        "/images/colors/hitam.svg",
        "/images/colors/merah.svg",
        "/images/colors/pink.svg",
        "/images/colors/ungu.svg",
        "/images/colors/biru.svg",
        "/images/colors/biru_muda.svg",
        "/images/colors/hijau.svg",
        "/images/colors/lime.svg",
        "/images/colors/kuning.svg",
        "/images/colors/jingga.svg"
        ];

    #endregion Current Project Only

    #region Item Search

    public static bool Search(this string text, string searchTerms)
    {
        return text.Contains(searchTerms, StringComparison.OrdinalIgnoreCase);
    }

    public static IEnumerable<string> Search(this IEnumerable<string> text, string searchTerms)
    {
        return text.Where(x => x.Contains(searchTerms, StringComparison.InvariantCultureIgnoreCase));
    }

    #endregion Item Search

    #region Conversions & Parsing

    public static int ToInt(object? obj)
    {
        _ = int.TryParse(obj?.ToString() ?? "0", out int result);
        return result;
    }

    public static decimal ToDecimal(object? obj)
    {
        _ = decimal.TryParse(obj?.ToString() ?? "0", out decimal result);
        return result;
    }

    public static double ToDouble(object? obj)
    {
        _ = double.TryParse(obj?.ToString() ?? "0", out double result);
        return result;
    }

    public static bool ToBool(object? obj)
    {
        _ = bool.TryParse(obj?.ToString() ?? "False", out bool result);
        return result;
    }

    public static T ToEnum<T>(this string value) => (T)Enum.Parse(typeof(T), value, true);

    public static DateTime ToDateTime(object? obj)
    {
        bool isSuccess = DateTime.TryParse(obj?.ToString() ?? string.Empty, out DateTime result);
        return isSuccess ? result : DateTime.Now;
    }

    #endregion Conversions & Parsing

    #region String Utility

    /// <summary>satuX-Xdua => IndexOf -? => satuX</summary>
    public static string Left(this string text, int characterCount)
    {
        text += string.Empty;
        if (characterCount > text.Length) characterCount = text.Length;
        return text[..characterCount];
    }

    /// <summary>satuX-Xdua => IndexOf -? => -Xdua</summary>
    public static string Right(this string text, int characterCount)
    {
        text += string.Empty;
        if (characterCount > text.Length) characterCount = text.Length;
        return text[^characterCount..];
    }

    /// <summary>satuX-Xdua => IndexOf t? => tuX-Xdua</summary>
    public static string Mid(this string text, int index)
    {
        text += string.Empty;
        return text[index..];
    }

    public static string Mid(this string text, int index, int characterCount)
    {
        text += string.Empty;
        if (characterCount > text.Length) characterCount = text.Length - index;
        return text.Substring(index, characterCount);
    }

    /// <summary>charType : true = Alphabet Only, false = Numeric Only, null = Both</summary>
    public static string GenerateRandomString(int length, bool? charType = null)
    {
        // LENGKAP chars = "abcdefghijklmnopqrstuvwxyz";
        string chars;
        if (charType == true)
            chars = "abcdefgjklmnopqrstuvwxyz";
        else if (charType == false)
            chars = "0123456789";
        else
            chars = "abcdefgjklmnopqrstuvwxyz0123456789";
        Random random = new();
        return new(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static string GenerateRandomSequence(int n)
    {
        Random rand = new();
        var aku = string.Join(",", Enumerable.Range(1, n).OrderBy(x => rand.Next()));
        return aku;
    }

    // Hint : Change the method signature and input parameter to use the type parameter T
    public static string GetDescription(this Enum GenericEnum)
    {
        Type genericEnumType = GenericEnum.GetType();
        MemberInfo[] memberInfo = genericEnumType.GetMember(GenericEnum.ToString());
        if (memberInfo?.Length > 0)
        {
            var _Attribs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (_Attribs?.Length > 0)
                return ((DescriptionAttribute)_Attribs[0]).Description;
        }
        return GenericEnum.ToString();
    }

    /// <summary>tHiS is a sEnTeNcE. one_tWo thREE-FOUR FIVE ———> THiS is a sEnTeNcE. one_tWo thREE-FOUR FIVE</summary>
    public static string CapitalizeSentence(this string input)
    {
        return string.IsNullOrWhiteSpace(input) ? string.Empty : string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1));
    }

    /// <summary>tHiS is a sEnTeNcE. one_tWo thREE-FOUR FIVE ———> T Hi S is a s En Te Nc E. one_t Wo th REE-FOUR FIVE</summary>
    public static string CapitalizeSentenceAndSeparateByCase(this string toSplit)
    {
        if (toSplit == null)
            return null!;

        if (string.IsNullOrWhiteSpace(toSplit))
            return string.Empty;

        toSplit = char.ToUpper(toSplit[0]) + toSplit[1..];
        toSplit = TitleCaseRegex().Replace(toSplit, " ").TrimStart().TrimEnd();

        return ReplaceTitleCaseRegex().Replace(toSplit, "$1 ");
    }

    /// <summary>tHiS is a sEnTeNcE. one_tWo thREE-FOUR FIVE ———> This Is A Sentence. One_Two Three-Four Five</summary>
    public static string CapitalizeWord(this string input)
    {
        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
        return textInfo.ToTitleCase(input.ToLower());
    }

    /// <summary>tHiS is a sEnTeNcE. one_tWo thREE-FOUR FIVE ———> t Hi S is a s En Te Nc E. one_t Wo th R E E- F O U R  F I V E</summary>
    public static string SeparateByCase(this string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "";
        return CapitalLetter().Replace(text, " $1").Trim();
    }

    /// <summary>Example result : One, Two and Three</summary>
    public static string CombineWords(this IEnumerable<string> list, bool useLastSeparator = true, string lastSeparator = "dan")
    {
        var nonEmptyList = list.Where(item => !string.IsNullOrEmpty(item)).ToList();
        if (useLastSeparator)
            return string.Join(", ", nonEmptyList.Take(nonEmptyList.Count - 1)) + (nonEmptyList.Count > 1 ? $" {lastSeparator} " : "") + nonEmptyList[^1];
        else
            return string.Join(", ", nonEmptyList);
    }


    public static bool IsAllLowerCase(this string input) => input.All(char.IsLower);

    public static bool IsAllUpperCase(this string input) => input.All(char.IsUpper);

    public static string FixedLength(string left, string right, int count = 30) => left.PadRight(count - right.Length) + right;

    /// <summary>First Word, Rest of Word (In Middles), Last Word</summary>
    public static string[] SplitThree(string text)
    {
        Match match = FirstRestLastWord().Match(text);
        if (match.Success) return [match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value];
        return null!;
    }

    /// <summary>"abcdefh".SplitByLimit(3) => [ "abc", "def", "g" ]</summary>
    public static List<string> SplitByLimit(this string input, int limit)
    {
        List<string> result = [];
        for (int i = 0; i < input.Length; i += limit)
        {
            if (i + limit > input.Length) limit = input.Length - i;
            result.Add(input.Substring(i, limit));
        }
        return result;
    }

    #endregion String Utility

    #region Number Utility

    public static decimal IncreaseBy(ref decimal x, decimal y) => x += y;

    public static decimal DecreaseBy(ref decimal x, decimal y) => x -= y;

    public static int IncreaseBy(ref int x, int y) => x += y;

    public static int DecreaseBy(ref int x, int y) => x -= y;

    /// <summary>Maks 3000 | I = 1 | V = 5 | X = 10 | L = 50 | C = 100 | D = 500 | M = 1000</summary>
    public static string IntToRoman(int num)
    {
        string[] M = ["", "M", "MM", "MMM"];
        string[] C = ["", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM"];
        string[] X = ["", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC"];
        string[] I = ["", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX"];
        return M[num / 1000] + C[num % 1000 / 100] + X[num % 100 / 10] + I[num % 10];
    }

    /// <summary>I = 1 | V = 5 | X = 10 | L = 50 | C = 100 | D = 500 | M = 1000</summary>
    public static int RomanToInt(string roman)
    {
        Dictionary<char, int> romanMap = new() { { 'I', 1 }, { 'V', 5 }, { 'X', 10 }, { 'L', 50 }, { 'C', 100 }, { 'D', 500 }, { 'M', 1000 } };
        int total = 0;
        int prevVal = 0;
        foreach (char c in roman.ToUpper())
        {
            if (!romanMap.TryGetValue(c, out int value)) return 0;
            int curVal = value;
            total += curVal;
            if (prevVal < curVal) total -= 2 * prevVal;
            prevVal = curVal;
        }
        return total;
    }

    /// <summary>Terbilang</summary>
    public static string Inword(long num)
    {
        string[] satuan = ["", "Satu", "Dua", "Tiga", "Empat", "Lima", "Enam", "Tujuh", "Delapan", "Sembilan", "Sepuluh", "Sebelas"];

        if (num < 12)
        {
            return satuan[num];
        }
        else if (num < 20)
        {
            return $"{Inword(num - 10)} Belas";
        }
        else if (num < 100)
        {
            return $"{Inword(num / 10)} Puluh {Inword(num % 10)}";
        }
        else if (num < 200)
        {
            return $"Seratus {Inword(num - 100)}";
        }
        else if (num < 1000)
        {
            return $"{Inword(num / 100)} Ratus {Inword(num % 100)}";
        }
        else if (num < 2000)
        {
            return $"Seribu {Inword(num - 1000)}";
        }
        else if (num < 1000000)
        {
            return $"{Inword(num / 1000)} Ribu {Inword(num % 1000)}";
        }
        else if (num < 1000000000)
        {
            return $"{Inword(num / 1000000)} Juta {Inword(num % 1000000)}";
        }
        else if (num >= 1000000000)
        {
            return $"{Inword(num / 1000000000)} Milyar {Inword(num % 1000000000)}";
        }
        return string.Empty;
    }

    /// <summary>1;,2cX3W4HD5f6./78 => 12345678</summary>
    public static int GetNumber(string str)
    {
        string result = GetNumberRegex().Replace(str, "");
        return int.Parse(result);
    }

    #endregion Number Utility

    #region Boolean Utility

    public static string ToIndicator(bool x)
    {
        return x ? "Ya" : !x ? "Tidak" : "—";
    }

    #endregion Boolean Utility

    #region Byte Utility

    public static byte[] GenerateRandomBytes(int length)
    {
        Random random = new();
        byte[] bytes = new byte[length];
        random.NextBytes(bytes);
        return bytes;
    }

    public static byte[] CombineBytes(byte[] first, byte[] second)
    {
        byte[] combinedBytes = new byte[first.Length + second.Length];
        Buffer.BlockCopy(first, 0, combinedBytes, 0, first.Length);
        Buffer.BlockCopy(second, 0, combinedBytes, first.Length, second.Length);
        return combinedBytes;
    }

    public static byte[] CombineBytes(byte[] first, byte[] second, byte[] third)
    {
        byte[] combinedBytes = new byte[first.Length + second.Length + third.Length];
        Buffer.BlockCopy(first, 0, combinedBytes, 0, first.Length);
        Buffer.BlockCopy(second, 0, combinedBytes, first.Length, second.Length);
        Buffer.BlockCopy(third, 0, combinedBytes, first.Length + second.Length, third.Length);
        return combinedBytes;
    }

    #endregion Byte Utility

    #region DateTime Utility

    public static DateTime GetDate()
    {
        var client = new TcpClient("time.nist.gov", 13);
        using var streamReader = new StreamReader(client.GetStream());
        var response = streamReader.ReadToEnd();
        var utcDateTimeString = response.Substring(7, 17);
        return DateTime.ParseExact(utcDateTimeString, "yy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
    }

    private static async Task<DateTime?> GetNetworkTime(string ntpServer = "pool.ntp.org")
    {
        // https://stackoverflow.com/questions/1193955/how-to-query-an-ntp-server-using-c

        ArgumentNullException.ThrowIfNull(ntpServer);

        try
        {
            const int daysTo1900 = (1900 * 365) + 95; // 95 = offset for leap-years etc.
            const long ticksPerSecond = 10000000L;
            const long ticksPerDay = 24 * 60 * 60 * ticksPerSecond;
            const long ticksTo1900 = daysTo1900 * ticksPerDay;

            var ntpData = new byte[48];
            ntpData[0] = 0x1B; // LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)

            var addresses = Dns.GetHostEntry(ntpServer).AddressList;
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            // ReSharper disable once RedundantAssignment

            var pingDuration = Stopwatch.GetTimestamp(); // temp access (JIT-Compiler need some time at first call)

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                await socket.ConnectAsync(ipEndPoint);
                socket.ReceiveTimeout = 5000;
                socket.Send(ntpData);
                pingDuration = Stopwatch.GetTimestamp(); // after Send-Method to reduce WinSocket API-Call time

                socket.Receive(ntpData);
                pingDuration = Stopwatch.GetTimestamp() - pingDuration;
            }

            var pingTicks = pingDuration * ticksPerSecond / Stopwatch.Frequency;

            // optional: display response-time
            // Console.WriteLine("{0:N2} ms", new TimeSpan(pingTicks).TotalMilliseconds);

            var intPart = (long)ntpData[40] << 24 | (long)ntpData[41] << 16 | (long)ntpData[42] << 8 | ntpData[43];
            var fractPart = (long)ntpData[44] << 24 | (long)ntpData[45] << 16 | (long)ntpData[46] << 8 | ntpData[47];
            var netTicks = (intPart * ticksPerSecond) + (fractPart * ticksPerSecond >> 32);

            var networkDateTime = new DateTime(ticksTo1900 + netTicks + (pingTicks / 2));

            return networkDateTime.ToLocalTime(); // without ToLocalTime() = faster
        }
        catch
        {
            // fail
            return null;
        }
    }

    #endregion DateTime Utility

    #region Object Utility

    public static void ClonesTo<T>(this T source, T target)
    {
        var type = typeof(T);
        foreach (var sourceProperty in type.GetProperties())
        {
            if (!sourceProperty.CanWrite) continue;
            var targetProperty = type.GetProperty(sourceProperty.Name);
            targetProperty!.SetValue(target, sourceProperty.GetValue(source, null), null);
        }
        foreach (var sourceField in type.GetFields())
        {
            var targetField = type.GetField(sourceField.Name);
            targetField!.SetValue(target, sourceField.GetValue(source));
        }
    }

    public static void ResetId<T>(T obj) where T : class
    {
        var propertyInfo = obj.GetType().GetProperty("Id");
        propertyInfo!.SetValue(obj, propertyInfo.PropertyType == typeof(int) ? 0 : null);
    }

    public static void ResetIds<T>(IEnumerable<T> list) where T : class
    {
        foreach (var obj in list) ResetId(obj);
    }

    public static T Nullify<T>(T obj)
    {
        var type = typeof(T);
        foreach (var x in type.GetProperties())
        {
            if ((x.PropertyType.IsClass && x.PropertyType != typeof(string) && x.PropertyType != typeof(byte[])) || (x.PropertyType.IsGenericType && x.PropertyType != typeof(DateTime?) && x.PropertyType != typeof(TimeSpan?)) || (x.PropertyType.IsArray && x.PropertyType != typeof(byte[]))) x!.SetValue(obj, null, null);
        }
        return obj;
    }

    public static List<T> Nullifies<T>(IEnumerable<T> obj)
    {
        foreach (var o in obj)
        {
            var type = typeof(T);
            foreach (var x in type.GetProperties())
            {
                if ((x.PropertyType.IsClass && x.PropertyType != typeof(string) && x.PropertyType != typeof(byte[])) || (x.PropertyType.IsGenericType && x.PropertyType != typeof(DateTime?) && x.PropertyType != typeof(TimeSpan?)) || (x.PropertyType.IsArray && x.PropertyType != typeof(byte[]))) x!.SetValue(o, null, null);
            }
        }
        return obj.ToList();
    }

    public static bool HaveNullProperty(object myObject)
    {
        return myObject.GetType().GetProperties()
            .Where(x => x.PropertyType == typeof(string) || x.PropertyType == typeof(int))
            .Select(y => y.GetValue(myObject)?.ToString())
            .Any(z => string.IsNullOrWhiteSpace(z) || z == "0");
    }

    #endregion Object Utility

    #region Collections Utility (LINQ Specific)

    public static IEnumerable<KeyValuePair<int, object>> GetPropertiesName(object obj)
    {
        int i = 0;
        foreach (var p in obj.GetType().GetProperties())
        {
            yield return new(i, p.Name);
            i++;
        }
    }

    public static IEnumerable<KeyValuePair<int, object>> GetPropertiesValues(object obj)
    {
        int i = 0;
        foreach (var p in obj.GetType().GetProperties())
        {
            yield return new(i, p.GetValue(obj, null)!);
            i++;
        }
    }

    public static KeyValuePair<TKey, TValue> GetKvp<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        return new(key, dictionary[key]);
    }

    public static List<string> GenerateYearPeriod(int startYear)
    {
        List<string> periods = [];
        for (int i = DateTime.Today.Year; i >= startYear; i--) periods.Add($"{i} - {i + 1}");
        return periods;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        Random rng = new();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

    #endregion Collections Utility (LINQ Specific)

    #region Color Utility

    //public static Color ToSWMColor(this System.Drawing.Color color) => Color.FromArgb(color.A, color.R, color.G, color.B);

    //public static System.Drawing.Color ToSDColor(this Color color) => System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);

    //public static SolidColorBrush ChangeForegroundColor(Color color)
    //{
    //    ColorToHsl(color, out _, out _, out double luminosity);
    //    if (luminosity < 0.5)
    //        return new(Color.FromRgb(255, 255, 255));
    //    else
    //        return new(Color.FromRgb(0, 0, 0));
    //}

    ///// <summary>
    ///// True = Terlalu Gelap, False = Terlalu Terang, Null = OK
    ///// </summary>
    //public static bool? ColorValidation(Color color)
    //{
    //    ColorToHsl(color, out _, out _, out double luminosity);
    //    if (luminosity < 0.21)
    //        return true;
    //    else if (luminosity > 0.79)
    //        return false;
    //    else
    //        return null;
    //}

    //public static void ColorToHsl(Color color, out double hue, out double saturation, out double luminosity)
    //{
    //    double r = color.R / 255.0;
    //    double g = color.G / 255.0;
    //    double b = color.B / 255.0;

    //    double max = Math.Max(r, Math.Max(g, b));
    //    double min = Math.Min(r, Math.Min(g, b));

    //    // Calculate the hue
    //    hue = 0.0;
    //    if (max == r && g >= b)
    //    {
    //        if (max - min > 0)
    //        {
    //            hue = 60 * (g - b) / (max - min);
    //        }
    //    }
    //    else if (max == r && g < b)
    //    {
    //        hue = (60 * (g - b) / (max - min)) + 360;
    //    }
    //    else if (max == g)
    //    {
    //        hue = (60 * (b - r) / (max - min)) + 120;
    //    }
    //    else if (max == b)
    //    {
    //        hue = (60 * (r - g) / (max - min)) + 240;
    //    }

    //    // Calculate the luminosity
    //    luminosity = (max + min) / 2;

    //    // Calculate the saturation
    //    saturation = 0.0;
    //    if (luminosity > 0 && luminosity < 1)
    //    {
    //        saturation = (max - min) / (1 - Math.Abs((2 * luminosity) - 1));
    //    }
    //}

    #endregion Color Utility

    #region ID Generator

    /// <summary>Membuat Id baru untuk string. Secara otomatis mengisi kekosongan Id</summary>
    /// <remarks>
    ///     <para>string id = GenerateId(await appDbContext.Barang.Select(x => x.Id).ToListAsync(), 5, "BRG");</para>
    /// </remarks>
    /// <returns>BRG-00001 sampai BRG-99999</returns>
    public static string GenerateId(IEnumerable<string> ids, int digit, string prefix)
    {
        int count = 1;
        foreach (var x in ids.Order())
        {
            if (ToInt(x.Right(digit)) != count) break;
            count++;
        }
        return $"{prefix}-{count.ToString(new string('0', digit))}";
    }

    /// <summary>Membuat kumpulan Id baru bertipe string (mengisi kekosongan Id)</summary>
    /// <remarks>
    ///     <para>string[] ids = GenerateId(await appDbContext.Barang.Select(x => x.Id).ToListAsync(), 5, "BRG", collections.Count);</para>
    /// </remarks>
    /// <returns>Jika pada database terdaftar ID (BRG-00001, BRG-00003, BRG-00005) dan ada 4 data baru, maka hasilnya (BRG-00002, BRG-00004, BRG-00006, BRG-00007)</returns>
    public static string[] GenerateId(IEnumerable<string> ids, int digit, string prefix, int newDataCount)
    {
        int count = 1;
        List<string> newIds = [];
        for (int i = 0; i < ids.Count() + newDataCount; i++)
        {
            newIds.Add($"{prefix}-{count.ToString(new string('0', digit))}");
            count++;
        }
        return newIds.Except(ids).Take(newDataCount).ToArray();
    }

    /// <summary>Membuat satu Id int baru (mengisi kekosongan)</summary>
    /// <remarks>
    ///     <para>int id = GenerateId(await appDbContext.DetailTransaksi.Select(x => x.Id).ToListAsync());</para>
    /// </remarks>
    /// <returns>Jika pada database terdaftar ID (1, 3, 4, 5), maka hasilnya 2. Jika pada database terdaftar ID dari 1 sampai 5, maka hasilnya 6.</returns>
    public static int GenerateId(IEnumerable<int> ids)
    {
        int count = 1;
        foreach (var x in ids.Order())
        {
            if (x != count) break;
            count++;
        }
        return count;
    }

    /// <summary>Membuat Id baru untuk transaksi berdasarkan tanggalnya.</summary>
    /// <remarks>
    ///     <para>string id = GenerateId("T", 3, transaksi.Tanggal, await appDbContext.Transaksi.Where(x => x.Tanggal.Date == transaksi.Tanggal.Date).Select(x => x.Id).ToListAsync());</para>
    /// </remarks>
    /// <returns>Jika hari ini tanggal 22 November 2024, maka : T-241122-001 sampai T-241122-999. Kemudian reset esoknya (tanggal 23).</returns>
    public static string GenerateId(string prefix, int idNumberDigitCount, DateTime? date, IEnumerable<string> idsThatDay)
    {
        string zeros = new('0', idNumberDigitCount);
        int lastNumber = ToInt(idsThatDay.Order().LastOrDefault(zeros).Right(idNumberDigitCount)) + 1;
        return $"{prefix}-{date:yyMMdd}-{lastNumber.ToString(zeros)}";
    }

    /// <summary>Membuat Id baru untuk transaksi berdasarkan tanggalnya (untuk multiple Ids). Tidak mengisi kekosongan</summary>
    /// <remarks>
    ///     <para>string[] ids = GenerateId("PBLN", 3, collections.Count, pembelian.Tanggal, await appDbContext.Pembelian.Where(x => x.Tanggal.Date == pembelian.Tanggal.Date).Select(x => x.Id).ToListAsync());</para>
    /// </remarks>
    /// <returns>Jika hari ini tanggal 22 November 2024, Nomor Id terakhirnya 84 dan ada 4 data baru, maka : (PBLN-241122-085, PBLN-241122-086, PBLN-241122-087, PBLN-241122-088).</returns>
    public static string[] GenerateId(string prefix, int idNumberDigitCount, int newDataCount, DateTime? date, IEnumerable<string> idsThatDay)
    {
        string zeros = new('0', idNumberDigitCount);
        List<string> newIds = [];
        int lastNumber = ToInt(idsThatDay.Order().LastOrDefault(zeros).Right(idNumberDigitCount));
        for (int i = lastNumber + 1; i <= lastNumber + 1 + newDataCount; i++)
        {
            newIds.Add($"{prefix}-{date:yyMMdd}-{i.ToString(zeros)}");
        }
        return [.. newIds];
    }

    /// <summary>Membuat kumpulan Id baru bertipe int (mengisi kekosongan Id)</summary>
    /// <remarks>
    ///     <para>int[] ids = GenerateId(appDbContext.FormulasiDetail.Select(x => x.Id).DefaultIfEmpty().Max(), appDbContext.FormulasiDetail.Select(x => x.Id), newDataCount);</para>
    /// </remarks>
    /// <returns>Jika pada database terdaftar ID (1, 3, 5) dan ada 4 data baru, maka hasilnya (2, 4, 6, 7)</returns>
    public static int[] GenerateId(int biggestId, IEnumerable<int> allIds, int newDataCount)
    {
        var list1 = allIds;
        var list2 = Enumerable.Range(1, biggestId + newDataCount);
        return list2.Except(list1).Take(newDataCount).ToArray();
    }

    #endregion ID Generator

    #region Others

    public static async void FireAndForgetSafeAsync(this Task task, IErrorHandler handler = null!)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            handler?.HandleError(ex);
        }
    }

    //public static void CheckToShutdown()
    //{
    //    bool mainViewExist = Application.Current.Windows.OfType<MainView>().Any(x => x.IsVisible);
    //    bool authViewExist = Application.Current.Windows.OfType<AuthView>().Any(x => x.IsVisible);
    //    if (!mainViewExist && !authViewExist) Application.Current.Shutdown();
    //}

    //public static bool ReadOnlyAccess(AuthorizationHandlerContext context, string entitas)
    //{
    //    return !context.User.HasClaim(entitas, "S0") && (context.User.HasClaim(entitas, "W1") || context.User.HasClaim(entitas, "W2") || context.User.HasClaim(entitas, "S1") || context.User.HasClaim(entitas, "S2"));
    //}

    //public static bool ReadWriteAccess(AuthorizationHandlerContext context, string entitas)
    //{
    //    return !context.User.HasClaim(entitas, "S0") && !context.User.HasClaim(entitas, "S1") && (context.User.HasClaim(entitas, "W2") || context.User.HasClaim(entitas, "S2"));
    //}

    #endregion Others

    #region Networking & Connectivity

    public static string GetIPv4()
    {
        var thisIPs = (Dns.GetHostEntry(Dns.GetHostName())).AddressList;
        foreach (var thisIP in thisIPs) if (thisIP.AddressFamily == AddressFamily.InterNetwork) return thisIP.ToString();
        return string.Empty;
    }

    /// <summary>Cara cek hostname : tulis "hostname" di Command Prompt. Berlaku untuk PC yg terkoneksi ke Jaringan / Wi-Fi yang sama </summary>
    public static string GetIPv4Of(string hostname)
    {
        var host = Dns.GetHostEntry(hostname);
        foreach (var ip in host.AddressList) if (ip.AddressFamily == AddressFamily.InterNetwork) return ip.ToString();
        return string.Empty;
    }

    public static async Task<string> GetIPv4Async()
    {
        var thisIPs = (await Dns.GetHostEntryAsync(Dns.GetHostName())).AddressList;
        foreach (var thisIP in thisIPs) if (thisIP.AddressFamily == AddressFamily.InterNetwork) return thisIP.ToString();
        return string.Empty;
    }

    /// <summary>Cara cek hostname : tulis "hostname" di Command Prompt. Berlaku untuk PC yg terkoneksi ke Jaringan / Wi-Fi yang sama </summary>
    public static async Task<string> GetIPv4OfAsync(string hostname)
    {
        var host = await Dns.GetHostEntryAsync(hostname);
        foreach (var ip in host.AddressList) if (ip.AddressFamily == AddressFamily.InterNetwork) return ip.ToString();
        return string.Empty;
    }

    #endregion Networking & Connectivity

    #region Regex
    public static bool IsNumberCommaSeparatorMatch(string text)
    {
        Regex rgx = NumberCommaSeparatorRegex();
        return rgx.IsMatch(text);
    }

    [GeneratedRegex(@"^\d+(,\d+)*$")] private static partial Regex NumberCommaSeparatorRegex();

    [GeneratedRegex("\\s+")] private static partial Regex TitleCaseRegex();

    [GeneratedRegex("([a-z](?=[A-Z]|[0-9])|[A-Z](?=[A-Z][a-z]|[0-9])|[0-9](?=[^0-9]))")] private static partial Regex ReplaceTitleCaseRegex();

    [GeneratedRegex("([A-Z])")] private static partial Regex CapitalLetter();

    [GeneratedRegex(@"(^\w+)\s+(.+)\s+(\w+$)")] private static partial Regex FirstRestLastWord();
    [GeneratedRegex("[^0-9]")]
    private static partial Regex GetNumberRegex();

    #endregion Regex
}

public interface IErrorHandler
{
    void HandleError(Exception ex);
}