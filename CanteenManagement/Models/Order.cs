using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CanteenManagement.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        [ForeignKey("Employee")]
        public string EmployeeId { get; set; }
        public virtual ApplicationUser Employee { get; set; }
    }
    public class OrderHistory
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        [ForeignKey("Employee")]
        public string EmployeeId { get; set; }

        public virtual ApplicationUser Employee { get; set; }
        public virtual Order Order { get; set; }

    }
}
