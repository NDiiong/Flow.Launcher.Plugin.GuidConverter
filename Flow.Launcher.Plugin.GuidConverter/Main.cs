using System.Windows;

namespace Flow.Launcher.Plugin.GuidConverter
{
    /// <summary>
    /// Main.cs
    /// </summary>
    public class Main : IPlugin
    {
        private const string HEX_DIGITS = "0123456789abcdef";

        private const string ICON_PATH = @"Images\icon.png";

        private const string BASE64_DIGITS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

        /// <summary>
        /// Invalid String
        /// </summary>
        public static Result InvalidString => new() { Title = "Bindata3", SubTitle = "String is invalid or missing", IcoPath = ICON_PATH };

        /// <summary>
        /// Empty
        /// </summary>
        public static Result Empty => new() { Title = "Bindata3", SubTitle = "Please input a string", IcoPath = ICON_PATH };

        /// <summary>
        /// Method Init
        /// </summary>
        /// <param name="context"></param>
        public void Init(PluginInitContext context)
        {
        }

        /// <summary>
        /// Method Query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<Result> Query(Query query)
        {
            if (string.IsNullOrWhiteSpace(query.Search))
                return new List<Result> { Empty };

            try
            {
                var results = new List<Result>();
                if (Guid.TryParse(query.Search, out var result))
                {
                    var base64 = ConvertGuid2Bindata3(result);
                    results.Add(new Result
                    {
                        Title = $"Bindata3: {base64}",
                        SubTitle = "Copy to clipboard",
                        Action = _ =>
                        {
                            Clipboard.SetText(base64);
                            return true;
                        },
                        IcoPath = ICON_PATH
                    });
                }
                else
                {
                    var uuid = ConvertBindata2Guid(query.Search);
                    results.Add(new Result
                    {
                        Title = $"Guid: {uuid}",
                        SubTitle = "Copy to clipboard",
                        Action = _ =>
                        {
                            Clipboard.SetText(uuid);
                            return true;
                        },
                        IcoPath = ICON_PATH
                    });
                }

                return results;
            }
            catch (Exception)
            {
                return new List<Result> { InvalidString };
            }
        }

        private static string ConvertBindata2Guid(string search)
        {
            var hex = Base64ToHex(search);
            var a = hex.Substring(6, 2) + hex.Substring(4, 2) + hex.Substring(2, 2) + hex.Substring(0, 2);
            var b = hex.Substring(10, 2) + hex.Substring(8, 2);
            var c = hex.Substring(14, 2) + hex.Substring(12, 2);
            var d = hex.Substring(16, 16);
            hex = a + b + c + d;
            var uuid = hex.Substring(0, 8) + '-' + hex.Substring(8, 4) + '-' + hex.Substring(12, 4) + '-' + hex.Substring(16, 4) + '-' + hex.Substring(20, 12);
            return uuid;
        }

        private static string Base64ToHex(string base64)
        {
            var hex = "";
            for (var i = 0; i < 24;)
            {
                var e1 = BASE64_DIGITS.IndexOf(base64[i++]);
                var e2 = BASE64_DIGITS.IndexOf(base64[i++]);
                var e3 = BASE64_DIGITS.IndexOf(base64[i++]);
                var e4 = BASE64_DIGITS.IndexOf(base64[i++]);
                var c1 = (e1 << 2) | (e2 >> 4);
                var c2 = ((e2 & 15) << 4) | (e3 >> 2);
                var c3 = ((e3 & 3) << 6) | e4;
                hex += HEX_DIGITS[c1 >> 4];
                hex += HEX_DIGITS[c1 & 15];
                if (e3 != 64)
                {
                    hex += HEX_DIGITS[c2 >> 4];
                    hex += HEX_DIGITS[c2 & 15];
                }
                if (e4 != 64)
                {
                    hex += HEX_DIGITS[c3 >> 4];
                    hex += HEX_DIGITS[c3 & 15];
                }
            }
            return hex;
        }

        private static string ConvertGuid2Bindata3(Guid result)
        {
            var hex = result.ToString("N");
            var a = hex.Substring(6, 2) + hex.Substring(4, 2) + hex.Substring(2, 2) + hex.Substring(0, 2);
            var b = hex.Substring(10, 2) + hex.Substring(8, 2);
            var c = hex.Substring(14, 2) + hex.Substring(12, 2);
            var d = hex.Substring(16, 16);
            hex = a + b + c + d;
            return HexToBase64(hex);
        }

        private static string HexToBase64(string hex)
        {
            var base64 = "";
            int group;
            for (var i = 0; i < 30; i += 6)
            {
                group = Convert.ToInt32(hex.Substring(i, 6), 16);
                base64 += BASE64_DIGITS[(group >> 18) & 0x3f];
                base64 += BASE64_DIGITS[(group >> 12) & 0x3f];
                base64 += BASE64_DIGITS[(group >> 6) & 0x3f];
                base64 += BASE64_DIGITS[group & 0x3f];
            }
            group = Convert.ToInt32(hex.Substring(30, 2), 16);
            base64 += BASE64_DIGITS[(group >> 2) & 0x3f];
            base64 += BASE64_DIGITS[(group << 4) & 0x3f];
            base64 += "==";
            return base64;
        }
    }
}