# Image Wave

A less simple method to match images

![](Images/screen-web-ui.png)

# Features

* runs on .NET Core
* runs in browser with webassembly
* commandline client
* less worse than other [algorithms](https://github.com/TrevorDArcyEvans/ImageMatch)

# Demo
[ImageWave](https://trevordarcyevans.github.io/ImageWave/)

# Getting started

<details>

```bash
git clone https://github.com/TrevorDArcyEvans/ImageWave.git
cd ImageWave/
dotnet restore
dotnet build
```

## Web UI
```bash
cd cd ImageWave.UI.Web/
dotnet restore
dotnet build
dotnet run
```
open [ImageWave](http://localhost:5010)

</details>

# Discussion
This algorithm uses a discrete wavelet transform (DWT) to calculate the root mean square (RMS)
distance between the details of two images.
1. resize each image to 256x256
2. convert each image to black and white
3. take DWT of each image
4. calculate RMS distance of details between images

## Comparison

<details>

There are several reference images from a previous
project [(ImageWave)](https://github.com/TrevorDArcyEvans/ImageMatch):

<details>
  <summary>Baseline</summary>

![](Images/open-clipart-270289.png)

</details>

<details>
  <summary>Baseline + brightness & contrast changed</summary>

![](Images/open-clipart-270289-bright.png)

</details>

<details>
  <summary>Baseline + resized 80%</summary>

![](Images/open-clipart-270289-resize.png)

</details>

<details>
  <summary>Baseline + rotated slightly</summary>

![](Images/open-clipart-270289-rot.png)

</details>

<details>
  <summary>Baseline + rotated more</summary>

![](Images/open-clipart-270289-rot-more.png)

</details>

| Image                 | ImageMatch | ImageWave |
|-----------------------|------------|-----------|
| brightness & contrast | 78%        | 93%       |
| resized               | 95%        | 92%       |
| rotated slightly      | 90%        | 89%       |
| rotated more          | 54%        | 88%       |

As should be obvious, _ImageWave_ performs significantly better than the naiive algorithm in _ImageMatch_, especially
when the other image has been rotated.

</details>

## Further comparison

<details>

<details>
  <summary>Baseline</summary>

![](Images/open-clipart-120655-400x349-resize.png)

</details>

<details>
  <summary>Baseline + rotated</summary>

![](Images/open-clipart-120655-400x349-rot-resize.png)

</details>

<details>
  <summary>Baseline + rotated 90</summary>

![](Images/open-clipart-120655-400x349-rot-90-resize.png)

</details>

| Image      | ImageMatch | ImageWave |
|------------|------------|-----------|
| rotated    | 46%        | 93%       |
| rotated-90 | 28%        | 93%       |

</details>

## Winding

<details>

Normally, pixel data is read in row by row.  Once the end of the row is reached, we then
go to the start of the next row and start at the beginning.

![](Images/signal-base-normal.png)

In pathalogically rotated cases eg rotated 90 degrees, the input signal to the DWT will contain a
sharp step.  This results in many higher frequency components which can degrade image matching.

![](Images/signal-90-normal.png)

To lessen the effects, 'winding' of the input signal was implemented.  At the end of each row
of the image, instead of going back to the **start** of the next row, pixels are read from the
**end** of the next row **backwards** to the start of the next row.

![](Images/signal-base-winding.png)

![](Images/signal-90-winding.png)

### Results

<details>

<details>
  <summary>Baseline</summary>

![](Images/block.png)

</details>

<details>
  <summary>Baseline + rotated</summary>

![](Images/block-90.png)

</details>

Comparison:
* before winding = 93.5%
* after  winding = 95.6%

</details>

</details>

# Further work
* experiment with different mother wavelets
  * current uses [Haar wavelet](https://en.wikipedia.org/wiki/Haar_wavelet)
* experiment with DWT depth level
  * currently set to 1
* RMS of details is currently not weighted
* ~~fix _WaveletStudio_ unit tests~~
* investigate regression in _DecompositionLevel_

# Further information

* [Algorithm to compare two images](https://stackoverflow.com/questions/23931/algorithm-to-compare-two-images)
* [Discrete Wavelet Transform](https://en.wikipedia.org/wiki/Discrete_wavelet_transform)

# Acknowledgements

* [WaveletStudio](https://github.com/DragonLi/waveletstudio)
* [WaveletStudio](https://github.com/walteram/waveletstudio)
* [Ocean Wave 3](https://openclipart.org/detail/120655/ocean-wave-3)

