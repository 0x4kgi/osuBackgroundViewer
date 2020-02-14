# osu! background viewer

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/29cb201e291b4d8fa9b8d2392c3b7077)](https://www.codacy.com/manual/0x4kgi/osuBackgroundViewer?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=0x4kgi/osuBackgroundViewer&amp;utm_campaign=Badge_Grade)
[![Build status](https://ci.appveyor.com/api/projects/status/g84xis70hquae5aj/branch/master?svg=true)](https://ci.appveyor.com/project/0x4kgi/osubackgroundviewer/branch/master)
[![CodeFactor](https://www.codefactor.io/repository/github/0x4kgi/osubackgroundviewer/badge)](https://www.codefactor.io/repository/github/0x4kgi/osubackgroundviewer)
[![GitHub license](https://img.shields.io/github/license/0x4kgi/osuBackgroundViewer)](https://github.com/0x4kgi/osuBackgroundViewer/blob/master/LICENSE)
[![HitCount](http://hits.dwyl.io/0x4kgi/osuBackgroundViewer.svg)](http://hits.dwyl.io/0x4kgi/osuBackgroundViewer)

![Image](https://i.imgur.com/saxtKc4.png)


If you find a cool bg while spamming F2 in-game and want to save the picture but you're too lazy to open the editor to open the song folder and has the time to open another exe to save the said bg, this program is for you.

## features

* search
* open in explorer

## issues

* no database, so it rescans your osu! songs directory everytime
* kinda slow on 10k+ folders
* looks really ugly

## planned

* quick save
* favorites
* support for video
* have a db to hopefully speed this up
* select the song on the osu! client (if possible)

## building from source

1. clone this repo
2. `cd osuBackgroundViewer`
3. `vs2019 .`
4. Build this in Visual Studio
5. Run

## installing without building

* [head over at the releases](https://github.com/0x4kgi/osuBackgroundViewer/releases)

## some code taken from
* Funtions to get files related to the beatmaps:
  * [henntix/osu-cleaner](https://github.com/henntix/osu-cleaner)
