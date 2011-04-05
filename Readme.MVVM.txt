impromptu-interface.mvvm http://code.google.com/p/impromptu-interface/

A Lightweight Dynamic MVVM View Model(.net4 &amp; silverlight4). Removes boilerplate code for MVVM, makes it practical to use MVVM in small apps.

Copyright 2011 Ekon Benefits
Apache Licensed: http://www.apache.org/licenses/LICENSE-2.0

Author:
Jay Tuley jay+code@tuley.name

Usage:
    Just subclass ImpromptuViewModel and set your properties using Dynamic.PropertyName. Your commands need to be 'void CommandName(object parameter)' and then they can be bound with the path Command.CommandName and optionally have a method 'bool CanCommandName(object parameter)'


Example:

Note: the below example is using WPF, for Silverlight since as of 4.0 it still doesn't support dynamic properties use indexers when binding, e.g. {Binding [Progress]} or {Binding Command[Search]} instead.

---
     <StackPanel DockPanel.Dock="Top" Orientation="Horizontal">
            <DatePicker SelectedDate="{Binding StartDate, Mode=TwoWay}" ></DatePicker>
            <Label>-</Label>
            <DatePicker SelectedDate="{Binding EndDate, Mode=TwoWay}"></DatePicker>
            <Label>Threshold:</Label>
            <TextBox Text="{Binding Limit, Mode=TwoWay}"></TextBox>
            <Button Command="{Binding Command.Search}">Search</Button>
            <ProgressBar Value="{Binding Progress}" Width="100" Height="20" ></ProgressBar>
     </StackPanel>
---	
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowVM();
        }
    }

---
    public class MainWindowVM:ImpromptuViewModel
    {
        public MainWindowVM()
        {
            Dynamic.StartDate = DateTime.Today.AddMonths(-3);
            Dynamic.EndDate = DateTime.Today;
            Dynamic.Limit = 4;
            Dynamic.Progress = 0d;
            Dynamic.Data = null;


            var tBackgroundWorker = new BackgroundWorker();
            Dynamic.BackgroundWorker = tBackgroundWorker;

            tBackgroundWorker.WorkerReportsProgress = true;
            tBackgroundWorker.DoWork += DoWork;
            tBackgroundWorker.ProgressChanged += (sender, e) => Dynamic.Progress = (double)e.ProgressPercentage;
            tBackgroundWorker.RunWorkerCompleted += (sender, e) =>  Dynamic.Data = e.Result;
        }  

	public void Search(object parameter)
        {

            Dynamic.Data = null; 
            Dynamic.Progress = 0d;
            Dynamic.BackgroundWorker.RunWorkerAsync(new { Dynamic.StartDate, Dynamic.EndDate, Dynamic.Limit });
        }

        public bool CanSearch(object parameter)
        {
            return !Dynamic.BackgroundWorker.IsBusy;
        }

	private static void DoWork(dynamic sender, DoWorkEventArgs e){ ... }
     }