using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace $rootnamespace$;

internal class $safeitemrootname$ : DbContext
{
    // TODO
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer();
}
