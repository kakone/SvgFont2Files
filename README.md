# SvgFont2Files
Command line tool to convert a SVG font to individual files.
```bat
SvgFont2Files.exe --input font.svg --output DestinationFolder
```
You can also customize the output files names, or use multiple input sources, with a .xlsx file (see [font.xlsx](SegoeMDL2AssetsAlternative/font.xlsx)).
```bat
SvgFont2Files.exe --config font.xlsx --output DestinationFolder
```
# SvgFont2SvgFont
Command line tool to extract some icons from one or multiple SVG fonts and make a new SVG font.
```bat
SvgFont2Files.exe --config font.xlsx --output DestinationFolder
```
The config file should respect this [xlsx](SegoeMDL2AssetsAlternative/font.xlsx) format.