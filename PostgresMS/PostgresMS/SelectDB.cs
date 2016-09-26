using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace PostgresMS
{
    public partial class SelectDB : Form
    {
        public NpgsqlConnection conn;

        public string bName;

        public SelectDB()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bName = comboBox1.SelectedItem.ToString();
            Close();
        }

        private void SelectDB_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            string query = "SELECT datname FROM pg_database WHERE datistemplate = false";

            using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
            {
                string val;
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    val = reader[0].ToString();
                    comboBox1.Items.Add(val);
                }
                reader.Close();
            }
        }

    }
}
