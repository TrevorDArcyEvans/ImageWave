namespace ImageWave.Core;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public sealed class ImageMatch
{
  public static double Match(Image<Rgba32> img1, Image<Rgba32> img2, int hashSideSize = 256)
  {
    var img1TopApproxCoeffs = GetDetailsCoeffs(img1, hashSideSize);
    var img2TopApproxCoeffs = GetDetailsCoeffs(img2, hashSideSize);
    if (img1TopApproxCoeffs.Count != img2TopApproxCoeffs.Count)
    {
      throw new ArgumentOutOfRangeException($"Cannot compare coeffients with different sizes: {img1TopApproxCoeffs.Count} vs {img2TopApproxCoeffs.Count}");
    }

    var sumDistSq = img1TopApproxCoeffs.Select((t, i) => Math.Pow(t - img2TopApproxCoeffs[i], 2)).Sum();
    var rmsDist = Math.Sqrt(sumDistSq / img1TopApproxCoeffs.Count);
    var match = (1d - rmsDist) * 100d;

    return match < 0 ? 0 : match;
  }

  private static List<double> GetDetailsCoeffs(Image<Rgba32> img, int hashSideSize = 256)
  {
    var imgDecomp = ImageDecomposition.Decompose(img, hashSideSize);
    var imgApprox = imgDecomp.Single().Details;
    return imgApprox.ToList();
  }
}
