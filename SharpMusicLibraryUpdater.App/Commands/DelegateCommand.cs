using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SharpMusicLibraryUpdater.App.Commands
{
    public class DelegateCommand : ICommand
    {
        private Func<object, bool> canExecute;
        private Action<object> executeAction;
        private bool canExecuteCache;

        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecute)
        {
            this.executeAction = executeAction;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            bool tmp = canExecute(parameter);
            if (canExecuteCache != tmp)
            {
                canExecuteCache = tmp;
                CanExecuteChanged?.Invoke(this, new EventArgs());
            }

            return canExecuteCache;
        }

        public void Execute(object parameter) => executeAction(parameter);

        public void RaiseCanExecuteChanged() =>
                CanExecuteChanged?.Invoke(this, new EventArgs());

    }
}
