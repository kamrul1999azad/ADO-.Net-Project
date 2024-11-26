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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace ADO_Project_Final
{
    public partial class AddFromStudent : Form
    {
        List<CourseDetails> details = new List<CourseDetails>();
        string currentFile = "";
        public AddFromStudent()
        {
            InitializeComponent();
        }
        public Form1 TheForm {  get; set; }

        private void AddFromStudent_Load(object sender, EventArgs e)
        {
            
            dataGridView1.AutoGenerateColumns = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                con.Open();
                using (SqlTransaction trx = con.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.Transaction = trx;
                        string ext = Path.GetExtension(currentFile);
                        string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                        string savePath = @"..\..\Pictures\" + f;
                        MemoryStream ms = new MemoryStream(File.ReadAllBytes(currentFile));
                        byte[] bytes = ms.ToArray();
                        FileStream fs = new FileStream(savePath, FileMode.Create);
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Close();
                        cmd.CommandText = "INSERT INTO students(studentName, dateOfBirth,insideDhaka, picture) VALUES(@sn, @dob, @isd, @pic); SELECT SCOPE_IDENTITY();";
                        cmd.Parameters.AddWithValue("@sn", textBox2.Text);
                       
                        cmd.Parameters.AddWithValue("@dob", dateTimePicker3.Value);
                        cmd.Parameters.AddWithValue("@isd", checkBox1.Checked);
                        cmd.Parameters.AddWithValue("@pic", f);
                        try
                        {
                            var sid = cmd.ExecuteScalar();
                            foreach (var s in details)
                            {
                                cmd.CommandText = "INSERT INTO courses (courseName, startDate, EndDate, courseFee, studentId) VALUES (@cn, @sd, @ed, @cf, @i)";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@cn", s.courseName);
                                cmd.Parameters.AddWithValue("@sd", s.startDate);
                                cmd.Parameters.AddWithValue("@ed", s.endDate);
                                cmd.Parameters.AddWithValue("@cf", s.courseFee);
                                cmd.Parameters.AddWithValue("@i", sid);
                                cmd.ExecuteNonQuery();
                            }
                            trx.Commit();
                            TheForm.LoadDataBindingSources();
                            MessageBox.Show("Data Saved", "Success");
                            details.Clear();
                            dataGridView1.DataSource = null;
                            dataGridView1.DataSource = details;
                            textBox2.Clear();
                            
                            dateTimePicker3.Value = DateTime.Now;
                            checkBox1.Checked = false;
                           
                            pictureBox1.Image = Image.FromFile(@"..\..\Pictures\No Photo.jpg");
                            textBox1.Clear();
                        }
                        catch
                        {
                            trx.Rollback();
                        }

                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            details.Add(new CourseDetails
            {
               courseName = textBox1.Text,
               startDate = dateTimePicker1.Value,
               endDate = dateTimePicker2.Value,
               courseFee=numericUpDown1.Value,
               
            });
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = details;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                details.RemoveAt(e.RowIndex);
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = details;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                currentFile = openFileDialog1.FileName;
                pictureBox1.Image = Image.FromFile(currentFile);
            }
        }
    }
    public class CourseDetails
    {
        public string courseName { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public decimal courseFee { get; set; }
    }
}
