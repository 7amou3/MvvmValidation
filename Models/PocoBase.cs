using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace proApp.Models
{
    /// <summary>
    /// A base class for all Models, provides properties validation throw data anotation, and notifications
    /// </summary>
    public class PocoBase : INotifyPropertyChanged, INotifyDataErrorInfo 
    {
        #region Fields

        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        /// <summary>
        /// dictionaire qui contient la liste des erreurs pour chaque propriété dans le model
        /// </summary>
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        #endregion

        #region Protected

        /// <summary>
        /// Gets the value of a property.
        /// </summary>
        /// <typeparam name="T">The type of the property value.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the property or default value if not exist.</returns>
        protected T GetValue<T>([CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Invalid property name", propertyName);
            }

            object value;
            if (!_values.TryGetValue(propertyName, out value))
            {
                value = default(T);
                _values.Add(propertyName, value);
            }

            return (T)value;
        }
        
        /// <summary>
        /// Sets the value of a property.
        /// </summary>
        /// <typeparam name="T">The type of the property value.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The property value.</param>
        protected void SetValue<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Invalid property name", propertyName);
            }

            _values[propertyName] = value;
            OnPropertyChanged(propertyName);
        }

        #endregion

        #region INotifyDataErrorInfo members

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        
        public IEnumerable GetErrors(string propertyName)
        {
            List<string> errorsForName;
            _errors.TryGetValue(propertyName, out errorsForName);
            return errorsForName;
        }
        
        public bool HasErrors
        {
            get { return _errors.Any(kv => kv.Value != null && kv.Value.Count > 0); }
        }
        
        protected void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

            // notify the subscribers (usualy your viewmodel) about the change
            FormIsValid?.Invoke(this, new EventArgs());
        }
        #endregion

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            ValidateForm();
        }
        #endregion

        #region Logic qui interagie avec INotifyDataErrorInfo 
        
        protected void ValidateForm()
        {
            
            var validationContext = new ValidationContext(this, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(this, validationContext, validationResults, true);
            
            foreach (var kv in _errors.ToList())
            {
                if (validationResults.All(r => r.MemberNames.All(m => m != kv.Key)))
                {
                    _errors.Remove(kv.Key);
                    OnErrorsChanged(kv.Key);
                }
            }

            var q = from r in validationResults
                    from m in r.MemberNames
                    group r by m into g
                    select g;

            foreach (var prop in q)
            {
                var messages = prop.Select(r => r.ErrorMessage).ToList();

                if (_errors.ContainsKey(prop.Key))
                {
                    _errors.Remove(prop.Key);
                }
                _errors.Add(prop.Key, messages);
                OnErrorsChanged(prop.Key);
            }

        }
        public event EventHandler FormIsValid;
        #endregion

    }
}
