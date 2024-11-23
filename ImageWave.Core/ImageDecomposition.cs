namespace ImageWave.Core;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using WaveletStudio;
using WaveletStudio.Wavelet;

internal sealed class ImageDecomposition
{
  public static List<DecompositionLevel> Decompose(Image<Rgba32> img, int hashSideSize = 256)
  {
    // resize img to 256x256px (by default) or with configured size 
    img.Mutate(ctx => ctx
      .Resize(new Size(hashSideSize, hashSideSize))
      .BlackWhite());

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
            ref var px = ref pxRow[x];
            var isWhite = IsWhite(ref px);
            input.Add(isWhite ? 1d : 0d);
          }
        }
        else
        {
          for (var x = 0; x < pxRow.Length - 1; x++)
          {
            ref var px = ref pxRow[x];
            var isWhite = IsWhite(ref px);
            input.Add(isWhite ? 1d : 0d);
          }
        }
      }
    });
    var signal = new Signal(input.ToArray());
    var wavelet = MotherWavelet.LoadFromName("haar");
    var decomp = DWT.ExecuteDWT(signal, wavelet, 1);

    return decomp;
  }

  private static bool IsWhite(ref Rgba32 px)
  {
    return ToGrayscale(px) > 45;
  }

  private static double ToGrayscale(Rgba32 px)
  {
    // standard RGB --> grayscale conversion
    return (0.299 * px.R + 0.587 * px.G + 0.114 * px.B);
  }
}
