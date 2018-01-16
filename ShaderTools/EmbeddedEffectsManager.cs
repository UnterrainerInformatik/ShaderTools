// *************************************************************************** 
// This is free and unencumbered software released into the public domain.
// 
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
// 
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org>
// ***************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;

namespace ShaderTools
{
    /// <summary>
    ///     Helps to load the bytecode of an <see cref="Effect" /> that is encapsulated inside a compiled assembly.<br />
    ///     The constructor takes an <see cref="System.Reflection.Assembly" /> to get the assembly in which to search for the
    ///     embedded shaders.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Files that are encapsulated inside a compiled assembly are commonly known as Manifiest or embedded resources.
    ///         Since embedded resources are added to the assembly at compiled time, they can not be accidentally deleted or
    ///         misplaced. However, if the file needs to be changed, the assembly will need to be re-compiled with the changed
    ///         file.
    ///     </para>
    ///     <para>
    ///         To add an embedded resource file to an assembly, first add it to the project and then change the Build Action
    ///         in the Properties of the file to <code>Embedded Resource</code>. The next time the project is compiled, the
    ///         compiler will add the file to the assembly as an embedded resource. The compiler adds namespace(s) to the
    ///         embedded resource so it matches with the path of where the file was added to the project.
    ///     </para>
    ///     <para>
    ///         You need to call <see cref="Load(Microsoft.Xna.Framework.Graphics.GraphicsDevice,string,System.Type)" /> in
    ///         order to load such an effect.
    ///         <see cref="Load(Microsoft.Xna.Framework.Graphics.GraphicsDevice,string,System.Type)" /> stores that effect
    ///         in an internal dictionary that allows us to cache it efficiently (if you want to call it for an effect
    ///         repeatedly) and to unload it automatically when you call <see cref="UnloadContent" />.
    ///     </para>
    /// </remarks>
    [PublicAPI]
    public class EmbeddedEffectsManager
    {
        private Dictionary<string, Effect> Effects { get; } = new Dictionary<string, Effect>();
        private Assembly Assembly { get; set; }

        public string RootPath { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EmbeddedEffectsManager" /> class.
        /// </summary>
        /// <param name="assembly">The assembly where to search for the embedded effects in.</param>
        /// <param name="rootPath">
        ///     The root path you want to load all your effects from in your assembly. Example:
        ///     <pre>"&lt;YourGame&gt;.{&lt;subDir&gt;}.&lt;shaderName&gt;"</pre>
        ///     Live example: <pre>'BloomEffectRenderer.Effects.Resources.BloomExtract'</pre> for the project 'BloomEffectRenderer'
        ///     subdir 'Effects/Resources' and the files 'BloomExtract.ogl.mgfxo' and 'BloomExtract.dx11.mgfxo'.
        /// </param>
        public EmbeddedEffectsManager(Assembly assembly, string rootPath = "")
        {
            RootPath = rootPath;
            if (!RootPath.EndsWith("."))
                RootPath += ".";
            Assembly = assembly;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EmbeddedEffectsManager" /> class.
        /// </summary>
        /// <param name="typeInAssembly">
        ///     A type within the assembly to search for the embedded effects in. Calls
        ///     .GetInfo().Assembly on it.
        /// </param>
        /// <param name="rootPath">
        ///     The root path you want to load all your effects from in your assembly. Example:
        ///     <pre>"&lt;YourGame&gt;.{&lt;subDir&gt;}.&lt;shaderName&gt;"</pre>
        ///     Live example: <pre>'BloomEffectRenderer.Effects.Resources.BloomExtract'</pre> for the project 'BloomEffectRenderer'
        ///     subdir 'Effects/Resources' and the files 'BloomExtract.ogl.mgfxo' and 'BloomExtract.dx11.mgfxo'.
        /// </param>
        public EmbeddedEffectsManager(Type typeInAssembly, string rootPath = "") : this(
            typeInAssembly.GetTypeInfo().Assembly,
            rootPath)
        {
        }

        /// <summary>
        ///     Loads the specified effect using the given graphicsdevice.<br />
        ///     Please call this in the LoadContent() method of your game and hold the effects in variables or load them again
        ///     using the effect's resource-name (this manager caches them for you).
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="resourceName">The name of the embedded resource. This must include the namespace(s).</param>
        /// <param name="assembly">The assembly which the embedded resource is apart of.</param>
        /// <returns></returns>
        public Effect Load(GraphicsDevice graphicsDevice, string resourceName, Assembly assembly = null)
        {
            Effect e;
            if (Effects.TryGetValue(resourceName, out e))
            {
                return e;
            }

            e = new Effect(graphicsDevice,
                GetByteCode(RootPath + resourceName + "." + SystemProbe.CurrentShaderExtension + ".mgfxo",
                    assembly ?? Assembly));
            Effects.Add(resourceName, e);
            return e;
        }

        /// <summary>
        ///     Loads the specified effect using the given graphicsdevice.<br />
        ///     Please call this in the LoadContent() method of your game and hold the effects in variables or load them again
        ///     using the effect's resource-name (this manager caches them for you).
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="resourceName">The name of the embedded resource. This must include the namespace(s).</param>
        /// <param name="typeInAssembly">
        ///     A type within the assembly to search for the embedded effects in. Calls
        ///     .GetInfo().Assembly on it.
        /// </param>
        /// <returns></returns>
        public Effect Load(GraphicsDevice graphicsDevice, string resourceName, Type typeInAssembly)
        {
            return Load(graphicsDevice, resourceName, typeInAssembly.GetTypeInfo().Assembly);
        }

        /// <summary>
        ///     Unloads the effects you loaded via this manager.<br />
        ///     Please call this in the UnloadContent() method of your game.
        /// </summary>
        public void UnloadContent()
        {
            foreach (var effect in Effects.Values)
            {
                effect.Dispose();
            }
        }

        private byte[] GetByteCode(string resourceName, Assembly assembly)
        {
            lock (this)
            {
                var stream = assembly.GetManifestResourceStream(resourceName);
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }
    }
}