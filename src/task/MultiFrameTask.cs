using System;

namespace VoxelSpace {

    // any task that takes place over multiple frames, regardless of wether or not it is threaded
    // T: data to operate on
    public interface IMultiFrameTask<T> {

        bool IsRunning { get; }
        bool HasCompleted { get; }

        void StartTask(T data);

        // return true on the frame that the task is completed
        bool UpdateTask();

    }

    public static class MultiFrameTaskExtensions {

        // returns true if the task has been started
        public static bool HasStarted<T>(this IMultiFrameTask<T> task) {
            return task.IsRunning || task.HasCompleted;
        }

    }

}