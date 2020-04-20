using Svg;
using System.Threading.Tasks;

namespace SvgFont2Files
{
    class Program
    {
        /// <summary>
        /// Converts a SVG font to individuals SVG files
        /// </summary>
        /// <param name="input">SVG font file path</param>
        /// <param name="output">output folder</param>
        /// <param name="config">config file path for advanced setting</param>
        /// <returns>a <see cref="Task"/> representing the asynchronous operation</returns>
        static Task Main(string? input = null, string? output = null, string? config = null)
        {
            return new SvgFont(true).ToFilesAsync(input, output, config);
        }
    }
}
