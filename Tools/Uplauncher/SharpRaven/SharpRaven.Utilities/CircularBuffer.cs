using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SharpRaven.Utilities
{
	public class CircularBuffer<T>
	{
		private readonly int size;

		private ConcurrentQueue<T> queue;

		public CircularBuffer(int size = 100)
		{
			this.size = size;
			queue = new ConcurrentQueue<T>();
		}

		public List<T> ToList()
		{
			List<T> list = queue.ToList();
			return list.Skip(Math.Max(0, list.Count - size)).ToList();
		}

		public void Clear()
		{
			queue = new ConcurrentQueue<T>();
		}

		public void Add(T item)
		{
			if (queue.Count >= size)
			{
				queue.TryDequeue(out var _);
			}
			queue.Enqueue(item);
		}

		public bool IsEmpty()
		{
			return queue.IsEmpty;
		}
	}
}
