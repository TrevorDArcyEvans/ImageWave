namespace ImageWave.Core;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using WaveletStudio;
using WaveletStudio.Wavelet;

internal static class ImageDecomposition
{
  public static List<DecompositionLevel> Decompose(Image<Rgba32> img, int hashSideSize = 256)
  {
    // resize img to 256x256px (by default) or with configured size 
    // binary threshold of 0.33 is empirically determined
    img.Mutate(ctx => ctx
      .Resize(new Size(hashSideSize, hashSideSize))
      .BinaryThreshold(0.33f));

    var input = new List<double>();

    img.ProcessPixelRows(acc =>
    {
      for (var y = 0; y < acc.Height; y++)
      {
        var pxRow = acc.GetRowSpan(y);

        // implement 'winding' to marginally improve pathologically rotated cases
        //
        //    1111    vs    1100
        //    1111          1100
        //    0000          1100
        //    0000          1100
        //
        // input signal:
        //
        //    1111111100000000 vs 1100001111000011
        if (y % 2 == 0)
        {
          for (var x = pxRow.Length - 1; x >= 0; x--)
          {
            ProcessPixel(pxRow, x);
          }
        }
        else
        {
          for (var x = 0; x < pxRow.Length - 1; x++)
          {
            ProcessPixel(pxRow, x);
          }
        }
      }
    });
    var signal = new Signal(input.ToArray());
    var wavelet = MotherWavelet.LoadFromName("haar");
    var decomp = DWT.ExecuteDWT(signal, wavelet, 1);

    return decomp;

    void ProcessPixel(Span<Rgba32> span, int i)
    {
      ref var px = ref span[i];
      input.Add(px.R == 255 ? 1d : 0d);
    }
  }
}
