using System;
using System.Collections;
using System.Collections.Generic;

namespace NodeQueue
{
    /// <summary>
    /// Интерфейс IQueue
    /// </summary>
    public interface IQueue<T> : IEnumerable
    {
        public T Dequeue();
        public void Enqueue(T data);
        public int Count { get; }
    }
    /// <summary>
    /// Класс, описывающий объект в очереди
    /// </summary>
    public class Node<T>
    {
        public Node(T data)
        {
            Data = data;
        }
        public T Data { get; }
        public Node<T> Next { get; set; }
    }

    /// <summary>
    /// Класс, описывающий очередь
    /// </summary>
    public class MyQueue<T> : IQueue<T>
    {
        Node<T> head;
        Node<T> tail;
        private int _size;
        private int _version = 0;

        /// <summary>
        /// Метод, с помощью которого происходит добавление элемента в очередь
        /// <param name="data"> содержимое объекта, добавляемого в очередь
        /// </summary>
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

        /// <summary>
        /// Метод, с помощью которого происходит извлечение элемента из очереди
        /// <returns> Извлеченный объект из очереди </returns>
        /// </summary>
        public T Dequeue()
        {
            if (_size == 0)
                throw new InvalidOperationException("There are no items in the queue");
            T output = head.Data;
            head = head.Next;
            _size--;
            _version++;
            return output;
        }

        /// <summary>
        /// Метод, возвращающий количество элементов в очереди       
        /// <returns> Количество элементов в очереди </returns>
        /// </summary>
        public int Count { get { return _size; } }

        /// <summary>
        /// Реализация интерфейса IEnumerable
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            return new QueueEnum<T>(this);
        }

        /// <summary>
        /// Класс, реализующий итератор
        /// </summary>
        public class QueueEnum<T> : IEnumerator<T>
        {
            private bool firstMove = false;
            private MyQueue<T> _queue;
            private int _version;
            private Node<T> _currentElement;
            private bool disposed = false;
            /// <summary>
            /// Конструктор класса QueueEnum
            /// </summary>
            public QueueEnum(MyQueue<T> q)
            {
                _queue = q;
                _version = _queue._version;
                _currentElement = _queue.head;
            }

            /// <summary>
            /// Обобщенная версия метода Current, с помощью которого происходит получение текущего элтемента в итераторе
            /// </summary>
            public T Current
            {
                get
                {
                    ThrowIfDisposed();
                    return _currentElement.Data;
                }
            }

            /// <summary>
            /// Необобщенная версия метода Current, с помощью которого происходит получение текущего элтемента в итераторе
            /// </summary>
            object IEnumerator.Current => Current;

            /// <summary>
            /// Реализация интерфейса IDisposable.
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Освобождение управляемых ресурсов
            /// </summary>
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
                    throw new ObjectDisposedException("Object Disposed");
            }

            /// <summary>
            /// Метод, с помощью которого происходит переход к следующему элементу
            /// <returns> Возвращает true, если есть следующий элемент, false, если следующего элемента не существует </returns>
            /// </summary>
            public bool MoveNext()
            {
                ThrowIfDisposed();
                if (_version != _queue._version)
                    throw new InvalidOperationException("Unable to change queue while loop is running");
                if (_currentElement.Next != null && firstMove == true)
                    _currentElement = _currentElement.Next;
                else if (firstMove == false)
                {
                    firstMove = true;
                }
                else
                    return false;
                return true;
            }

            /// <summary>
            /// Метод, устанавливающий итератор в начальное положение
            /// </summary>
            public void Reset()
            {
                ThrowIfDisposed();
                if (_version != _queue._version)
                    throw new InvalidOperationException("Unable to change queue while loop is running");
                _currentElement = _queue.head;
                firstMove = false;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

        }
    }
}