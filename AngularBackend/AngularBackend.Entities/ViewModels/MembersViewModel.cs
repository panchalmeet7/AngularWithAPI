using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularBackend.Entities.ViewModels
{
    public class MembersViewModel
    {

        public string? firstName { get; set; }
        public string? lastName { get; set; }

        public string? emailId { get; set; }
        public int? phoneNo { get; set; }

        public int? weight { get; set; }
        public int? height { get; set; }

        public string? bmi { get; set; }
        public string? bmiResult { get; set; }

        public string? requireTrainer { get; set; }

        public string? gender { get; set; }

        public string[]? interestList { get; set; }

        public string? packageType { get; set; }

        public string? beenGym { get; set; }

        public string? enquiryDate { get; set; }

        public int? id { get; set; }



        //firstName: string = '';
        //lastName: string = '';
        //emailId: string = '';
        //phoneNo!: number;
        //weight!: number;
        //height!: number;
        //bmi!: number;
        //bmiResult!: number;
        //requireTrainer!: string;
        //gender!: string;
        //interestsList!: string[];
        //packageType!: string;
        //beenGym!: string;
        //enquiryDate!: string;
        //id!: number;


    }
}
