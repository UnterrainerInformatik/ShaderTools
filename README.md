[![NuGet](https://img.shields.io/nuget/v/ShaderTools.svg)](https://www.nuget.org/packages/ShaderTools/) [![NuGet](https://img.shields.io/nuget/dt/ShaderTools.svg)](https://www.nuget.org/packages/ShaderTools/) [![license](https://img.shields.io/github/license/unterrainerinformatik/ShaderTools.svg?maxAge=2592000)](http://unlicense.org)  [![Twitter Follow](https://img.shields.io/twitter/follow/throbax.svg?style=social&label=Follow&maxAge=2592000)](https://twitter.com/throbax)  

# General

This section contains various useful projects that should help your development-process.  

This section of our GIT repositories is free. You may copy, use or rewrite every single one of its contained projects to your hearts content.  
In order to get help with basic GIT commands you may try [the GIT cheat-sheet][coding] on our [homepage][homepage].  

This repository located on our  [homepage][homepage] is private since this is the master- and release-branch. You may clone it, but it will be read-only.  
If you want to contribute to our repository (push, open pull requests), please use the copy on github located here: [the public github repository][github]  

# ![Icon](https://github.com/UnterrainerInformatik/ShaderTools/raw/master/icon.png) ShaderTools

A PCL library that helps with some useful tools when working with shaders in MonoGame, like a reflection helper that determines if the project gets used for OpenGL or DirectX. 


> **If you like this repo, please don't forget to star it.**
> **Thank you.**



## Getting Started

### GraphicsDeviceExtensions

```c#
public static void Clear(this GraphicsDevice graphicsDevice, RenderTarget2D renderTarget, Color? clearColor = null)
{
  graphicsDevice.SetRenderTarget(renderTarget);
  graphicsDevice.Clear(Color.Black);
}
```



### SpriteBatchExtensions

Draw a fullscreen quad to the rendertarget of your choice.

```c#
spriteBatch.DrawFullscreenQuad(BloomRenderTarget2,
  BloomRenderTarget1,
  GaussianBlurEffect,
  null,
  SamplerState.AnisotropicClamp);
```

Begins the spriteBatch and draws the given texture on to the given RenderTarget using the given parameters. Tidies up your code a bit. Nothing more, nothing less.

### SystemProbe

Probes your game upon start (static class constructor) via reflection for the graphics-profile you use (OpenGL or DirectX). After that you can get it using a static accessor, or you can get the file-extension a shader file would have (ogl or dx11) using another accessor.

```c#
GraphicsApi api = SystemProbe.CurrentGraphicsApi;
string name = resourceName + "." + SystemProbe.CurrentShaderExtension + ".mgfxo"
```



### EmbeddedEffectManager

Loads effects that have been embedded in a DLL. Needs the Assembly that is this DLL of course and a fully qualified name to get the file.

```c#
private EmbeddedEffectsManager EmbeddedEffectsManager { get; } =
  new EmbeddedEffectsManager(typeof(Renderer), "BloomEffectRenderer.Effects.Resources");

public void LoadContent(GraphicsDevice graphicsDevice)
{
  ExtractEffect = EmbeddedEffectsManager.Load(graphicsDevice, "BloomExtract");
  GaussianBlurEffect = EmbeddedEffectsManager.Load(graphicsDevice, "GaussianBlur");
  CombineEffect = EmbeddedEffectsManager.Load(graphicsDevice, "BloomCombine");
}

public void UnloadContent()
{
  EmbeddedEffectsManager.UnloadContent();
}
```



[homepage]: http://www.unterrainer.info
[coding]: http://www.unterrainer.info/Home/Coding
[github]: https://github.com/UnterrainerInformatik/ShaderTools