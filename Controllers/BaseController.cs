using FashionAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.EntityFrameworkCore;
using FashionAPI.Databases.FashionDB;
using FashionAPI.Configuaration;
using FashionAPI.Enums;
using Newtonsoft.Json.Linq;

namespace FashionAPI.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    [SwaggerTag("Base Controller")]
    public class BaseController : Controller
    {
        protected TokenInfo getTokenInfo(DBContext _context)
        {
            if (HttpContext.Request.Headers.ContainsKey("Authorization") &&
            HttpContext.Request.Headers["Authorization"][0].StartsWith("Bearer "))
            {
                var token = HttpContext.Request.Headers["Authorization"][0]
                    .Substring("Bearer ".Length);

                var result = TokenManager.getTokenInfoByToken(token);

                if (result != null)
                    return result;

                //load token tu database
                var session = _context.Sessions.Include(x => x.UserUu).Where(x => x.Uuid == token && x.Status == 0).SingleOrDefault();

                if (session != null)
                {
                    var _token = new TokenInfo()
                    {
                        Token = session.Uuid,
                        UserName = session.UserUu.Email,
                        UserUuid = session.UserUuid
                    };

                    TokenManager.addToken(_token);

                    return _token;
                }
            }

            return null;
        }

        protected void logOutAllSession(DBContext _context, string userUuid)
        {
            //LogOut khoi session
            var session = _context.Sessions.Where(x => x.UserUuid == userUuid && x.Status == 0).ToList();

            if (session != null && session.Count > 0)
            {
                foreach (var item in session)
                {
                    if (item.Status == 0)
                    {
                        item.TimeLogout = DateTime.Now;
                        item.Status = 1;
                    }
                }
                _context.SaveChanges();
            }
        }

        protected TokenInfo validateToken(DBContext _context)
        {
            var token = getTokenInfo(_context);
            if (token != null)
            {
                if (token.IsExpired())
                {
                    logOutAllSession(_context, token.UserUuid);
                    return null;
                }
                else
                {
                    token.ResetExpired();

                    return token;
                }
            }

            return null;

        }
        protected void CheckLockedAccount(TokenInfo validToken, DbSet<User> user)
        {
            var locked = user.Where(x => x.Uuid == validToken.UserUuid).Select(s => s.Status).FirstOrDefault();
            if (locked == 0)
            {
                throw new ErrorException(ErrorCode.LOCKED_ACCOUNT);
            }
        }
    }

}
