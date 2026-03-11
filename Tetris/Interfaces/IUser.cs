namespace Tetris.Interfaces
{
    public interface IUser
    {
        #region Public Methods
        public Task<bool> Login();
        public Task<bool> Register();
        public void SignOut();
        public Task ResetPassword();
        public bool CanLogin();
        public bool CanRegister(string repeatPassword);
        public void Reset();
        #endregion
    }
}
