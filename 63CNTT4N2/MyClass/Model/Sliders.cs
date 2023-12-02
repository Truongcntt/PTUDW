using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClass.Model
{
    [Table("Sliders")]
    public class Sliders    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên Slider không được để trống")]
        [Display(Name = "Tên Slider")]
        public string Name { get; set; }
        [Display(Name = "Liên kết")]
        public string URL { get; set; }
        [Display(Name = "Hình ảnh")]
        public string Img { get; set; }
        [Display(Name = "Sắp xếp")]
        public int? Order { get; set; }
        [Required(ErrorMessage = "Vị trí không được để trống")]
        [Display(Name = "Vị trí")]
        public string Position { get; set; }

        public int CreateBy { get; set; }

        public DateTime? CreateAt { get; set; }

        public int UpdateBy { get; set; }

        public DateTime? UpdateAt { get; set; }
        [Required(ErrorMessage = "Trạng thái không được để trống")]
        [Display(Name = "Trạng thái")]
        public int Status { get; set; }


    }
}
