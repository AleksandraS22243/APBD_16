namespace App.Models;
using System.ComponentModel.DataAnnotations;

public class Animal
{
    public int IdAnimal { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
    public string Name { get; set; } 

    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
    public string Description { get; set; } 

    [Required]
    [StringLength(50, ErrorMessage = "Category cannot be longer than 50 characters.")]
    public string Category { get; set; } 

    [Required]
    [StringLength(100, ErrorMessage = "Area cannot be longer than 100 characters.")]
    public string Area { get; set; } 
}