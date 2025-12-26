using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lv.BIM.Base
{
    public class Pair<T>
    {
        private T t_first;
        private T t_second;
        public T First { get { return t_first; } set { t_first = value; } }
        public T Second { get { return t_second; } set { t_second = value; } }
        public T Left => t_first;
        public T Right => t_second;
        public Pair(T class1, T class2) 
        { 
            t_first = class1;
            t_second = class2;
        }
        public void RemoveFirst()
        {
            t_first = default(T);
        }
        public void RemoveSecond()
        {
            t_second = default(T);
        }

    }

    public class Pair<T1,T2>
    {
        private T1 t_first;
        private T2 t_second;
        public T1 First { get { return t_first; } set { t_first = value; } }
        public T2 Second { get { return t_second; } set { t_second = value; } }
        public T1 Left => t_first;
        public T2 Right => t_second;
        public Pair(T1 class1, T2 class2)
        {
            t_first = class1;
            t_second = class2;
        }
        public void RemoveFirst()
        {
            t_first = default(T1);
        }
        public void RemoveSecond()
        {
            t_second = default(T2);
        }

    }
}
