using System;
namespace HearSay.Models
{
    public class User
    {
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
