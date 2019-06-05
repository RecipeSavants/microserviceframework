
using System;
using System.Collections.Generic;
using System.Text;

namespace RecipeSavants.Microservices.GraphRepository.Models
{
    public class UserVertex
    {
        public string id {get;set;}
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdateDate { get; set; }

        public List<UserVertex> Follows { get; set; }
        public List<UserVertex> Block { get; set; }

        public string UserName { get; set; }
        public string FullName { get; set; }
        public string PostalCode { get; set; }
        public string Cusines { get; set; }
        public string Allergies { get; set; }
        public string Diets { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public DateTime LastActivityDate { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string AboutMe { get; set; }
        public string PhotoPath { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Pintrest { get; set; }
        public string Instagram { get; set; }
        public bool IsFacebookLinkVisible { get; set; }
        public bool IsTwitterLinkVisible { get; set; }
        public bool IsPinterestLinkVisible { get; set; }
        public bool IsInstagramVisible { get; set; }
        public string BackgroundUrl { get; set; }

    }
}
