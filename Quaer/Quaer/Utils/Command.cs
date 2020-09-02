using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Quaer.Utils
{
    /// <summary>
    /// Implements the command class for interacting with Viewmodels
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Command<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        /// <summary>
        /// Called when conditions change, indicating whether the command can be executed
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="execute">Reference to the function</param>
        /// <param name="canExecute">Specifies whether the lambda function can be executed</param>
        public Command(Action<T> execute, Func<T, bool> canExecute = null)
        {
            this._execute = execute;
            this._canExecute = canExecute;
        }

        /// <summary>
        /// Resets the event of changing conditions 
        /// </summary>
        public void Refresh()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Determines whether the command can be executed
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>A bool</returns>
        public bool CanExecute(object parameter)
        {
            return this._canExecute == null || this._canExecute((T)parameter);
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public void Execute(object parameter)
        {
            this._execute((T)parameter);
        }
    }

    /// <summary>
    /// The Command class for the lambda functions
    /// </summary>
    public class Command : Command<object>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="execute">Reference to the lambda function</param>
        /// <param name="canExecute">Specifies whether the lambda function can be executed</param>
        public Command(Action execute, Func<bool> canExecute = null)
            : base(_ => execute(), _ => canExecute == null || canExecute())
        { }
    }
}
