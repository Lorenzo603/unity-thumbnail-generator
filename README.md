# Unity Thumbnail Generator
Custom thumbnail generator for the Unity game engine.

## Summary
Generate custom thumbnails for prefabs with possibility of controlling lighting/camera and in generally everything in the scene that is used for the generation.
For example you can control the thumbnail background color by changing the "Environment->Background Type" setting on the scene Main Camera.
The script will traverse a folder recursively looking for prefabs, it will then instantiate each prefab in the ThumbnailScene and take a "picture" (a.k.a rendering the camera output on a texture). 
This texture will be saved to the Thumbnails folder and will be used as thumbnail for said prefab.
The editor extension matches thumbnails and prefabs by looking for thumbnails in the Thumbnails folder following the same tree structure as the Asset folder.

## Important Classes
* Editor/CustomThumbnailRenderer.cs: Editor extension that looks for matching thumbnails when it's time to draw a prefab in the Project Window. If it finds a match, it draws it instead of the default one.
* MultiObjectProcessor.cs: Traverses the folders recursively to find prefabs.
* ObjectThumbnailActivity.cs: Positions the camera (you can modify this or position the camera whatever way you want.)
* ThumbnailGenerator.cs: renders the camera output on a Texture and saves it as PNG.

## Settings
* ThumbGen GameObject -> ThumbnailGenerator Component: 
  * Target Render Texture: 2D texture image that will be used to take snapshots of the prefabs. Width and Height must match with the setting below.
  * Thumbnail Camera: Main Camera that will take snapshots of the prefab. When the scene runs, the camera will not actually render to the display.
  * Thumbnail Width
  * Thumbnail Height
  * Export File Path Root: root folder where the thumnails will be saved. The tree structure will be maintained.
* ThumbGen GameObject -> Multi Object Processor Component: 
  * Asset Dir: root folder to traverse looking for prefabs
  * Excluded Dirs: comma-separated list of directory name to exclude from processing


# Thanks
Thanks to this blog post for the large majority of the generation code: https://undertheweathersoftware.com/automatic-thumbnail-image-generator-for-3d-objects-in-unity/
Thanks to this other blog post for explaining the general idea of how to draw customizations on the project window: https://sahildhanju.com/posts/unity-asset-thumbnail-icons/
