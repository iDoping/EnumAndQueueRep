using System;
using System.Collections;
using System.Collections.Generic;

namespace NodeQueue
{
    /// <summary xml:lang="ru">
    /// Контракт коллекции типа очередь
    /// </summary>
    public interface IQueue<T> : IEnumerable
    {
        public T Dequeue();
        public void Enqueue(T data);
        public int Count { get; }
    }

    /// <summary xml:lang="ru">
    /// Представляет коллекцию типа очередь, которая хранит элементы типа 
    /// <typeparam 
    /// name = "Node"> 
    /// </typeparam>
    /// </summary>
    public class MyQueue<T> : IQueue<T>
    {
        private class Node<T>
        {
            public Node(T data)
            {
                Data = data;
            }
            public T Data { get; }
            public Node<T> Next { get; set; }
        }

        /// <summary xml:lang="ru">
        /// Количество элементов в очереди     
        /// <returns> 
        /// Количество элементов в очереди 
        /// </returns>
        /// </summary>
        public int Count { get { return _size; } }       

        Node<T> head;
        Node<T> tail;
        private int _size;
        private int _version = 0;

        /// <summary xml:lang="ru">
        /// Добавляет элемент в очередь       
        /// </summary>
        /// <param xml:lang="ru" name="data"> 
        /// элемент, который необходимо добавить
        /// </param>
        public void Enqueue(T data)
        {
            if (data == null)
                throw new InvalidOperationException("Object content is null");
            Node<T> node = new Node<T>(data);
            Node<T> tempNode = tail;
            tail = node;
            if (_size == 0)
                head = tail;
            else
                tempNode.Next = tail;
            _size++;
            _version++;
        }

        /// <summary xml:lang="ru">
        /// Возвращает элемент из очереди
        /// <returns> Извлеченный объект из очереди </returns>
        /// </summary>
        public T Dequeue()
        {
            if (_size == 0)
            {
                throw new InvalidOperationException("There are no items in the queue");
            }
            T output = head.Data;
            head = head.Next;
            _size--;
            _version++;
            return output;
        }

        /// <summary xml:lang="ru">
        /// <inheritdoc cref="IEnumerable"/>
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            return new QueueEnumerator(this);
        }

        private class QueueEnumerator : IEnumerator<T>
        {
            private bool firstMove = false;
            private MyQueue<T> _queue;
            private int _version;
            private Node<T> _currentElement;
            private bool disposed = false;

            public T Current
            {
                get
                {
                    ThrowIfDisposed();
                    return _currentElement.Data;
                }
            }

            object IEnumerator.Current => Current;

            public QueueEnumerator(MyQueue<T> q)
            {
                _queue = q;
                _version = _queue._version;
                _currentElement = _queue.head;
            }

            public bool MoveNext()
            {
                ThrowIfDisposed();
                if (_version != _queue._version)
                {
                    throw new InvalidOperationException("Unable to change queue while loop is running");
                }
                if (_currentElement.Next != null && firstMove == true)
                {
                    _currentElement = _currentElement.Next;
                }
                else if (firstMove == false)
                {
                    firstMove = true;
                }
                else
                {
                    return false;
                }
                return true;
            }

            public void Reset()
            {
                ThrowIfDisposed();
                if (_version != _queue._version)
                {
                    throw new InvalidOperationException("There are no items in the queue");
                }
                _currentElement = _queue.head;
                firstMove = false;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposed)
                {
                    if (disposing)
                    {
                        _currentElement = null;
                        _queue = null;
                    }
                    disposed = true;
                }
            }

            private void ThrowIfDisposed()
            {
                if (disposed)
                {
                    throw new ObjectDisposedException("Object Disposed");
                }
            }
        }
    }
}