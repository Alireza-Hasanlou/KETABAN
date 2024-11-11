using KETEBAN.DATA.Entities.BookEN;
using KETEBAN.DATA.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace KETEBAN.DATA.Context
{
    public class _KetabanContext : IdentityDbContext<Librarian>
    {
        public _KetabanContext(DbContextOptions<_KetabanContext> options) : base(options)
        {

        }


        public DbSet<Student> Students { get; set; }
        public DbSet<StudentsLevels> StudentsLevels { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<BookDetails> BookDetails { get; set; }
        public DbSet<BookLanguage> languages { get; set; }
        public DbSet<Loan> loans { get; set; }

        protected override void OnModelCreating(ModelBuilder Builder)
        {

            base.OnModelCreating(Builder);


            Builder.Entity<Librarian>()
                   .ToTable("Librarians");

            Builder.Entity<StudentsLevels>()
                   .HasKey(k => k.StudentLevelId);

            Builder.Entity<Student>()
                   .HasKey(k => k.StudentNum);

            Builder.Entity<Student>()
                   .HasOne(s => s.StudentsLevel)
                   .WithMany(s => s.Students)
                   .HasForeignKey(f => f.StudentLevelId);

            Builder.Entity<Student>()
                   .HasMany(l => l.Loans)
                   .WithOne(s => s.Student)
                   .HasForeignKey(f=>f.StudentNumber);

           






            #region Book 
            Builder.Entity<BookDetails>()
                   .HasKey(k => k.BookId);

            Builder.Entity<Book>()
                   .HasKey(k => k.Id);

            Builder.Entity<Book>()
                .HasMany(b => b.Categories)
                .WithMany(c => c.books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookCategory", // نام جدول واسط که به صورت خودکار ایجاد می‌شود
                    j => j
                        .HasOne<Categories>()
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade), // تنظیم حذف Cascade برای Category
                    j => j
                        .HasOne<Book>()
                        .WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade) // تنظیم حذف Cascade برای Book
                );
        
        Builder.Entity<Book>()
                   .HasOne(a => a.bookDetails)
                   .WithOne(b => b.Book)
                   .HasForeignKey<BookDetails>(f => f.BookId)
                   .OnDelete(DeleteBehavior.Cascade); ;

            Builder.Entity<Book>()
                   .HasOne(l => l.language)
                   .WithMany(b => b.Book)
                   .OnDelete(DeleteBehavior.Cascade);

            Builder.Entity<Book>()
                   .HasMany(l => l.Loans)
                   .WithOne(b=>b.Book)
                   .HasForeignKey(f => f.BookId);

            Builder.Entity<Loan>()
                   .HasKey(k => k.LoanId);

            #endregion



        }
    }


}
