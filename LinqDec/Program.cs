using System;
using System.Collections;
using System.Collections.Generic;

namespace LinqDec
{
    static class MyExtensions
    {
        private class MyTakeEx<T> : IEnumerable<T>, IEnumerator<T>, IDisposable
        {
            private int _count;
            private T _current;
            private IEnumerable<T> _source;
            private IEnumerator<T> _sourceE;

            public MyTakeEx(IEnumerable<T> source, int count)
            {
                _count = count;
                _source = source;
                _sourceE = _source.GetEnumerator();
            }

            void IDisposable.Dispose()
            {
            }

            bool IEnumerator.MoveNext()
            {
                if (_count != 0 && _sourceE.MoveNext())
                {
                        _count--;
                        return true;                  
                }
                else return false;
            }

            public T Current => _sourceE.Current;

            object IEnumerator.Current => _current;

            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }
         
            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _source.GetEnumerator();
            }
        }

        private class MyWhereEx<T> : IEnumerable<T>, IEnumerator<T>, IDisposable
        {
            private Func<T, bool> _predicate;
            private T _current;
            private IEnumerable<T> _source;
            private IEnumerator<T> _sourceE;

            public MyWhereEx(Func<T, bool> predicate, IEnumerable<T> source)
            {
                _predicate = predicate;
                _source = source;
                _sourceE = _source.GetEnumerator();
            }

            public bool MoveNext()
            {
                while (_sourceE.MoveNext())
                {
                    var temp = _sourceE.Current; //убрать
                    if (_predicate(temp))
                    {
                        _current = temp;
                        return true;
                    }
                }
                return false;
            }

            public T Current => _sourceE.Current;

            object IEnumerator.Current => _current;
           
            public void Dispose()
            {
            }

            public IEnumerator<T> GetEnumerator()
            {
                return this;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _source.GetEnumerator();
            }
        }

        public static IEnumerable<T> MyWhere<T>(this IEnumerable<T> source, Func<T, bool> condition)
        {
            var temp = new MyWhereEx<T>(condition, source);
            return temp;
        }

        public static IEnumerable<T> MyTake<T>(this IEnumerable<T> source, int count)
        {
            var temp = new MyTakeEx<T>(source, count);
            return temp;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            foreach (var item in GetEnumeration().MyTake(5).MyWhere(x => x % 2 == 0))
            {
                Console.WriteLine(item);
            }
        }

        public static IEnumerable<int> GetEnumeration()
        {
            int i = 0;
            while (true)
            {
                yield return i++;
            }
        }
    }
}