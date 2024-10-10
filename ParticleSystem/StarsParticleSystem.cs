using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StateManagement;

namespace ParticleSystem
{
    public class StarsParticleSystem : ParticleSystem
    {
        // Where the particle is coming from
        Rectangle _source;

        /// <summary>
        /// Used to turn the rain on and off
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Constructor for the RainParticleSystem
        /// </summary>
        /// <param name="game">The game</param>
        /// <param name="source">The source rectangle</param>
        public StarsParticleSystem(ScreenManager screen, Rectangle source) : base(screen, 5000)
        {
            _source = source;
        }

        /// <summary>
        /// Helper method for initializing the constants of the particle system
        /// </summary>
        protected override void InitializeConstants()
        {
            textureFilename = "particle"; //obtained from Nathan Bean via Github (https://github.com/CIS580/particle-system-example)
            minNumParticles = 10;
            maxNumParticles = 20;
        }

        protected override void InitializeParticle(ref Particle p, Vector2 where)
        {
            p.Initialize(where, Vector2.UnitY * 140, Vector2.Zero, Color.White, scale: RandomHelper.NextFloat(0.1f, 0.4f), lifetime: 4);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(IsActive) AddParticles(_source);
        }

    }
}