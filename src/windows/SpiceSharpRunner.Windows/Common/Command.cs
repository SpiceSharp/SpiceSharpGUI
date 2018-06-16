using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpiceSharpRunner.Windows.Common
{
    public class Command : ICommand
    {
        private Action<object> ExecuteAction { get; set; }
        private Predicate<object> CanExecutePredicate { get; set; }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public Command(Action<object> executeAction)
           : this(executeAction, p => true)
        {
        }

        public Command(Action<object> executeAction, Predicate<object> canExecutePredicate)
        {
            this.ExecuteAction = executeAction;
            this.CanExecutePredicate = canExecutePredicate;
        }

        public bool CanExecute(object parameter)
        {
            return this.CanExecutePredicate == null || this.CanExecutePredicate(parameter);
        }

        public void Execute(object parameter)
        {
            ExecuteAction?.Invoke(parameter);
        }
    }
}
