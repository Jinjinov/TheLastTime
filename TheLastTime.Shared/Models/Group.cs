using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheLastTime.Shared.Models
{
    public class Group : IEntity<Group>
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Color { get; set; } = string.Empty;

        [Required]
        public string Icon { get; set; } = string.Empty;

        public List<Category> CategoryList = new List<Category>();

        public void CopyTo(Group group)
        {
            group.Description = Description;
            group.Color = Color;
            group.Icon = Icon;
        }
    }
}
