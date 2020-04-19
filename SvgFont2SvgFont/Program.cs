using Svg;
using System.Threading.Tasks;

namespace SvgFont2SvgFont
{
    class Program
    {
        /// <summary>
        /// Gets some icons from one or multiple SVG fonts and make a new SVG font
        /// </summary>
        /// <param name="config">configuration file path for advanced setting</param>
        /// <param name="output">output file path</param>
        /// <returns>a <see cref="Task"/> representing the asynchronous operation</returns>
        static async Task Main(string config, string? output = null)
        {
            await new SvgFont().ToSvgFontAsync(config, output);
        }
    }
}
