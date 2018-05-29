using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryData;
using Library_test.Models.Branch;
using Microsoft.AspNetCore.Mvc;

namespace Library_test.Controllers
{
    public class BranchController : Controller
    {
        private ILibraryBranch _branch;
        public BranchController(ILibraryBranch branch)
        {
            _branch = branch;
        }

        public IActionResult Index()
        {
            var branches = _branch.GetAll().Select(b => new BranchDetailModel
            {
                Id = b.Id,
                Name = b.Name,
                IsOpen = _branch.IsBranchOpen(b.Id),
                NuberOFPatons = _branch.GetPatrons(b.Id).Count(),
                NumberOfAssets = _branch.GetAssets(b.Id).Count()
            });

            var model = new BranchIndexModel()
            {
                Branches = branches
            };

            return View(model);
        }

        public IActionResult Detail(int id)
        {
            var b = _branch.GetById(id);

            var model = new BranchDetailModel()
            {
                Id = b.Id,
                Name = b.Name,
                Address = b.Address,
                Telephone = b.Telephone,
                OpenDate = b.OpenDate.ToString("dd-MM-yyyy"),
                NumberOfAssets = _branch.GetAssets(b.Id).Count(),
                NuberOFPatons = _branch.GetPatrons(b.Id).Count(),
                TotalAssetValue = _branch.GetAssets(id).Sum(a=>a.Cost),
                ImageUrl = b.ImageUrl,
                HoursOpen = _branch.GetBranchHours(id)
            };

            return View(model);
        }
    }
}
