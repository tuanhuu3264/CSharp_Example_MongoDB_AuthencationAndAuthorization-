using _Repository;
using AutoMapper;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessService
{
    public class AuthService
    {
        UserRepository _repository; 
        public AuthService()
        {
            string connection = "Project";
            string table = "Users";
            var config = new MapperConfiguration(cfg =>
            {
                Config.UserCreateMap(cfg);
            });
            var mapper = config.CreateMapper();
            _repository = new UserRepository(connection, table, mapper);
        }
        public UserDTO GetByEmail(string email)
        {
            var userDTO = _repository.LodUserByEmail(email);
            return userDTO; 
        }
         
    }
}
