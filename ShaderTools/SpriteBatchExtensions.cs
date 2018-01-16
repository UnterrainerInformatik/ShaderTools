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

using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShaderTools
{
    [PublicAPI]
    public static class SpriteBatchExtensions
    {
        /// <summary>
        ///     Draws a whole, arbitrarily sized texture into a whole, arbitrarily sized renderTarget, using a custom shader to
        ///     apply postProcessing effects.<br />
        ///     Clears the target-rendertarget to black before drawing by default.
        /// </summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch" /> to use for drawing.</param>
        /// <param name="texture">The <see cref="Texture2D" /> to draw onto the target.</param>
        /// <param name="renderTarget">The <see cref="RenderTarget2D" /> to draw to.</param>
        /// <param name="spriteBatchIsBegun">
        ///     If set to <c>true</c> the method won't open it a second time and it will not close it.
        ///     Be aware that all the parameters you used when calling <c>SpriteBatch.Begin()</c> will be used for drawing if you
        ///     specify
        ///     <c>true</c>.
        /// </param>
        /// <param name="effect">The <see cref="Effect" /> to use when beginning the SpriteBatch (default is null -&gt; none).</param>
        /// <param name="blendState">The <see cref="BlendState" /> to use for drawing (default is BlendState.Opaque).</param>
        /// <param name="samplerState">The <see cref="SamplerState" /> to use for drawing (default is SamplerState.PointClamp).</param>
        /// <param name="clearColor">
        ///     The <see cref="Color" /> to use when clearing the RenderTarget before drawing (default is
        ///     Color.Black).
        /// </param>
        public static void DrawFullscreenQuad(this SpriteBatch spriteBatch, Texture2D texture,
            RenderTarget2D renderTarget, Effect effect = null, BlendState blendState = null,
            SamplerState samplerState = null, Color? clearColor = null, bool spriteBatchIsBegun = false)
        {
            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            graphicsDevice.SetRenderTarget(renderTarget);
            graphicsDevice.Clear(clearColor ?? Color.Black);

            if (texture == null)
                return;

            if (!spriteBatchIsBegun)
                spriteBatch.Begin(SpriteSortMode.Immediate,
                    blendState ?? BlendState.Opaque,
                    samplerState ?? SamplerState.PointClamp,
                    DepthStencilState.None,
                    RasterizerState.CullNone,
                    effect);

            int w, h;
            if (renderTarget == null)
            {
                w = graphicsDevice.PresentationParameters.BackBufferWidth;
                h = graphicsDevice.PresentationParameters.BackBufferHeight;
            }
            else
            {
                w = renderTarget.Width;
                h = renderTarget.Height;
            }

            spriteBatch.Draw(texture,
                new Rectangle(0, 0, w, h),
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                1f);

            if (!spriteBatchIsBegun)
                spriteBatch.End();
        }
    }
}