using Desktop_Amethyst_Audio.Models.Clients.Abstraction;
using Desktop_Amethyst_Audio.Models.Clients.Implementation;
using Desktop_Amethyst_Audio.Models.DTO.Pages;
using Desktop_Amethyst_Audio.Models.DTO.Playlists;
using Desktop_Amethyst_Audio.Models.DTO.Tracks;
using Desktop_Amethyst_Audio.Models.DTO.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Desktop_Amethyst_Audio.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для TestApiClientWindow.xaml
    /// </summary>
    public partial class TestApiClientWindow : Window
    {
        private IAuthApiClient _authApiClient;
        private IProfileApiClient _profileApiClient;
        private IRecommendationApiClient _recommendationApiClient;
        private ISearchApiClient _searchApiClient;
        private ITrackApiClient _trackApiClient;
        private IAlbumApiClient _albumApiClient;
        private IPlaylistApiClient _playlistApiClient;
        private IReportApiClient _reportApiClient;

        public TestApiClientWindow()
        {
            InitializeComponent();
            _authApiClient = new AuthApiClient();
            _profileApiClient = new ProfileApiClient();
            _recommendationApiClient = new RecommendationApiClient();
            _searchApiClient = new SearchApiClient();
            _trackApiClient = new TrackApiClient();
            _albumApiClient = new AlbumApiClient();
            _playlistApiClient = new PlaylistApiClient();
            _reportApiClient = new ReportApiClient();
        }

        private async void BeginAuthApiClientTestButton_Click(object sender, RoutedEventArgs e)
        {
            TestResultTB.Text = string.Empty;
            await TestRegister();
            await TestLogin();
        }

        private async Task TestLogin()
        {
            //Test Login
            LoginDto loginDto = new LoginDto()
            {
                Email = "test.email@mail.com",
                Password = "1111"
            };

            try
            {
                UserInfoDto dtoLogin = await _authApiClient.LoginUserAsync(loginDto);
                TestResultTB.Text += 
                    "Test Login\n" +
                    GetUserString(dtoLogin);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
        }

        private async Task TestRegister()
        {
            //Test Register
            CreateUserDto createUserDto = new CreateUserDto()
            {
                Nickname = "TestUser123321123321",
                Email = "test.email@mail.com",
                Password = "1111",
            };

            try
            {
                UserInfoDto dtoRegister = await _authApiClient.RegisterAsync(createUserDto);
                TestResultTB.Text += 
                    "Test Register\n" +
                    GetUserString(dtoRegister);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
        }

        private async void BeginProfileApiClientTestButton_Click(object sender, RoutedEventArgs e)
        {
            TestResultTB.Text = string.Empty;

            int userId = 7;
            int trackId = 5;
            int seconds = 125;
            await TestGetUserById(userId);
            await TestGetUserAll();
            await TestUpdateUser(userId);
            await DeleteUser(userId);
            await GetUserHistory();
            await AddTrackToUserHistory(trackId);
            await UpdateListatningTime(trackId, seconds);

            //Test GetUserLibrary
            try
            {
                List<TrackInfoDto> trackdDtos = await _profileApiClient.GetUserLibraryAsync();
                TestResultTB.Text += "Test GetUserLibrary\n";
                foreach (TrackInfoDto trackDto in trackdDtos)
                {
                    TestResultTB.Text += GetTrackString(trackDto);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.InnerException);
            }

            //Test AddTrackToUserLibrary
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.InnerException);
            }

            //Test RemoveTrackFromUserLibrary
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.InnerException);
            }

            //Test GetUserSavedPlaylists
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.InnerException);
            }

            //Test GetUserSavedAlbums
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.InnerException);
            }

            //Test GetUserAvatar
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.InnerException);
            }

            //Test GetUserHeader
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.InnerException);
            }

            //Test FollowUser
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.InnerException);
            }

            //Test UnfollowUser
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
        }

        private async Task UpdateListatningTime(int trackId, int listeningTime)
        {
            //Test UpdateListningTime
            try
            {
                await _profileApiClient.UpdateListeningTimeAsync(trackId, listeningTime);
                TestResultTB.Text += "Test UpdateListeningTime" +
                    $"TrackId - {trackId}" +
                    $"Track's time - {listeningTime}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
        }

        private async Task AddTrackToUserHistory(int trackId)
        {
            //Test AddToHistory
            try
            {
                await _profileApiClient.AddToHistoryAsync(trackId);
                TestResultTB.Text += "Test AddToHistory\n" +
                    $"Track - {trackId} was successfully addded to history\n";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
        }

        private async Task GetUserHistory()
        {
            //Test GetUserHistory
            try
            {
                List<UserHistoryDto> userHistoryDtos = await _profileApiClient
                    .GetUserHistoryAsync();
                TestResultTB.Text += "Test GetUserHistory\n";
                foreach (var dto in userHistoryDtos)
                {
                    TestResultTB.Text += GetUserString(dto.User);
                    TestResultTB.Text += GetTrackString(dto.Track);
                    TestResultTB.Text += dto.TotalListeningSec + "\n";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
        }
        
        private async Task DeleteUser(int userId)
        {
            //Test Delete
            try
            {
                await _profileApiClient.DeleteUserAsync(userId);
                TestResultTB.Text += 
                    "Test DeleteUser\n" +
                    $"User {userId} was successfully deleted\n";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
        }

        private async Task TestUpdateUser(int userId)
        {
            //Test Update
            try
            {
                ChangeUserInfoDto changeUserInfoDto = new ChangeUserInfoDto()
                {
                    Nickname = "Editted User",
                    Lastname = "Satherland",
                    Firstname = "James",
                    Email = "james.satherland@mail.com",
                    Country = "Ru",
                    Gender = "M"
                };
                UserInfoDto userDto = await _profileApiClient.GetUserByIdAsync(userId);

                TestResultTB.Text += 
                    "Test before UpdateUse\n" +
                    GetUserString(userDto);

                userDto.Nickname = "Editted User";
                userDto = await _profileApiClient.UpdateUserAsync(changeUserInfoDto);

                TestResultTB.Text += 
                    "Test after UpdateUser\n" +
                    GetUserString(userDto);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
        }

        private async Task TestGetUserAll()
        {
            //Test GetAll
            try
            {
                List<UserInfoDto> userDtos = await _profileApiClient.GetUserAllAsync();
                TestResultTB.Text += 
                    "Test GetUserAll\n" +
                    $"UserList - {userDtos.Count}\n";
                foreach (UserInfoDto userDto in userDtos)
                {
                    TestResultTB.Text += GetUserString(userDto);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
        }

        private async Task TestGetUserById(int userId)
        {
            //Test GetById
            try
            {
                UserInfoDto userDto = await _profileApiClient.GetUserByIdAsync(userId);
                TestResultTB.Text += 
                    "Test GetById\n" +
                    GetUserString(userDto);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
        }

        private async void BeginRecommendationApiClientTestButton_Click(object sender, RoutedEventArgs e)
        {
            TestResultTB.Text = string.Empty;

            //Test GetRecommendationConfig
            try
            {
                ResonanceConfigDto resonanceConfigDto = await _recommendationApiClient
                    .GetRecommendationConfigAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.InnerException);
            }

            //Test GetPersonalizedRecommendation
            PageResonanceDto resonanceDto = new PageResonanceDto()
            {
                PaceId = 1,
                MoodId = 1,
                IsTextless = true
            };
            
            try
            {
                List<TrackInfoDto> trackInfoDtoResult = await _recommendationApiClient
                    .GetPersonalizedRecommendationsAsync(resonanceDto);
                TestResultTB.Text += $"Test GetPersonalizedRecommendations";
                foreach(TrackInfoDto dto in trackInfoDtoResult)
                {
                    TestResultTB.Text +=
                        $"Id - {dto.Id}\n" +
                        $"Name - {dto.Name}\n" +
                        $"UserList - {dto.UserList.Count}\n";
                    foreach (UserInfoDto userInfoDto in dto.UserList)
                    {
                        TestResultTB.Text += 
                            $"---" +
                            $"\tId - {userInfoDto.Id}\n" +
                            $"\tLastname - {userInfoDto.Lastname}\n" +
                            $"\tFirstname - {userInfoDto.Firstname}\n" +
                            $"\tNickname - {userInfoDto.Nickname}\n" +
                            $"\tAvatarUrl - {userInfoDto.AvatarUrl}\n" +
                            $"\tHeaderUrl - {userInfoDto.HeaderUrl}\n" +
                            $"\tIsVerified - {userInfoDto.IsVerified}\n" +
                            $"\tToken - {userInfoDto.Token}\n";
                    }
                    TestResultTB.Text +=
                        $"CoverUrl - {dto.CoverUrl}\n" +
                        $"TrackUrl - {dto.TrackUrl}\n" +
                        $"IsExplicit - {dto.IsExplicit}\n" +
                        $"DurationSec - {dto.DurationSec}\n\n";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.InnerException);
            }
        }

        private void BeginSearchApiClientTestButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BeginTrackApiClientTestButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BeginAlbumApiClientTestButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BeginPlalistApiClientTestButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BeginReportApiClientTestButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private string GetUserString(UserInfoDto dto)
        {
            string result = string.Empty;
            result =
                "User:\n" +
                $"\tId - {dto.Id}\n" +
                $"\tLastname - {dto.Lastname}\n" +
                $"\tFirstname - {dto.Firstname}\n" +
                $"\tNickname - {dto.Nickname}\n" +
                $"\tAvatarUrl - {dto.AvatarUrl}\n" +
                $"\tHeaderUrl - {dto.HeaderUrl}\n" +
                $"\tIsVerified - {dto.IsVerified}\n" +
                $"\tToken - {dto.Token}\n";
            return result;
        }

        private string GetTrackString(TrackInfoDto dto)
        {
            string result = string.Empty;
            result =
                "Track:\n" +
                $"\tId - {dto.Id}\n" +
                $"\tName - {dto.Name}\n" +
                $"\tCoverUrl - {dto.CoverUrl}\n" +
                $"\tTrackUrl - {dto.TrackUrl}\n" +
                $"\tIsExplicit - {dto.IsExplicit}\n" +
                $"\tDurationSec - {dto.DurationSec}\n" +
                $"\tUserList - {dto.UserList.Count}\n";
            foreach (UserInfoDto userDto in dto.UserList)
            {
                result += GetUserString(userDto);
            }
            return result;
        }

        private string GetPlaylistString(PlaylistInfoDto dto)
        {
            string result = string.Empty;
            result =
                "Playlist: \n" +
                $"\tId - {dto.Id}\n" +
                $"\tOwnerId - {dto.OwnerId}\n" +
                $"\tName - {dto.Name}\n" +
                $"\tDescription - {dto.Description}\n" +
                $"\tCoverUrl - {dto.CoverUrl}\n" +
                $"\tTrackList - {dto.TrackList.Count}\n";
            foreach (TrackInfoDto trackDto in dto.TrackList)
            {
                result += GetTrackString(trackDto);
            }
            return result;
        }
    }
}
