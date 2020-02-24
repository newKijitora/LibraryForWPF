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
        private readonly Action<object> _execute1;
        private readonly Action _execute2;
        private readonly Func<object, bool> _canExecute1;
        private readonly Func<bool> _canExecute2;

        /// <summary>
        /// <see cref="DelegateCommand"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        public DelegateCommand(Action<object> action, Func<object, bool> func)
        {
            _execute1 = action ?? throw new ArgumentNullException();
            _canExecute1 = func ?? throw new ArgumentNullException();
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
            _execute2 = action ?? throw new ArgumentNullException();
            _canExecute2 = func ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// <see cref="DelegateCommand"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        public DelegateCommand(Action action) : this(action, () => true) { }

        public void Execute(object parameter)
        {
            _execute1?.Invoke(parameter);
        }

        public void Execute()
        {
            _execute2?.Invoke();
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute1 != null)
                return _canExecute1(parameter);

            if (_canExecute2 != null)
                return _canExecute2();

            throw new InvalidOperationException();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
