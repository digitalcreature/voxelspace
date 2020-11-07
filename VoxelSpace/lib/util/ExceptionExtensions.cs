using System;
using System.Collections.Generic;

namespace VoxelSpace {

    static class ExceptionExtensions {

        /// <summary>
        /// Filter out exceptions of a certain type.
        /// When e is an AggregateException, return a new AggregateException with a filtered list of inner exceptions.
        /// If the given exception or all inner exceptions are filtered, null is returned.
        /// Applied recursively to AggregateExceptions
        /// </summary>
        /// <param name="e">The exception to filter</param>
        /// <typeparam name="T">The exception type to remove</typeparam>
        public static Exception Filter<T>(this Exception e) where T : Exception {
            if (e is AggregateException ag) {
                var inners = new List<Exception>();
                foreach (var inner in ag.InnerExceptions) {
                    var ex = inner.Filter<T>();
                    if (ex != null) {
                        inners.Add(ex);
                    }
                }
                if (inners.Count > 0) {
                    return new AggregateException(inners);
                }
                else {
                    return null;
                }
            }
            else {
                if (e is T) {
                    return null;
                }
                else {
                    return e;
                }
            }
        }

        public static void Throw(this Exception e) {
            throw e;
        }
        
    }

}
