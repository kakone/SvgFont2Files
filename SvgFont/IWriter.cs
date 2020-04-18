using System.Threading;
using System.Threading.Tasks;

namespace Svg
{
    /// <summary>
    /// Interface for writers
    /// </summary>
    internal interface IWriter
    {
        /// <summary>
        /// Gets a value indicating whether the icon should be flipped or not
        /// </summary>
        bool Flip { get; }

        /// <summary>
        /// Saves a SVG file
        /// </summary>
        /// <param name="svgFile">svg file</param>
        /// <param name="outputFolder">output folder</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>a <see cref="Task"/> representing the asynchronous operation</returns>
        Task WriteAsync(SvgFile svgFile, string? outputFolder, CancellationToken cancellationToken);
    }
}
