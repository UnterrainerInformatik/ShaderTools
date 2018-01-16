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
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace ShaderTools
{
    [PublicAPI]
    public enum GraphicsApi
    {
        OPEN_GL,
        DIRECT_X
    }

    [PublicAPI]
    public class ShaderLookupException : Exception
    {
        public ShaderLookupException(string message) : base(message)
        {
        }

        public ShaderLookupException(string message, Exception e) : base(message, e)
        {
        }
    }

    [PublicAPI]
    public static class SystemProbe
    {
        private const string SHADER_TYPE = "Microsoft.Xna.Framework.Graphics.Shader";
        private const string PROFILE = "Profile";

        public static GraphicsApi CurrentGraphicsApi { get; private set; } = GraphicsApi.OPEN_GL;
        public static string CurrentShaderExtension => CurrentGraphicsApi == GraphicsApi.OPEN_GL ? "ogl" : "dx11";

        static SystemProbe()
        {
            DetermineShaderExtension();
        }

        private static void DetermineShaderExtension()
        {
            // Use reflection to figure out if Shader.Profile is OpenGL (0) or DirectX (1).
            // May need to be changed / fixed for future shader profiles.

            var assembly = typeof(Game).GetTypeInfo().Assembly;
            if (assembly == null)
                throw new ShaderLookupException(
                    "Error determining shader profile. Couldn't find assembly. Falling back to OpenGL.");

            var shaderType = assembly.GetType(SHADER_TYPE);
            if (shaderType == null)
                throw new ShaderLookupException(
                    $"Error determining shader profile. Couldn't find shader type of '{SHADER_TYPE}'. Falling back to OpenGL.");

            var shaderTypeInfo = shaderType.GetTypeInfo();
            if (shaderTypeInfo == null)
                throw new ShaderLookupException(
                    "Error determining shader profile. Couldn't get TypeInfo of shadertype. Falling back to OpenGL.");

            // https://github.com/MonoGame/MonoGame/blob/develop/MonoGame.Framework/Graphics/Shader/Shader.cs#L47
            var profileProperty = shaderTypeInfo.GetDeclaredProperty(PROFILE);
            var value = (int) profileProperty.GetValue(null);

            switch (value)
            {
                case 0:
                    CurrentGraphicsApi = GraphicsApi.OPEN_GL;
                    break;
                case 1:
                    CurrentGraphicsApi = GraphicsApi.DIRECT_X;
                    break;
                default:
                    throw new ShaderLookupException("Unknown shader profile.");
            }
        }
    }
}