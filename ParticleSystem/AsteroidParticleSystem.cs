using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using StateManagement;
using Collisions;

namespace ParticleSystem{
    public class AsteroidParticleSystem : ParticleSystem{
        // Where the asteroids are coming from
        private Rectangle _source;

        private float _particleTimer;
        private const float TimeBetweenParticles = 1.0f;

        public int NumParticles { get; set; } = 0;

        
        /// <summary>
        /// Determines if the asteroid particle system is active or not
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Constructor for the AsteroidParticleSystem
        /// </summary>
        /// <param name="game">The game</param>
        /// <param name="source">Where the particles should be coming from</param>
        public AsteroidParticleSystem(ScreenManager screen, Rectangle source) : base(screen, 50){
            _source = source;
        }

        /// <summary>
        /// Helper method for initializing the constants of the particle system
        /// </summary>
        protected override void InitializeConstants(){
            textureFilename = "meteorBrown_big3";
            minNumParticles = 5;
            maxNumParticles = 10;
        }

        /// <summary>
        /// Initializes the particles
        /// </summary>
        /// <param name="p">Reference to the particle to be initialized</param>
        /// <param name="where">Where the particle should be</param>
        protected override void InitializeParticle(ref Particle p, Vector2 where){
            float pScale = RandomHelper.NextFloat(0.3f, 0.4f);
            p.Initialize(where, Vector2.UnitY * 100, Vector2.Zero, Color.White, scale: pScale, lifetime: 6);
            p.Rotation = RandomHelper.NextFloat(10, MathHelper.TwoPi);
            p.AngularVelocity = RandomHelper.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4);
            p.HitBox = new BoundingCircle(p.Position, 89 * pScale / 2);
        }

        /// <summary>
        /// The update method for the particle system
        /// </summary>
        /// <param name="gameTime">The current gametime</param>
        public override void Update(GameTime gameTime){
            base.Update(gameTime);

            if(IsActive){
                _particleTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if(_particleTimer >= TimeBetweenParticles){
                    AddParticles(_source);
                    _particleTimer = 0;
                    NumParticles++;
                }

                CheckCollisions();
            }
        }

        /// <summary>
        /// Updates the rotation of the particle and the position of the hitbox
        /// </summary>
        /// <param name="particle">A reference to the particle to be updated</param>
        /// <param name="dt">The amount of time that has passed</param>
        protected override void UpdateParticle(ref Particle particle, float dt)
        {
            base.UpdateParticle(ref particle, dt);
            particle.Rotation += particle.AngularVelocity * dt;
            particle.HitBox.Center = particle.Position;
        }

        /// <summary>
        /// Checks for collisions between particles
        /// </summary>
        private void CheckCollisions(){
            for(int i = 0; i < particles.Count(); i++){
                if(!particles[i].Active) continue;

                for(int j = i + 1; j < particles.Count(); j++){
                    if(CollisionHelper.Collides(particles[i].HitBox, particles[j].HitBox)){
                        ResolveCollision(ref particles[i], ref particles[j]);
                    }
                }
            }
        }

        /// <summary>
        /// If a collision occurs between the particles, this method will move the particles until the collision is resolved
        /// </summary>
        /// <param name="p1">A reference to the first particle involved in the collision</param>
        /// <param name="p2">A reerence to the second particle involved in the collision</param>
        private void ResolveCollision(ref Particle p1, ref Particle p2){
            Vector2 direction = p1.Position - p2.Position;
            direction.Normalize();

            float overlap = p1.HitBox.Radius + p2.HitBox.Radius - Vector2.Distance(p1.Position, p2.Position);


            // Counter and maxIterations are put in place to ensure the while loop will not run indefinitely 
            int counter = 0;
            const int maxIterations = 15;

            while(overlap > 0 && counter < maxIterations){
                p1.Position += direction * overlap / 2;
                p2.Position -= direction * overlap / 2;

                p1.HitBox.Center = p1.Position;
                p2.HitBox.Center = p2.Position;

                overlap = p1.HitBox.Radius + p2.HitBox.Radius - Vector2.Distance(p1.Position, p2.Position);

                counter++;
            }
        }

        /// <summary>
        /// Checks for collisions between the player and the particles
        /// </summary>
        /// <param name="player">The BoundingTriangle of the player</param>
        /// <param name="particle">Outs the particle that the collision occurred on</param>
        /// <returns>True for collision, false otherwise</returns>
        public bool CheckForPlayerCollision(BoundingTriangle player, out Particle p){
            for (int i = 0; i < particles.Count(); i++)
            {
                if(particles[i].Active && !particles[i].DeadParticle){
                    if (CollisionHelper.Collides(player, particles[i].HitBox)) 
                    {
                        p = particles[i];
                        particles[i].DeadParticle = true;
                        particles[i].HitBox = new BoundingCircle(Vector2.Zero, 0);
                        return true;
                    }
                }
            }

            p = new Particle();
            return false;
        }
    }
}