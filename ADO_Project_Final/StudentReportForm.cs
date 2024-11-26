using ADO_Project_Final.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADO_Project_Final
{
    public partial class StudentReportForm : Form
    {
        public StudentReportForm()
        {
            InitializeComponent();
        }

        private void StudentReportForm_Load(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM students", con))
                {
                    DataSet ds = new DataSet();
                    da.Fill(ds, "students");
                    da.SelectCommand.CommandText = "SELECT * FROM courses";
                    da.Fill(ds, "courses");
                    StudentReport srpt = new StudentReport();
                    srpt.SetDataSource(ds);
                    crystalReportViewer1.ReportSource=srpt;
                    srpt.Refresh();
                    crystalReportViewer1.Refresh();
                }
            }
        }
    }
}
