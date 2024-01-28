dotnet publish Flow.Launcher.Plugin.GuidConverter -c Release -r win-x64
Compress-Archive -LiteralPath Flow.Launcher.Plugin.GuidConverter/bin/Release/win-x64/publish -DestinationPath Flow.Launcher.Plugin.GuidConverter/bin/GuidConverter.zip -Force
