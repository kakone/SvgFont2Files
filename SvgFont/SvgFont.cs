using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Svg
{
    /// <summary>
    /// SVG font class
    /// </summary>
    public class SvgFont
    {
        private const string DefaultFontFilename = "font.svg";
        private const string FontTagName = "font";
        private const string FontFaceTagName = "font-face";
        private const string GlyphTagName = "glyph";
        private const string HorizAdvXAttributeName = "horiz-adv-x";
        private const string UnitsPerEmAttributeName = "units-per-em";
        private const string AscentAttributeName = "ascent";
        private const string DescentAttributeName = "descent";
        private const string PathDataAttributeName = "d";
        private const string UnicodeAttributeName = "unicode";
        private const string GlyphNameAttributeName = "glyph-name";
        internal const int DefaultSize = 2048;
        private const string SvgFontFileHeader = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>{0}" +
            "<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.0//EN\" \"http://www.w3.org/TR/2001/REC-SVG-20010904/DTD/svg10.dtd\">{0}" +
            "<svg xmlns=\"http://www.w3.org/2000/svg\">{0}" +
            "<defs>{0}" +
            "<font id=\"{2}\" horiz-adv-x=\"{1}\">{0}" +
            "<font-face units-per-em=\"{1}\" ascent=\"{1}\" />{0}" +
            "<missing-glyph horiz-adv-x=\"{1}\" />{0}";
        private const string SvgFontFileFooter = "</font>{0}" +
            "</defs>{0}" +
            "</svg>{0}";

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgFont"/> class
        /// </summary>
        public SvgFont() : this(new SvgFileWriter())
        {
        }

        private SvgFont(IWriter writer)
        {
            Writer = writer;
        }

        private IWriter Writer { get; }

        private int Parse(XmlNode xmlNode, string attributeName, int defaultValue)
        {
            var attributeValue = xmlNode?.Attributes[attributeName]?.Value;
            return string.IsNullOrEmpty(attributeValue) ? defaultValue : int.Parse(attributeValue);
        }

        private string? GetCellValue(SharedStringTable? stringTable, IEnumerable<Cell> cells, int index)
        {
            if (index >= cells.Count())
            {
                return null;
            }

            var cell = cells.ElementAt(index);
            return (cell.DataType.Value == CellValues.SharedString ? stringTable.ElementAt(int.Parse(cell.CellValue.Text)).InnerText :
                cell.CellValue.Text).Trim();
        }

        private async Task<IEnumerable<GlyphSetting>?> ReadConfigFileAsync(string? config, CancellationToken cancellationToken)
        {
            if (config == null)
            {
                return null;
            }

            if (Path.GetExtension(config).Equals(".xlsx", StringComparison.InvariantCultureIgnoreCase))
            {
                using var spreadsheetDocument = SpreadsheetDocument.Open(config, false);
                var firstRow = true;
                var glyphSettings = new List<GlyphSetting>();
                var workbookPart = spreadsheetDocument.WorkbookPart;
                var stringTable = workbookPart.SharedStringTablePart.SharedStringTable;
                foreach (var worksheetPart in workbookPart.WorksheetParts)
                {
                    foreach (var sheetData in worksheetPart.Worksheet.Elements<SheetData>())
                    {
                        foreach (var row in sheetData.Elements<Row>())
                        {
                            if (firstRow)
                            {
                                firstRow = false;
                                continue;
                            }

                            var cells = row.Elements<Cell>();
                            var destination = GetCellValue(stringTable, cells, 3);
                            if (!string.IsNullOrWhiteSpace(destination))
                            {
                                glyphSettings.Add(new GlyphSetting()
                                {
                                    GlyphName = GetCellValue(stringTable, cells, 0),
                                    SourceFile = GetCellValue(stringTable, cells, 1),
                                    SourceUnicode = GetCellValue(stringTable, cells, 2)!,
                                    DestinationUnicode = destination
                                });
                            }
                        }
                    }
                }
                return glyphSettings;
            }

            using var settingsFile = File.OpenRead(config);
            return await JsonSerializer.DeserializeAsync<GlyphSetting[]>(settingsFile,
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true },
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Converts a SVG font to individuals SVG files
        /// </summary>
        /// <param name="config">configuration file path</param>
        /// <param name="outputFolder">output folder</param>
        /// <param name="cancellationToken">a token that may be used to cancel the operation</param>
        /// <returns>a <see cref="Task"/> representing the asynchronous operation</returns>
        public async Task ToFilesAsync(string config, string? outputFolder = null, CancellationToken cancellationToken = default)
        {
            var settings = await ReadConfigFileAsync(config, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            foreach (var group in settings.Where(s => s.DestinationUnicode != null).GroupBy(s => s.SourceFile))
            {
                var input = group.Key;
                if (input == null)
                {
                    throw new ArgumentNullException(nameof(GlyphSetting.SourceFile));
                }
                await ToFilesAsync(Path.IsPathRooted(input) ? input : Path.Combine(Path.GetDirectoryName(config), input), outputFolder, group.ToList());
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Converts a SVG font to individuals SVG files
        /// </summary>
        /// <param name="input">SVG font file path</param>
        /// <param name="outputFolder">output folder</param>
        /// <param name="config">configuration file path</param>
        /// <param name="cancellationToken">a token that may be used to cancel the operation</param>
        /// <returns>a <see cref="Task"/> representing the asynchronous operation</returns>
        public async Task ToFilesAsync(string? input = null, string? outputFolder = null, string? config = null, CancellationToken cancellationToken = default)
        {
            if (input == null)
            {
                await ToFilesAsync(config!, outputFolder, cancellationToken);
                return;
            }

            var settings = await ReadConfigFileAsync(config, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            await ToFilesAsync(input, outputFolder, settings, cancellationToken);
        }

        private Task ToFilesAsync(string input, string? outputFolder = null, IEnumerable<GlyphSetting>? settings = null, CancellationToken cancellationToken = default)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(input);
            return ToFilesAsync(xmlDocument, outputFolder, settings, cancellationToken);
        }

        /// <summary>
        /// Converts a SVG font to individuals SVG files
        /// </summary>
        /// <param name="xmlDocument">XML document</param>
        /// <param name="outputFolder">output folder</param>
        /// <param name="settings">advanced settings</param>
        /// <param name="cancellationToken">a token that may be used to cancel the operation</param>
        /// <returns>a <see cref="Task"/> representing the asynchronous operation</returns>
        public async Task ToFilesAsync(XmlDocument xmlDocument, string? outputFolder = null, IEnumerable<GlyphSetting>? settings = null,
            CancellationToken cancellationToken = default)
        {
            var defaultCharWidth = Parse(xmlDocument.GetElementsByTagName(FontTagName).Cast<XmlNode>().FirstOrDefault(), HorizAdvXAttributeName, DefaultSize);
            var fontFace = xmlDocument.GetElementsByTagName(FontFaceTagName).Cast<XmlNode>().FirstOrDefault();
            var charHeight = Parse(fontFace, UnitsPerEmAttributeName, DefaultSize);
            var charAscent = Parse(fontFace, AscentAttributeName, DefaultSize);
            var charDescent = Parse(fontFace, DescentAttributeName, 0);

            if (!string.IsNullOrEmpty(outputFolder))
            {
                new DirectoryInfo(outputFolder).Create();
            }

            foreach (XmlNode? xmlNode in xmlDocument.GetElementsByTagName(GlyphTagName))
            {
                if (xmlNode == null)
                {
                    continue;
                }

                foreach (var glyph in GetFiles(xmlNode, defaultCharWidth, charHeight, charAscent, charDescent, settings))
                {
                    Console.WriteLine($"Save {glyph.GlyphName}");
                    await Writer.WriteAsync(glyph, outputFolder, cancellationToken);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        private IEnumerable<SvgFile> GetFiles(XmlNode xmlNode, int defaultCharWidth = DefaultSize, int charHeight = DefaultSize, int charAscent = DefaultSize,
            int charDescent = 0, IEnumerable<GlyphSetting>? settings = null)
        {
            var pathData = xmlNode.Attributes[PathDataAttributeName]?.Value;
            if (string.IsNullOrEmpty(pathData))
            {
                return Array.Empty<SvgFile>();
            }

            var sourceUnicode = xmlNode.Attributes[UnicodeAttributeName]?.Value;
            if (string.IsNullOrEmpty(sourceUnicode))
            {
                return Array.Empty<SvgFile>();
            }

            if (sourceUnicode.Length == 1)
            {
                sourceUnicode = ((int)sourceUnicode.First()).ToString("X");
            }

            var files = new List<SvgFile>();
            if (settings == null)
            {
                files.Add(GetFile(xmlNode, sourceUnicode, sourceUnicode, pathData, defaultCharWidth, charHeight, charAscent, charDescent));
            }
            else
            {
                foreach (var setting in settings.Where(s => s.SourceUnicode.Equals(sourceUnicode, StringComparison.InvariantCultureIgnoreCase)))
                {
                    files.Add(GetFile(xmlNode, sourceUnicode, setting?.DestinationUnicode ?? sourceUnicode, pathData,
                        defaultCharWidth, charHeight, charAscent, charDescent, setting!.GlyphName));
                }
            }
            return files;
        }

        private SvgFile GetFile(XmlNode xmlNode, string sourceUnicode, string destUnicode, string pathData, int defaultCharWidth, int charHeight,
            int charAscent, int charDescent, string? glyphName = null)
        {
            if (string.IsNullOrEmpty(glyphName))
            {
                glyphName = xmlNode.Attributes[GlyphNameAttributeName]?.Value;
                if (string.IsNullOrEmpty(glyphName))
                {
                    glyphName = $"uni{sourceUnicode:X}";
                }
            }

            var skPath = SKPath.ParseSvgPathData(pathData);
            skPath.Transform(SKMatrix.MakeTranslation(0, Writer.Flip ? -charAscent : charDescent));
            skPath.Transform(SKMatrix.MakeScale((float)DefaultSize / Parse(xmlNode, HorizAdvXAttributeName, defaultCharWidth),
                (Writer.Flip ? -1 : 1) * (float)DefaultSize / charHeight));
            return new SvgFile(glyphName, destUnicode, skPath.Simplify().ToSvgPathData());
        }

        /// <summary>
        /// Gets some icons from one or multiple SVG fonts and make a new SVG font
        /// </summary>
        /// <param name="config">configuration file path for advanced setting</param>
        /// <param name="output">output file path</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>a <see cref="Task"/> representing the asynchronous operation</returns>
        public async Task ToSvgFontAsync(string config, string? output = null, CancellationToken cancellationToken = default)
        {
            using var textWriter = new StreamWriter(output ?? DefaultFontFilename, false, Encoding.UTF8);
            await textWriter.WriteAsync(string.Format(SvgFontFileHeader, Environment.NewLine, DefaultSize, Path.GetFileNameWithoutExtension(output)));
            await new SvgFont(new GlyphWriter(textWriter)).ToFilesAsync(null, Path.GetDirectoryName(output), config, cancellationToken);
            await textWriter.WriteAsync(string.Format(SvgFontFileFooter, Environment.NewLine));
        }
    }
}
