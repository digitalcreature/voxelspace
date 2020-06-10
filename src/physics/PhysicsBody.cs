using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public class PhysicsBody : IPhysicsBody {

        public PhysicsDomain domain { get; private set; }

        public void _SetPhysicsDomain(PhysicsDomain domain) {
            this.domain = domain;
        }

        public virtual void Update(GameTime time) {
            
        }

    }

    public interface IPhysicsBody {

        PhysicsDomain domain { get; }

        // only called from PhysicsDomain
        void _SetPhysicsDomain(PhysicsDomain domain);

        void Update(GameTime time);

    }

}