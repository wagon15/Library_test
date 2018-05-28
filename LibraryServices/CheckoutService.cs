using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryServices
{
    public class CheckoutService : ICheckout
    {
        private LibraryContext _context;
        public CheckoutService(LibraryContext context)
        {
            _context = context;
        }
        public IEnumerable<Checkout> GetAll()
        {
            return _context.Checkouts;
        }

        public Checkout GetById(int checkoutId)
        {
            return GetAll()
                .FirstOrDefault(checkout => checkout.Id == checkoutId);
        }

        public void Add(Checkout newCheckout)
        {
            _context.Add(newCheckout);
            _context.SaveChanges();
        }

        public void CheckOutItem(int assetId, int libraryCardId)
        {
            if (IsCheckedOut(assetId))
            {
                return;
                ;
            }

            var item = _context.LibraryAssets
                .FirstOrDefault(a => a.Id == assetId);

            UpdateAssetStatus(assetId,"Checked Out");

            var libraryCard = _context.LibraryCards
                .Include(card=>card.Checkouts)
                .FirstOrDefault(lc => lc.Id == libraryCardId);

            var now = DateTime.Now;

            var checkout = new Checkout
            {
                LibraryAsset = item,
                LibraryCard = libraryCard,
                Since = now,
                Until = GetDefaultCheckoutTime(now)
            };

            _context.Add(checkout);

            var checkoutHistory = new CheckoutsHistory
            {
                CheckedOut = now,
                LibraryAsset = item,
                LibraryCard = libraryCard
            };

            _context.Add(checkoutHistory);

            _context.SaveChanges();
        }

        public bool IsCheckedOut(int assetId)
        {
            return _context.Checkouts
                .Where(co => co.Id == assetId)
                .Any();
        }

        private DateTime GetDefaultCheckoutTime(DateTime now)
        {
            return now.AddDays(30);
        }


        public void CheckInItem(int assetId)
        {
            var now = DateTime.Now;

            var item = _context.LibraryAssets.FirstOrDefault(a => a.Id == assetId);

          

            RemoveExistingCheckouts(assetId);
            CloseExistingCheckoutHistory(assetId, now);

            var currentHolds = _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .Where(h => h.LibraryAsset.Id == assetId);

            if (currentHolds.Any())
            {
                CheckoutToEarliestHold(assetId, currentHolds);
            }
            else
            {
                UpdateAssetStatus(assetId, "Available");
            }

            _context.SaveChanges();
        }

        private void CheckoutToEarliestHold(int assetId, IQueryable<Hold> currentHolds)
        {
            var earliestHold = currentHolds
                .OrderBy(holds => holds.HoldPlaced)
                .FirstOrDefault();
            var card = earliestHold.LibraryCard;

            _context.Remove(earliestHold);
            _context.SaveChanges();
            CheckOutItem(assetId, card.Id);
        }

        public IEnumerable<CheckoutsHistory> GetCheckoutsHistory(int id)
        {
            return _context.CheckoutsHistories
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .Where(h => h.LibraryAsset.Id == id);
        }

        public Checkout GetLastestCheckout(int assetId)
        {
            return _context.Checkouts
                .Where(c => c.LibraryAsset.Id == assetId)
                .OrderByDescending(c => c.Since)
                .FirstOrDefault();
        }

        public string GetCurrentCheckoutPatron(int assetId)
        {
            var checkout = GetCheckoutByAssetId(assetId);
            if (checkout == null)
            {
                return "";
            }

            var cardId = checkout.LibraryCard.Id;

            var patron = _context.Patrons
                .Include(p => p.LibraryCard)
                .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return patron.FirstName + " " + patron.LastName;
        }

        private Checkout GetCheckoutByAssetId(int assetId)
        {
           return _context.Checkouts
                .Include(c => c.LibraryAsset)
                .Include(c => c.LibraryCard)
                .FirstOrDefault(c => c.LibraryAsset.Id == assetId);

        }

        public void PlaceHold(int asetId, int libraryCardId)
        {
            var now = DateTime.Now;

            var asset = _context.LibraryAssets
                .Include(a=>a.Status)
                .FirstOrDefault(a => a.Id == asetId);

            var card = _context.LibraryCards
                .FirstOrDefault(c => c.Id == libraryCardId);

            if (asset.Status.Name == "Available")
            {
                UpdateAssetStatus(asetId,"On Hold");
            }

            var hold = new Hold
            {
                HoldPlaced = now,
                LibraryAsset = asset,
                LibraryCard = card
            };

            _context.Add(hold);
            _context.SaveChanges();
        }

        public string GetCurrentHoldParonName(int HoldId)
        {
            var hold = _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .FirstOrDefault(h => h.Id == HoldId);

            var cardId = hold?.LibraryCard.Id;

            var patron = _context.Patrons
                .Include(p => p.LibraryCard)
                .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return patron?.FirstName + " " + patron?.LastName;
        }

        public DateTime GetCurrentHoldPlaced(int HoldId)
        {
            return _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .FirstOrDefault(h => h.Id == HoldId)
                .HoldPlaced;
        }

        public IEnumerable<Hold> GetCurrentHolds(int id)
        {
            return _context.Holds
                .Include(h => h.LibraryAsset)
                .Where(h => h.LibraryAsset.Id == id);
        }

        public void MarkLost(int id)
        {
            UpdateAssetStatus(id, "Lost");

            _context.SaveChanges();
        }

        public void MarkFound(int id)
        {
            var now = DateTime.Now;

            UpdateAssetStatus(id, "Available");
            //remove any existing checkouts
            RemoveExistingCheckouts(id);

            // close any existing checkout history
            CloseExistingCheckoutHistory(id, now);
            
            _context.SaveChanges();
        }

        private void UpdateAssetStatus(int id, string statusName)
        {
            var item = _context.LibraryAssets
                .FirstOrDefault(a => a.Id == id);

            _context.Update(item);

            item.Status = _context.Statuses
                .FirstOrDefault(status => status.Name == statusName);
            
        }

        private void CloseExistingCheckoutHistory(int id, DateTime now)
        {
            var history =
                _context.CheckoutsHistories
                    .FirstOrDefault(ch => ch.LibraryAsset.Id == id
                                          && ch.CheckedIn == null);
            if (history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;
            }
        }

        private void RemoveExistingCheckouts(int id)
        {
            var checkout = _context.Checkouts
                .FirstOrDefault(co => co.LibraryAsset.Id == id);

            if (checkout != null)
            {
                _context.Remove(checkout);
            }
        }
    }
}
