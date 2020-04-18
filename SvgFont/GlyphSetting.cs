using System.ComponentModel;

namespace Svg
{
    /// <summary>
    /// Setting
    /// </summary>
    public class GlyphSetting
    {
        /// <summary>
        /// Gets or sets the glyph name
        /// </summary>
        public string? GlyphName { get; set; }

        /// <summary>
        /// Gets the source file
        /// </summary>
        public string? SourceFile { get; set; }

        /// <summary>
        /// Gets or sets the source unicode
        /// </summary>
        public int SourceUnicode { get; set; }

        /// <summary>
        /// Gets or sets the source unicode as hexadecimal string
        /// </summary>
        public string Source
        {
            get => SourceUnicode.ToString("X");
            set => SourceUnicode = (int)new Int32Converter().ConvertFromInvariantString(value);
        }

        /// <summary>
        /// Gets or sets the destination unicode
        /// </summary>
        public int? DestinationUnicode { get; set; }

        /// <summary>
        /// Gets or sets the source unicode as hexadecimal string
        /// </summary>
        public string? Destination
        {
            get => DestinationUnicode?.ToString("X");
            set => DestinationUnicode = string.IsNullOrEmpty(value) ? (int?)null : (int)new Int32Converter().ConvertFromInvariantString(value);
        }
    }
}
