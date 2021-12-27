using CodeWalker.GameFiles;
using SharpDX;

namespace YND_Mover
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> YNDfiles = lbFiles.Items.Cast<string>().ToList();

            foreach (var YNDf in YNDfiles)
            {
                YndFile yndFile = new();
                yndFile.Load(File.ReadAllBytes(YNDf));

                foreach (var node in yndFile.Nodes)
                {
                    node.SetPosition(node.Position += new Vector3(float.Parse(tbX.Text), float.Parse(tbY.Text), float.Parse(tbZ.Text)));
                    node.AreaID = (ushort)GetAreaIDfromPosition(node.Position);
                    node.UpdateLinkLengths();
                    yndFile.UpdateBvhForNode(node);

                }

                yndFile.BuildStructsOnSave = true;
                yndFile.UpdateAllNodePositions();
                yndFile.UpdateBoundingBox();

            }
        }

        private void selectFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "(*.ynd)|*.ynd"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                lbFiles.DataSource = ofd.FileNames;
            }
        }

        private void clearListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbFiles.DataSource = null;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit(); 
        }


        public int GetAreaIDfromPosition(Vector3 abc)
        {
            //code ported from ofio.v.xml.nodes.io.ms by Mixazzz
            int AreaID = 0;
            int Result = 0;
            float Xpos = -8192f;
            float Ypos = -8192f;

            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    float xmin = Xpos;
                    float xmax = Xpos + 512f;
                    float ymin = Ypos;
                    float ymax = Ypos + 512f;

                    float node_x = abc.X;
                    float node_y = abc.Y;
                    bool v = ymin < node_y && node_y < ymax;
                    bool v1 = xmin < node_x && node_x < xmax;
                    if (v1 && v)
                    {
                        Result = AreaID;
                    }
                    AreaID += 1;
                    Xpos += 512f;
                }

                Ypos += 512f;
                Xpos = -8192f;
            }

            return Result;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                tbOutput.Text = fbd.SelectedPath;
            }
        }
    }
}