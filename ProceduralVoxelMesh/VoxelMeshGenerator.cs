// Copyright 2015 afuzzyllama. All Rights Reserved.
using System.Collections.Generic;
using System.Threading;

namespace ProceduralVoxelMesh
{
    /// <summary>
    /// Voxel mesh generator that generator thread uses
    /// </summary>
    public class VoxelMeshGenerator
    {
        readonly object _locker = new object();
        private readonly Queue<VoxelMeshGeneratorTask> _queue = new Queue<VoxelMeshGeneratorTask>();

        public void Shutdown()
        {
            if(_queue != null)
            {
                // Shutdown the thread with a null task
                _queue.Enqueue(null);
            }
        }

        /// <summary>
        /// Enqueue task for the generator thread
        /// </summary>
        /// <param name="item"></param>
        public void EnqueueTask(VoxelMeshGeneratorTask item)
        {
            lock (_locker)
            {
                _queue.Enqueue(item);
                Monitor.Pulse(_locker);
            }
        }

        /// <summary>
        /// Main generator loop
        /// </summary>
        public void Run()
        {
            // Run this generator thread until a null task is provided.  
            // This null task represents that the thread should be exited gracefully.
            while(true)
            {
                VoxelMeshGeneratorTask work;
                lock (_locker)
                {
                    while(_queue.Count == 0)
                    {
                        Monitor.Wait(_locker);
                    }
                    work = _queue.Dequeue();
                }

                if(work == null)
                {
                    return;
                }
                work.CreateMesh();
            }
        }
    }
}
