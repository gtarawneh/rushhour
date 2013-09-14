using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumberOfPossibilities
{
    class State
    {
        public State()
        {
        }
        public State(State s)
        {
            Array.Copy(s.heights, heights, 6);
        }
        public int[] heights = new int[6];
        public override int GetHashCode()
        {
            int hash = 0;
            for (int i = 0; i < 6; i++)
            {
                hash *= 5;
                hash += (heights[i]+1);
            }
            return hash;
        }
        public override bool Equals(object obj)
        {
            State s = (State)obj;
            for (int i = 0; i < 6; i++)
                if (heights[i] != s.heights[i])
                    return false;
            return true;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<State, long> states = new Dictionary<State, long>();

            states[new State()] = 1;

            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 6; x++)
                {
                    Dictionary<State, long> newStates = new Dictionary<State,long>();

                    foreach(State s in states.Keys)
                    {
                        for(int i=0;i<5;i++)
                        {
                            State sn = place(s, i, x);
                            if (sn != null)
                            {
                                long oldCount;
                                newStates.TryGetValue(sn, out oldCount);
                                newStates[sn] = oldCount + states[s];
                            }
                        }
                    }
                    states = newStates;
                }

                {
                    Dictionary<State, long> newStates = new Dictionary<State,long>();
                    foreach(State s in states.Keys)
                    {
                        var sn = crop(s);
                        long oldCount;
                        newStates.TryGetValue(sn, out oldCount);
                        newStates[sn] = oldCount + states[s];
                    }
                    states = newStates;
                }
            }

            Console.WriteLine(states.Values.Sum());
        }

        private static State crop(State s)
        {
            State ns = new State(s);
            for (int i = 0; i < 6; i++)
            {
                if (ns.heights[i] > -1)
                    ns.heights[i]--;
            }
            return ns;
        }

        private static State place(State s, int i, int x)
        {
            switch (i)
            {
                case 4:
                    return s;
                case 0:
                    return placeHor(s, x, 2);
                case 1:
                    return placeHor(s, x, 3);
                case 2:
                    return placeVert(s, x, 2);
                case 3:
                    return placeVert(s, x, 3);
                default:
                    throw new Exception();
            }
        }

        private static State placeVert(State s, int x, int p)
        {
            State newState = new State(s);
            if (x + 1 > 6)
                return null;
                if (s.heights[x] != 0)
                    return null;
                newState.heights[x] = p;
            return newState;
        }

        private static State placeHor(State s, int x, int p)
        {
            State newState = new State(s);
            if (x + p > 6)
                return null;
            if (x > 0 && s.heights[x - 1] <= 0)
                return null;
            for (int i = x; i < x + p; i++)
            {
                if (s.heights[i] > 0)
                    return null;
                newState.heights[i] = 1;
            }
            return newState;
        }
    }
}
