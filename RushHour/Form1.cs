using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using cpark;

namespace CarPark
{
    
    public partial class Form1 : Form
    {
        park p1;

        Color[] ctab;

        int selected_car = 0;

        int PUZZLE_WIDTH = 6;

        int PUZZLE_HEIGHT = 5;

        int CARS = 8;

        int MIN_SOLS = 20;

        int bw;

        bool ABORT_FLAG = false;

        float  angle = 0;

        public Form1()
        {
            InitializeComponent();

            this.button1_Click(this, null);

            p1 = new park ("{6x6:12-0-2,34-0-2,21-1-2,4-1-3,1-0-3,6-0-3,9-1-2,14-1-2,11-1-2,24-1-2,28-0-2,22-0-2}");
        }

        private void generateColorTab()
        {
            int c_min = 60;
            int c_max = 220;

            int color_step = 25;

            

            int color_r_min = Convert.ToInt32(Math.Floor((double)c_min  / color_step));

            int color_r_max = Convert.ToInt32(Math.Floor((double)c_max / color_step));

            Random R = new Random();

            ctab = new Color[50];

            for (int i = 0; i < 50; i++) ctab[i] = Color.FromArgb(R.Next(color_r_min, color_r_max) * color_step, R.Next(color_r_min, color_r_max) * color_step, R.Next(color_r_min, color_r_max) * color_step);

            //ctab[0] = Color.DarkBlue   ;
        }

        public string printPark(park p)
        {
            string res = "";

            for (int i = 0; i < p.occupancy.Length; i++)
            {
                res += p.occupancy[i].ToString() + " ";

                if (i % p.width == p.width - 1) res += Environment.NewLine;
            }

            return res;

        }

        public void listMoves(movesList l, ListBox lb)
        {
            lb.Items.Clear();

            for (int i = 0; i < l.count; i++)
            {
                string res = "car " + (l.car_ind[i] + 1).ToString() + " by " + l.dir[i].ToString();

                lb.Items.Add(res);
            }


        }



        private void button1_Click(object sender, EventArgs e)
        {
            PUZZLE_WIDTH = (int)numericUpDown1.Value;

            PUZZLE_HEIGHT = (int)numericUpDown2.Value;

            CARS = (int)numericUpDown3.Value;

            p1 = parkLogic.generatePark(PUZZLE_WIDTH, PUZZLE_HEIGHT, CARS);

            selected_car = 0;

            generateColorTab();

            updateInterface();

        }

        private void updateInterface()
        {
            textBox1.Text = p1.ToString();

            pictureBox1.Refresh();
        }


        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            bw = pictureBox1.Width / Math.Max(PUZZLE_HEIGHT, PUZZLE_WIDTH);

            Graphics G = e.Graphics;

          //  G.RotateTransform(angle);

            G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias ;

            G.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            int h_blocks = p1.height;

            int w_blocks = p1.width;

            int h = h_blocks * bw;

            int w = w_blocks * bw;

            int margin = 3;

            G.FillRectangle(Brushes.White, 0, 0, w, h);


            for (int x = 0; x < w; x += bw)
                for (int y = 0; y < h; y += bw) ;
                    //G.DrawRectangle(new Pen(Brushes.LightGray, 1), x, y, bw, bw);


            for (int i = 0; i < p1.count; i++)
            {
                int y = p1.pos[i] / p1.width;

                int x = p1.pos[i] % p1.width;

                int lx = 1;

                int ly = 1;

                if (p1.alignment[i] == 0) lx = p1.length[i]; else ly = p1.length[i];

                Brush border_brush = Brushes.Black ;

                Brush fill_brush = new SolidBrush(ctab[i]);

                G.FillRectangle(fill_brush, x * bw + margin, y * bw + margin, bw * lx - 2 * margin, bw * ly-2 * margin);

                G.DrawRectangle(new Pen(border_brush, 2), x * bw + margin, y * bw + margin, bw * lx-2 * margin, bw * ly-2 * margin);

                string block_label = (i + 1).ToString();

                Font block_font = new Font("Consolas", 22);

                SizeF s = G.MeasureString(block_label, block_font);

                float label_x = (x + (float)lx / 2) * bw - s.Width /2;

                float label_y = (y + (float)ly / 2) * bw - s.Height /2;


                G.DrawString(block_label , block_font  , Brushes.White, label_x , label_y);
            }

            if (selected_car > 0)
            {
                //int alpha = 100;

                int i = selected_car - 1;

                int y = p1.pos[i] / p1.width;

                int x = p1.pos[i] % p1.width;

                int lx = 1;

                int ly = 1;

                if (p1.alignment[i] == 0) lx = p1.length[i]; else ly = p1.length[i];

                G.DrawRectangle(new Pen(Color.Red, 4), x * bw, y * bw, bw * lx, bw * ly);

                //G.FillRectangle(new SolidBrush(Color.FromArgb(alpha, Color.Red)), x * bw, y * bw, bw * lx, bw * ly);


            }



        }







        private void button3_Click(object sender, EventArgs e)
        {

            MIN_SOLS = (int)numericUpDown4.Value;

            int trials = 0;

            int max_sol_steps = 0;

            //string sols_vector = "";

            park p = new park();

            park best_park = new park();

            button3.Enabled = false;

            button2.Enabled = true;

            Application.DoEvents();

            DateTime st = DateTime.Now;

            while (!ABORT_FLAG)
            //for (int i=0; i<1000; i++)
            {
                p = parkLogic.generatePark(PUZZLE_WIDTH, PUZZLE_HEIGHT, CARS);

                trials++;

                label1.Text = "Searching ... (tried " + trials.ToString() + " configuations, max solution steps = " + max_sol_steps.ToString() + ")";

                this.Refresh();

                Application.DoEvents();

                if (p.count < CARS) continue;

                Queue<park> Q = new Queue<park>();

                Q.Enqueue(p);

                searchResults r = parkSpaceLogic.SpaceSearch2(Q);

                if (r.states < 1) continue;

                if (r.solutions.Count == 0) continue;

                r = parkSpaceLogic.SpaceSearch2(r.solutions);

                p = r.lastNode;

                int steps_to_sol = r.distance_counter - 1;



                if (steps_to_sol > max_sol_steps)
                {
                    best_park = p;

                    max_sol_steps = steps_to_sol;
                }

                if (steps_to_sol >= MIN_SOLS)
                {
                    StreamWriter  res_file = File.AppendText (Directory.GetCurrentDirectory () + "\\car_park_puzzles.txt");

                    res_file.WriteLine (steps_to_sol.ToString ("00 solutions: ") +  p.ToString ());

                    res_file.Flush();

                    res_file.Close();
                }
                
               

            }

           // this.Text  = DateTime.Now.Subtract(st).TotalSeconds.ToString();

            p1 = best_park;

            generateColorTab();

            updateInterface();

            button4_Click(null, null);

            button3.Enabled = true;

            button2.Enabled = false;

            ABORT_FLAG = false;

            //Clipboard.SetText(sols_vector);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int xblock = e.X / bw;

            int yblock = e.Y / bw;

            int block = yblock * p1.width + xblock;

            int car = p1.occupancy[block];

            if (car != 0)
            {
                selected_car = car;
            }
            else
            {
                if (selected_car != 0)
                {
                    int i = selected_car - 1;

                    if (p1.alignment[i] == 0)
                    {
                        if (block >= p1.pos[i] + p1.length[i])
                        {
                            int dir = block - p1.pos[i] - p1.length[i] + 1;

                            if (parkLogic.checkMove(p1, i, dir)) p1 = parkLogic.makeMove2(p1, i, dir);
                        }

                        if (block < p1.pos[i])
                        {
                            int dir = block - p1.pos[i];
                            if (parkLogic.checkMove(p1, i, dir)) p1 = parkLogic.makeMove2(p1, i, dir);
                        }
                    }
                    else
                    {

                        //if (block == p1.pos[i] + p1.length[i]*p1.width)
                        if (block > p1.pos[i])
                        {
                            double dir_db = ((double)(block - p1.pos[i]) / p1.width) - p1.length[i] + 1;

                            if (dir_db % 1 == 0)
                            {
                                int dir = Convert.ToInt32(dir_db);

                                if (parkLogic.checkMove(p1, i, dir)) p1 = parkLogic.makeMove2(p1, i, dir);
                            }
                        }

                        //if (block == p1.pos[i] - p1.width)
                        if (block < p1.pos[i])
                        {
                            double dir_db = ((double)(block - p1.pos[i]) / p1.width);

                            if (dir_db % 1 == 0)
                            {
                                int dir = Convert.ToInt32(dir_db);

                                if (parkLogic.checkMove(p1, i, dir)) p1 = parkLogic.makeMove2(p1, i, dir);
                            }
                        }

                    }

                    selected_car = 0;

                }
            }

           // textBox1.Text = p1.getString();

            pictureBox1.Refresh();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Queue <park> Q = new Queue<park> ();

            Q.Enqueue (p1);

            label1.Text = "Working ..."; Application.DoEvents();

            searchResults r = parkSpaceLogic.SpaceSearch2(Q);

            int sol_count = 0;

            int steps_to_sol = -1;

            if (r.solutions != null) sol_count = r.solutions.Count;

            int state_count = r.states;

            if (r.solutions.Count > 0)
            {
                searchResults r2 = parkSpaceLogic.SpaceSearch2(r.solutions);

                steps_to_sol = r2.D[p1.GetHashCode()];
            }

            label1.Text = state_count.ToString() + " states, " + sol_count.ToString() + " solution(s), " + steps_to_sol.ToString() + " steps to solution";

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.button1_Click(null, null);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            this.button1_Click(null, null);

        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            this.button1_Click(null, null);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ABORT_FLAG = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {


            listMoves(parkLogic.generateMoves2(p1), listBox1);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox1.Text);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                p1 = new park(Clipboard.GetText());

                PUZZLE_WIDTH = p1.width;

                PUZZLE_HEIGHT = p1.height;

                CARS = p1.count;

                generateColorTab();

                updateInterface();
            }
            catch (Exception)
            {
                MessageBox.Show("Data in clipboard does not represent a valid state!", "Error",MessageBoxButtons.OK ,MessageBoxIcon.Exclamation   );


            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            label1.Text = "Solving ...";

            Queue<park> Q = new Queue<park>();

            Q.Enqueue(p1);

            searchResults r = parkSpaceLogic.SpaceSearch2(Q);

            if (r.solutions.Count == 0)
            {
                MessageBox.Show("No solutions found!", "Error");

                return;
            }

            searchResults r2 = parkSpaceLogic.SpaceSearch2(r.solutions);

            int current_steps_to_sol = r2.D[p1.GetHashCode()];

            int moveCount = 0;

            while (current_steps_to_sol > 0)
            {
                moveCount++;

                movesList L = parkLogic.generateMoves2(p1);

                for (int i = 0; i < L.count; i++)
                {
                    park p2 = p1.Clone();
                    
                    p2 = parkLogic.makeMove2(p2, L.car_ind[i], L.dir[i]);

                    int steps_to_sol = r2.D[p2.GetHashCode ()];

                    if (steps_to_sol < current_steps_to_sol)
                    {
                        current_steps_to_sol = steps_to_sol;

                        p1 = parkLogic.makeMove2(p1, L.car_ind[i], L.dir[i]);

                        pictureBox1.Refresh();

                        Application.DoEvents();

                        System.Threading.Thread.Sleep(250);

                        break;
                    }

                }

            }

            label1.Text = "Done1";

        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox1.Text = p1.ToString();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Queue<park> Q = new Queue<park>();

            Q.Enqueue(p1);

            label1.Text = "Working ..."; Application.DoEvents();

            searchResults r = parkSpaceLogic.SpaceSearch2(Q);

            int sol_count = r.solutions.Count;

            int steps_to_sol = -1;

            if (sol_count > 0)
            {
                r = parkSpaceLogic.SpaceSearch2(r.solutions);

                p1 = r.lastNode;

                steps_to_sol = r.distance_counter-1;

                pictureBox1.Refresh();
            }

            label1.Text = r.states.ToString() + " states, " + sol_count.ToString() + " solution(s), steps to solution: " + steps_to_sol.ToString(); ;

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            angle = (float) e.X / pictureBox1.Width * 15;
            //this.Text = e.X.ToString();
            //this.Text = pictureBox1.Width.ToString ();
            //this.Text = angle.ToString();
            pictureBox1.Refresh();
            
        }







    }
}
