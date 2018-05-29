using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryServices
{
    public class PatronService : IPatron
    {
        private LibraryContext _context;

        public PatronService(LibraryContext context)
        {
            _context = context;
        }

        public Patron GetById(int patronId)
        {
            return _context.Patrons
                .Include(p=>p.LibraryCard)
                .Include(p=>p.HomeLibraryBranch)
                .FirstOrDefault(p => p.Id == patronId);
        }

        public IEnumerable<Patron> GetAll()
        {
            return _context.Patrons
                .Include(p => p.LibraryCard)
                .Include(p => p.HomeLibraryBranch);
        }

        public void Add(Patron newPatron)
        {
            _context.Add(newPatron);
            _context.SaveChanges();
        }

        public IEnumerable<CheckoutsHistory> GetCheckoutHistory(int patronId)
        {
            var cardId = GetById(patronId)
                .LibraryCard.Id;
            return _context.CheckoutsHistories
                .Include(ch=>ch.LibraryCard)
                .Include(ch=>ch.LibraryAsset)
                .Where(ch => ch.LibraryCard.Id == cardId)
                .OrderByDescending(ch=>ch.CheckedOut);
        }

        public IEnumerable<Hold> GetHolds(int patronId)
        {
            var cardId = GetById(patronId)
                .LibraryCard.Id;
            return _context.Holds
                .Include(h => h.LibraryCard)
                .Include(h=>h.LibraryAsset)
                .Where(h => h.LibraryCard.Id == cardId)
                .OrderByDescending(h => h.HoldPlaced);
        }

        public IEnumerable<Checkout> GetCheckouts(int patronId)
        {
            var cardId = GetById(patronId)
                .LibraryCard.Id;
            return _context.Checkouts
                .Include(ch => ch.LibraryCard)
                .Include(ch=>ch.LibraryAsset)
                .Where(ch => ch.LibraryCard.Id == cardId);
        }
    }
}
