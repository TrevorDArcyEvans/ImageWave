# Image Wave
A less simple method to match images

# Features
* runs on .NET Core
* runs in browser with webassembly
* commandline client

# Discussion

<details>
There are several reference images:

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

| Image                 | Match |
|-----------------------|-------|
| brightness & contrast | 73%   |
| resized               | 79%   |
| rotated slightly      | 72%   |
| rotated more          | 51%   |

</details>

# Further information
* [Algorithm to compare two images](https://stackoverflow.com/questions/23931/algorithm-to-compare-two-images)
* [Discrete Wavelet Transform](https://en.wikipedia.org/wiki/Discrete_wavelet_transform)

# Acknowledgements
* [WaveletStudio](https://github.com/DragonLi/waveletstudio)
* [WaveletStudio](https://github.com/walteram/waveletstudio)
* [Great Wave Off Kanagawa](https://openclipart.org/detail/219610/great-wave-off-kanagawa)
* [wave of happiness](https://openclipart.org/detail/178023/wave-of-happiness)
* [Ocean Wave 3](https://openclipart.org/detail/120655/ocean-wave-3)

