namespace ImageWave.UI.Web.Pages;

using Microsoft.AspNetCore.Components.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Core;

public sealed partial class Index
{
  private Image<Rgba32> _img1;
  private Image<Rgba32> _img2;
  private string _img1FileName { get; set; } = "Upload image 1";
  private string _img2FileName { get; set; } = "Upload image 2";
  private string _img1Url { get; set; } = GetDefaultImageString();
  private string _img2Url { get; set; } = GetDefaultImageString();
  private string _text { get; set; }

  private bool _canMatch
  {
    get => _img1 is not null && _img2 is not null;
  }

  private async Task LoadFile1(InputFileChangeEventArgs e)
  {
    _img1 = await GetImage(e.File);
    _img1FileName = e.File.Name;
    _text = _img1 is null ? $"<b>{_img1FileName}</b> --> unknown format" : string.Empty;
    _img1Url = await GetImageString(e.File);
  }

  private async Task LoadFile2(InputFileChangeEventArgs e)
  {
    _img2 = await GetImage(e.File);
    _img2FileName = e.File.Name;
    _text = _img2 is null ? $"<b>{_img2FileName}</b> --> unknown format" : string.Empty;
    _img2Url = await GetImageString(e.File);
  }

  private static async Task<Image<Rgba32>> GetImage(IBrowserFile file)
  {
    var data = file.OpenReadStream();
    var ms = new MemoryStream();
    await data.CopyToAsync(ms);
    ms.Seek(0, SeekOrigin.Begin);

    var info = await Image.IdentifyAsync(ms);
    if (info is null)
    {
      return null;
    }

    ms.Seek(0, SeekOrigin.Begin);
    return Image.Load<Rgba32>(ms);
  }

  private static async Task<string> GetImageString(IBrowserFile file)
  {
    var buffers = new byte[file.Size];
    await file.OpenReadStream().ReadAsync(buffers);
    return $"data:{file.ContentType};base64,{Convert.ToBase64String(buffers)}";
  }

  private static string GetDefaultImageString(int width = 64, int height = 64)
  {
    var img = new Image<Rgba32>(Configuration.Default, width, height);
    using var ms = new MemoryStream();
    img.SaveAsPng(ms);
    var bytes = ms.ToArray();
    return $"data:img/png;base64,{Convert.ToBase64String(bytes)}";
  }

  private async Task OnMatch()
  {
    _text = $"Matching images ...";
    StateHasChanged();
    await Task.Run(() =>
    {
      var match = ImageMatch.Match(_img1, _img2);
      _text = $"Match = {match:F1}%";
    });
  }
}
