using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADO_Project_Final
{
    public partial class Form1 : Form
    {
       
        BindingSource bsS = new BindingSource();
        BindingSource bsC = new BindingSource();
        DataSet ds;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AddFromStudent { TheForm = this }.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            dataGridView1.AutoGenerateColumns = false;
            LoadDataBindingSources();

        }

        public void LoadDataBindingSources()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM students", con))
                {
                    ds = new DataSet();
                    da.Fill(ds, "students");
                    da.SelectCommand.CommandText = "SELECT * FROM courses";
                    da.Fill(ds, "courses");

                    ds.Tables["students"].Columns.Add(new DataColumn("image", typeof(byte[])));
                    for (var i = 0; i < ds.Tables["students"].Rows.Count; i++)
                    {
                        ds.Tables["students"].Rows[i]["image"] = File.ReadAllBytes($@"..\..\Pictures\{ds.Tables["students"].Rows[i]["Picture"]}");
                    }

                    DataRelation rel = new DataRelation("FK_S_S", ds.Tables["students"].Columns["studentId"], ds.Tables["courses"].Columns["studentId"]);
                    ds.Relations.Add(rel);
                    bsS.DataSource = ds;
                    bsS.DataMember = "students";

                    bsC.DataSource = bsS;
                    bsC.DataMember = "FK_S_S";
                    dataGridView1.DataSource = bsC;
                    AddDataBindings();

                }
            }
        }

        private void AddDataBindings()
        {
            lblId.DataBindings.Clear();
            lblId.DataBindings.Add("Text", bsS, "studentId");
            lblName.DataBindings.Clear();
            lblName.DataBindings.Add("Text", bsS, "studentName");
            lbldob.DataBindings.Clear();
            lbldob.DataBindings.Add("Text", bsS, "dateOfBirth");
            Binding bm = new Binding("Text", bsS, "dateOfBirth",true);
            bm.Format += Bm_Format;
            lbldob.DataBindings.Clear();
            lbldob.DataBindings.Add(bm);
            pictureBox1.DataBindings.Clear();
            pictureBox1.DataBindings.Add(new Binding("Image", bsS, "image", true));
            checkBox1.DataBindings.Clear();
            checkBox1.DataBindings.Add("Checked", bsS, "insideDhaka", true);
        }

        private void Bm_Format(object sender, ConvertEventArgs e)
        {
            DateTime d = (DateTime)e.Value;
            e.Value = d.ToString("dd-MM-yyyy");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bsS.Position < bsS.Count - 1)
            {
                bsS.MoveNext();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (bsS.Position > 0)
            {
                bsS.MovePrevious();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bsS.MoveLast();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bsS.MoveFirst();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int v = int.Parse((bsS.Current as DataRowView).Row[0].ToString());
            new EditForm { TheForm = this, IdToEdit = v }.ShowDialog();
        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void addToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            new AddFromStudent { TheForm = this }.ShowDialog();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void editDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int v = int.Parse((bsS.Current as DataRowView).Row[0].ToString());
            new EditForm { TheForm = this, IdToEdit = v }.ShowDialog();
        }

        private void studentReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new StudentReportForm().Show();
        }
    }
}
