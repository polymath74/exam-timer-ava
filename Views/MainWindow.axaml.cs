using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using exam_timer_ava.ViewModels;

namespace exam_timer_ava.Views;

public partial class MainWindow : Window
{
    MainWindowViewModel? ViewModel => DataContext as MainWindowViewModel;

    public MainWindow()
    {
        InitializeComponent();

        this.Activated += (sender, e) => StartTimer();
        // this.Deactivated += (sender, e) => stop.Cancel();
    }

    CancellationTokenSource stop = new();

    private void StartTimer()
    {
        Task.Run(async () =>
        {
            using PeriodicTimer timer = new(TimeSpan.FromSeconds(0.15));
            while (!stop.IsCancellationRequested)
            {
                var result = await timer.WaitForNextTickAsync(stop.Token);
                if (result)
                    Dispatcher.UIThread.Post(UpdateTime);
                else
                    break;
            }
        });
    }

    string lastActivity = string.Empty;

    private void UpdateTime()
    {
        if (ViewModel == null)
            return;

        TimeSpan now = DateTime.Now.TimeOfDay;

        if (now < (ViewModel.HasEffectivePreparation ? ViewModel.PreparationStartTime : ViewModel.WorkingStartTime))
        {
            if (!string.IsNullOrEmpty(lastActivity))
            {
                ViewModel.CurrentActivity = string.Empty;
                ViewModel.RemainingTime = string.Empty;
                mainPanel.Classes.Remove("preparation");
                mainPanel.Classes.Remove("working");
                mainPanel.Classes.Remove("finished");
                lastActivity = string.Empty;
            }
        }
        else if (now < ViewModel.WorkingStartTime)
        {
            ViewModel.RemainingTime = ViewModel.WorkingStartTime.Subtract(now).ToString(@"hh\:mm\:ss");
            ViewModel.CurrentActivity = ViewModel.PreparationName;
            if (lastActivity != "preparation")
            {
                mainPanel.Classes.Add("preparation");
                mainPanel.Classes.Remove("working");
                mainPanel.Classes.Remove("finished");
                lastActivity = "preparation";
            }
        }
        else if (now < ViewModel.FinishTime)
        {
            ViewModel.RemainingTime = ViewModel.FinishTime.Subtract(now).ToString(@"hh\:mm\:ss");
            if (lastActivity != "working")
            {
                ViewModel.CurrentActivity = "Working";
                mainPanel.Classes.Remove("preparation");
                mainPanel.Classes.Add("working");
                mainPanel.Classes.Remove("finished");
                lastActivity = "working";
            }
        }
        else
        {
            if (lastActivity != "finished") {
                ViewModel.CurrentActivity = "finished";
                ViewModel.RemainingTime = string.Empty;
                mainPanel.Classes.Remove("preparation");
                mainPanel.Classes.Remove("working");
                mainPanel.Classes.Add("finished");
                lastActivity = "finished";
            }
        }
    }
}