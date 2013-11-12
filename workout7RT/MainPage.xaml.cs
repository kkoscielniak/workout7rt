using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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

        private Stream stream;
        //private SoundEffect soundTick;
        //private SoundEffect soundBeep;
        //private SoundEffect soundSwitch;
        //
        public MainPage()
        {
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
                "side plank"
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
                "exercise.side-plank.png"
            };

            currentActivity = Activity.GettingReady;

            /* Prepare dispatcher Timer */
            if (this.dispatcherTimer == null)
            {
                this.dispatcherTimer = new DispatcherTimer();
                this.dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                this.dispatcherTimer.Tick += new EventHandler<object>(dispatcherTimer_Tick);
            }
        }

        ~MainPage()
        {
            // stream.Close();
            this.dispatcherTimer = null;
            // to do - zezwól  na gaszenie ekranu
        }
        
        private void dispatcherTimer_Tick(object sender, object e)
        {
            TimeSpan tmps = (endTime - DateTime.Now);
            //this.lTimer.Text = String.Format("00:{1:00}", tmps.Minutes, tmps.Seconds + 1);

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
            // to-do:
            // lock the lockscreen during workout (?)
            

            if (this.dispatcherTimer != null)
            {
                switch (currentActivity)
                {
                    /* In case of practitioner is getting ready to workout just 
                     * set timespan to 5 seconds and label text.
                     */
                    case Activity.GettingReady:
                        this.timeSpan = new TimeSpan(0, 0, 5);
                        this.currentExerciseLabel.Text = "get ready!";
                        /*this.image.Source = new BitmapImage(new Uri("Images/rest.png",
                            UriKind.Relative));
                        this.lTimer.Text = "00:05";*/
                        beepEffect.Play();
                        break;

                    /* In case of activity is a workout set the timespan to 30 secs, label text
                     * and load image from Images/ folder. 
                     */
                    case Activity.Exercise:
                        this.timeSpan = new TimeSpan(0, 0, 30);

                        this.currentExerciseLabel.Text = this.exerciseNames[this.exerciseIndex];
                        /*this.image.Source = new BitmapImage(new Uri("Images/" + this.imageNames[this.exerciseIndex],
                            UriKind.Relative));
                        this.lTimer.Text = "00:30";*/

                        beepEffect.Play();
                        break;

                    /* In case of activity is a rest set the timespan to 10 secs 
                     * and the label to combined "rest" word [...]
                     */
                    case Activity.Rest:
                        // this.lExercise.FontSize = 72;
                        if (this.exerciseIndex < TOTAL_EXERCISES - 1)
                        {
                            this.timeSpan = new TimeSpan(0, 0, 10);
                            currentExerciseLabel.Text = "rest";

                            /* [...] and next exercise name with lower font.
                             */
                            this.currentExerciseLabel.Inlines.Add(new Run
                            {
                                Text = " \u2192" +
                                    this.exerciseNames[this.exerciseIndex + 1],
                                FontSize = 32
                            });
                            /*this.image.Source = new BitmapImage(new Uri("Images/rest.png",
                            UriKind.Relative));
                            this.lTimer.Text = "00:10";*/
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
            /*if (PhoneApplicationService.Current.UserIdleDetectionMode != IdleDetectionMode.Enabled)
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Enabled;*/

            this.dispatcherTimer.Stop();
            this.timerLocked = true;

            //this.lTimer.Text = "";
            currentExerciseLabel.Text = "";
            currentExerciseLabel.Inlines.Add(new Run { Text = "you've finished!" });
            //this.image.Source = new BitmapImage(new Uri("Images/main.png", UriKind.Relative));

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
