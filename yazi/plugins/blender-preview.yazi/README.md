# blender-preview.yazi

A [Yazi](https://github.com/sxyazi/yazi) plugin to preview Blender's .blend and
.blend1 files.

> [!TIP]
> Blender's thumbnails are limited to 128x128 pixels.
> [zoom.yazi](https://github.com/yazi-rs/plugins/tree/main/zoom.yazi) can help
increase the size, but won't change the resolution.

## Requirements

`blender-thumbnailer` needs to be available on the path
(usually done automatically when Blender is installed).

## Installation

```sh
ya pkg add walldmtd/blender-preview
```

## Usage

Add this to `yazi.toml`:

```toml
[plugin]
prepend_preloaders = [
    { name = "*.blend", run = "blender-preview" },
    { name = "*.blend1", run = "blender-preview" },
]
prepend_previewers = [
    { name = "*.blend", run = "blender-preview" },
    { name = "*.blend1", run = "blender-preview" },
]
```

## Acknowledgements

- [Yazi](https://github.com/sxyazi/yazi), for
[magick.lua](https://github.com/sxyazi/yazi/blob/main/yazi-plugin/preset/plugins/magick.lua)
(used as a base for this plugin)
