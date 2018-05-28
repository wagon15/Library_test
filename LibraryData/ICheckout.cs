using System;
using System.Collections.Generic;
using LibraryData.Models;

namespace LibraryData
{
    public interface ICheckout
    {
        IEnumerable<Checkout> GetAll();
        Checkout GetById(int checkoutId);
        void Add(Checkout newCheckout);
        void CheckOutItem(int assetId, int libraryCardId);
        void CheckInItem(int assetId);
        IEnumerable<CheckoutsHistory> GetCheckoutsHistory(int id);
        Checkout GetLastestCheckout(int assetId);
        string GetCurrentCheckoutPatron(int assetId);

        void PlaceHold(int asetId, int libraryCardId);
        string GetCurrentHoldParonName(int id);
        DateTime GetCurrentHoldPlaced(int id);
        IEnumerable<Hold> GetCurrentHolds(int id);
        bool IsCheckedOut(int id);

        void MarkLost(int id);
        void MarkFound(int id);
    }
}
