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
                result = 0;
            }

            public int team1;
            public int team2;
            public int score_team1;
            public int score_team2;
            public int result;
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


        private Team[] m_TeamsPremier= new Team[10];
        private Team[] m_TeamsTwo= new Team[10];

        private List<Round> m_RoundsPremierLeague = new List<Round>();
        private List<Round> m_RoundsTwoLeague = new List<Round>();

        private int m_CountRounds=9;
        private int m_SizeOneRound=5;

        //private List<Match> m_TimeTable=new List<Match>();

        private void button1_Click(object sender, EventArgs e)
        {
        }


        private void Form1_Load(object sender, EventArgs e)
        {

            FileTeamsRead();
            FillTable();
            m_RoundsPremierLeague = CreateTimeTable(m_TeamsPremier, m_CountRounds, m_SizeOneRound);
            m_RoundsTwoLeague = CreateTimeTable(m_TeamsTwo, m_CountRounds, m_SizeOneRound);
        }

        private void FileTeamsRead()
        {
            StreamReader sr = new StreamReader("first_table.txt");
            string[] table_line;
            int i = 0;
            while (!sr.EndOfStream)
            {
                table_line = sr.ReadLine().Split(' ');
                if (i < 10) CreateOneTeam(m_TeamsPremier, table_line, i);
                else CreateOneTeam(m_TeamsTwo, table_line, i - 10);
                i++;
            }
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
                        match.team1 = i;
                        match.team2 = j;
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
            //list_matches[0] = m;
            return list_matches;
        }
    }
}
