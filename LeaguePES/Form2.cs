using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LeaguePES
{
    public partial class Form2 : Form
    {
        private Form1 m_MainForm;

        public Form1.Team[] m_TeamsPremier = new Form1.Team[10];
        public Form1.Team[] m_TeamsTwo = new Form1.Team[10];
        public Form2()
        {
            InitializeComponent();
        }


        public Form2(Form1 f)
        {
            InitializeComponent();
            m_MainForm = f;
            FillTableNewSeason();
        }

        private void FillTableNewSeason()
        {
            for(int i = 0; i < m_MainForm.m_TeamsPremier.Length; i++)
            {
                tableLayoutPanel1.GetControlFromPosition(1, i + 1).Text = m_MainForm.m_TeamsPremier[i].c_owner;
                tableLayoutPanel1.GetControlFromPosition(2, i + 1).Text = m_MainForm.m_TeamsPremier[i].c_name;

                //if(i==m_MainForm.m_TeamsPremier.Length-3)
                //{
                //    tableLayoutPanel1.GetControlFromPosition(1, i + 1).Text = m_MainForm.m_TeamsTwo[0].c_owner;
                //    tableLayoutPanel1.GetControlFromPosition(2, i + 1).Text = m_MainForm.m_TeamsTwo[0].c_name;
                //}
                //else if (i == m_MainForm.m_TeamsPremier.Length - 2)
                //{
                //    tableLayoutPanel1.GetControlFromPosition(1, i + 1).Text = m_MainForm.m_TeamsTwo[1].c_owner;
                //    tableLayoutPanel1.GetControlFromPosition(2, i + 1).Text = m_MainForm.m_TeamsTwo[1].c_name;
                //}
                //else if (i == m_MainForm.m_TeamsPremier.Length - 1)
                //{
                //    tableLayoutPanel1.GetControlFromPosition(1, i + 1).Text = m_MainForm.m_TeamsTwo[2].c_owner;
                //    tableLayoutPanel1.GetControlFromPosition(2, i + 1).Text = m_MainForm.m_TeamsTwo[2].c_name;
                //}
            }

            for (int i = 0; i < m_MainForm.m_TeamsTwo.Length; i++)
            {
                tableLayoutPanel2.GetControlFromPosition(1, i + 1).Text = m_MainForm.m_TeamsTwo[i].c_owner;
                tableLayoutPanel2.GetControlFromPosition(2, i + 1).Text = m_MainForm.m_TeamsTwo[i].c_name;

                //if (i == 0)
                //{
                //    tableLayoutPanel2.GetControlFromPosition(1, i + 1).Text = m_MainForm.m_TeamsPremier[m_MainForm.m_TeamsPremier.Length-3].c_owner;
                //    tableLayoutPanel2.GetControlFromPosition(2, i + 1).Text = m_MainForm.m_TeamsPremier[m_MainForm.m_TeamsPremier.Length - 3].c_name;
                //}
                //else if (i == 1)
                //{
                //    tableLayoutPanel2.GetControlFromPosition(1, i + 1).Text = m_MainForm.m_TeamsPremier[m_MainForm.m_TeamsPremier.Length - 2].c_owner;
                //    tableLayoutPanel2.GetControlFromPosition(2, i + 1).Text = m_MainForm.m_TeamsPremier[m_MainForm.m_TeamsPremier.Length - 2].c_name;
                //}
                //else if (i == 2)
                //{
                //    tableLayoutPanel2.GetControlFromPosition(1, i + 1).Text = m_MainForm.m_TeamsPremier[m_MainForm.m_TeamsPremier.Length - 1].c_owner;
                //    tableLayoutPanel2.GetControlFromPosition(2, i + 1).Text = m_MainForm.m_TeamsPremier[m_MainForm.m_TeamsPremier.Length - 1].c_name;
                //}
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            m_MainForm.CalculateChampionScore();
            m_TeamsPremier=CheckNewTeams(tableLayoutPanel1,true);
            m_TeamsTwo=CheckNewTeams(tableLayoutPanel2,false);
            m_MainForm.CreateNewSeason(this);
            this.Close();
        }

        private Form1.Team[] CheckNewTeams(TableLayoutPanel tableLayout,bool bonus)
        {
            
            Form1.Team[] new_teams=new Form1.Team[tableLayout.RowCount - 1];

            Form1.Team t;

            for (int i = 0; i < m_MainForm.m_TeamsAll.Count; i++)
            {
                t = m_MainForm.m_TeamsAll[i];
                t.c_bonus_score = 0;
                m_MainForm.m_TeamsAll[i] = t;
            }


            for (int i = 0; i < tableLayout.RowCount - 1; i++)
            {
                bool is_contains = false;
                for (int j = 0; j < m_MainForm.m_TeamsAll.Count; j++)
                {
                    if (tableLayout.GetControlFromPosition(1, i + 1).Text == m_MainForm.m_TeamsAll[j].c_owner && tableLayout.GetControlFromPosition(2, i + 1).Text == m_MainForm.m_TeamsAll[j].c_name)
                    {
                        new_teams[i] = m_MainForm.m_TeamsAll[j];
                        is_contains = true;
                        break;
                    }
                }

                if (!is_contains)
                {
                    new_teams[i] = CreateNewTeam(tableLayout.GetControlFromPosition(1, i + 1).Text, tableLayout.GetControlFromPosition(2, i + 1).Text, m_MainForm.m_TeamsAll.Count + 1);
                    m_MainForm.m_TeamsAll.Add(new_teams[i]);
                }

                if (i == 0)
                {
                    new_teams[i].c_bonus_score = 1;
                }
                else if (i == 1)
                {
                    new_teams[i].c_bonus_score = 1;
                }
                else if (i == 2)
                {
                    new_teams[i].c_bonus_score = 1;
                }
                else if (i <= new_teams.Length - 1 && i >= new_teams.Length - 3)
                {
                    new_teams[i].c_bonus_score = -2;
                }
                else new_teams[i].c_bonus_score = 0;

            }

            return new_teams;
        }

        private Form1.Team CreateNewTeam(string owner, string name, int id)
        {
            Form1.Team team = new Form1.Team();
            team.c_owner = owner;
            team.c_name = name;
            team.c_id = id;
            team.c_number = 0;
            team.c_games = 0;
            team.c_wins = 0;
            team.c_score = 0;
            team.c_loses = 0;
            team.c_goals_scored = 0;
            team.c_goals_conceded = 0;
            team.c_draws = 0;
            team.c_champion_score = 0;
            team.c_3rd_place = 0;
            team.c_2nd_place = 0;
            team.c_1st_place = 0;
            team.c_bonus_score = 0;
            return team;
        }
    }
}
