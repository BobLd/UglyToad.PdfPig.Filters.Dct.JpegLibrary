using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace UglyToad.PdfPig.Filters.Dct.JpegLibrary.Jpeg
{
    internal readonly struct JpegAdobeApplicationSpecific
    {
        private static ReadOnlySpan<byte> AdobeKey => "Adobe"u8;

        public byte ColorTransformCode { get; }

        private JpegAdobeApplicationSpecific(byte colorTransformCode)
        {
            ColorTransformCode = colorTransformCode;
        }

        public static bool TryParse(ReadOnlySequence<byte> buffer, [NotNullWhen(true)] out JpegAdobeApplicationSpecific? adobeApplicationSpecific)
        {
#if NO_READONLYSEQUENCE_FISTSPAN
            ReadOnlySpan<byte> firstSpan = buffer.First.Span;
#else
            ReadOnlySpan<byte> firstSpan = buffer.FirstSpan;
#endif

            // See 'Adobe Technical Note #5116'

            // DCTDecode ignores and skips any APPE marker segment that does not begin with the 'Adobe' 5-character string.
            if (firstSpan.Length >= 12 && firstSpan.Slice(0, 5).SequenceEqual(AdobeKey))
            {
                // One-byte color transform code
                byte colorTransformCode = firstSpan[11];

                adobeApplicationSpecific = new JpegAdobeApplicationSpecific(colorTransformCode);
                return true;
            }

            adobeApplicationSpecific = null;
            return false;
        }
    }
}
