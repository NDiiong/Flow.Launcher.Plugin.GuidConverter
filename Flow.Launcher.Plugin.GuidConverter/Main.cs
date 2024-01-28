using MongoDB.Bson;
using System.Windows;

namespace Flow.Launcher.Plugin.GuidConverter
{
    /// <summary>
    /// Main.cs
    /// </summary>
    public class Main : IPlugin
    {
        private const string ICON_PATH = @"Images\icon.png";

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
                }
                else
                {
                    var bytes = Convert.FromBase64String(query.Search);
                    var binaryData = new BsonBinaryData(bytes, BsonBinarySubType.UuidLegacy);
                    var guidValue = new Guid(bytes);

                    results.Add(new Result
                    {
                        Title = guidValue.ToString(),
                        SubTitle = "Copy to clipboard",
                        Action = _ =>
                        {
                            Clipboard.SetText(guidValue.ToString());
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
    }
}