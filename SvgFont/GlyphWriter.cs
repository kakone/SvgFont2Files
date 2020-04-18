using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Svg
{
    /// <summary>
    /// Glyph writer to add a glyph in a SVG font
    /// </summary>
    internal class GlyphWriter : IWriter
    {
        private const string Glyph = "<glyph glyph-name=\"{0}\" unicode=\"&#x{1:X};\" d=\"{2}\" />";

        /// <summary>
        /// Initializes a new instance of the <see cref="GlyphWriter"/> class
        /// </summary>
        /// <param name="textWriter">text writer</param>
        public GlyphWriter(TextWriter textWriter)
        {
            TextWriter = textWriter;
        }

        private TextWriter TextWriter { get; }

        /// <inheritdoc/>
        public bool Flip => false;

        /// <inheritdoc/>
        public async Task WriteAsync(SvgFile svgFile, string? outputFolder, CancellationToken cancellationToken)
        {
            await TextWriter.WriteLineAsync(string.Format(Glyph, svgFile.GlyphName, svgFile.Unicode, svgFile.PathData));
        }
    }
}
