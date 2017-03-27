# ImageResizer.FolderThumb
Plugin for [ImageResizer](https://imageresizing.net/) for .NET to generate a thumbnail image for a folder on disk using a randomly-selected image from that folder.

When a request comes in for an image with one of the specified filenames, _if that image does not exist on disk_ then an image is chosen at random from the same directory and a thumbnail of it is served instead. If there are no images in the folder then a default fallback is used, if specified, as a last resort.

Add something like the following to the `resizer` section in `web.config` to specify one or more virtual filenames to activate the thumbnail generator and a default fallback.

```xml
  <folderThumb default="default-folder-thumb.jpg" activateOnFilename="folder.jpg,thumbnail.jpg" />
```

