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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.VisualBasic;

namespace ArrayFuncties_4___Reaction_speed
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string[] _imageNames = new string[4] { "click_monster", "dog", "cat", "baby" };
        private const int MonsterIndex = 0;
        private const int NonMonsterStartIndex = 3;

        // Random wordt gebruikt om een willekeurige afbeelding te genereren 
        // iedere keer ChangeMonster() wordt opgeroepen.
        private Random _rand = new Random();
        private int _randomIndex = 0;

        private int _penaltyCounter = 0;
        // milliseconds telt het aantal milliseconden dat de speler
        // nodig heeft om op het monster te klikken.
        private int _milliseconds = 0;
        private int _fastestTime = int.MaxValue;

        private string[] _highScoresNames = new string[5];
        private int[] _highScoresTimes = new int[5];

        #region DispatcherTimers
        // changeMonsterTimer is de dispatcher die iedere tick de afbeelding verandert.
        private DispatcherTimer _changeMonsterTimer;
        // millisecondsTimer wordt gebruikt om het aantal milliseconden 
        // te tellen dat de speler nodig heeft om te klikken.
        private DispatcherTimer _reactionTimer;
        private DispatcherTimer _clock;
        #endregion

        private bool _isGameRunning = false;

        public MainWindow()
        {
            InitializeComponent();
            SetStartImage();

            // Maak timer om images te tonen
            _changeMonsterTimer = new DispatcherTimer();
            _changeMonsterTimer.Interval = new TimeSpan(0, 0, 1);
            _changeMonsterTimer.Tick += new EventHandler(ChangeMonster);

            // Maak klok dispatcher aan die de tijd op het scherm up to date houd.
            _clock = new DispatcherTimer();
            _clock.Interval = new TimeSpan(0, 0, 1);
            _clock.Tick += new EventHandler(KlokTick);
            _clock.Start();

            // Maak millisecondsTimer dispatcher die start 
            // iedere keer als er een monster verschijnt.
            _reactionTimer = new DispatcherTimer();
            _reactionTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            _reactionTimer.Tick += new EventHandler(TimerTick);
            // De timer start pas wanneer er een monster verschijnt

            //ResetLabels();
        }

        /// <summary>
        /// Stelt de eerste afbeelding in op een niet monster afbeelding.
        /// </summary>
        private void SetStartImage()
        {
            gameImage.Source = new BitmapImage(new Uri($"images\\{_imageNames[NonMonsterStartIndex]}.png", UriKind.Relative));
        }

        //private void ResetLabels()
        //{
        //    SnelsteNaamTextBlock.Text = "???";
        //    SnelsteTijdTextBlock.Text = "---";
        //}

        #region Tick event handlers

        /// <summary>Timet de reactiesnelheid van de gebruiker.</summary>
        private void TimerTick(object sender, EventArgs e)
        {
            _milliseconds++;
        }

        /// <summary>Update en toont het uur aan de speler.</summary>
        private void KlokTick(object sender, EventArgs e)
        {
            timeTextBlock.Text = DateTime.Now.ToLongTimeString();
        }

        /// <summary>
        /// Een random index wordt gebruikt om een willekeurige afbeelding
        /// te kiezen.
        /// Wanneer er een Monster wordt gekozen, dan start te millisecondsTimer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeMonster(object sender, EventArgs e)
        {
            _randomIndex = _rand.Next(0, _imageNames.Length);
            gameImage.Source = new BitmapImage(new Uri($"images\\{_imageNames[_randomIndex]}.png", UriKind.Relative));
            if (_randomIndex == MonsterIndex)
            {
                _milliseconds = 0;
                _reactionTimer.Start();
            }
            else
            {
                _reactionTimer.Stop();
            }
        }
        #endregion

        #region Click handlers
        /// <summary>
        /// De DispatcherTimer, changeMonsterTimer, wordt geïnitialiseerd.
        /// De startknop wordt disabled.
        /// De status isGameRunning wordt aangepast.
        /// De strafpunten worden terug op nul gezet.
        /// De startafbeelding wordt klaargezet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            _changeMonsterTimer.Start();
            startButton.IsEnabled = false;
            _penaltyCounter = 0;
            _isGameRunning = true;

            SetStartImage();
        }

        /// <summary>
        /// <para>GameImage_MouseDown behandelt het klikken op de afbeelding.</para>
        /// 
        /// <para>De millisecondsTimer wordt gestopt indien de afbeelding een
        /// monster is. Verder wordt de eindscore getoond in een messagebox,
        /// wordt de status van isGameRunning aangepast
        /// en kan de speler zijn of haar naam ingeven indien er een
        /// nieuw record is gehaald.</para>
        /// 
        /// <para>Indien de afbeelding geen monster is, dan wordt er een
        /// strafpunt aangerekend.</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gameImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_randomIndex == MonsterIndex && _isGameRunning)
            {
                _reactionTimer.Stop();
                _changeMonsterTimer.Stop();
                startButton.IsEnabled = true;
                _isGameRunning = false;
                SetHighScores((_milliseconds + _penaltyCounter * 100));

                //if (snelsteTijd > (milliseconds + penaltyCounter * 100))
                //{
                //    snelsteTijd = milliseconds + penaltyCounter * 100;
                //    SnelsteTijdTextBlock.Text = snelsteTijd.ToString();
                //    MessageBox.Show($"Je reageerde in {milliseconds} milliseconden en had {penaltyCounter} strafpunten",
                //    "Hoogste score",
                //    MessageBoxButton.OK,
                //    MessageBoxImage.Information);
                //    string naamSpeler = Interaction.InputBox(
                //        "Proficiat een nieuwe hoogste score! Geef je naam in",
                //        "Naam Speler", "Player 1", 500);
                //    SnelsteNaamTextBlock.Text = naamSpeler;
                //}
                //else
                //{
                //    MessageBox.Show($"Je reageerde in {milliseconds} milliseconden en had {penaltyCounter} strafpunten",
                //    "Resultaat",
                //    MessageBoxButton.OK,
                //    MessageBoxImage.Information);
                //}
            }
            else
            {
                _penaltyCounter++;
            }
        }

        private void SetHighScores(int time)
        {
            int idx = -1;
            for (int i = 0; i < _highScoresTimes.Length - 1; i++)
            {
                if (time < _highScoresTimes[i] || _highScoresTimes[i] == 0)
                {
                    idx = i;
                    break;
                }
            }
            if (idx != -1)
            {
                string namePlayer = Interaction.InputBox(
                        "Proficiat een nieuwe hoogste score! Geef je naam in",
                        "Naam Speler", "Player 1", 500);
                int prevTime = time;
                int tempTime;
                string prevName = namePlayer;
                string tempName;
                for (int i = idx; i < _highScoresTimes.Length; i++)
                {
                    tempTime = _highScoresTimes[i];
                    tempName = _highScoresNames[i];
                    _highScoresTimes[i] = prevTime;
                    _highScoresNames[i] = prevName;
                    prevName = tempName;
                    prevTime = tempTime;
                }
            }
            fastestNameTextBlock.Text = "";
            for (int i = 0; i < _highScoresNames.Length; i++)
            {
                fastestNameTextBlock.Text += $"{_highScoresNames[i]}\t{_highScoresTimes[i]}\r\n";
            }
        }
        #endregion
    }
}
