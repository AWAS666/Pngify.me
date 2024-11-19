dotnet publish .\PngifyMe.Desktop\PngifyMe.Desktop.csproj -p:PublishProfile=Windows
dotnet publish .\PngifyMe.Desktop\PngifyMe.Desktop.csproj -p:PublishProfile=Linux
dotnet publish .\PngifyMe.Desktop\PngifyMe.Desktop.csproj -p:PublishProfile=MacOs

7z a ".\PngifyMe.Desktop\bin\Release\net9.0\publish\Pngify.Me-linux.zip" ".\PngifyMe.Desktop\bin\Release\net9.0\publish\linux-x64\"
7z a ".\PngifyMe.Desktop\bin\Release\net9.0\publish\Pngify.Me-win.zip" ".\PngifyMe.Desktop\bin\Release\net9.0\publish\win-x64\"
7z a ".\PngifyMe.Desktop\bin\Release\net9.0\publish\Pngify.Me-osx.zip" ".\PngifyMe.Desktop\bin\Release\net9.0\publish\osx-arm64\"

explorer ".\PngifyMe.Desktop\bin\Release\net9.0\publish\"