using System;
using System.Collections;
using System.Collections.Generic;

namespace EnumExtencions
{
    public static class MyExtensions
    {
        private class MyTakeEx<T> : IEnumerable<T>, IEnumerator<T>, IDisposable
        {
            private int _count;
            private T _current;
            private IEnumerable<T> _source;
            private IEnumerator<T> _sourceE;

            public T Current => _sourceE.Current;

            object IEnumerator.Current => _current;

            public MyTakeEx(IEnumerable<T> source, int count)
            {
                _count = count;
                _source = source;
                _sourceE = _source.GetEnumerator();
            }

            public bool MoveNext()
            {
                if (_count != 0 && _sourceE.MoveNext())
                {
                    _count--;
                    return true;
                }
                else return false;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
            }

            public IEnumerator<T> GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _source.GetEnumerator();
            }
        }

        private class MyWhereEx<T> : IEnumerator<T>, IEnumerable<T>, IDisposable
        {
            private Func<T, bool> _predicate;
            private T _current;
            private IEnumerable<T> _source;
            private IEnumerator<T> _sourceE;

            public T Current => _sourceE.Current;

            object IEnumerator.Current => _current;

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
                    if (_predicate(_sourceE.Current))
                    {
                        _current = _sourceE.Current;
                        return true;
                    }
                }
                return false;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
            }

            public IEnumerator<T> GetEnumerator()
            {
                return this;
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
}