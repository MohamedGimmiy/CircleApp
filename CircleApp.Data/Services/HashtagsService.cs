using CircleApp.Data.Helpers;
using CircleApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleApp.Data.Services
{
    public class HashtagsService : IHashtagsService
    {
        private readonly AppDbContext _context;

        public HashtagsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task ProcessHashtagsForNewPostAsync(string content)
        {
            var postHashtags = HashtagHelper.GetHashtags(content);
            foreach (var hashtag in postHashtags)
            {
                var hashtagDb = await _context.Hashtags.FirstOrDefaultAsync(n => n.Name == hashtag);
                if (hashtagDb != null)
                {
                    hashtagDb.Count++;
                    hashtagDb.DateUpdated = DateTime.UtcNow;
                    _context.Hashtags.Update(hashtagDb);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var newHashtag = new HashTag()
                    {
                        Name = hashtag,
                        Count = 1,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow,
                    };

                    await _context.Hashtags.AddAsync(newHashtag);
                    await _context.SaveChangesAsync();
                }

            }
        }

        public async Task ProcessHashtagsForRemovePostAsync(string content)
        {
            var postHashtags = HashtagHelper.GetHashtags(content);
            foreach (var hashtag in postHashtags)
            {
                var hashTagDb = await _context
                    .Hashtags
                    .FirstOrDefaultAsync(h => h.Name == hashtag);

                if (hashTagDb != null)
                {
                    hashTagDb.Count--;
                    hashTagDb.DateUpdated = DateTime.UtcNow;
                    _context.Hashtags.Update(hashTagDb);
                    await _context.SaveChangesAsync();
                }

            }
        }
    }
    }
