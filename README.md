# Tinify Console

.NET console app wrapper for the [Tinify API](https://tinypng.com/developers/reference/dotnet).

## Running on Linux or Mac

Quick and dirty way to run on a Linux or Mac:

- Compile release
- Create a wrapper script:
    - `vi ~/usr/local/bin/tinify` (Or somewhere in your PATH)
    - Add content like the code block below
    - `chmod +x /usr/local/bin/tinify`

```bash
#!/usr/bin/env bash

# Assuming your dotnet installation is here...
/usr/local/share/dotnet/x64/dotnet  /MY-SWEET-REPOS/TinifyConsole/bin/Release/net6.0/TinifyConsole.dll "$@"
```

### Run it

```bash
api_key="MY_API_KEY"
tinify --api-key $api_key --file-pattern '*.jpg' --recurse --overwrite
```