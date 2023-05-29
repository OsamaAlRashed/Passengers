using System.ComponentModel.DataAnnotations;

namespace Passengers.SharedKernel.Enums;

public enum BloodType
{
    [Display(Name = "A+")]
    APostive = 1,
    [Display(Name = "B+")]
    BPostive,
    [Display(Name = "AB+")]
    ABPostive,
    [Display(Name = "O+")]
    OPostive,
    [Display(Name = "A-")]
    ANegative,
    [Display(Name = "B-")]
    BNegative,
    [Display(Name = "AB-")]
    ABNegative,
    [Display(Name = "O-")]
    ONegative,
}
