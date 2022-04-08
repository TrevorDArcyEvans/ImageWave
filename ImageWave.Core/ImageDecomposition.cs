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
    // resize img to 16x16px (by default) or with configured size 
    img.Mutate(ctx => ctx
      .Resize(new Size(hashSideSize, hashSideSize))
      .BlackWhite());

    var input = new List<double>();
    img.ProcessPixelRows(acc =>
    {
      for (var y = 0; y < acc.Height; y++)
      {
        var pxRow = acc.GetRowSpan(y);
        for (var x = 0; x < pxRow.Length - 1; x++)
        {
          ref var px = ref pxRow[x];
          var isWhite = (px.R + px.G + px.B) > 45;
          input.Add(isWhite ? 1d : 0d);
        }
      }
    });
    var signal = new Signal(input.ToArray());
    var wavelet = MotherWavelet.LoadFromName("haar");
    var decomp = DWT.ExecuteDWT(signal, wavelet, 1);

    return decomp;
  }
}
