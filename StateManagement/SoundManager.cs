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
            _backgroundMusic = content.Load<Song>("Ludum Dare 28 01");
            _damageSound = content.Load<SoundEffect>("Explosion6");
            _loseSound = content.Load<SoundEffect>("lose");

            MediaPlayer.Volume = GameSettings.MusicVolume / 3; // hardcoded at 1/3rd because it is SUPER loud and you cant hear the sound effects unless turning it down
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_backgroundMusic);
        }

        /// <summary>
        /// Plays the explosion sound effect
        /// </summary>
        public void PlayDamageSound(){
            _damageSound.Play(GameSettings.SFXVolume, 0, 0);
        }

        /// <summary>
        /// Plays the losing sound effect
        /// </summary>
        public void PlayLoseSound(){
            MediaPlayer.Pause();
            _loseSound.Play(GameSettings.SFXVolume, 0, 0);
        }

        /// <summary>
        /// Pauses the MediaPlayer (background music)
        /// </summary>
        public void PauseMusic(){
            MediaPlayer.Pause();
        }

        /// <summary>
        /// Resumes the MediaPlayer (background music)
        /// </summary>
        public void StartMusic(){
            MediaPlayer.Resume();
        }

        /// <summary>
        /// Updates the volume of the MediaPlayer
        /// </summary>
        public void UpdateVolume(){
            MediaPlayer.Volume = GameSettings.MusicVolume / 3;
        }
        

        
    }
}