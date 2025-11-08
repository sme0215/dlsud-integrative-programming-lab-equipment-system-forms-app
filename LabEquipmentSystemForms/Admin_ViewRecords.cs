using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataHelper;

namespace LabEquipmentSystemForms
{
    public partial class FormAdminViewRecords : Form
    {
        string formCmd;
        public FormAdminViewRecords(string cmd)
        {
            InitializeComponent();
            formCmd = cmd;

            if (cmd.Equals("students"))
            {
                this.Text = "View Student Records";
                LoadStudentRecords();
            }
        }

        private void LoadStudentRecords()
        {
            dataGridView.DataSource = null;

            DataTable studentRecords = DataAccess.ViewStudents();
            dataGridView.DataSource = studentRecords;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (formCmd.Equals("students"))
            {
                LoadStudentRecords();
            }
        }
    }
}
