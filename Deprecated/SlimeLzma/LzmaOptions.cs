using SevenZip;
using SevenZip.Compression.LZMA;

namespace SlimeLzma;

internal static class LzmaOptions
{
    public static void ApplyMaxCompression(Encoder encoder)
    {
        encoder.SetCoderProperties(
            new CoderPropID[]
            {
                CoderPropID.DictionarySize,
                CoderPropID.PosStateBits,
                CoderPropID.LitContextBits,
                CoderPropID.LitPosBits,
                CoderPropID.Algorithm,
                CoderPropID.NumFastBytes,
                CoderPropID.MatchFinder,
                CoderPropID.EndMarker
            },
            new object[]
            {
                1 << 26,   // 64 MB dictionary
                2,
                3,
                0,
                2,
                273,
                "BT4",
                false
            });
    }
}