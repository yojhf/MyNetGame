using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Dapper;

namespace MyNetGame.Component
{
    public class MyUser
    {
        //Empty
    }

    /// <summary>
    /// 리포지토리 인터페이스 - 기능 정의
    /// </summary>
    public interface IUserRepository
    {
        User Add(User user);
        List<User> GetAll();
        User GetByUserId(string UserId);
        User Upadte(User user);
        void Remove(string UserId);
    }

    /// <summary>
    /// 리포지토리 클래스 - 기능 구현
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private IConfiguration _config;
        private IDbConnection db;

        public UserRepository(IConfiguration config)
        {
            _config = config;
            db = new SqlConnection(config.GetSection("ConnectionStrings")
                .GetSection("DefaultConnection").Value);
        }

        public User Add(User user)
        {
            string sql = @"INSERT INTO userTbl (userId, password, mobile, level, health, gold, mDate) VALUES (@UserId, @Password, @Mobile, @Level, @Health, @Gold, GETDATE());
                            SELECT * FROM userTbl WHERE userId = @UserId;";
            return db.Query<User>(sql, user).Single();
        }

        public List<User> GetAll()
        {
            string sql = "SELECT * FROM userTbl";
            return db.Query<User>(sql).ToList();
        }

        public User GetByUserId(string UserId)
        {
            string sql = "SELECT * FROM userTbl WHERE userId = @UserId";
            return db.Query<User>(sql, new {UserId = UserId}).Single();
        }

        public void Remove(string UserId)
        {
            string sql = "DELETE FROM userTbl WHERE userId = @UserId";
            db.Execute(sql, new {UserId = UserId});
        }

        public User Upadte(User user)
        {
            string sql = @"UPDATE userTbl SET level = level + 1, gold = @Gold WHERE userId = @UserId;
                         SELECT * FROM userTbl WHERE userId = @UserId;";
            return db.Query<User>(sql, user).Single();
        }
    }

    /// <summary>
    /// Web-Api 구현
    /// </summary>
    [Route("api/[controller]")]
    public class UserServicesController : ControllerBase
    {
        private IUserRepository _repository;

        public UserServicesController(IUserRepository repository)
        {
            _repository = repository;
        }

        //api/UserServices
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var users = _repository.GetAll();
                if (users == null || users.Count == 0)
                {
                    return NotFound("유저 데이터가 없습니다");
                }
                return Ok(users);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("{UserId}")]
        public IActionResult Get(string UserId)
        {
            try
            {
                var user = _repository.GetByUserId(UserId);
                if (user == null)
                {
                    return NotFound($"{UserId} 유저 데이터가 없습니다");
                }
                return Ok(user);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            try
            {
                var model = _repository.Add(user);
                if (model == null)
                {
                    return BadRequest("유저 추가에 실패 했습니다");
                }
                return Ok(model);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut("{UserId}")]
        public IActionResult Put(string UserId, [FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            try
            {
                var OldUser = _repository.GetByUserId(UserId);
                if (OldUser == null)
                {
                    return NotFound($"{UserId} 유저 데이터가 없습니다");
                }

                user.UserId = UserId;
                var model = _repository.Upadte(user);
                return Ok(model);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpDelete("{UserId}")]
        public IActionResult Delete(string UserId)
        {
            try
            {
                var OldUser = _repository.GetByUserId(UserId);
                if (OldUser == null)
                {
                    return NotFound($"{UserId} 유저 데이터가 없습니다");
                }

                _repository.Remove(UserId);
                return NoContent();
            }
            catch
            {
                return BadRequest();
            }
        }
    }

    /// <summary>
    /// 모델 클래스 - 유저 정보
    /// </summary>
    public class User
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Mobile { get; set; }
        public int Level { get; set; }
        public int Health { get; set; }
        public int Gold { get; set; }
        public DateTime mDate { get; set; }
    }
}
