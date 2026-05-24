using SevenZip;
using SevenZip.Compression.LZMA;

namespace SlimeLzma;

internal static class LzmaOptions
{
    public static void ApplyMaxCompression(Encoder encoder)
    {
        // Dictionary size (VERY important)
        encoder.SetCoderProperties(new[]
        {
            new CoderPropID[] {
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
                1 << 26,   // 64MB dictionary (ή 1<<27 = 128MB αν θες πιο βαριά συμπίεση)
                2,
                3,
                0,
                2,
                273,      // MAX
                "BT4",    // καλύτερο compression
                false
            }
        });
    }
}