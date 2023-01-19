using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BLauncher.ViewModels;


public class AsyncBaseCommand : ICommand
{
    public event EventHandler CanExecuteChanged;
    private readonly bool noParams = true;

    private readonly Func<Task> execute;
    private bool isRunning = false;
    private readonly Func<bool> canExecute;

    public void OnCanExecuteChanged(object @this)
    {
        CanExecuteChanged?.Invoke(@this, EventArgs.Empty);
    }

    public AsyncBaseCommand(Func<Task> execute, Func<bool> canExecute = null)
    {
        //TODO add async handler
        this.execute = execute;
        this.canExecute = canExecute ?? (() => !isRunning);
    }



    public bool CanExecute(object parameter) => canExecute.Invoke();

    public async void Execute(object parameter)
    {
        if (noParams)
        {
            isRunning = true;
            CommandManager.InvalidateRequerySuggested();
            await execute?.Invoke();
            isRunning = false;
            CommandManager.InvalidateRequerySuggested();
        }
    }
}

public class AsyncBaseCommand<T> : ICommand
{
    public event EventHandler CanExecuteChanged;
    private readonly bool noParams = true;

    private readonly Func<Task<T>> execute;
    private bool isRunning = false;
    private readonly Func<bool> canExecute;

    public void OnCanExecuteChanged(object @this)
    {
        CanExecuteChanged?.Invoke(@this, EventArgs.Empty);
    }

    public AsyncBaseCommand(Func<Task<T>> execute, Func<bool> canExecute = null)
    {
        //TODO add async handler
        this.execute = execute;
        this.canExecute = canExecute ?? (() => !isRunning);
    }



    public bool CanExecute(object parameter) => canExecute.Invoke();

    public async void Execute(object parameter)
    {
        if (noParams)
        {
            isRunning = true;
            CommandManager.InvalidateRequerySuggested();
            await execute?.Invoke();
            isRunning = false;
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
