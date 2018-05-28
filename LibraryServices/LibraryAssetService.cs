using System;
using System.Collections.Generic;
using System.Linq;
using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;


namespace LibraryServices
{
    public class LibraryAssetService : ILibraryAsset
    {
        private LibraryContext _Context;

        public LibraryAssetService(LibraryContext context)
        {
            _Context = context;
        }

        public IEnumerable<LibraryAsset> GetAll()
        {
            return _Context.LibraryAssets
                .Include(asset => asset.Status)
                .Include(asset => asset.Location);
        }

        public LibraryAsset GetById(int id)
        {
            return GetAll()
                .FirstOrDefault(asset => asset.Id == id);
        }

        public void Add(LibraryAsset newAsset)
        {
            _Context.Add(newAsset);
            _Context.SaveChanges();
        }

        public string GetDeweyIndex(int id)
        {
            if (_Context.Books.Any(book => book.Id == id))
            {
                return _Context.Books
                    .FirstOrDefault(book => book.Id == id).DeweyIndex;
            }
            else
                return "";
        }

        public string GetType(int id)
        {
            return _Context.LibraryAssets.Where(asset => asset.Id == id).OfType<Book>().Any() ? "Book" : "Video";
        }

        public string GetTitle(int id)
        {
            return _Context.LibraryAssets
                .FirstOrDefault(asset => asset.Id == id)
                .Title;
        }

        public string GetIsbn(int id)
        {
            if (_Context.Books.Any(asset => asset.Id == id))
            {
                return _Context.Books
                    .FirstOrDefault(book => book.Id == id).ISBN;
            }
            else
                return "";
        }

        public LibraryBranch GetCurrentLocation(int id)
        {
            return GetById(id).Location;
        }
        public string GetAuthorOrDirector(int id)
        {
            var isBook = _Context.LibraryAssets.OfType<Book>()
                .Where(asset => asset.Id == id).Any();
            var isVideo = _Context.LibraryAssets.OfType<Video>()
                .Where(asset => asset.Id == id).Any();

            return isBook
                ? _Context.Books.FirstOrDefault(book => book.Id == id).Author
                : _Context.Videos.FirstOrDefault(video => video.Id == id).Director
                ?? "Unknown";

        }
    }
}