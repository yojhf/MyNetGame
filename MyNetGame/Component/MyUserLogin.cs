using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.ComponentModel.DataAnnotations;

namespace MyNetGame.Component
{
    public class MyUserLogin
    {
        //Empty
    }

    /// <summary>
    /// 로그인 기능 정의
    /// </summary>
    public interface IUserLoginRepository
    {
        UserLoingResult LoginUser(UserLogin userLogin);
    }

    /// <summary>
    /// 로그인 기능 구현
    /// </summary>
    public class UserLoginRepository : IUserLoginRepository
    {
        private IConfiguration _config;
        private IDbConnection db;

        public UserLoginRepository(IConfiguration config)
        {
            _config = config;
            db = new SqlConnection(config.GetSection("ConnectionStrings")
                .GetSection("DefaultConnection").Value);
        }

        public UserLoingResult LoginUser(UserLogin userLogin)
        {
            string sql = "usp_Login @UserId, @Password";
            var result = db.Query<int>(sql, userLogin).Single();

            UserLoingResult loingResult = new UserLoingResult
            {
                Protocol = -userLogin.Protocol,
                Result = result,
                UserId = userLogin.UserId
            };

            return loingResult;
        }
    }

    /// <summary>
    /// 로그인 Web-Api
    /// </summary>
    [Route("api/[controller]")]
    public class UserLoginServicesController : ControllerBase
    {
        private IUserLoginRepository _repository;

        public UserLoginServicesController(IUserLoginRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public IActionResult Post([FromBody] UserLogin userLogin)
        {
            if(userLogin == null)
            {
                return BadRequest();
            }

            try
            {
                UserLoingResult userLoingResult =  _repository.LoginUser(userLogin);
                return Ok(userLoingResult);
            }
            catch
            {
                UserLoingResult result = new UserLoingResult
                {
                    Protocol = -userLogin.Protocol,
                    Result = 2,
                    UserId = userLogin.UserId
                };
                return Ok(result);
            }
        }
    }


    /// <summary>
    /// 유저 로그인 요청 : protocol 1101
    /// </summary>
    public class UserLogin
    {
        public int Protocol { get; set; }
        [MaxLength(20)]
        public string UserId { get; set; }
        [MaxLength(20)]
        public string Password { get; set; }
    }

    /// <summary>
    /// 유저 로그인 응답 : protocol -1101
    /// </summary>
    public class UserLoingResult
    {
        public int Protocol { get; set; }
        public int Result { get; set; }
        public string UserId { get; set; }
    }

}
