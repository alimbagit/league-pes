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
                row_number = 0;
            }

            public int team1;
            public int team2;
            public int score_team1;
            public int score_team2;
            public int is_played;
            public int row_number;
        }

        private int[,] m_TemplateCalendar9 = {
            {4,6},{7,3 },{5,0 },{2,1 },
            {6,8 }, {3,0 }, {7,5 }, {4,2}, 
            {8,3 }, {6,1}, {4,5 }, {7,2}, 
            {8,0 }, {4,1}, {3,2 }, {6,7 }, 
            {8,5 }, {6,2 }, {7,0 }, {3,1 }, 
            {5,2 }, {8,1 }, {4,3}, {6,0 }, 
            {4,8 }, {3,5}, {0,2}, {7,1}, 
            {6,3},{4,0},{5,1},{8,7},
            {6,5},{4,7},{8,2},{0,1}};

        private int[,] m_TemplateCalendar10 = {{4, 6},{8,9 },{7,3 },{5,0 },{2,1 },{6,8 },{3,0 }, {9,1 }, {7,5 }, {4,2 }, {9,0 } ,
            {8,3 }, {6,1 }, {4,5 }, { 7,2}, {8,0 } , {9,5 }, {4,1 }, {3,2 }, {6,7 }, {8,5 } , {6,2 }, {7,0 }, {4,9 }, {3,1 }, {9,7 } , {5,2 }, {8,1 },
            {4,3 }, {6,0 }, {4,8 },{3,5},{0,2},{6,9},{7,1},{6,3},{4,0},{9,2},{5,1},{8,7},{6,5},{9,3},{4,7},{8,2},{0,1}};

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
            public int count_teams;
            public int size_one_round;
        }

        private static int m_CountRounds = 9;

        public List<Team> m_TeamsAll = new List<Team>();
        //public Team[] m_TeamsPremier= new Team[10];
        //public Team[] m_TeamsTwo= new Team[10];
        public Team[] m_TeamsPremier;
        public Team[] m_TeamsTwo;

        private List<Round> m_RoundsPremierLeague = new List<Round>();
        private List<Round> m_RoundsTwoLeague = new List<Round>();

        public Config m_Config;


        private void Form1_Load(object sender, EventArgs e)
        {
            LoadConfig();
            InitializeTeamsArray(); //Объявление массивов текущей таблицы
            FileTeamsLoad(); //Чтение текущих команд из файла
            m_TeamsAll= FileAllTeamsLoad(); //Чтение всех команд из файла

            m_RoundsPremierLeague = FileRoundsRead(m_Config.current_season,1); //Чтение раундов матчей из файла
            m_RoundsTwoLeague = FileRoundsRead(m_Config.current_season, 2); 
            m_TeamsPremier =MatchesCalculation(m_RoundsPremierLeague, m_TeamsPremier); //расчет таблиц по результатам матчей
            m_TeamsTwo = MatchesCalculation(m_RoundsTwoLeague, m_TeamsTwo);
            FillTable(tableLayoutPanel1,m_TeamsPremier); //заполнение таблиц
            FillTable(tableLayoutPanel2, m_TeamsTwo);
            FillCalendar(tableLayoutPanel3, m_RoundsPremierLeague, m_TeamsPremier); //заполнение календаря игр
            FillCalendar(tableLayoutPanel4, m_RoundsTwoLeague, m_TeamsTwo);
            FillChampionTeams(); //заполнение таблицы чемпионов
        }

        private void InitializeTeamsArray()
        {
            m_TeamsPremier = new Team[m_Config.count_teams];
            m_TeamsTwo = new Team[m_Config.count_teams];
        }

    private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveConfig();
            FileCurrentTeamsSave();
            SaveCurrentSeasonToFile();
        }

        private void SetCountTeams(int count_teams)
        {
            m_Config.count_teams = count_teams;
            if (count_teams == 9)
            {
                m_Config.size_one_round = 4;
                m_TemplateCalendar = m_TemplateCalendar9;
            }
            else if (count_teams == 10)
            {
                m_Config.size_one_round = 5;
                m_TemplateCalendar = m_TemplateCalendar10;
            }
        }

        private void buttonNewSeason_Click(object sender, EventArgs e)
        {
            if (EndSeasonCheck())
            {
                Form2 newForm = new Form2(this);
                newForm.Show();
            }
            else
            {
                MessageBox.Show("Завершите текущий сезон!");
            }
        }

        public void CreateNewSeason(Form2 f)
        {
            SaveCurrentSeasonToFile();
            SetCountTeams(f.m_CountTeams);
            m_Config.current_season++;
            m_TeamsPremier = f.m_TeamsPremier;
            m_TeamsTwo = f.m_TeamsTwo;

            m_RoundsPremierLeague = CreateTimeTable(m_TeamsPremier, m_CountRounds, m_Config.size_one_round);
            m_RoundsTwoLeague = CreateTimeTable(m_TeamsTwo, m_CountRounds, m_Config.size_one_round);
            m_TeamsPremier = MatchesCalculation(m_RoundsPremierLeague, m_TeamsPremier);
            m_TeamsTwo = MatchesCalculation(m_RoundsTwoLeague, m_TeamsTwo);

            FileCurrentTeamsSave();
            FillTable(tableLayoutPanel1, m_TeamsPremier);
            FillTable(tableLayoutPanel2, m_TeamsTwo);
            FillCalendar(tableLayoutPanel3, m_RoundsPremierLeague, m_TeamsPremier);
            FillCalendar(tableLayoutPanel4, m_RoundsTwoLeague, m_TeamsTwo);
            FillChampionTeams();
            UpdateSeasonLables();
        }

        private void LoadConfig()
        {
            StreamReader sr = new StreamReader("config.txt");
            m_Config.current_season = int.Parse(sr.ReadLine());
            m_Config.count_teams= int.Parse(sr.ReadLine());
            m_Config.size_one_round = int.Parse(sr.ReadLine());
            sr.Close();
            UpdateSeasonLables();
        }

        private void SaveConfig()
        {
            StreamWriter sw = new StreamWriter("config.txt");
            sw.WriteLine( m_Config.current_season.ToString());
            sw.WriteLine(m_Config.count_teams.ToString());
            sw.WriteLine(m_Config.size_one_round.ToString());
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
                if (i < m_Config.count_teams) m_TeamsPremier[i]= CreateOneTeam( table_line);
                else m_TeamsTwo[i-m_Config.count_teams]=CreateOneTeam(table_line);
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
                if (one_round.matches.Count < m_Config.size_one_round)
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

        private int TeamSearchById(List<Team> teams, int id)
        {
            for (int i = 0; i < teams.Count; i++)
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
            //Очистка таблицы от пустых ячеек
            for (int i = 1; i < tableLayout.RowCount; i++)
            {
                for (int j = 0; j < tableLayout.ColumnCount; j++)
                {
                    tableLayout.Controls.Remove(tableLayout.GetControlFromPosition(j, i));
                }
            }
            tableLayout.RowCount = 1;
            //Создание таблицы
            tableLayout.Size = new Size(tableLayout.Size.Width, 40);
            tableLayout.RowStyles[0].SizeType = SizeType.Absolute;
            tableLayout.RowStyles[0].Height = 30;
            for (int row = 0; row < teams.Length; row++)
            {
                tableLayout.Size = new Size(tableLayout.Size.Width, tableLayout.Size.Height + 23);
                tableLayout.RowCount++;

                for(int col = 0; col < 9; col++)
                {
                    Label label = new Label();
                    label.Font = new Font("Microsoft Sans Serif", 12f);
                    if (row < 3) label.ForeColor = Color.FromArgb(0, 0, 120);
                    else if(row>=m_Config.count_teams-3) label.ForeColor = Color.FromArgb(120, 0, 0);
                    tableLayout.Controls.Add(label, col, tableLayout.RowCount - 1);
                }
            }


            //Вывод в таблицу отсортированного списка команд
            for (int row = 0; row < teams.Length; row++)
            {
                tableLayout.GetControlFromPosition(0, row + 1).Text = (row + 1).ToString();
                tableLayout.GetControlFromPosition(1, row + 1).Text = teams[row].c_owner;
                tableLayout.GetControlFromPosition(2, row + 1).Text = teams[row].c_name;
                tableLayout.GetControlFromPosition(3, row + 1).Text = teams[row].c_games.ToString();
                tableLayout.GetControlFromPosition(4, row + 1).Text = teams[row].c_wins.ToString();
                tableLayout.GetControlFromPosition(5, row + 1).Text = teams[row].c_draws.ToString();
                tableLayout.GetControlFromPosition(6, row + 1).Text = teams[row].c_loses.ToString();
                tableLayout.GetControlFromPosition(7, row + 1).Text = teams[row].c_goals_scored.ToString() + "-" + teams[row].c_goals_conceded.ToString();
                tableLayout.GetControlFromPosition(8, row + 1).Text = teams[row].c_score.ToString();
            }
        }

        private void FillCalendar(TableLayoutPanel tableLayout, List<Round> rounds, Team[] teams)
        {
            //Очистка расписания от пустых ячеек
            for (int i = 1; i < tableLayout.RowCount; i++)
            {
                for (int j = 0; j < tableLayout.ColumnCount; j++)
                {
                    tableLayout.Controls.Remove(tableLayout.GetControlFromPosition(j, i));
                }
            }
            tableLayout.RowCount = 1;
            tableLayout.Size = new Size(tableLayout.Size.Width, 0);

            //Заполнение расписания
            int row = 0;
            for(int round=0;round<rounds.Count;round++)
            {
                tableLayout.RowStyles[0].SizeType = SizeType.Absolute;
                tableLayout.RowStyles[0].Height = 30;
                tableLayout.Size = new Size(tableLayout.Size.Width, tableLayout.Size.Height + 23);
                tableLayout.RowCount++;
                Label label = new Label();
                label.Font = new Font("Microsoft Sans Serif", 11f, FontStyle.Bold);
                label.Anchor = AnchorStyles.Right;
                label.AutoSize = true;
                tableLayout.Controls.Add(label, 0, row);
                label.Text= "Тур " + (round + 1).ToString();

                row++;
                
                foreach(Match match in rounds[round].matches)
                {
                    tableLayout.Size = new Size(tableLayout.Size.Width, tableLayout.Size.Height + 30);
                    tableLayout.RowCount++;
                    

                    int m = TeamSearchById(teams, match.team1);
                    label = new Label();
                    label.Text = "(" + teams[m].c_owner + ") " + teams[m].c_name;
                    label.Font = new Font("Microsoft Sans Serif", 11f);
                    label.Anchor = AnchorStyles.Right;
                    label.AutoSize = true;
                    tableLayout.Controls.Add(label, 0, row);

                    m = TeamSearchById(teams, match.team2);
                    label = new Label();
                    label.Text = "(" + teams[m].c_owner + ") " + teams[m].c_name;
                    label.Font = new Font("Microsoft Sans Serif", 11f);
                    label.Anchor = AnchorStyles.Left;
                    label.AutoSize = true;
                    tableLayout.Controls.Add(label, 2, row);

                    TextBox txtbx=new TextBox();
                    txtbx.Font = new Font("Microsoft Sans Serif", 11f);
                    txtbx.Leave += new EventHandler(Text_Score_Leave);
                    txtbx.KeyPress += new KeyPressEventHandler(TextBoxFilter);
                    txtbx.AutoSize = true;
                    txtbx.TextAlign = HorizontalAlignment.Center;

                    if (match.is_played == 1)
                    {
                        txtbx.Text = match.score_team1 + ":" + match.score_team2;
                    }
                    else if (match.is_played == 0)
                    {
                        txtbx.Text = "";
                    }
                    tableLayout.Controls.Add(txtbx, 1, row);
                    match.row_number = row;
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

            for(int i = 0; i < m_Config.size_one_round*m_CountRounds; i++)
            {
                Match match = new Match();
                match.team1 = teams[m_TemplateCalendar[i,0]].c_id;
                match.team2 = teams[m_TemplateCalendar[i,1]].c_id;
                temp_matches.Add(match);
            }

            int next_match=0;
            for (int i = 0; i < count_rounds; i++)
            {
                rounds.Add(new Round());
                for (int j = 0; j < size_one_round; j++)
                {
                    rounds[i].matches.Add(temp_matches[next_match]);
                    next_match++;
                }
            }

            return rounds;
        }

        private bool EndSeasonCheck()
        {
            bool is_end = true;
            for (int i = 0; i < m_TeamsPremier.Length; i++)
            {
                if (m_TeamsPremier[i].c_games < m_CountRounds || m_TeamsTwo[i].c_games < m_CountRounds) is_end = false;
            }
            return is_end;
        }

        private void Text_Score_Leave(object sender, EventArgs e)
        {
            TextBox txtBx=(TextBox)sender;

            string[] text = txtBx.Text.Split(':');
            if (txtBx.Text.Length != 3 || text.Length != 2)
            {
                txtBx.Text = "";
            }
            TableLayoutPanel tableLayout =(TableLayoutPanel) txtBx.Parent;
            int row_number = tableLayout.GetRow(txtBx);
            SetScoreMatch(row_number,txtBx);
        }

        private void SetScoreMatch(int row_number, TextBox txtbx)
        {
            string[] text = txtbx.Text.Split(':');
            if (txtbx.Parent.Name == tableLayoutPanel3.Name)
            {
                foreach(Round round in m_RoundsPremierLeague)
                {
                    int match_index=MatchSearchByTxtBx(round, row_number);
                    if (match_index != -1)
                    {
                        if (txtbx.Text == "")
                        {
                            round.matches[match_index].score_team1 = 0;
                            round.matches[match_index].score_team2 = 0;
                            round.matches[match_index].is_played = 0;
                        }
                        else
                        {
                            round.matches[match_index].score_team1 = int.Parse(text[0]);
                            round.matches[match_index].score_team2 = int.Parse(text[1]);
                            round.matches[match_index].is_played = 1;
                        }
                        break;
                    }
                }
                MatchesCalculation(m_RoundsPremierLeague, m_TeamsPremier);
                FillTable(tableLayoutPanel1, m_TeamsPremier);
            }
            else if (txtbx.Parent.Name == tableLayoutPanel4.Name)
            {
                foreach (Round round in m_RoundsTwoLeague)
                {
                    int match_index = MatchSearchByTxtBx(round, row_number);
                    if (match_index != -1)
                    {
                        if (txtbx.Text == "")
                        {
                            round.matches[match_index].score_team1 = 0;
                            round.matches[match_index].score_team2 = 0;
                            round.matches[match_index].is_played = 0;
                        }
                        else
                        {
                            round.matches[match_index].score_team1 = int.Parse(text[0]);
                            round.matches[match_index].score_team2 = int.Parse(text[1]);
                            round.matches[match_index].is_played = 1;
                        }
                        break;
                    }
                }
                MatchesCalculation(m_RoundsTwoLeague, m_TeamsTwo);
                FillTable(tableLayoutPanel2, m_TeamsTwo);
            }
        }

        private int MatchSearchByTxtBx(Round round,int row_number)
        {
            for(int i=0; i < round.matches.Count; i++)
            {
                if (round.matches[i].row_number == row_number) return i;
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


        //Установить чемпионские очки (используется в конце сезона)
        public void CalculateChampionScore()
        {
            Team team;
            for(int i = 0; i < m_TeamsPremier.Length; i++)
            {
                team = m_TeamsAll[TeamSearchById(m_TeamsAll, m_TeamsPremier[i].c_id)];
                if (i == 0)
                {
                    team.c_champion_score += 25;
                    team.c_1st_place += 1;
                }
                else if (i == 1)
                {
                    team.c_champion_score += 5;
                    team.c_2nd_place += 1;
                }
                else if (i == 2)
                {
                    team.c_champion_score += 1;
                    team.c_3rd_place += 1;
                }
                m_TeamsAll[TeamSearchById(m_TeamsAll, m_TeamsPremier[i].c_id)] = team;
            }
        }

        private void FillChampionTeams()
        {
            //Очистка таблицы чемпионов от пустых ячеек
            for(int i = 1; i < tableLayoutPanel5.RowCount; i++)
            {
                for(int j = 0; j < tableLayoutPanel5.ColumnCount; j++)
                {
                    tableLayoutPanel5.Controls.Remove(tableLayoutPanel5.GetControlFromPosition(j, i));
                }
            }


            List<Team> teams_temp = m_TeamsAll;

            //Сортировка общего списка команд по чемпионским очкам
            Team t;
            for (int j = 0; j < teams_temp.Count; j++)
            {
                for (int i = 1; i < teams_temp.Count; i++)
                {
                    if (teams_temp[i - 1].c_champion_score < teams_temp[i].c_champion_score)
                    {
                        t = teams_temp[i - 1];
                        teams_temp[i - 1] = teams_temp[i];
                        teams_temp[i] = t;
                    }
                }
            }

            //Вывод в таблицу отсортированного списка чемпионов
            int champions_count = 0;
            for (int i=0; i < teams_temp.Count; i++)
            {
                if (teams_temp[i].c_champion_score > 0)
                {
                    champions_count++;
                    tableLayoutPanel5.Size = new Size(tableLayoutPanel5.Size.Width, tableLayoutPanel5.Size.Height + 30);
                    tableLayoutPanel5.RowCount++;
                    tableLayoutPanel5.RowStyles[0].SizeType = SizeType.Absolute;
                    tableLayoutPanel5.RowStyles[0].Height=30;

                    Label label = new Label();
                    label.AutoSize = true;
                    label.Text = (champions_count).ToString();
                    tableLayoutPanel5.Controls.Add(label, 0, tableLayoutPanel5.RowCount - 1);
                    label = new Label();
                    label.Text = teams_temp[i].c_owner;
                    tableLayoutPanel5.Controls.Add(label, 1, tableLayoutPanel5.RowCount - 1);
                    label = new Label();
                    label.Text = teams_temp[i].c_name;
                    tableLayoutPanel5.Controls.Add(label, 2, tableLayoutPanel5.RowCount - 1);
                    label = new Label();
                    label.Text = teams_temp[i].c_1st_place.ToString();
                    tableLayoutPanel5.Controls.Add(label, 3, tableLayoutPanel5.RowCount-1);
                    label = new Label();
                    label.Text = teams_temp[i].c_2nd_place.ToString();
                    tableLayoutPanel5.Controls.Add(label, 4, tableLayoutPanel5.RowCount-1);
                    label = new Label();
                    label.Text = teams_temp[i].c_3rd_place.ToString();
                    tableLayoutPanel5.Controls.Add(label, 5, tableLayoutPanel5.RowCount-1);
                }
            }
        }
    }
}
