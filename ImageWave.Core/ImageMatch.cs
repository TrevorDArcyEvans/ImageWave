namespace ImageWave.Core;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using WaveletStudio.Wavelet;

public sealed class ImageMatch
{
  public static double Match(Image<Rgba32> img1, Image<Rgba32> img2, int hashSideSize = 32)
  {
    var imgDecomp1 = ImageDecomposition.Decompose(img1);
    var imgDecomp2 = ImageDecomposition.Decompose(img2);
    return 1.0;
  }
}
