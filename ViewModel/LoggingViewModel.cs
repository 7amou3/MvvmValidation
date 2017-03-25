using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using proApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace proApp.ViewModel
{
    public class LoggingViewModel 
    {
        private readonly LoggingModel _logingModel;
        private RelayCommand _loggingCommand;

        public LoggingViewModel()
        {
            _loggingCommand = new RelayCommand(checkCredentials, canExecuteLogginForm);
            _logingModel = new LoggingModel();

            // subscribe to the change of the Model in order to validate the form, because 'canExecute' of the 'RelayCommand only execute
            // once, we should refresh it by calling RaiseCanExecuteChanged()
            _logingModel.FormIsValid += (o, e) => _loggingCommand.RaiseCanExecuteChanged();
        }
        
        private bool canExecuteLogginForm()
        {
            return !_logingModel.HasErrors;
        }

        #region bindable properties exposed to the 'LoginView' view
        /// <summary>
        /// LoggingModel's properties
        /// </summary>
        public LoggingModel LoggingProp
        {
            get { return _logingModel; }
        }

        private void checkCredentials()
        {
            if(canExecuteLogginForm())
                MessageBox.Show(_logingModel.UserName);
        }
        
        public RelayCommand loggingCommand
        {
            get { return _loggingCommand; }
        }
        #endregion
    }
}
