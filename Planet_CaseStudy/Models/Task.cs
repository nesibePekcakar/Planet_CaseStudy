using System;
using System.ComponentModel.DataAnnotations;

namespace Planet_CaseStudy.Models
{
    public enum TaskStatus
    {
        [Display(Name = "Bekliyor")]
        Bekliyor,

        [Display(Name = "Devam Ediyor")]
        DevamEdiyor,

        [Display(Name = "Tamamlandı")]
        Tamamlandı
    }

    public class Task
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Görev adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Görev adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Durum")]
        public TaskStatus Status { get; set; }

        [Display(Name = "Son Tarih")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }
    }
}