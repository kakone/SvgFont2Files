using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Svg
{
    /// <summary>
    /// SVG file writer to save a glyph on disk
    /// </summary>
    internal class SvgFileWriter : IWriter
    {
        private const string SvgFile = "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 {0} {0}\"><path d=\"{1}\"/></svg>";

        /// <inheritdoc/>
        public bool Flip => true;

        /// <inheritdoc/>
        public Task WriteAsync(SvgFile svgFile, string? outputFolder, CancellationToken cancellationToken)
        {
            return File.WriteAllTextAsync(svgFile.GetPath(outputFolder), string.Format(SvgFile, SvgFont.DefaultSize, svgFile.PathData), Encoding.UTF8, cancellationToken);
        }
    }
}
