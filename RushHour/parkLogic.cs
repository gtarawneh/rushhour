using System;
using System.Windows.Forms;

namespace cpark
{
    public class park
    {
        public int width;
        public int height;
        public int count;
        public int[] pos; //index of the object's head
        public int[] alignment; //0 for hor, 1 for ver
        public int[] length;
        public int[] occupancy;

        public Int64 hash;

        public park()
        {
            hash = width = height = count = 0;
            
            pos = alignment = length = occupancy = new int[0];
        }

        public park(string source)
        {
            string[] strss= source.Split(':');

            string str1 = strss[0];

            string str2 = strss[1];

            string str1_1 = str1.Split('x')[0].Replace ('{','0');

            string str1_2 = str1.Split ('x')[1];

            width = Convert.ToInt32(str1_1);

            height = Convert.ToInt32(str1_2);

            str2 = str2.Remove(str2.Length-1);

            string[] str2_s = str2.Split(',');

            count = str2_s.Length ;

            pos = new int[count];
            alignment = new int[count];
            length = new int[count];
            occupancy = new int[width *height];

            for (int i = 0; i < str2_s.Length; i++)
            {
                string[] item = str2_s[i].Split ('-');

                pos[i] = Convert.ToInt32(item[0]);
                alignment [i] = Convert.ToInt32(item[1]);
                length[i] = Convert.ToInt32(item[2]);

                int current_pos = pos[i];

                for (int k = 0; k < length[i]; k++)
                {
                    occupancy[current_pos] = i + 1;

                    if (alignment[i] == 1) current_pos += width; else current_pos++;
                }

            }
        }

        public park Clone()
        {
            park clone = new park();

            clone.width = width;
            clone.height = height;
            clone.count = count;

            clone.pos = new int[count];
            clone.alignment = new int[count];
            clone.length = new int[count];
            clone.occupancy = new int[width * height];

            for (int i = 0; i < count; i++)
            {
                clone.pos[i] = pos[i]; clone.alignment[i] = alignment[i]; clone.length[i] = length[i];
            }

            for (int i = 0; i < width * height; i++)
            {
                clone.occupancy[i] = occupancy[i];
            }

            return clone;
        }

        override public string ToString() 
        {
            string res = "{" + width.ToString() + "x" + height.ToString() + ":";

            for (int i = 0; i < count; i++)
            {
                res += pos[i].ToString() + "-" + alignment[i].ToString() + "-" + length[i].ToString();

                if (i < count - 1) res += ",";
            }

            res += "}";

            return res;
        }

        public override int GetHashCode()
        {
            int hash = 0;

            for (int i = 0; i < count; i++)
            {
                hash *= 6;

                if (alignment[i] == 0)
                {
                    hash += (pos[i] % width);
                }
                else
                {
                    hash += (pos[i] / width);

                }
            }
            

            return hash;
        }

    };


    public class movesList
    {
        public int count;
        public int[] car_ind;
        public int[] dir; //0 for head direction, 1 for tail

        public movesList()
        {
            count = 0;
            car_ind = new int[80];
            dir = new int[80];
        }
    }

  

    public class parkLogic
    {
        static public park generatePark(int w, int h, int c)
        {
            park res = new park();

            Random R = new Random();

            res.width = w; res.height = h;

            res.pos = new int[c];
            res.alignment = new int[c];
            res.length = new int[c];
            res.occupancy = new int[w * h];

            // creating primary car (car #1)

            res.alignment[0] = 0;

            res.length[0] = 2;

            res.pos[0] = w * 2;

            res.occupancy[w * 2 + 1] = res.occupancy[w * 2] = 1;

            res.count++;

            int iters = 0;

            int MAX_iters = 100;

            int lower_limit = w * 2; 

            int upper_limit = w * 3 - 1;

            // creating other cars

            while (res.count < c)
            {
                int pos = R.Next(w * h);

                int alignment = R.Next(2);

                int length = 2 + R.Next(2);

                if (alignment == 0) if (pos <= upper_limit) if (pos >= lower_limit) continue;// prevent horizontal blocks within [lower_limit, upper_limit]

                bool valid_pos = true;

                int current_pos = pos;

                for (int i = 0; i < length; i++)
                {


                    if ((alignment == 0) && (w - (pos % w) < length)) { valid_pos = false; break; } // checking if horizontal cars aren't too close to the right edge

                    if (current_pos >= res.occupancy.Length) { valid_pos = false; break; }


                    if (res.occupancy[current_pos] > 0) { valid_pos = false; break; }

                    if (alignment == 1) current_pos += w; else current_pos++;
                }

                if (valid_pos)
                {
                    int j = res.count++;

                    res.pos[j] = pos; res.alignment[j] = alignment; res.length[j] = length;

                    current_pos = pos;

                    for (int i = 0; i < length; i++)
                    {
                        res.occupancy[current_pos] = j + 1;

                        if (alignment == 1) current_pos += w; else current_pos++;
                    }

                }

                iters++;

                if (iters > MAX_iters)
                {
                   // MessageBox.Show("Unable to position required number of cars within park space!", "Error");

                    break;
                }
            }

            return res;

        }

        static public movesList generateMoves2(park p)
        {
            bool ONEBLOCK_MOVES_ONLY = false ;

            movesList L = new movesList();

            for (int i = 0; i < p.count; i++)
            {
                if (p.alignment[i] == 0)
                {
                    // horizontal car

                    for (int k = 0; k < p.width; k++)
                    {
                        if ((p.pos[i] % p.width) + p.length[i] + k < p.width)
                        {
                            if (p.occupancy[p.pos[i] + p.length[i] + k] == 0)
                            {
                                int j = L.count;
                                L.car_ind[j] = i;
                                L.dir[j] = 1 + k;
                                L.count++;
                                if (ONEBLOCK_MOVES_ONLY)if (Math.Abs(L.dir [j])>1) L.count --;
                            }
                            else break;
                        }
                        else break;
                    }

                    for (int k = 0; k > -p.width; k--)
                    {
                        if ((p.pos[i] % p.width) + k > 0)
                        {
                            if (p.occupancy[p.pos[i] - 1 + k] == 0)
                            {
                                int j = L.count;
                                L.car_ind[j] = i;
                                L.dir[j] = -1 + k;
                                L.count++;
                                if (ONEBLOCK_MOVES_ONLY) if (Math.Abs(L.dir[j]) > 1) L.count--;
                            }
                            else break;
                        }
                        else break;
                    }
                }
                else
                {
                    // vertical car

                    for (int k = 0; k < p.height; k++)
                    {
                        if (p.pos[i] + p.length[i] * (p.width) + k * p.width < (p.width * p.height))
                        {
                            if (p.occupancy[p.pos[i] + p.length[i] * (p.width) + k * p.width] == 0)
                            {
                                int j = L.count;
                                L.car_ind[j] = i;
                                L.dir[j] = 1 + k;
                                L.count++;
                                if (ONEBLOCK_MOVES_ONLY) if (Math.Abs(L.dir[j]) > 1) L.count--;
                            }
                            else break;
                        }
                        else break;
                    }


                    for (int k = 0; k > -p.height; k--)
                    {
                        if (p.pos[i] + k * p.width > p.width - 1)
                        {
                            if (p.occupancy[p.pos[i] - p.width + k * p.width] == 0)
                            {
                                int j = L.count;
                                L.car_ind[j] = i;
                                L.dir[j] = -1 + k;
                                L.count++;
                                if (ONEBLOCK_MOVES_ONLY) if (Math.Abs(L.dir[j]) > 1) L.count--;
                            }
                            else break;
                        }
                        else break;
                    }

                }

            }

            return L;

        }

        static public park makeMove2(park p, int car, int dir)
        {
            // clearing old car occupancy

            if (p.alignment[car] == 0)
            {
                for (int i = 0; i < p.length[car]; i++) p.occupancy[p.pos[car] + i] = 0;

                p.pos[car] += dir;

                for (int i = 0; i < p.length[car]; i++) p.occupancy[p.pos[car] + i] = car + 1;
            }
            else
            {
                for (int i = 0; i < p.length[car]; i++) p.occupancy[p.pos[car] + i * p.width] = 0;

                p.pos[car] += dir * p.width;

                for (int i = 0; i < p.length[car]; i++) p.occupancy[p.pos[car] + i * p.width] = car + 1;
            }

            return p;
        }

        static public bool checkMove(park p, int car, int dir)
        {
            movesList l = parkLogic.generateMoves2(p);

            for (int i = 0; i < l.count; i++)
            {
                if ((car == l.car_ind[i]) && (dir == l.dir[i])) return true;
            }

            return false;
        }
    };
}