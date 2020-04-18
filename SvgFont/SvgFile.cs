using System.IO;

namespace Svg
{
    /// <summary>
    /// SVG file
    /// </summary>
    public class SvgFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SvgFile"/> class
        /// </summary>
        /// <param name="glyphName">glyph name</param>
        /// <param name="unicode">unicode value</param>
        /// <param name="pathData">path data</param>
        public SvgFile(string glyphName, int unicode, string pathData)
        {
            GlyphName = glyphName;
            Unicode = unicode;
            PathData = pathData;
        }

        /// <summary>
        /// Gets the glyph name
        /// </summary>
        public string GlyphName { get; }

        /// <summary>
        /// Gets the unicode value
        /// </summary>
        public int Unicode { get; }

        /// <summary>
        /// Gets the path data
        /// </summary>
        public string PathData { get; }

        /// <summary>
        /// Gets the file path
        /// </summary>
        /// <param name="outputFolder">output folder</param>
        /// <returns>the file path</returns>
        public string GetPath(string? outputFolder)
        {
            return Path.Combine(outputFolder ?? string.Empty, $"{GlyphName}-U0x{Unicode:X}.svg");
        }
    }
}
