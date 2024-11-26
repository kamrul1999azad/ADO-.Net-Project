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
    public partial class EditForm : Form
    {
        List<CourseDetails> details = new List<CourseDetails>();
        string currentFile = "";
        string oldFile = "";
        public EditForm()
        {
            InitializeComponent();
        }
        public Form1 TheForm { get; set; }
        public int IdToEdit { get; set; }

        private void EditForm_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            LoadInForm();
        }

        private void LoadInForm()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM students WHERE studentId=@i", con))
                {
                    cmd.Parameters.AddWithValue("@i", IdToEdit);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        textBox2.Text = dr.GetString(1);

                        dateTimePicker3.Value = dr.GetDateTime(2).Date;
                        checkBox1.Checked = dr.GetBoolean(3);
                        pictureBox1.Image = Image.FromFile(@"..\..\Pictures\" + dr.GetString(4));
                        oldFile = dr.GetString(4);
                    }
                    dr.Close();
                    cmd.CommandText = @"SELECT * FROM courses WHERE courseId = @i";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@i", IdToEdit);
                    SqlDataReader dr2 = cmd.ExecuteReader();
                    while (dr2.Read())
                    {
                        details.Add(new CourseDetails { courseName = dr2.GetString(1), startDate = dr2.GetDateTime(2), endDate = dr2.GetDateTime(3), courseFee = dr2.GetDecimal(4) });
                    }
                    SetDataSources();
                    con.Close();
                }
            }
        }

        private void SetDataSources()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = details;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                currentFile = openFileDialog1.FileName;
                pictureBox1.Image = Image.FromFile(currentFile);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            details.Add(new CourseDetails
            {
                courseName = textBox1.Text,
                startDate = dateTimePicker1.Value,
                endDate = dateTimePicker2.Value,
                courseFee = numericUpDown1.Value,

            });
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = details;
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
                        string f = oldFile;
                        if (currentFile != "")
                        {
                            string ext = Path.GetExtension(currentFile);
                            f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                            string savePath = @"..\..\Pictures\" + f;
                            MemoryStream ms = new MemoryStream(File.ReadAllBytes(currentFile));
                            byte[] bytes = ms.ToArray();
                            FileStream fs = new FileStream(savePath, FileMode.Create);
                            fs.Write(bytes, 0, bytes.Length);
                            fs.Close();
                        }
                        cmd.CommandText = "UPDATE students SET studentName=@sn, dateOfBirth=@dob,insideDhaka=@isd, picture=@pic WHERE studentId=@id";
                        cmd.Parameters.AddWithValue("@id", IdToEdit);
                        cmd.Parameters.AddWithValue("@sn", textBox2.Text);
                       
                        cmd.Parameters.AddWithValue("@dob", dateTimePicker3.Value);
                        cmd.Parameters.AddWithValue("@isd", checkBox1.Checked);
                        cmd.Parameters.AddWithValue("@pic", f);
                        try
                        {
                            
                            cmd.ExecuteNonQuery();
                           
                            cmd.CommandText = "DELETE FROM courses WHERE studentId=@id";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@id", IdToEdit);
                            cmd.ExecuteNonQuery();
                            foreach (var s in details)
                            {
                                cmd.CommandText = " INSERT INTO courses (courseName,startDate,endDate,courseFee,studentId) VALUES (@cn,@sd,@ed,@cf,@i)";

                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@cn", s.courseName);
                                cmd.Parameters.AddWithValue("@sd", s.startDate);
                                cmd.Parameters.AddWithValue("@ed", s.endDate);
                                cmd.Parameters.AddWithValue("@cf", s.courseFee);
                                cmd.Parameters.AddWithValue("@i", IdToEdit);
                                cmd.ExecuteNonQuery();
                            }
                            trx.Commit();
                            TheForm.LoadDataBindingSources();
                            MessageBox.Show("Data Updated", "Success");
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                details.RemoveAt(e.RowIndex);
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = details;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {
                    string sql = @"DELETE  courses
                                    WHERE studentId=@id";
                    using (SqlCommand cmd = new SqlCommand(sql, con, tran))
                    {
                        cmd.Parameters.AddWithValue("@id", IdToEdit);
                        try
                        {
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            cmd.CommandText = "DELETE FROM students WHERE studentId=@id";
                            cmd.Parameters.AddWithValue("@id", IdToEdit);
                            cmd.ExecuteNonQuery();
                            tran.Commit();
                            MessageBox.Show("Data Deleted", "Success");
                            TheForm.LoadDataBindingSources();
                            this.Close();
                        }
                        catch
                        {
                            tran.Rollback();
                            MessageBox.Show("Failed to Delete", "Error");
                        }
                        con.Close();
                    }
                }
            }
        }
    }
}

