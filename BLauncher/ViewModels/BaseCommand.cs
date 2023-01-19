using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BLauncher.ViewModels;

public class BaseCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly bool noParams = true;

        private readonly Action execute;
        private readonly Func<bool> canExecute;

        private readonly Action<object> executeParam;
        private readonly Func<object, bool> canExecuteParam;

        public void OnCanExecuteChanged(object @this)
        {
            CanExecuteChanged?.Invoke(@this, EventArgs.Empty);
        }
        
        public BaseCommand(Action execute, Func<bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute ?? (() => true);
        }

        //public BaseCommand(Func<Task> execute, Func<bool> canExecute = null)
        //{
        //    //TODO add async handler
        //    //this.execute = execute;
        //    this.canExecute = canExecute ?? (() => true);
        //}

        public BaseCommand(Action<object> executeParam, Func<object, bool> canExecuteParam = null)
        {
            this.executeParam = executeParam;
            this.canExecuteParam = canExecuteParam ?? ((_) => true);
            noParams = false;
        }
        

        public bool CanExecute(object parameter) => noParams ? canExecute.Invoke() : canExecuteParam.Invoke(parameter);

        public void Execute(object parameter)
        {

            if(noParams)
            {
                execute?.Invoke();
            }
            else
            {
                executeParam?.Invoke(parameter);
            }
        }
    }
