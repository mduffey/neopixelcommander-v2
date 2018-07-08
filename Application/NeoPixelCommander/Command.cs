using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NeoPixelCommander
{
    public class Command : ICommand
    {
        public Command(Func<object, bool> canExecute, Action<object> execute)
        {
            _canExecuteFunc = canExecute;
            _executeAction = execute;
        }

        private readonly Func<object, bool> _canExecuteFunc;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecuteFunc(parameter);
        }

        private readonly Action<object> _executeAction;

        public void Execute(object parameter)
        {
            _executeAction(parameter);
        }
    }
}
