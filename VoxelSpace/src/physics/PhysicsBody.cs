using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public class PhysicsBody : IPhysicsBody {

        public PhysicsDomain Domain { get; private set; }

        public void _SetPhysicsDomain(PhysicsDomain domain) {
            Domain = domain;
        }

        public virtual void Update(GameTime time) {
            
        }

    }

    public interface IPhysicsBody {

        PhysicsDomain Domain { get; }

        // only called from PhysicsDomain
        void _SetPhysicsDomain(PhysicsDomain domain);

        void Update(GameTime time);

    }

}