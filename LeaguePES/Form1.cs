﻿using System;
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
        public Form1()
        {
            InitializeComponent();
        }
        public struct Team
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
            public int c_id;
            public int c_bonus_score;

            public int c_1st_place;
            public int c_2nd_place;
            public int c_3rd_place;
            public int c_champion_score;
        }

        private class Match
        {
            public Match()
            {
                team1 = 0;
                team2 = 0;
                score_team1 = 0;
                score_team2 = 0;
                is_played = 0;
                textbox = "";
            }

            public int team1;
            public int team2;
            public int score_team1;
            public int score_team2;
            public int is_played;
            public string textbox;
        }

        private int[,] m_TemplateCalendar = {{4, 6},{8,9 },{7,3 },{5,0 },{2,1 },{6,8 },{3,0 }, {9,1 }, {7,5 }, {4,2 }, {9,0 } ,
            {8,3 }, {6,1 }, {4,5 }, { 7,2}, {8,0 } , {9,5 }, {4,1 }, {3,2 }, {6,7 }, {8,5 } , {6,2 }, {7,0 }, {4,9 }, {3,1 }, {9,7 } , {5,2 }, {8,1 },
            {4,3 }, {6,0 }, {4,8 },{3,5},{0,2},{6,9},{7,1},{6,3},{4,0},{9,2},{5,1},{8,7},{6,5},{9,3},{4,7},{8,2},{0,1}};

        private class Round
        {
            public Round()
            {
                end_round = false;
                matches = new List<Match>();
            }
            public bool end_round;
            public List< Match> matches; 
        }

        public struct Config
        {
            public int current_season;
            public bool end_current_season;

        }
        private static int m_CountRounds = 9;
        private static int m_SizeOneRound = 5;

        public List<Team> m_TeamsAll = new List<Team>();
        public Team[] m_TeamsPremier= new Team[10];
        public Team[] m_TeamsTwo= new Team[10];

        private List<Round> m_RoundsPremierLeague = new List<Round>();
        private List<Round> m_RoundsTwoLeague = new List<Round>();

        private Config m_Config;


        private void Form1_Load(object sender, EventArgs e)
        {
            LoadConfig();
            FileTeamsLoad(); //Чтение текущих команд из файла
            m_TeamsAll= FileAllTeamsLoad(); //Чтение всех команд из файла

            m_RoundsPremierLeague = FileRoundsRead(m_Config.current_season,1); //Чтение раундов матчей из файла
            m_RoundsTwoLeague = FileRoundsRead(m_Config.current_season, 2);
            //m_RoundsTwoLeague = CreateTimeTable(m_TeamsTwo, m_CountRounds, m_SizeOneRound); 
            m_TeamsPremier =MatchesCalculation(m_RoundsPremierLeague, m_TeamsPremier);
            m_TeamsTwo = MatchesCalculation(m_RoundsTwoLeague, m_TeamsTwo);
            FillTable(tableLayoutPanel1,m_TeamsPremier);
            FillTable(tableLayoutPanel2, m_TeamsTwo);
            FillCalendar(tableLayoutPanel3, m_RoundsPremierLeague, m_TeamsPremier);
            FillCalendar(tableLayoutPanel4, m_RoundsTwoLeague, m_TeamsTwo);

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveConfig();
            FileCurrentTeamsSave();
            SaveCurrentSeasonToFile();

        }



        private void buttonNewSeason_Click(object sender, EventArgs e)
        {
            //if (EndSeasonCheck())
            //{
                Form2 newForm = new Form2(this);
                newForm.Show();
            //}
            //else
            //{
            //    MessageBox.Show("Завершите текущий сезон!");
            //}
        }

        public void CreateNewSeason(Form2 f)
        {
            SaveCurrentSeasonToFile();
            m_Config.current_season++;
            m_TeamsPremier = f.m_TeamsPremier;
            m_TeamsTwo = f.m_TeamsTwo;
            m_RoundsPremierLeague = CreateTimeTable(m_TeamsPremier, m_CountRounds, m_SizeOneRound);
            m_RoundsTwoLeague = CreateTimeTable(m_TeamsTwo, m_CountRounds, m_SizeOneRound);
            m_TeamsPremier = MatchesCalculation(m_RoundsPremierLeague, m_TeamsPremier);
            m_TeamsTwo = MatchesCalculation(m_RoundsTwoLeague, m_TeamsTwo);

            FileCurrentTeamsSave();
            FillTable(tableLayoutPanel1, m_TeamsPremier);
            FillTable(tableLayoutPanel2, m_TeamsTwo);
            FillCalendar(tableLayoutPanel3, m_RoundsPremierLeague, m_TeamsPremier);
            FillCalendar(tableLayoutPanel4, m_RoundsTwoLeague, m_TeamsTwo);
            UpdateSeasonLables();
        }

        private void LoadConfig()
        {
            StreamReader sr = new StreamReader("config.txt");
            m_Config.current_season = int.Parse(sr.ReadLine());
            m_Config.end_current_season = int.Parse(sr.ReadLine())==1;
            sr.Close();
            UpdateSeasonLables();
        }

        private void SaveConfig()
        {
            StreamWriter sw = new StreamWriter("config.txt");
            sw.WriteLine( m_Config.current_season.ToString());
            sw.WriteLine(m_Config.end_current_season ? "1" :"0");
            sw.Close();
        }

        private void UpdateSeasonLables()
        {
            labelSeasonNumber1.Text = "Сезон " + m_Config.current_season.ToString();
            labelSeasonNumber2.Text = "Сезон " + m_Config.current_season.ToString();
        }

        private void FileTeamsLoad()
        {
            StreamReader sr = new StreamReader("current_table.txt");
            string[] table_line;
            int i = 0;
            while (!sr.EndOfStream)
            {
                table_line = sr.ReadLine().Split(' ');
                if (i < 10) m_TeamsPremier[i]= CreateOneTeam( table_line);
                else m_TeamsTwo[i-10]=CreateOneTeam(table_line);
                i++;
            }
            sr.Close();
        }

        private List<Team> FileAllTeamsLoad()
        {
            List<Team> teams = new List<Team>();
            StreamReader sr = new StreamReader("teams_all.txt");
            string[] line;
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine().Split(' ');
                teams.Add(CreateOneTeam(line));

            }
            sr.Close();
            return teams;
        }

        
        private Team CreateOneTeam(string[] table_line)
        {
            Team team = new Team();
            team.c_id = int.Parse(table_line[0]);
            team.c_owner = table_line[1];
            team.c_name = table_line[2];
            team.c_bonus_score = int.Parse(table_line[3]);
            team.c_1st_place = int.Parse(table_line[4]);
            team.c_2nd_place = int.Parse(table_line[5]);
            team.c_3rd_place = int.Parse(table_line[6]);
            team.c_champion_score = int.Parse(table_line[7]);
            return team;
        }

        private void FileCurrentTeamsSave()
        {
            StreamWriter swall = new StreamWriter("teams_all.txt");
            string line;
            foreach (Team team in m_TeamsAll)
            {
                line = team.c_id.ToString() + " " + team.c_owner + " ";
                line += team.c_name + " "+team.c_bonus_score.ToString()+" "+team.c_1st_place.ToString()+ " ";
                line += team.c_2nd_place.ToString() + " " + team.c_3rd_place.ToString() + " " + team.c_champion_score.ToString();
                swall.WriteLine(line);
            }
            swall.Close();

            StreamWriter sw = new StreamWriter("current_table.txt");
            for (int i = 0; i < m_TeamsPremier.Length+m_TeamsTwo.Length; i++)
            {
                if (i < m_TeamsPremier.Length)
                {
                    line = m_TeamsPremier[i].c_id.ToString() + " " + m_TeamsPremier[i].c_owner + " ";
                    line += m_TeamsPremier[i].c_name + " " + m_TeamsPremier[i].c_bonus_score.ToString() + " "+ m_TeamsPremier[i].c_1st_place.ToString() + " ";
                    line += m_TeamsPremier[i].c_2nd_place.ToString() + " " + m_TeamsPremier[i].c_3rd_place.ToString() + " " + m_TeamsPremier[i].c_champion_score.ToString();
                    sw.WriteLine(line);
                }
                else
                {
                    line = m_TeamsTwo[i- m_TeamsPremier.Length].c_id.ToString() + " " + m_TeamsTwo[i - m_TeamsPremier.Length].c_owner + " ";
                    line += m_TeamsTwo[i - m_TeamsPremier.Length].c_name + " "+m_TeamsTwo[i- m_TeamsPremier.Length].c_bonus_score.ToString()+" " + m_TeamsTwo[i - m_TeamsPremier.Length].c_1st_place.ToString() + " ";
                    line += m_TeamsTwo[i - m_TeamsPremier.Length].c_2nd_place.ToString() + " " + m_TeamsTwo[i-m_TeamsPremier.Length].c_3rd_place.ToString() + " ";
                    line += m_TeamsTwo[i - m_TeamsPremier.Length].c_champion_score.ToString();
                    sw.WriteLine(line);
                }
            }
            sw.Close();
        }

        private List<Round> FileRoundsRead(int season_number, int league_number)
        {
            List<Round> result = new List<Round>();
            StreamReader sr;
            if (league_number == 1) sr = new StreamReader("season" + season_number + "premier.txt");
            else sr= new StreamReader("season" + season_number + "two.txt");
            string[] line;
            Round one_round = new Round();
            while (!sr.EndOfStream)
            {
                Match one_match = new Match();
                if (one_round.matches.Count < m_SizeOneRound)
                {
                    line = sr.ReadLine().Split(' ');
                    one_match.team1 = int.Parse(line[0]);
                    one_match.score_team1= int.Parse(line[1]);
                    one_match.score_team2 = int.Parse(line[2]);
                    one_match.team2 = int.Parse(line[3]);
                    one_match.is_played = int.Parse(line[4]);
                    one_round.matches.Add(one_match);
                }
                else
                {
                    result.Add(one_round);
                    one_round = new Round();
                }
            }
            sr.Close();
            result.Add(one_round);
            return result;
        }

        private Team[] MatchesCalculation(List<Round> rounds,Team[] teams)
        {
            for(int i = 0; i < teams.Length; i++)
            {
                teams[i].c_games = 0;
                teams[i].c_wins = 0;
                teams[i].c_draws = 0;
                teams[i].c_loses = 0;
                teams[i].c_goals_scored = 0;
                teams[i].c_goals_conceded = 0;
                teams[i].c_score = 0;
                teams[i].c_score += teams[i].c_bonus_score;
            }
            Team[] result;
            foreach(Round round in rounds)
            {
                foreach(Match match in round.matches)
                {
                    if (match.is_played == 1)
                    {
                        teams[TeamSearchById(teams, match.team2)].c_games += 1;
                        teams[TeamSearchById(teams, match.team1)].c_games += 1;
                        teams[TeamSearchById(teams, match.team2)].c_goals_scored += match.score_team2;
                        teams[TeamSearchById(teams, match.team2)].c_goals_conceded += match.score_team1;
                        teams[TeamSearchById(teams, match.team1)].c_goals_scored += match.score_team1;
                        teams[TeamSearchById(teams, match.team1)].c_goals_conceded += match.score_team2;

                        if (match.score_team1 < match.score_team2)
                        {
                            teams[TeamSearchById(teams, match.team2)].c_wins += 1;
                            teams[TeamSearchById(teams, match.team2)].c_score += 3;
                            teams[TeamSearchById(teams, match.team1)].c_loses += 1;
                        }
                        else if (match.score_team1 == match.score_team2)
                        {
                            teams[TeamSearchById(teams, match.team2)].c_draws += 1;
                            teams[TeamSearchById(teams, match.team1)].c_draws += 1;
                            teams[TeamSearchById(teams, match.team1)].c_score += 1;
                            teams[TeamSearchById(teams, match.team2)].c_score += 1;
                        }
                        else if (match.score_team1 > match.score_team2)
                        {
                            teams[TeamSearchById(teams, match.team1)].c_wins += 1;
                            teams[TeamSearchById(teams, match.team1)].c_score += 3;
                            teams[TeamSearchById(teams, match.team2)].c_loses += 1;
                        }
                    }
                }
            }
            result = TeamsSort(teams);
            return result;
        }

        private int TeamSearchById(Team[] teams, int id)
        {
            for(int i = 0; i < teams.Length; i++)
            {
                if (teams[i].c_id == id) return i;
            }
            return -1;
        }

        private Team[] TeamsSort(Team[] teams)
        {
            Team t;
            for (int r = 0; r < m_CountRounds; r++) 
            {
                for (int i = 0; i < teams.Length - 1; i++)
                {
                    if (teams[i].c_score == teams[i + 1].c_score)
                    {
                        if (teams[i].c_goals_scored - teams[i].c_goals_conceded == teams[i + 1].c_goals_scored - teams[i + 1].c_goals_conceded)
                        {
                            if (teams[i].c_wins == teams[i + 1].c_wins)
                            {
                                if (teams[i].c_goals_scored < teams[i + 1].c_goals_scored)
                                {
                                    t = teams[i];
                                    teams[i] = teams[i + 1];
                                    teams[i + 1] = t;
                                    continue;
                                }
                            }
                            else if (teams[i].c_wins < teams[i + 1].c_wins)
                            {
                                t = teams[i];
                                teams[i] = teams[i + 1];
                                teams[i + 1] = t;
                                continue;
                            }
                        }
                        else if (teams[i].c_goals_scored - teams[i].c_goals_conceded < teams[i + 1].c_goals_scored - teams[i + 1].c_goals_conceded)
                        {
                            t = teams[i];
                            teams[i] = teams[i + 1];
                            teams[i + 1] = t;
                            continue;
                        }
                    }
                    else if (teams[i].c_score < teams[i + 1].c_score)
                    {
                        t = teams[i];
                        teams[i] = teams[i + 1];
                        teams[i + 1] = t;
                        continue;
                    }
                }
            }

            for (int i = 0; i < teams.Length; i++)
            {
                teams[i].c_number = i +1;
            }
            return teams;
        }

        private void FillTable(TableLayoutPanel tableLayout, Team[] teams)
        {
            for (int row=0; row < teams.Length; row++)
            {
                tableLayout.GetControlFromPosition(1, row+1).Text = teams[row].c_owner;
                tableLayout.GetControlFromPosition(2, row+1).Text = teams[row].c_name;
                tableLayout.GetControlFromPosition(3, row+1).Text = teams[row].c_games.ToString();
                tableLayout.GetControlFromPosition(4, row+1).Text = teams[row].c_wins.ToString();
                tableLayout.GetControlFromPosition(5, row+1).Text = teams[row].c_draws.ToString();
                tableLayout.GetControlFromPosition(6, row+1).Text = teams[row].c_loses.ToString();
                tableLayout.GetControlFromPosition(7, row+1).Text = teams[row].c_goals_scored.ToString() + "-" + teams[row].c_goals_conceded.ToString();
                tableLayout.GetControlFromPosition(8, row+1).Text = teams[row].c_score.ToString();
            }
        }

        private void FillCalendar(TableLayoutPanel tableLayout, List<Round> rounds, Team[] teams)
        {
            int row = 0;
            for(int round=0;round<rounds.Count;round++)
            {
                Control lab = tableLayout.GetControlFromPosition(0, row);
                tableLayout.GetControlFromPosition(0, row).Font= new Font(lab.Font, lab.Font.Style ^ FontStyle.Bold);
                tableLayout.GetControlFromPosition(0, row).Text= "Тур " + (round+1).ToString();
                tableLayout.GetControlFromPosition(1, row).Visible = false;
                tableLayout.GetControlFromPosition(2, row).Visible = false;
                row++;
                foreach(Match match in rounds[round].matches)
                {
                    int m = TeamSearchById(teams, match.team1);
                    tableLayout.GetControlFromPosition(0, row).Text = "("+teams[m].c_owner+") "+ teams[m].c_name;
                    m = TeamSearchById(teams, match.team2);
                    tableLayout.GetControlFromPosition(2, row).Text = "(" + teams[m].c_owner + ") " + teams[m].c_name;
                    if (match.is_played == 1)
                    {
                        tableLayout.GetControlFromPosition(1, row).Text = match.score_team1 + ":" + match.score_team2;
                    }
                    else if (match.is_played == 0)
                    {
                        tableLayout.GetControlFromPosition(1, row).Text = "";
                    }
                    match.textbox = tableLayout.GetControlFromPosition(1, row).Name;
                    row++;
                }
            }
        }

        private void SaveCurrentSeasonToFile()
        {
            StreamWriter sw1 = new StreamWriter("season" + m_Config.current_season.ToString()+"premier.txt");
            foreach (Round one_round in m_RoundsPremierLeague)
            {
                foreach (Match one_match in one_round.matches)
                {
                    string line = "";
                    line = one_match.team1 + " " + one_match.score_team1 + " " + one_match.score_team2 + " " + one_match.team2 + " " + one_match.is_played;
                    sw1.WriteLine(line);
                }
            }
            sw1.Close();

            StreamWriter sw2 = new StreamWriter("season" + m_Config.current_season + "two.txt");
            foreach (Round one_round in m_RoundsTwoLeague)
            {
                foreach (Match one_match in one_round.matches)
                {
                    string line = "";
                    line = one_match.team1 + " " + one_match.score_team1 + " " + one_match.score_team2 + " " + one_match.team2 + " " + one_match.is_played;
                    sw2.WriteLine(line);
                }
            }
            sw2.Close();
        }

        private List<Round> CreateTimeTable(Team[] teams,int count_rounds,int size_one_round)
        {
            List<Round> rounds = new List<Round>();
            List<Match> temp_matches = new List<Match>();

            for(int i = 0; i < m_SizeOneRound*m_CountRounds; i++)
            {
                Match match = new Match();
                match.team1 = teams[m_TemplateCalendar[i,0]].c_id;
                match.team2 = teams[m_TemplateCalendar[i,1]].c_id;
                temp_matches.Add(match);
            }

            for (int i = 0; i < count_rounds; i++)
            {
                rounds.Add(new Round());
                for (int next_match = 0; next_match < size_one_round; next_match++)
                {
                    rounds[i].matches.Add(temp_matches[next_match]);
                }
            }

            return rounds;
        }


        private bool IsTeamOfRound(Match match, List<Match> one_round)
        {
            for(int i = 0; i < one_round.Count; i++)
            {
                if (match.team1 == one_round[i].team1 || match.team1 == one_round[i].team2 || match.team2 == one_round[i].team1 || match.team2 == one_round[i].team2)
                {
                    return false;
                }
            }
            return true;
        }

        private List<Match> MixList(List<Match> list_matches)
        {
            Random rnd = new Random();
            int rand_index;
            Match m;
            for(int i = 0; i < list_matches.Count - 1; i++)
            {
                rand_index = rnd.Next(0, list_matches.Count-1);
                m = list_matches[i];
                list_matches[i] = list_matches[rand_index];
                list_matches[rand_index] = m;
            }
            return list_matches;
        }

        private bool EndSeasonCheck()
        {
            bool is_end = true;
            for (int i = 0; i < m_TeamsPremier.Length; i++)
            {
                if (m_TeamsPremier[i].c_games < m_CountRounds || m_TeamsTwo[i].c_games < m_CountRounds) is_end = false;
            }
            m_Config.end_current_season = is_end;
            return is_end;
        }

        private void Text_Score_Leave(object sender, EventArgs e)
        {
            TextBox txtBx=(TextBox)sender;
            string[] text = txtBx.Text.Split(':');
            if (txtBx.Text.Length == 3 && text.Length == 2)
            {
                    SetScoreMatch(txtBx, txtBx.Parent.Name);
            }
            else
            {
                txtBx.Text = "";
            }
        }

        private void SetScoreMatch(TextBox txtbx, string table_layout)
        {
            string[] text = txtbx.Text.Split(':');
            if (table_layout == tableLayoutPanel3.Name)
            {
                foreach(Round round in m_RoundsPremierLeague)
                {
                    int match_index=MatchSearchByTxtBx(round, txtbx.Name);
                    if (match_index != -1)
                    {
                        round.matches[match_index].score_team1 = int.Parse( text[0]);
                        round.matches[match_index].score_team2 = int.Parse(text[1]);
                        round.matches[match_index].is_played = 1;
                        break;
                    }
                }
                MatchesCalculation(m_RoundsPremierLeague, m_TeamsPremier);
                FillTable(tableLayoutPanel1, m_TeamsPremier);
            }
            else if (table_layout == tableLayoutPanel4.Name)
            {
                foreach (Round round in m_RoundsTwoLeague)
                {
                    int match_index = MatchSearchByTxtBx(round, txtbx.Name);
                    if (match_index != -1)
                    {
                        round.matches[match_index].score_team1 = int.Parse(text[0]);
                        round.matches[match_index].score_team2 = int.Parse(text[1]);
                        round.matches[match_index].is_played = 1;
                        break;
                    }
                }
                MatchesCalculation(m_RoundsTwoLeague, m_TeamsTwo);
                FillTable(tableLayoutPanel2, m_TeamsTwo);
            }
        }

        private int MatchSearchByTxtBx(Round round,string name_txtbx)
        {
            for(int i=0; i < round.matches.Count; i++)
            {
                if (round.matches[i].textbox == name_txtbx) return i;
            }
            return -1;
        }

        private void TextBoxFilter(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && number != 8 && number != 58) //цифры, клавиша BackSpace и запятая а ASCII
            {
                e.Handled = true;
            }
        }
        private void buttonRandomScore1_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            string score1= rnd.Next(0, 4).ToString();
            string score2 = rnd.Next(0, 4).ToString();
            labelRandomScore1.Text = score1 + ":" + score2;
        }

        private void buttonRandomScore2_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            string score1 = rnd.Next(0, 4).ToString();
            string score2 = rnd.Next(0, 4).ToString();
            labelRandomScore2.Text = score1 + ":" + score2;
        }

        public void SetTeamTwo(Team[] teams)
        {
            m_TeamsTwo = teams;
        }

        public void SetTeamPremier(Team[] teams)
        {
            m_TeamsPremier = teams;
        }
    }
}
