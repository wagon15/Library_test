using LibraryData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryServices
{
    public class LibraryBranchService : ILibraryBranch
    {
        private LibraryContext _context;

        public LibraryBranchService(LibraryContext context)
        {
            _context = context;
        }

        public IEnumerable<LibraryBranch> GetAll()
        {
            return _context.LibraryBranches
                .Include(b=>b.Patrons)
                .Include(b=>b.LibraryAssets);
        }

        public IEnumerable<Patron> GetPatrons(int branchId)
        {
            return GetById(branchId).Patrons;
        }

        public IEnumerable<LibraryAsset> GetAssets(int branchId)
        {
            return GetById(branchId).LibraryAssets;
        }

        public IEnumerable<string> GetBranchHours(int branchId)
        {
            var hours = _context.BranchHours.Where(bh => bh.Branch.Id == branchId);
            return DataHelpers.HumanizeBizHours(hours);

        }

        public LibraryBranch GetById(int branchId)
        {
            return GetAll().FirstOrDefault(b => b.Id == branchId);
        }

        public void Add(LibraryBranch newBranch)
        {
            _context.Add(newBranch);
            _context.SaveChanges();
        }

        public bool IsBranchOpen(int branchId)
        {
            var currentTimeHour = DateTime.Now.Hour;
            var currentDayOfWeek = (int) DateTime.Now.DayOfWeek;
            var hours = _context.BranchHours.Where(h => h.Branch.Id == branchId);
            var dayHours = hours.FirstOrDefault(h => h.DayOfWeek == currentDayOfWeek);

            return currentTimeHour < dayHours.CloseTime && currentTimeHour > dayHours.OpenTime;
        }
    }
}

