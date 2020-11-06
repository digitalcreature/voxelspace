using System;
using System.Reflection;

namespace VoxelSpace.IO {

    [AttributeUsage(AttributeTargets.Class)]
    public class SubclassAttribute : Attribute {

        public string ID { get; private set; }
        public Type Type { get; private set; }

        public SubclassAttribute(string id, Type type) {
            ID = id;
            Type = type;
        }

    }

}