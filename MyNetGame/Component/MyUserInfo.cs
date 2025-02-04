using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.ComponentModel.DataAnnotations;

namespace MyNetGame.Component
{
    public class MyUserInfo
    {
        //Empty
    }

    /// <summary>
    /// 유저 정보 가져오기 기능 정의
    /// </summary>
    public interface IUserInfoRepository
    {
        UserInfoResult GetUserInfo(UserInfo userInfo);
    }

    /// <summary>
    /// 유저 정보 가져오기 기능 구현
    /// </summary>
    public class UserInfoRepository : IUserInfoRepository
    {
        private IConfiguration _config;
        private IDbConnection db;

        public UserInfoRepository(IConfiguration config)
        {
            _config = config;
            db = new SqlConnection(config.GetSection("ConnectionStrings")
                .GetSection("DefaultConnection").Value);
        }

        public UserInfoResult GetUserInfo(UserInfo userInfo)
        {
            string sql = "usp_UserInfo @UserId";
            User resultUser = db.Query<User>(sql, userInfo).Single();

            if(resultUser != null)
            {
                return new UserInfoResult
                {
                    Protocol = -userInfo.Protocol,
                    Result = 0,
                    UserId = resultUser.UserId,
                    Level = resultUser.Level,
                    Gold = resultUser.Gold
                };
            }
            else
            {
                return new UserInfoResult
                {
                    Protocol = -userInfo.Protocol,
                    Result = 1,
                    UserId = userInfo.UserId,
                    Level = 0,
                    Gold = 0
                };
            }
        }
    }

    /// <summary>
    /// 유저 정보 가져오기 Web-Api 구현
    /// </summary>
    [Route("api/[controller]")]
    public class UserInfoServicesController : ControllerBase
    {
        private IUserInfoRepository _repository;

        public UserInfoServicesController(IUserInfoRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public IActionResult Post([FromBody] UserInfo userInfo)
        {
            if(userInfo == null)
            {
                return BadRequest();
            }

            try
            {
                UserInfoResult infoResult = _repository.GetUserInfo(userInfo);
                return Ok(infoResult);
            }
            catch
            {
                UserInfoResult result = new UserInfoResult
                {
                    Protocol = -userInfo.Protocol,
                    Result = 1,
                    UserId = userInfo.UserId,
                    Level = 0,
                    Gold = 0
                };
                return Ok(result);
            }
        }
    }

    /// <summary>
    /// 유저정보 가져오기 요청 : protocol 1103
    /// </summary>
    public class UserInfo
    {
        public int Protocol { get; set; }
        [MaxLength(20)]
        public string UserId { get; set; }
    }

    /// <summary>
    /// 유저정보 가져오기 응답 : protocol -1103
    /// </summary>
    public class UserInfoResult
    {
        public int Protocol { get; set; }
        public int Result { get; set; }
        public string UserId { get; set; }
        public int Level { get; set; }
        public int Gold { get; set; }
    }
}
