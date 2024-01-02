using AutoMapper;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _Repository
{
    public class UserRepository : MongoCRUD
    {
        string _table;
        IMapper _mapper;
        public UserRepository(string database, string userTable, IMapper mapper) : base(database)
        {
            _mapper = mapper;
            _table = userTable;
        }
        public UserDTO LoadUserById(Guid id)
        {
            var user = LoadRecordById<User>(_table, id);
            var userDTO = _mapper.Map<User, UserDTO>(user);
            return userDTO;
        }
        public List<UserDTO> LoadUsers ()
        {
            var users = LoadRecord<User>(_table);
            var userDTOs = users.Select(u=>_mapper.Map<User, UserDTO>(u)).ToList();
            return userDTOs;
        }
        public UserDTO LodUserByEmail(string email)
        {
            var user = LoadRecordByField<User>(_table, "Email : " + email.ToLower().Trim()); 
            var userDTO = _mapper.Map<User,UserDTO>(user);
            return userDTO;
        }
    }
}
