using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;


namespace StateManagement
{
    /// <summary>
    /// Manages sound effects and the background music for the game
    /// </summary>
    public class SoundManager{
        // The background music for the game
        private Song _backgroundMusic;

        // The sound effect to be played when a player impacts an asteroid
        private SoundEffect _damageSound;

        // The sound effect to be played when the player loses
        private SoundEffect _loseSound;

        
        /// <summary>
        /// Loads the content for the sound effects and background music
        /// </summary>
        /// <param name="content">The content manager to load with</param>
        public void LoadContent(ContentManager content){
            
        }

        

        
    }
}