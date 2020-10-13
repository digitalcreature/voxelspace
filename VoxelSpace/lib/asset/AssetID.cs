using System;
using System.Collections.Generic;

namespace VoxelSpace.Assets {

    public struct AssetID {

        public Type Type;
        public string Name;
        public string ModuleName;
        public string QualifiedName => $"{ModuleName}:{Name}";

        public AssetID(Type type, string moduleName, string name) {
            Type = type;
            ModuleName = moduleName;
            Name = name;
        }

        public AssetID(Type type, string qualifiedName) {
            Type = type;
            (ModuleName, Name) = AssetManager.SplitQualifiedAssetName(qualifiedName);
        }

        public override bool Equals(object obj) {
            if (obj is AssetID aid) {
                return aid.QualifiedName == QualifiedName && aid.Type == Type;
            }
            return false;
        }

        public static bool operator ==(AssetID a, AssetID b) {
            return a.Equals(b);
        }

        public static bool operator !=(AssetID a, AssetID b) {
            return !a.Equals(b);
        }

        public override int GetHashCode() {
            return Type.GetHashCode() << 16 ^ QualifiedName.GetHashCode();
        }

        public override string ToString() {
            return $"{QualifiedName} ({Type.Name})";
        }

    }

}