using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KijitoraClassLibrary.ForWpf
{
    /// <summary>
    /// デリゲートコマンドです。
    /// </summary>
    public sealed class DelegateCommand : ICommand
    {
        private readonly Action<object> _parameterExecute;
        private readonly Action _nonParameterExecute;
        private readonly Func<object, bool> _canParameterExecute;
        private readonly Func<bool> _canNonParameterExecute;

        /// <summary>
        /// <see cref="DelegateCommand"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        public DelegateCommand(Action<object> action, Func<object, bool> func)
        {
            _parameterExecute = action ?? throw new ArgumentNullException();
            _canParameterExecute = func ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// <see cref="DelegateCommand"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        public DelegateCommand(Action<object> action) : this(action, (obj) => true) { }

        /// <summary>
        /// <see cref="DelegateCommand"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        public DelegateCommand(Action action, Func<bool> func)
        {
            _nonParameterExecute = action ?? throw new ArgumentNullException();
            _canNonParameterExecute = func ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// <see cref="DelegateCommand"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        public DelegateCommand(Action action) : this(action, () => true) { }

        public void Execute(object parameter)
        {
            if (_parameterExecute != null)
                _parameterExecute.Invoke(parameter);

            if (_nonParameterExecute != null)
                _nonParameterExecute.Invoke();
        }

        public bool CanExecute(object parameter)
        {
            if (_canParameterExecute != null)
                return _canParameterExecute(parameter);

            if (_canNonParameterExecute != null)
                return _canNonParameterExecute();

            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
