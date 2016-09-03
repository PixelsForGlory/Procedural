// Copyright 2015-2016 afuzzyllama. All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace PixelsForGlory.Procedural
{
    /// <summary>
    /// Voxel mesh generator that generator thread uses
    /// </summary>
    public class VoxelMeshGenerator
    {
        readonly object _locker = new object();
        private readonly Queue<IVoxelMeshGeneratorTask> _queue = new Queue<IVoxelMeshGeneratorTask>();

        /// <summary>
        /// Shutdown the generator
        /// </summary>
        public void Shutdown()
        {
            // Shutdown the thread with a null task
            _queue?.Enqueue(null);
        }

        /// <summary>
        /// Enqueue task for the generator thread
        /// </summary>
        /// <param name="item"></param>
        public void EnqueueTask(IVoxelMeshGeneratorTask item)
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
            while (true)
            {
                try
                {
                    IVoxelMeshGeneratorTask work;
                    lock (_locker)
                    {
                        while (_queue.Count == 0)
                        {
                            Monitor.Wait(_locker);
                        }
                        work = _queue.Dequeue();
                    }

                    if (work == null)
                    {
                        return;
                    }
                    work.CreateMesh();
                }
                catch (Exception e)
                {
                    if(e.Message != "Thread was being aborted")
                    {
                        Debug.Log(e.Message);
                        Debug.Log(e.StackTrace);
                    }
                }
            }
        }
    }
}
