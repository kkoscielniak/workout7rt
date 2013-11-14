using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using workout7RT.Helpers;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace workout7RT
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int TOTAL_EXERCISES = 12;

        private TimeSpan timeSpan;
        private DispatcherTimer dispatcherTimer;
        private DateTime endTime;

        private bool timerLocked;   // simple method, but still does the job ;-) 

        private int exerciseIndex;
        private readonly string[] exerciseNames;
        private readonly string[] imageNames;
        private readonly bool[] switchSides;

        enum Activity
        {
            GettingReady,
            Exercise,
            Rest
        };

        private Activity currentActivity;

        public MainPage()
        {
            TileHelper.SetUpTiles(0);
            this.InitializeComponent();

            this.exerciseNames = new string[]{
                "jumping jacks",
                "wall-sit",
                "push up",
                "abdominal crunch",
                "step up on chair",
                "squat",
                "triceps dip",
                "plank",
                "high knees running",
                "lunge",
                "push up and rotation",
                "side plank",
                ""
            };

            this.switchSides = new bool[]
            {
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                false,
                true,
                true,
                true
            };

            this.imageNames = new string[] {
                "exercise.jacks.png", 
                "exercise.wall-sit.png",
                "exercise.push-up.png",
                "exercise.crunch.png",
                "exercise.step-up.png", 
                "exercise.squat.png", 
                "exercise.triceps-dip.png", 
                "exercise.plank.png",
                "exercise.running.png", 
                "exercise.lunge.png", 
                "exercise.push-up-rotate.png", 
                "exercise.side-plank.png",
                ""
            };

            currentActivity = Activity.GettingReady;

            /* Prepare dispatcher Timer */
            if (this.dispatcherTimer == null)
            {
                this.dispatcherTimer = new DispatcherTimer();
                this.dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                this.dispatcherTimer.Tick += new EventHandler<object>(dispatcherTimer_Tick);
            }

            nextExerciseLabel.Text = "\u2192 jumping jacks";
        }

        ~MainPage()
        {
            this.dispatcherTimer = null;
            // to do - zezwól  na gaszenie ekranu
        }
        
        private void dispatcherTimer_Tick(object sender, object e)
        {
            TimeSpan tmps = (endTime - DateTime.Now);
            timerLabel.Text = String.Format("00:{1:00}", tmps.Minutes, tmps.Seconds + 1);

            if (this.currentActivity == Activity.Exercise)
            {
                if (tmps.Seconds + 1 != 30 && tmps.Seconds + 1 != 0) 
                    tickEffect.Play();

                if (this.switchSides[this.exerciseIndex] == true && tmps.Seconds == 15) 
                    switchSidesEffect.Play();
            }

            if (tmps <= TimeSpan.Zero)
            {
                this.dispatcherTimer.Stop();

                switch (this.currentActivity)
                {
                    case Activity.GettingReady:
                        this.currentActivity = Activity.Exercise;
                        break;
                    case Activity.Exercise:
                        if (this.exerciseIndex < TOTAL_EXERCISES)
                            this.currentActivity = Activity.Rest;
                        break;
                    case Activity.Rest:
                        this.currentActivity = Activity.Exercise;
                        this.exerciseIndex++;
                        beepEffect.Play();
                        break;
                }

                if (this.exerciseIndex < TOTAL_EXERCISES)
                    this.NextActivity();
            }
        }

        private void NextActivity()
        {
            if (this.dispatcherTimer != null)
            {
                switch (currentActivity)
                {
                    /* In case of practitioner is getting ready to workout just 
                     * set timespan to 5 seconds and label text.
                     */
                    case Activity.GettingReady:
                        this.timeSpan = new TimeSpan(0, 0, 5);
                        
                        currentExerciseLabel.Text = "get ready!";
                        currentExerciseImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/main.png", UriKind.Absolute));
                        nextExerciseLabel.Text = "\u2192 jumping jacks";
                        nextExerciseImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/exercise.jacks.png", UriKind.Absolute));
                        timerLabel.Text = "00:05";
                        beepEffect.Play();
                        break;

                    /* In case of activity is a workout set the timespan to 30 secs, label text
                     * and load image from Images/ folder. 
                     */
                    case Activity.Exercise:
                        this.timeSpan = new TimeSpan(0, 0, 30);

                        currentExerciseLabel.Text = exerciseNames[this.exerciseIndex];
                        nextExerciseLabel.Text = "\u2192 " + exerciseNames[exerciseIndex + 1];
                        currentExerciseImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/" + this.imageNames[this.exerciseIndex],
                            UriKind.Absolute));
                        nextExerciseImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/" + this.imageNames[this.exerciseIndex+1],
                            UriKind.Absolute));
                        timerLabel.Text = "00:30";

                        beepEffect.Play();
                        break;

                    /* In case of activity is a rest set the timespan to 10 secs 
                     * and the label to combined "rest" word [...]
                     */
                    case Activity.Rest:
                        if (this.exerciseIndex < TOTAL_EXERCISES - 1)
                        {
                            this.timeSpan = new TimeSpan(0, 0, 10);
                            currentExerciseLabel.Text = "rest";

                            
                            currentExerciseImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/rest.png",
                            UriKind.Absolute));
                            timerLabel.Text = "00:10";
                            beepEffect.Play();
                        }
                        else Finish();
                        break;
                }

                /* set endTime to time after [timeSpan] seconds 
                 */
                if (timerLocked == false)
                {
                    this.endTime = DateTime.Now;
                    this.endTime = this.endTime.Add(timeSpan);
                    this.dispatcherTimer.Start();
                }
            }
        }

        private void Finish()
        {
            this.dispatcherTimer.Stop();
            this.timerLocked = true;

            timerLabel.Text = "Workout 7";
            currentExerciseLabel.Text = "you've finished!";
            currentExerciseImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Images/main.png", UriKind.Absolute));
            nextExerciseLabel.Text="";
            nextExerciseImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/fake.png", UriKind.Absolute));

            beepEffect.Play();
        }



        protected override void OnNavigatedTo(NavigationEventArgs e) {}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.timerLocked = false;
            this.exerciseIndex = 0;
            this.currentActivity = Activity.GettingReady;
            this.NextActivity();
        }
    }
}
