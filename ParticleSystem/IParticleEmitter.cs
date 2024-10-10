using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;

namespace ParticleSystem
{
    public interface IParticleEmitter
    {
        /// <summary>
        /// The position
        /// </summary>
        public Vector2 Position { get; }

        /// <summary>
        /// The velocity
        /// </summary>
        public Vector2 Velocity { get; }
    }
}