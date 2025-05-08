using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qa_dotnet_cucumber.Models
{// Model for the JSON data
    public class TestData
    {
        public List<EducationData> EducationData { get; set; }
    }

    public class EducationData
    {
        public string University { get; set; }
        public string Country { get; set; }
        public string Title { get; set; }
        public string Degree { get; set; }
        public string Year { get; set; }
    }
}