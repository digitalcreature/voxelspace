using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace VoxelSpace.SceneGraph {

    public abstract class Scene : IDisposable {

        List<SceneObject> _objects;

        IReadOnlyList<SceneObject> Objects => _objects;

        public Transform Root { get; private set; }

        public Scene() {
            _objects = new List<SceneObject>();
            Root = new RootTransform(this);
        }

        public virtual void Update() {
            UpdateObjects();
        }

        public void UpdateObjects() {
            for (int i = 0; i < _objects.Count; i ++) {
                _objects[i].Update();
            }
        }

        public void AddObject(SceneObject obj) {
            if (obj.Scene != this) {
                if (obj.Scene != null) {
                    obj.Scene.RemoveObject(obj);
                }
                _objects.Add(obj);
                obj.setScene(this);
                if (obj.Transform.Parent == null) {
                    obj.Transform.SetParent(Root);
                }
            }
        }

        public bool RemoveObject(SceneObject obj) {
            if (obj.Scene == this) {
                var state = _objects.Remove(obj);
                if (state) {
                    obj.setScene(null);
                }
                if (obj.Transform.Parent == Root) {
                    obj.Transform.SetParent(null);
                }
                return state;
            }
            else {
                return false;
            }
        }

        public virtual void Dispose() {}

        class RootTransform : Transform {

            Scene _scene;
            public override Scene Scene => _scene;
            public override bool IsSceneRoot => true;

            public RootTransform(Scene scene) : base(null) {
                _scene = scene;
            }

        }

    }

}