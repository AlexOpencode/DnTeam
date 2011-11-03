using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DnTeamData.Models;

namespace DnTeam.Attributes
{
    public class TechnologySkillsAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var skills = (List<Specialty>)value;
            
            if (skills.Where(o=>o.Level > 0).Count() <= 0)
            {
                return false;
            }

            return true;
        }
    }
}