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
        public Form1()
        {
            InitializeComponent();
        }
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
            public int c_id;
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


        private Team[] m_TeamsPremier= new Team[10];
        private Team[] m_TeamsTwo= new Team[10];

        private List<Round> m_RoundsPremierLeague = new List<Round>();
        private List<Round> m_RoundsTwoLeague = new List<Round>();

        private int m_CountRounds=9;
        private int m_SizeOneRound=5;
        private Config m_Config;

        private void button1_Click(object sender, EventArgs e)
        {
            CreateNewSeason();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadConfig();
            FileTeamsRead(); //Чтение команд из файла

            m_RoundsPremierLeague= FileRoundsRead(m_Config.current_season,m_TeamsPremier,1); //Чтение раундов матчей из файла
            m_RoundsTwoLeague = FileRoundsRead(m_Config.current_season, m_TeamsTwo, 2);
            //m_RoundsTwoLeague = CreateTimeTable(m_TeamsTwo, m_CountRounds, m_SizeOneRound); 
            m_TeamsPremier =MatchesCalculation(m_RoundsPremierLeague, m_TeamsPremier);
            m_TeamsTwo = MatchesCalculation(m_RoundsTwoLeague, m_TeamsTwo);
            
            FillTable(tableLayoutPanel1,m_TeamsPremier);
            FillTable(tableLayoutPanel2, m_TeamsTwo);
            FillCalendar(tableLayoutPanel3, m_RoundsPremierLeague,m_TeamsPremier);
            FillCalendar(tableLayoutPanel4, m_RoundsTwoLeague, m_TeamsTwo);

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveConfig();
            //SaveCurrentSeasonToFile();
        }

        private void LoadConfig()
        {
            StreamReader sr = new StreamReader("config.txt");
            m_Config.current_season = int.Parse(sr.ReadLine());
            m_Config.end_current_season = int.Parse(sr.ReadLine())==1;
            sr.Close();
        }

        private void SaveConfig()
        {
            StreamWriter sw = new StreamWriter("config.txt");
            sw.WriteLine( m_Config.current_season.ToString());
            sw.WriteLine(m_Config.end_current_season ? "1" :"0");
            sw.Close();
        }

        private void FileTeamsRead()
        {
            StreamReader sr = new StreamReader("current_table.txt");
            string[] table_line;
            int i = 0;
            while (!sr.EndOfStream)
            {
                table_line = sr.ReadLine().Split(' ');
                if (i < 10) CreateOneTeam(m_TeamsPremier, table_line, i);
                else CreateOneTeam(m_TeamsTwo, table_line, i - 10);
                i++;
            }
            sr.Close();
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
            teams[i].c_id = int.Parse(table_line[10]);
        }

        private List<Round> FileRoundsRead(int season_number,Team[] teams,int league_number)
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
            for (int r = 0; r < m_CountRounds; r++) {
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
            return teams;
        }

        private void FillTable(TableLayoutPanel tableLayout, Team[] teams)
        {
            for (int row=0; row < teams.Length; row++)
            {
                tableLayout.GetControlFromPosition(0, row+1).Text = teams[row].c_number.ToString();
                tableLayout.GetControlFromPosition(1, row+1).Text = teams[row].c_owner;
                tableLayout.GetControlFromPosition(2, row+1).Text = teams[row].c_name;
                tableLayout.GetControlFromPosition(3, row+1).Text = teams[row].c_games.ToString();
                tableLayout.GetControlFromPosition(4, row+1).Text = teams[row].c_wins.ToString();
                tableLayout.GetControlFromPosition(5, row+1).Text = teams[row].c_draws.ToString();
                tableLayout.GetControlFromPosition(6, row+1).Text = teams[row].c_loses.ToString();
                tableLayout.GetControlFromPosition(7, row+1).Text = teams[row].c_goals_scored.ToString() + "-" + m_TeamsPremier[row].c_goals_conceded.ToString();
                tableLayout.GetControlFromPosition(8, row+1).Text = teams[row].c_score.ToString();
            }
        }

        private void FillCalendar(TableLayoutPanel tableLayout, List<Round> rounds, Team[] teams)
        {
            int row = 0;
            for(int round=0;round<rounds.Count;round++)
            {
                // tableLayout.GetControlFromPosition(0, row).Text = "Тур " + round;

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
                    match.textbox = tableLayout.GetControlFromPosition(1, row).Name;
                    row++;
                }
            }
        }

        private void CreateNewSeason()
        {
            if (EndSeasonCheck())
            {
                SaveCurrentSeasonToFile();
                m_RoundsPremierLeague = CreateTimeTable(m_TeamsPremier, m_CountRounds, m_SizeOneRound);
                m_RoundsTwoLeague = CreateTimeTable(m_TeamsTwo, m_CountRounds, m_SizeOneRound);
                m_Config.current_season++;
                SaveCurrentSeasonToFile();
            }
            else
            {
                MessageBox.Show("Завершите текущий сезон!");
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

            for (int i=0;i< teams.Length; i++)
            {
                for(int j = i; j < teams.Length; j++)
                {
                    if (i != j)
                    {
                        Match match=new Match();
                        match.team1 = teams[i].c_id;
                        match.team2 = teams[j].c_id;
                        temp_matches.Add(match);
                    }
                }
            }

            for(int i = 0; i < count_rounds; i++)
            {
                rounds.Add(new Round());
                for (int next_match = 0; next_match < size_one_round; next_match++)
                {
                    for(int m = 0; m < temp_matches.Count; m++)
                    {
                        if (IsTeamOfRound(temp_matches[m], rounds[i].matches))
                        {
                            rounds[i].matches.Add(temp_matches[m]);
                            break;
                        }
                    }
                }
                if(rounds[i].matches.Count< size_one_round)
                {
                    rounds.RemoveAt(i);
                    i--;
                    temp_matches = MixList(temp_matches);
                }
                else if (rounds[i].matches.Count == size_one_round)
                {
                    for(int j=0;j<rounds[i].matches.Count;j++)
                        temp_matches.Remove(rounds[i].matches[j]);
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
                if (m_TeamsPremier[i].c_games < 9 || m_TeamsTwo[i].c_games < 9) is_end = false;
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
    }
}
