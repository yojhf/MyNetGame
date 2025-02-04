using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.ComponentModel.DataAnnotations;

namespace MyNetGame.Component
{
    public class MyUserLevelup
    {
        //Empty
    }

    /// <summary>
    /// 레벨업 기능 정의
    /// </summary>
    public interface IUserLevelupRepository
    {
        UserLevelupResult LevelupUser(UserLevelup userLevelup);
    }

    /// <summary>
    /// 레벨업 기능 구현
    /// </summary>
    public class UserLevelupRepository : IUserLevelupRepository
    {
        private IConfiguration _config;
        private IDbConnection db;

        public UserLevelupRepository(IConfiguration config)
        {
            _config = config;
            db = new SqlConnection(config.GetSection("ConnectionStrings")
                .GetSection("DefaultConnection").Value);
        }

        public UserLevelupResult LevelupUser(UserLevelup userLevelup)
        {
            string sql = "usp_Levelup @UserId";
            var result = db.Query<int>(sql, userLevelup).Single();
            if(result > 0)  //레벨업 성공
            {
                return new UserLevelupResult
                {
                    Protocol = -userLevelup.Protocol,
                    Result = 0,
                    UserId = userLevelup.UserId,
                    Level = result
                };
            }
            else //레벨업 실패
            {
                return new UserLevelupResult
                {
                    Protocol = -userLevelup.Protocol,
                    Result = 1,
                    UserId = userLevelup.UserId,
                    Level = 0
                };
            }
        }
    }

    /// <summary>
    /// 유저 레벨업 Web-Api 구현
    /// </summary>
    [Route("api/[controller]")]
    public class UserLevelupServicesController : ControllerBase
    {
        private IUserLevelupRepository _repository;

        public UserLevelupServicesController(IUserLevelupRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public IActionResult Post([FromBody] UserLevelup userLevelup)
        {
            if(userLevelup == null)
            {
                return BadRequest();
            }

            try
            {
                UserLevelupResult levelupResult = _repository.LevelupUser(userLevelup);
                return Ok(levelupResult);
            }
            catch
            {
                UserLevelupResult result = new UserLevelupResult
                {
                    Protocol= -userLevelup.Protocol,
                    Result = 1,
                    UserId = userLevelup.UserId,
                    Level = 0
                };
                return Ok(result);
            }
        }
    }

    /// <summary>
    /// 유저 레벨업 요청 : protocol 1104
    /// </summary>
    public class UserLevelup
    {
        public int Protocol { get; set; }
        [MaxLength(20)]
        public string UserId { get; set; }
    }

    /// <summary>
    /// 유저 레벨업 요청 : protocol -1104
    /// </summary>
    public class UserLevelupResult
    {
        public int Protocol { get; set; }
        public int Result { get; set; }
        public string UserId { get; set; }
        public int Level { get; set; }
    }

}
