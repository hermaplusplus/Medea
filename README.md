![Travis (.com)](https://img.shields.io/travis/com/hermaplusplus/Medea?style=for-the-badge) ![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/hermaplusplus/Medea?style=for-the-badge) ![GitHub top language](https://img.shields.io/github/languages/top/hermaplusplus/Medea?style=for-the-badge) ![GitHub](https://img.shields.io/github/license/hermaplusplus/Medea?style=for-the-badge)
# Medea - WIP Discord Bot

## Installation

1. Install the .NET 3.1 SDK (including the runtime)
```
sudo apt-get update; \
  sudo apt-get install -y apt-transport-https && \
  sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-3.1
```

https://docs.microsoft.com/en-us/dotnet/core/install/linux-ubuntu

2. Clone and enter the repository
```
git clone https://github.com/hermaplusplus/Medea
cd Medea
```

3. Create a text file containing the token
```
nano token.txt
TOKENHERE
```
```ctrl+x``` --> ```y``` --> ```enter```

4. Run the bot
```
./run.sh
```
or
```
dotnet restore
dotnet build
dotnet run .\bin\Debug\netcoreapp3.1\Medea.dll
```