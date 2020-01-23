using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace LeaguePES
{
    public partial class Form1 : Form
    {
        private struct Team
        {
            public int c_number;
            public string c_owner;
            public string c_name;
            public int c_games;
            public int c_wins;
            public int c_draws;
            public int c_loses;
            public int c_goals_scored;
            public int c_goals_conceded;
            public int c_score;
        }


    private Team[] m_TeamsPremier= new Team[10];
    private Team[] m_TeamsTwo= new Team[10];
    public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader("first_table.txt");
            string[] table_line;
            int i = 0;
            while (!sr.EndOfStream)
            {
                table_line = sr.ReadLine().Split(' ');
                if(i<10) CreateOneTeam(m_TeamsPremier, table_line, i);
                else CreateOneTeam(m_TeamsTwo, table_line, i-10);
                i++;
            }
            FillTable();
        }

        private void CreateOneTeam(Team[] teams, string[] table_line, int i)
        {
            teams[i].c_number = int.Parse(table_line[0]);
            teams[i].c_owner = table_line[1];
            teams[i].c_name = table_line[2];
            teams[i].c_games = int.Parse(table_line[3]);
            teams[i].c_wins = int.Parse(table_line[4]);
            teams[i].c_draws = int.Parse(table_line[5]);
            teams[i].c_loses = int.Parse(table_line[6]);
            teams[i].c_goals_scored = int.Parse(table_line[7]);
            teams[i].c_goals_conceded = int.Parse(table_line[8]);
            teams[i].c_score = int.Parse(table_line[9]);
        }

        private void FillTable()
        {
            for (int row=0; row < 10; row++)
            {
                tableLayoutPanel1.GetControlFromPosition(0, row+1).Text = m_TeamsPremier[row].c_number.ToString();
                tableLayoutPanel1.GetControlFromPosition(1, row+1).Text = m_TeamsPremier[row].c_owner;
                tableLayoutPanel1.GetControlFromPosition(2, row+1).Text = m_TeamsPremier[row].c_name;
                tableLayoutPanel1.GetControlFromPosition(3, row+1).Text = m_TeamsPremier[row].c_games.ToString();
                tableLayoutPanel1.GetControlFromPosition(4, row+1).Text = m_TeamsPremier[row].c_wins.ToString();
                tableLayoutPanel1.GetControlFromPosition(5, row+1).Text = m_TeamsPremier[row].c_draws.ToString();
                tableLayoutPanel1.GetControlFromPosition(6, row+1).Text = m_TeamsPremier[row].c_loses.ToString();
                tableLayoutPanel1.GetControlFromPosition(7, row+1).Text = m_TeamsPremier[row].c_goals_scored.ToString() + "-" + m_TeamsPremier[row].c_goals_conceded.ToString();
                tableLayoutPanel1.GetControlFromPosition(8, row+1).Text = m_TeamsPremier[row].c_score.ToString();

                tableLayoutPanel2.GetControlFromPosition(0, row+1).Text = m_TeamsTwo[row].c_number.ToString();
                tableLayoutPanel2.GetControlFromPosition(1, row+1).Text = m_TeamsTwo[row].c_owner;
                tableLayoutPanel2.GetControlFromPosition(2, row+1).Text = m_TeamsTwo[row ].c_name;
                tableLayoutPanel2.GetControlFromPosition(3, row+1).Text = m_TeamsTwo[row ].c_games.ToString();
                tableLayoutPanel2.GetControlFromPosition(4, row+1).Text = m_TeamsTwo[row ].c_wins.ToString();
                tableLayoutPanel2.GetControlFromPosition(5, row+1).Text = m_TeamsTwo[row ].c_draws.ToString();
                tableLayoutPanel2.GetControlFromPosition(6, row+1).Text = m_TeamsTwo[row ].c_loses.ToString();
                tableLayoutPanel2.GetControlFromPosition(7, row+1).Text = m_TeamsTwo[row ].c_goals_scored.ToString() + "-" + m_TeamsTwo[row].c_goals_conceded.ToString();
                tableLayoutPanel2.GetControlFromPosition(8, row+1).Text = m_TeamsTwo[row ].c_score.ToString();
            }
        }


    }
}
