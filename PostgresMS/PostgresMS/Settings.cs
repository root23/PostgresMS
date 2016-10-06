using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IniFiles;

namespace PostgresMS
{
    public partial class Settings : Form
    {

        IniFile file = new IniFile("settings.ini");
        
        public Settings()
        {
            InitializeComponent();
            autoRead();
        }

        private void autoRead()
        {
            if (file.KeyExists("host", "server1"))
                textBox1.Text = file.ReadINI("server1", "host");
            else
                textBox1.Text = "-";
            if (file.KeyExists("port", "server1"))
                textBox2.Text = file.ReadINI("server1", "port");
            else
                textBox2.Text = "-";
            if (file.KeyExists("username", "server1"))
                textBox3.Text = file.ReadINI("server1", "username");
            else
                textBox3.Text = "-";
            if (file.KeyExists("password", "server1"))
                textBox4.Text = file.ReadINI("server1", "password");
            else
                textBox4.Text = "-";
            if (file.KeyExists("database", "server1"))
                textBox5.Text = file.ReadINI("server1", "database");
            else
                textBox5.Text = "-";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            file.Write("server1", "host", textBox1.Text);
            file.Write("server1", "port", textBox2.Text);
            file.Write("server1", "username", textBox3.Text);
            file.Write("server1", "password", textBox4.Text);
            file.Write("server1", "database", textBox5.Text);
            MessageBox.Show("Данные сохранены", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Asterisk); // Говорим пользователю, что сохранили текст.
        }

        private void button3_Click(object sender, EventArgs e)
        {
            autoRead();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            file.Write("server1", "host", textBox1.Text);
            file.Write("server1", "port", textBox2.Text);
            file.Write("server1", "username", textBox3.Text);
            file.Write("server1", "password", textBox4.Text);
            file.Write("server1", "database", textBox5.Text);
            MessageBox.Show("Настройки сохранены и применены", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Asterisk); // Говорим пользователю, что сохранили текст.
        }
    }
}
