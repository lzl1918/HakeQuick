# Hake Quick

## What's new:
2017/7/3.1
- now log file is not always occupied and is able to be read.
- log buffer is changed from 5 to 1, log to file IMMEDIATELY!
- arguments are supported in runner configuration file.
```JSON
{
    "command": "network and sharing center",
    "path": "control.exe",
    "icon": "network.png",
    "args": ["/name", "Microsoft.NetworkAndSharingCenter"],
    "workingdir": "C:\\Windows\\System32"
}
```

2017/6/27.1
- `ILoggerFactory` and `ILogger` are added to built-in services, plugins can output logs.

2017/6/26.1
- now you can set default working directory to shortcuts.
- you can set option `admin` to `true` in runner configuration file to run programs in administrator privilege.

## Introduction
A tool adding context menu to program window.

Currently supported feature:
- Shortcuts
- chrome bookmarks search

![demo](https://raw.githubusercontent.com/lzl1918/HakeQuick/master/docs/sample.gif)

## How to build
1. Clone current repository, as well as submodules.
```
git clone https://github.com/lzl1918/HakeQuick
git submodule update --init --recursive
```
2. Open solution in visual studio, and select `Build`

3. Build results can be located in directory `build`

## How to contribute/build your own
[Program Guides](https://github.com/lzl1918/HakeQuick/tree/master/docs/index.md)