using System;

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

        private string? _sourceUnicode;
        /// <summary>
        /// Gets or sets the source unicode
        /// </summary>
        public string SourceUnicode
        {
            get => _sourceUnicode ?? throw new ArgumentNullException();
            set => _sourceUnicode = GetHexValue(value);
        }

        private string? _destinationUnicode;
        /// <summary>
        /// Gets or sets the destination unicode
        /// </summary>
        public string? DestinationUnicode
        {
            get => _destinationUnicode;
            set => _destinationUnicode = GetHexValue(value);
        }

        private string? GetHexValue(string? value)
        {
            if (value == null)
            {
                return null;
            }

            if (value.StartsWith("&#x"))
            {
                value = value.Substring(3);
                return value.EndsWith(";") ? value[0..^1] : value;
            }
            if (value.StartsWith("0x"))
            {
                return value.Substring(2);
            }
            return value;
        }
    }
}
