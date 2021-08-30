using System;
using System.Collections.Generic;

namespace EnumExtencions
{   
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
