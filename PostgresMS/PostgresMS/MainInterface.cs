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
    public partial class MainInterface : Form
    {
        private SelectDB dbSelector = new SelectDB();
        private AddTable tableForm = new AddTable();

        private DataSet ds = new DataSet();
        private DataTable dt = new DataTable();

        public NpgsqlConnection conn;

        private string bName;

        public MainInterface()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            DataGridViewRow row = dataGridView1.Rows[index];
            string tName = row.Cells["table_name"].Value.ToString();

            string delQuery = String.Format("DROP TABLE {0}", tName);
            NpgsqlCommand command = new NpgsqlCommand(delQuery, conn);
            command.ExecuteScalar();


            //NpgsqlDataAdapter da = new NpgsqlDataAdapter(delQuery, conn);

            UpdateData();
        }
        
        private void UpdateData()
        {
            //Очистка таблицы с данными
            dataGridView1.SelectAll();
            dataGridView1.ClearSelection();
            dataGridView1.Columns.Clear();

            if (conn != null)
            {
                string query = "SELECT table_name FROM information_schema.tables  where table_schema='public' ORDER BY table_name;";
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn);
                
                ds.Reset();
                // Заполнение данными из запроса
                da.Fill(ds);
                dt = ds.Tables[0];


                DataGridViewButtonColumn col = new DataGridViewButtonColumn();
                col.HeaderText = "Удаление";
                col.Text = "Удалить";
                col.Name = "btn";
                col.UseColumnTextForButtonValue = true;

                dataGridView1.Columns.Add(col);
                
                dataGridView1.DataSource = dt;

            }
            else
                MessageBox.Show("Для выполнения запроса необходимо подключиться к базе данных:\nФайл->Настройки подключения", "Ошибка");
        }
        
        private void MainInterface_Load(object sender, EventArgs e)
        {
            bName = conn.Database;
            dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            InitializeTreeView();
        }
        

        private void сменитьБазуДанныхToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dbSelector.conn = conn;
            dbSelector.ShowDialog();

            if (bName != dbSelector.bName)
            {
                bName = dbSelector.bName;
                conn.ChangeDatabase(bName);
            }
            
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void обновитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateData();
        }

        private void добавитьТаблицуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tableForm.ShowDialog();
            tabControl1.SelectedIndex = 1;
        }

        private void InitializeTreeView()
        {
            treeView1.BeginUpdate();
            treeView1.Nodes.Add(String.Format("Сервер {0}", conn.Host));
            string query = "SELECT datname FROM pg_database WHERE datistemplate = false";

            using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
            {
                string val;
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    val = reader[0].ToString();
                    treeView1.Nodes[0].Nodes.Add(val);
                }
                reader.Close();
            }

            using (DataTable schema = conn.GetSchema("Tables"))
            {
                foreach (DataRow row in schema.Rows)
                {
                    treeView1.Nodes[0].Nodes[1].Nodes.Add(row["table_name"].ToString());
                }

            }
            
            treeView1.EndUpdate();

            /*treeView1.BeginUpdate();
            treeView1.Nodes.Add("Parent");
            treeView1.Nodes[0].Nodes.Add("Child 1");
            treeView1.Nodes[0].Nodes.Add("Child 2");
            treeView1.Nodes[0].Nodes[1].Nodes.Add("Grandchild");
            treeView1.Nodes[0].Nodes[1].Nodes[0].Nodes.Add("Great Grandchild");
            treeView1.EndUpdate();*/
        }
        
    }
}
