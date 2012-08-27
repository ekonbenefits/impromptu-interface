impromptu-interface.mvvm http://code.google.com/p/impromptu-interface/

A Lightweight Dynamic MVVM View Model(.net4 & silverlight4 & silverlight5). Removes boilerplate code for MVVM, makes it practical to use MVVM in small apps.

Copyright 2011-2012 Ekon Benefits
Apache Licensed: http://www.apache.org/licenses/LICENSE-2.0

Authors:
Jay Tuley jay+code@tuley.name
Jonathan Peppers jonathan.peppers+mvvm@gmail.com

Usage:
    Just subclass ImpromptuViewModel and set your properties using Dynamic.PropertyName. Your commands need to be 'void CommandName(object parameter)' and then they can be bound with the path Command.CommandName and optionally have a method 'bool CanCommandName(object parameter)'

Has IOC support for MEF, Unity, NInject & TinyIOC

Samples can be found at http://code.google.com/p/impromptu-interface/source/browse/?repo=sample

Example:

Note: the below example is using WPF, for Silverlight below 5.0 it doesn't support dynamic properties so use indexers when binding, e.g. {Binding [Progress]} or {Binding Command[Search]} instead.

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

	    PropertyChanged += (sender, e) => Command.Search.RaiseCanExecuteChanged();


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