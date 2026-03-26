using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TranHuyenTran_2122110389.Models;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")] // Thêm dòng này
    public decimal Price { get; set; }

    public int Quantity { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    // Thêm trường này để lưu link ảnh
    [StringLength(1000)]
    [Display(Name = "Đường dẫn hình ảnh")]
    public string? ImageUrl { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;
}