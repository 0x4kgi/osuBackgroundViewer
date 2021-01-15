# osu! background viewer
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
* Functions to get files related to the beatmaps:
  * [henntix/osu-cleaner](https://github.com/henntix/osu-cleaner)
