using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;

namespace Backend.Services
{
    public class HelpService : DatabaseConnection
    {

        public HelpService() : base()
        {
            
        }

        public List<HelpDetails> getHelpDetails()
        {
            List<HelpDetails> questionData = new List<HelpDetails>();

            query = @"SELECT * FROM HelpDetails";

            SqlDataReader reader = executeQuery();

            while (reader.Read())
            {
                questionData.Add(new HelpDetails { Id = reader.GetInt32(0), Question = reader.GetString(1), AnswerText = reader.GetString(2) });
            }
            return questionData;

        }

        public List<HelpDetails> getHelpDetailsAfterSearch(string search)
        {
            List<HelpDetails> questionData = new List<HelpDetails>();

            query = @"SELECT * FROM HelpDetails WHERE question LIKE '%" + search + @"%'";

            SqlDataReader reader = executeQuery();

            while (reader.Read())
            {
                questionData.Add(new HelpDetails { Id = reader.GetInt32(0), Question = reader.GetString(1), AnswerText = reader.GetString(2) });
            }
            return questionData;

        }

        public HelpDetails getHelpAnswer(int questionId)
        {
            HelpDetails answer = new HelpDetails();

            query = @"SELECT * FROM HelpDetails WHERE Id = " + questionId;

            SqlDataReader reader = executeQuery();

            while (reader.Read())
            {
                answer=new HelpDetails { Id = reader.GetInt32(0), Question = reader.GetString(1), AnswerText = reader.GetString(2) };
            }
            return answer;
        }
    }
}
