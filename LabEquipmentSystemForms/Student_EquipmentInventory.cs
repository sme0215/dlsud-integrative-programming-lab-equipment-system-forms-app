using DataHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabEquipmentSystemForms
{
    public partial class FormStudentEquipmentInventory : Form
    {
        private string studentID;
        private BindingSource bs = new BindingSource();

        public FormStudentEquipmentInventory(string studentID)
        {
            InitializeComponent();
            this.studentID = studentID;
            LoadStudentInventory();
        }

        private void LoadStudentInventory()
        {
            try
            {
                // Assuming you have a method to get student's inventory
                DataTable dt = DataAccess.GetStudentInventory(studentID);
                bs.DataSource = dt;
                dataGridView.DataSource = bs;

                // Set up data binding for textboxes if needed
                BindDataSourceToControls();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading inventory: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindDataSourceToControls()
        {
            txtEquipmentID.DataBindings.Clear();

            txtEquipmentID.DataBindings.Add("Text", bs, "EquipmentID");
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null)
            {
                MessageBox.Show("Please select an item to return.", "Selection Required",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Get data from selected row
                string requestID = dataGridView.CurrentRow.Cells["RequestID"].Value?.ToString();
                string currentStudentID = dataGridView.CurrentRow.Cells["StudentID"].Value?.ToString();
                string status = dataGridView.CurrentRow.Cells["Status"].Value?.ToString();

                int returnAmount = Convert.ToInt32(txtReturnAmount.Text.ToString());
                int remainingQty = Convert.ToInt32(dataGridView.CurrentRow.Cells["RemainingQuantity"].Value);

                // Validate inputs
                if (string.IsNullOrEmpty(requestID))
                {
                    MessageBox.Show("Invalid request ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (status == "CLOSED")
                {
                    MessageBox.Show("This item has already been fully returned and closed.",
                                  "Already Returned", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (returnAmount <= 0)
                {
                    MessageBox.Show("Return amount must be greater than 0.",
                                  "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (returnAmount > remainingQty)
                {
                    MessageBox.Show($"Return amount cannot exceed remaining quantity ({remainingQty}).",
                                  "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Confirm return
                DialogResult confirm = MessageBox.Show(
                    $"Are you sure you want to return {returnAmount} item(s)?",
                    "Confirm Return",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;

                // Process return
                bool success = DataAccess.ReturnEquipment(requestID, currentStudentID, returnAmount);

                if (success)
                {
                    MessageBox.Show("Equipment returned successfully!", "Success",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reset return amount
                    txtReturnAmount.Text = "0";

                    // Refresh the inventory
                    LoadStudentInventory();
                }
                else
                {
                    MessageBox.Show($"Failed to return equipment.", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadStudentInventory();
            MessageBox.Show("Inventory refreshed.", "Refresh",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow != null)
            {
                bs.Position = dataGridView.CurrentRow.Index;

                // Update the maximum return amount based on remaining quantity
                if (dataGridView.CurrentRow.Cells["RemainingQuantity"].Value != null)
                {
                    int remainingQty = Convert.ToInt32(dataGridView.CurrentRow.Cells["RemainingQuantity"].Value);
                    txtReturnAmount.Text = remainingQty.ToString();
                }
            }
        }
    }
}
