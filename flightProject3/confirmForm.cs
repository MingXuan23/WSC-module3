using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace flightProject3
{
    public partial class confirmForm : Form
    {
        module3Entities ent=new module3Entities();
        public confirmForm()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = -1;
        }

        public confirmForm(Schedule outSchedule,Schedule returnSchedule)
        {
            InitializeComponent();
            comboBox1.SelectedIndex = -1;
            if(returnSchedule==null)
            {
                groupBox2.Visible = false;
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text==""||textBox2.Text=="" || textBox3.Text == "" || maskedTextBox1.Text == "")
            {
                MessageBox.Show("Please enter all the fields!");
                return;
            }
            if(dateTimePicker1.Value>DateTime.Now)
            {
                MessageBox.Show("Invalid birthdate");
                return;
            }
            if(comboBox1.SelectedIndex==-1)
            {
                MessageBox.Show("Please select your country");
                return;
            }

        }
    }
}
