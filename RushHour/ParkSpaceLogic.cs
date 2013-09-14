using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace cpark
{
    public struct searchResults
    {
        public int states;
       
        public Queue<park> solutions;

        public park lastNode;

        public int distance_counter;

        public Dictionary<int, int> D;

    };

    public class parkSpaceLogic
    {
   
        static public searchResults SpaceSearch2(Queue<park> current)
        {
            Queue<park> neighbours = new Queue<park>();

            Queue<park> solutions = new Queue<park>();

            Dictionary<int, int> D = new Dictionary<int, int>();

            int distance_counter = 0;

            bool has_nodes = false;

            park lastNode = null;

            while (current.Count > 0) // exit loop when current nodes are empty (graph is fully searched)
            {
                neighbours = new Queue<park>();

                has_nodes = false;

                while (current.Count > 0) // iterating through current nodes
                {
                    park node = current.Dequeue();

                    int node_hash = node.GetHashCode();

                    if (D.ContainsKey(node_hash)) continue; // bail if node was searched before

                    lastNode = node;

                    has_nodes = true;

                    D.Add(node_hash, distance_counter);

                    if (node.pos[0] == (node.width * 3 - 2)) solutions.Enqueue(node);                    
                    
                    // end of state checking

                    movesList L = parkLogic.generateMoves2(node);

                    for (int i = 0; i < L.count; i++)
                    {
                        park clone = node.Clone();

                        park next_node = parkLogic.makeMove2(clone, L.car_ind[i], L.dir[i]);

                        neighbours.Enqueue(next_node);

                        if (D.Count > 20000)
                        if (false )
                        {
                            // state graph too large, bailing ...

                            searchResults r = new searchResults();

                            r.states = -1;

                            r.solutions = solutions;

                            return r;

                        }
                    }

                }

                current = neighbours;

                if (has_nodes) distance_counter++;
            }

            searchResults r2 = new searchResults();

            r2.states = D.Count;

            r2.solutions = solutions;

            r2.lastNode = lastNode;

            r2.distance_counter = distance_counter;

            r2.D = D;

            return r2;

        }

    };

}