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
    public partial class Form1 : Form
    {
        private DataSet ds = new DataSet();
        private DataTable dt = new DataTable();

        private Settings form = new Settings();
        private Query queryForm = new Query();

        private NpgsqlConnection conn;

        private String query;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label7.ForeColor = Color.Red;
            label7.Text = "Не подключено";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Формированиестроку с данными для входа
                string connString = String.Format("Server={0};Port={1};" +
                    "User Id={2};Password={3};Database={4};", textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text, textBox5.Text);
                // Инициализация данных для подключения
                conn = new NpgsqlConnection(connString);
                conn.Open();
                label7.Text = "Соединение установлено";
                label7.ForeColor = Color.Green;

            }

            catch (Exception msg)
            {
                // Вывод ошибки
                MessageBox.Show(msg.ToString(), "Ошибка");
                throw;
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            form.ShowDialog();
            textBox1.Text = form.textBox1.Text;
            textBox2.Text = form.textBox2.Text;
            textBox3.Text = form.textBox3.Text;
            textBox4.Text = form.textBox4.Text;
            textBox5.Text = form.textBox5.Text;
        }

        private void отсоединитьсяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //Закрытие соединения
                if (conn != null)
                {
                    conn.Close();
                    conn = null;
                    label7.Text = "Соединение остановлено";
                    label7.ForeColor = Color.SteelBlue;
                }
                
            }
            catch (Exception msg)
            {
                // Вывод ошибки
                MessageBox.Show(msg.ToString());
                throw;
            }

        }

        private void выполнитьЗапросToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (conn != null)
            {
                queryForm.ShowDialog();
                query = queryForm.textBox1.Text;

                NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn);

                ds.Reset();
                // Заполнение данными из запроса
                da.Fill(ds);
                dt = ds.Tables[0];
                dataGridView1.DataSource = dt;
            }
            else
                MessageBox.Show("Для выполнения запроса необходимо подключиться к базе данных:\nФайл->Настройки подключения", "Ошибка");
        }

    }
}
