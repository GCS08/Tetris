using System;
using System.Collections.Generic;
using System.Text;

namespace Tetris.Interfaces
{
    public interface IUser
    {
        #region Public Methods
        public abstract Task<bool> Login();
        public abstract Task<bool> Register();
        public abstract void SignOut();
        public abstract Task ResetPassword();
        public abstract bool CanLogin();
        public abstract bool CanRegister(string repeatPassword);
        public abstract void Reset();
        #endregion
    }
}
