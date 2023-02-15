using Dapper;
using DapperExampl.ConstantsHelper;
using DapperExampl.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DapperExampl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public async Task<List<Player>> GetAll()
        {
            List<Player> players = new List<Player>();
            var conn = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (var connection = new SqlConnection(conn))
            {
                var result = await connection.QueryAsync<Player>(@"SELECT Id,Name,Score,IsStar
                                                        FROM Players");
                players = result.ToList();
            }
            return players;
        }
        public async void LoadPlayers()
        {
            myDataGrid.ItemsSource = await GetAll();
        }

        public async void LoadPlayerOne()
        {
            var player = await GetById(2);
            myDataGrid.ItemsSource = new List<Player>
            {
                player
            };
        }

        public async Task<Player> GetById(int id)
        {
            var conn = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (var connection = new SqlConnection(conn))
            {
                var query = "SELECT * FROM Players WHERE Id=@pId";
                var player = await connection.QueryFirstOrDefaultAsync<Player>(query,
                    new { pId = id });
                return player;
            }
        }

        public async void CallSP(double score)
        {
            var conn = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (var connection = new SqlConnection(conn))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@pScore", score, System.Data.DbType.Double);

                var data = await connection.QueryAsync<Player>(SqlProcedureConstants.ShowPlayersGreaterThanScore, parameters, commandType: CommandType.StoredProcedure);
                myDataGrid.ItemsSource = data;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            //Insert(new Player
            //{
            //    Name = "New Player 2",
            //    Score = 78,
            //    IsStar = true
            //});
            LoadPlayers();
            //LoadPlayerOne();
            //DeleteAsync(2);
            //UpdateCall();

            //CallSP(75);
        }

        private async void UpdateCall()
        {
            var player = await GetById(1);
            player.Score = 5555;
            Update(player);
            LoadPlayers();
        }

        public async void Insert(Player player)
        {
            var conn = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;


            using (var connection = new SqlConnection(conn))
            {
                var row = await connection.ExecuteAsync(@"
                           INSERT INTO Players(Name,Score,IsStar)
                            VALUES(@pName,@pScore,@pIsStar)",
                            new
                            {
                                pName = player.Name,
                                pScore = player.Score,
                                pIsStar = player.IsStar
                            });
                MessageBox.Show($"New Player Added succssfully. {row} affected");

            }
            LoadPlayers();
        }

        public async void DeleteAsync(int id)
        {
            var conn = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (var connection = new SqlConnection(conn))
            {
                var query = "DELETE FROM Players WHERE Id=@pId";
                await connection.ExecuteAsync(query, new { pId = id });
            }
        }


        public async void Update(Player player)
        {
            var conn = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (var connection = new SqlConnection(conn))
            {
                var row = await connection.ExecuteAsync(@"
        UPDATE Players
        SET Name=@pName,Score=@pScore,IsStar=@pIsStar
        WHERE Id=@pId
                    ",
         new { pName = player.Name, pScore = player.Score, pIsStar = player.IsStar, pId = player.Id });
            }
        }
        public bool HasAlreadyDeleted { get; set; } = false;
        private void myDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var player = myDataGrid.SelectedItem as Player;
            if (!HasAlreadyDeleted)
            {
                var result = MessageBox.Show($"Are you sure to delete {player?.Name}", "Info", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    DeleteAsync(player.Id);
                    LoadPlayers();
                    HasAlreadyDeleted = true;
                }
                else
                {
                    Name.Text = player.Name;
                        ;
                    Score.Value = player.Score;

                    IsStar.IsChecked = player.IsStar;
                    HasAlreadyDeleted = false;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var player=myDataGrid.SelectedItem as Player;
            var newPlayer = new Player
            {
                Id = player.Id,
                Name = Name.Text,
                IsStar = IsStar.IsChecked.Value,
                Score = (float)Score.Value
            };
            Update(newPlayer);
            HasAlreadyDeleted = true;
            LoadPlayers();
        }
    }
}
