namespace BX.Service
{
    public interface IJwtService
    {
        /// <summary>
        /// 建立JWT
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="IsUpdatExp">是否更新JWT時效</param>
        /// <returns></returns>
        string ResponseJWT(string Account);
    }
}