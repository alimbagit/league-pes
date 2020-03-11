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

        public int m_CountTeams;
        public Form1.Team[] m_TeamsPremier;
        public Form1.Team[] m_TeamsTwo;


        public Form2()
        {
            InitializeComponent();
        }

        public Form2(Form1 f)
        {
            InitializeComponent();
            m_MainForm = f;
            m_CountTeams=m_MainForm.m_Config.count_teams;
            CorrectRadioButtonChecked();
            m_TeamsPremier = new Form1.Team[m_CountTeams];
            m_TeamsTwo = new Form1.Team[m_CountTeams];
            FillTableNewSeason();
        }

        private void CorrectRadioButtonChecked()
        {
            if (m_CountTeams == 9) radioButton1.Checked = true;
            else if (m_CountTeams == 10) radioButton2.Checked = true;
        }

        private void FillTableNewSeason()
        {
            for(int i = 0; i < m_MainForm.m_TeamsPremier.Length; i++)
            {
                tableLayoutPanel1.GetControlFromPosition(1, i + 1).Text = m_MainForm.m_TeamsPremier[i].c_owner;
                tableLayoutPanel1.GetControlFromPosition(2, i + 1).Text = m_MainForm.m_TeamsPremier[i].c_name;
            }

            for (int i = 0; i < m_MainForm.m_TeamsTwo.Length; i++)
            {
                tableLayoutPanel2.GetControlFromPosition(1, i + 1).Text = m_MainForm.m_TeamsTwo[i].c_owner;
                tableLayoutPanel2.GetControlFromPosition(2, i + 1).Text = m_MainForm.m_TeamsTwo[i].c_name;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (VerificationTableLayouts())
            {
                m_MainForm.CalculateChampionScore();
                m_TeamsPremier = CheckNewTeams(tableLayoutPanel1, true);
                m_TeamsTwo = CheckNewTeams(tableLayoutPanel2, false);
                m_MainForm.CreateNewSeason(this);
                this.Close();
            }
        }

        private Form1.Team[] CheckNewTeams(TableLayoutPanel tableLayout,bool bonus)
        {
            List<Form1.Team> teams_list = new List<Form1.Team>();

            //Обнуление бонусных очков за прошлый сезон
            Form1.Team t;
            for (int i = 0; i < m_MainForm.m_TeamsAll.Count; i++)
            {
                t = m_MainForm.m_TeamsAll[i];
                t.c_bonus_score = 0;
                m_MainForm.m_TeamsAll[i] = t;
            }


            for (int i = 0; i < tableLayout.RowCount - 1; i++)
            {
                if (tableLayout.GetControlFromPosition(2, i + 1).Text == "")
                {
                    continue;
                }
                bool is_contains = false;
                for (int j = 0; j < m_MainForm.m_TeamsAll.Count; j++)
                {
                    if (tableLayout.GetControlFromPosition(1, i + 1).Text == m_MainForm.m_TeamsAll[j].c_owner && tableLayout.GetControlFromPosition(2, i + 1).Text == m_MainForm.m_TeamsAll[j].c_name)
                    {
                        teams_list.Add(m_MainForm.m_TeamsAll[j]);
                        is_contains = true;
                        break;
                    }
                }

                if (!is_contains)
                {
                    teams_list.Add(CreateNewTeam(tableLayout.GetControlFromPosition(1, i + 1).Text, tableLayout.GetControlFromPosition(2, i + 1).Text, m_MainForm.m_TeamsAll.Count + 1));
                    m_MainForm.m_TeamsAll.Add(teams_list[teams_list.Count - 1]);
                }

                if (i < 3)
                {
                    Form1.Team tt = teams_list[teams_list.Count - 1];
                    tt.c_bonus_score = 1;
                    teams_list[teams_list.Count - 1] = tt;
                }

                else if (i >= tableLayout.RowCount - 4)
                {
                    Form1.Team tt = teams_list[teams_list.Count - 1];
                    tt.c_bonus_score = -2;
                    teams_list[teams_list.Count - 1] = tt;
                }
                else
                {
                    Form1.Team tt = teams_list[teams_list.Count - 1];
                    tt.c_bonus_score = 0;
                    teams_list[teams_list.Count - 1] = tt;
                }
            }
            Form1.Team[] new_teams = teams_list.ToArray();
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

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            m_CountTeams = 9;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            m_CountTeams = 10;
        }

        private bool VerificationTableLayouts()
        {
            if (VerificationOfFillingConditions(tableLayoutPanel1))
            {
                if(VerificationOfFillingConditions(tableLayoutPanel2))
                {
                    return true;
                }
                else MessageBox.Show("Выполните условия заполнения таблицы во Второй лиге!");
            }
            else MessageBox.Show("Выполните условия заполнения таблицы в Высшей лиге!");
            return false;
        }
        private bool VerificationOfFillingConditions(TableLayoutPanel tableLayout)
        {
            bool verification_true=false;
            int free_textbox = 0;
            for(int row = 1; row < tableLayout.RowCount; row++)
            {
                if (tableLayout.GetControlFromPosition(2, row).Text == "")
                {
                    free_textbox++;
                }
            }
            if (free_textbox == 0 && m_CountTeams == 10)
            {
                verification_true = true;
            }
            else if (free_textbox == 1 && m_CountTeams == 9)
            {
                verification_true = true;
            }

            return verification_true;
        }
    }
}
