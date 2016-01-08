using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SourceMax.DocumentDB {

    public static class Extensions {

        public static Task<List<T>> ToListAsync<T>(this IQueryable<T> query) {
            return Task.Run(() => query.ToList());
        }
    }
}