using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryData.Models;

namespace Library_test.Models.Patron
{
    public class PatronDetailModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName {
            get { return FirstName + " " + LastName; } }

        public int LibraryCardId { get; set; }
        public string Address { get; set; }
        public DateTime MemberSince { get; set; }
        public string Telephone { get; set; }
        public string HomeLibrary { get; set; }
        public decimal OverdueFees { get; set; }
        public IEnumerable<Checkout> AssetsCheckOut { get; set; }
        public IEnumerable<CheckoutsHistory> CheckoutsHistory { get; set; }
        public IEnumerable<Hold> Holds { get; set; }
    }
}
