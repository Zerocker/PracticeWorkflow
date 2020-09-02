using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Quaer.Utils;

namespace Quaer.Model
{
    /// <summary>
    /// Class for displaying objects in menus or tables with checkboxes
    /// </summary>
    public class Selectable<T> : Observable
    {
        private bool _selected;
        private T _object;
        private ICommand _command;

        /// <summary>
        /// Whether the object was selected
        /// </summary>
        public bool IsSelected
        {
            get => _selected;
            set => Set(ref _selected, value);
        }

        /// <summary>
        /// The display object
        /// </summary>
        public T Object
        {
            get => _object;
            set => Set(ref _object, value);
        }

        /// <summary>
        /// Reference to the command
        /// </summary>
        public ICommand Command
        {
            get => _command;
            set => Set(ref _command, value);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="object">The object instance</param>
        public Selectable(T @object) => Object = @object;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="object">The object instance</param>
        /// <param name="selected">Whether the object will be selected</param>
        public Selectable(T @object, bool selected)
        {
            IsSelected = selected;
            Object = @object;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="object">The object instance</param>
        /// <param name="selected">Whether the object will be selected</param>
        /// <param name="command">Reference to the command as function</param>
        public Selectable(T @object, bool selected, ICommand command)
        {
            Object = @object;
            IsSelected = selected;
            Command = command;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Selectable()
        {
            Object = default;
        }
    }
}
