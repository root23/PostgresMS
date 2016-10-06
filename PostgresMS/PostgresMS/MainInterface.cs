using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Npgsql;

namespace PostgresMS
{
    public partial class MainInterface : Form
    {
        private Query que = new Query();
        private Settings settings = new Settings();

        private DataSet ds = new DataSet();
        private DataTable dt = new DataTable();

        public string[] conf = new string[5];

        public NpgsqlConnection conn = null;
        
        public MainInterface()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }
        
        private void UpdateData()
        {
            //Заполнение древа сервера
            InitializeTreeView();
            //Очистка таблицы с данными
            dataGridView1.SelectAll();
            dataGridView1.ClearSelection();
            dataGridView1.Columns.Clear();
        }

        private void MainInterface_Load(object sender, EventArgs e)
        {
            label3.Text = "Не подключено";
            label3.ForeColor = Color.Red;

            //Загрузка конфигурации
            FileStream file = new FileStream("settings.cfg", FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(file);
            for (int i = 0; i < 5; i++)
                conf[i] = reader.ReadLine();
            reader.Close();
            dataGridView1.ReadOnly = true;
            dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
            contextMenuStrip1.ItemClicked += new ToolStripItemClickedEventHandler(this.menuStrip_ItemClicked);

            treeView1.Nodes.Add("Сервер 127.0.0.1");
        }
        
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void обновитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (conn != null)
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
            
        }

        void ShowDataFromTable(string tableName, string bName)
        {
            if (conn.Database != bName)
                conn.ChangeDatabase(bName);
            
            string query = String.Format("SELECT * FROM {0}", tableName);

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn);
            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            dataGridView1.DataSource = dt;
        }

        void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                if (conn == null)
                {
                    string connString = String.Format("Server={0};Port={1};" +
                                      "User Id={2};Password={3};Database={4};", conf[0], conf[1], conf[2], conf[3], conf[4]);
                    conn = new NpgsqlConnection(connString);
                    conn.Open();
                    UpdateData();
                    label3.Text = String.Format("Подключено: {0}", conn.Host);
                    label3.ForeColor = Color.Green;
                }
            }

            if (e.Node.Level == 1)
            {
                conn.ChangeDatabase(e.Node.Text);
                UpdateData();
            }

            if (e.Node.Level == 2)
            {
                ShowDataFromTable(e.Node.Text, e.Node.Parent.Text);
            }
        }

        private void выполнитьЗапросToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (conn != null)
            {
                que.ShowDialog();
                tabControl1.SelectedIndex = 2;
                string query = que.textBox1.Text;
            }
            
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 a = new AboutBox1();
            a.ShowDialog();
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settings.ShowDialog();
        }


    }
}
