namespace ImageWave.Core;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public sealed class ImageMatch
{
  public static double Match(Image<Rgba32> img1, Image<Rgba32> img2, int hashSideSize = 256)
  {
    var img1TopApproxCoeffs = GetTopApproxCoeffs(img1, hashSideSize);
    var img2TopApproxCoeffs = GetTopApproxCoeffs(img2, hashSideSize);
    if (img1TopApproxCoeffs.Count != img2TopApproxCoeffs.Count)
    {
      throw new ArgumentOutOfRangeException($"Cannot compare coeffients with different sizes: {img1TopApproxCoeffs.Count} vs {img2TopApproxCoeffs.Count}");
    }

    var sumDistSq = 0d;
    for (var i = 0; i < img1TopApproxCoeffs.Count; i++)
    {
      var distSq = Math.Pow(img1TopApproxCoeffs[i] - img2TopApproxCoeffs[i], 2);
      sumDistSq += distSq;
    }

    var rmsDist = Math.Sqrt(sumDistSq / img1TopApproxCoeffs.Count);
    var match = (1d - rmsDist) * 100d;

    return match < 0 ? 0 : match;
  }

  private static List<double> GetTopApproxCoeffs(Image<Rgba32> img, int hashSideSize = 256)
  {
    var imgDecomp = ImageDecomposition.Decompose(img, hashSideSize);
    var imgApprox = imgDecomp.Single().Approximation;
    return imgApprox.ToList();
  }
}
