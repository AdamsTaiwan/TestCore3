using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AppBase0500
{
    public class NotifyBase : INotifyPropertyChanged
    {
        #region INotify
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            /*
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }*/
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = "")
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            if (propertyName != null)
            {
                OnPropertyChanged(propertyName);
            }
            else
            {
                throw new Exception("propertyName must not be null value");
            }

            return true;
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the property that has a new value</typeparam>
        /// <param name="propertyExpression">A Lambda expression representing the property that has a new value.</param>
        //protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        //{
        //    var propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
        //    this.OnPropertyChanged(propertyName);
        //}
        #endregion

    }
}
