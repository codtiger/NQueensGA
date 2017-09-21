using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
using NQueensGA.Annotations;

namespace NQueensGA
{
 
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,INotifyPropertyChanged
    {


        public int N { get { return n; }set { n = value; OnPropertyChanged(nameof(N));} }
        private int n = 16;
        private int iterations = 200;
        public int Iterations { get { return iterations; }set { iterations = value;
            OnPropertyChanged(nameof(Iterations));
        } }
        private double crossRate = 0.6;
        public double CrossRate { get { return crossRate; }set { crossRate = value;OnPropertyChanged(nameof(CrossRate)); } }
        private double muteRate = 0.05;
        public double MuteRate { get { return muteRate; } set { muteRate = value; OnPropertyChanged(nameof(MuteRate));} }
        private int initpop = 100;
        public int InitPop { get { return initpop; }set { initpop = value;OnPropertyChanged(nameof(InitPop)); } }
        private Dispatcher dispatcher;
      

        public MainWindow()
        {
            InitializeComponent();
            dispatcher = UniformGrid.Dispatcher;
            this.DataContext = this;
            UniformGrid.DataContext = this;
        }

        public void DrawAndStart()
        {
            UniformGrid.Children.Clear();
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    if ((i + j) % 2 == 0)
                        UniformGrid.Children.Add(new Canvas
                        {
                            Background = new SolidColorBrush(Color.FromRgb(255, 255, 255))
                            
                        });
                        
                    else
                        UniformGrid.Children.Add(new Canvas
                        {
                            Background = new SolidColorBrush(Color.FromRgb(0, 0, 0))
                        });
            BackgroundWorker worker=new BackgroundWorker();
            worker.DoWork += (sender, args) =>
            {
                Genetics genetics = new Genetics(InitPop, CrossRate, MuteRate, N);
                Task<Chromosome> task = new Task<Chromosome>(() => genetics.Start(Iterations));
                task.Start();
                task.Wait();
                Chromosome result = task.Result;
                Debug.WriteLine(result);
                var dim = result.Dimension;
                for (int i = 0; i < result.Genes.Length; i++)
                    dispatcher.Invoke(
                        () =>
                        {
                            (UniformGrid.Children[i*dim+result.Genes[i]] as Canvas).Background =
                                new SolidColorBrush(Color.FromRgb(10, 150, 10));
                        });
            };
            worker.RunWorkerAsync();
        }
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                int.TryParse(IterTextBox.Text, out iterations);
                double.TryParse(CrossOverTextBox.Text, out crossRate);
                double.TryParse(MutationTextBox.Text, out muteRate);
                int.TryParse(DimTextBox.Text, out n);
                UniformGrid.Rows = n;
                UniformGrid.Columns = n;
                int.TryParse(InitPopTextBox.Text, out initpop);
                if (crossRate >= 1) throw new Exception("Probability less than 1 expected");
                if(muteRate>=1) throw new Exception("Probability less than 1 expected");
                DrawAndStart();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Input is not in the correct format!", "Error", MessageBoxButton.OK,MessageBoxImage.Error);
            }
          
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            
        }
        
    }
}
