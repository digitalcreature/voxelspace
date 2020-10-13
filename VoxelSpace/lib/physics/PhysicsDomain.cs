using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public class PhysicsDomain {

        public GravityField Gravity { get; private set; }

        public ICollisionGrid CollisionGrid { get; private set; }

        HashSet<IPhysicsBody> _bodies;

        public PhysicsDomain(GravityField gravity, ICollisionGrid collisionGrid) {
            Gravity = gravity;
            CollisionGrid = collisionGrid;
            _bodies = new HashSet<IPhysicsBody>();
        }

        public void AddBody(IPhysicsBody body) {
            if (body != null) {
                _bodies.Add(body);
                body._SetPhysicsDomain(this);
            }
            else {
                throw new ArgumentNullException(nameof(body));
            }
        }

        public bool RemoveBody(IPhysicsBody body) {
            if (body != null) {
                if (body.Domain == this) {
                    return _bodies.Remove(body);
                }
                return false;
            }
            else {
                throw new ArgumentNullException(nameof(body));
            }
        }

        public void UpdateBodies(GameTime time) {
            foreach (var body in _bodies) {
                body.Update(time);
            }
        }


    }

}