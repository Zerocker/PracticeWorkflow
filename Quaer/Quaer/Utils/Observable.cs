using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Quaer.Utils
{
    /// <summary>
    /// Base class for displayed objects
    /// </summary>
    public class Observable : INotifyPropertyChanged
    {
        /// <summary>
        /// Sets a new value for the field and notifies when this field changes
        /// </summary>
        /// <typeparam name="T">The type of field and value</typeparam>
        /// <param name="field">The reference of the field</param>
        /// <param name="value">A new value</param>
        /// <param name="propertyName">The name of the property</param>
        /// <returns>A bool</returns>
        protected bool Set<T> (ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Called when a property was changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// Calls the change of the property
        /// </summary>
        /// <param name="prop">The property name</param>
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
