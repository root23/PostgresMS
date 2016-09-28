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
        private Settings newTable = new Settings();

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

            if (tName != "")
            {
                string delQuery = String.Format("DROP TABLE {0}", tName);
                NpgsqlCommand command = new NpgsqlCommand(delQuery, conn);
                command.ExecuteNonQuery();
            }
            
            UpdateData();
        }
        
        private void UpdateData()
        {
            //Заполнение древа сервера
            InitializeTreeView();
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
            treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
            InitializeTreeView();
            contextMenuStrip1.ItemClicked += new ToolStripItemClickedEventHandler(this.menuStrip_ItemClicked);
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
            tabControl1.SelectedIndex = 1;
        }

        private void InitializeTreeView()
        {
            treeView1.Nodes.Clear();
            treeView1.BeginUpdate();
            treeView1.Nodes.Add(String.Format("Сервер {0}", conn.Host));
            string query = "SELECT datname FROM pg_database WHERE datistemplate = false";

            using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
            {
                string val;
                NpgsqlDataReader reader = command.ExecuteReader();
                DataTable dt = reader.GetSchemaTable();
                while (reader.Read())
                {                    
                    val = reader[0].ToString();
                    treeView1.Nodes[0].Nodes.Add(val);                    
                }
                reader.Close();
            }

            //getting tables
            for (int i = 0; i < treeView1.Nodes[0].Nodes.Count; i++)
            {
                conn.ChangeDatabase(treeView1.Nodes[0].Nodes[i].Text);
                string q = "SELECT table_name FROM information_schema.tables  where table_schema='public' ORDER BY table_name;";
                NpgsqlCommand cmd = new NpgsqlCommand(q, conn);
                NpgsqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    treeView1.Nodes[0].Nodes[i].Nodes.Add(rd[0].ToString());
                }           
                rd.Close();                
               
            }
            
            treeView1.EndUpdate();

        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView1.SelectedNode = e.Node;
            if (e.Button == MouseButtons.Right && e.Node.Level == 1)
            {
                contextMenuStrip1.Show(treeView1, e.Location);  
            }
            
        }

        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string bName = treeView1.SelectedNode.Text;
            if (e.ClickedItem.Text == "Добавить таблицу")
            {
                newTable.ShowDialog();                
                string tName = newTable.textBox1.Text;
                conn.ChangeDatabase(tName);
                string query = String.Format("CREATE TABLE {0}(id int);", tName);

                NpgsqlCommand com = new NpgsqlCommand(query, conn);
                com.ExecuteNonQuery();
                UpdateData();
            }

            
        }

        void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Level == 1)
            {
                conn.ChangeDatabase(e.Node.Text);
                UpdateData();
            }
        }


    }
}
