using ColossalFramework.UI;


namespace RON
{
    /// <summary>
    /// Textures.
    /// </summary>
    internal static class Textures
    {
        // RON button icon texture atlas.
        private static UITextureAtlas ronButtonSprites;
        internal static UITextureAtlas RonButtonSprites
        {
            get
            {
                if (ronButtonSprites == null)
                {
                    ronButtonSprites = TextureUtils.LoadSpriteAtlas("RonButton");
                }

                return ronButtonSprites;
            }
        }
    }
}