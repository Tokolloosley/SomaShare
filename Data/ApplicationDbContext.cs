using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SomaShare.Models;
using System.Reflection.Emit;

namespace SomaShare.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Textbook> Textbooks { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<WantedAd> WantedAds { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Textbook relationships
            builder.Entity<Textbook>()
                .HasOne(t => t.Seller)
                .WithMany(u => u.TextbooksListed)
                .HasForeignKey(t => t.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Offer relationships
            builder.Entity<Offer>()
                .HasOne(o => o.Textbook)
                .WithMany(t => t.Offers)
                .HasForeignKey(o => o.TextbookId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Offer>()
                .HasOne(o => o.Buyer)
                .WithMany(u => u.OffersPlaced)
                .HasForeignKey(o => o.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Offer>()
                .HasOne(o => o.Seller)
                .WithMany(u => u.OffersReceived)
                .HasForeignKey(o => o.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Transaction relationships
            builder.Entity<Transaction>()
                .HasOne(t => t.Offer)
                .WithOne(o => o.Transaction)
                .HasForeignKey<Transaction>(t => t.OfferId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Transaction>()
                .HasOne(t => t.Buyer)
                .WithMany(u => u.PurchaseTransactions)
                .HasForeignKey(t => t.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Transaction>()
                .HasOne(t => t.Seller)
                .WithMany(u => u.SaleTransactions)
                .HasForeignKey(t => t.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Review relationships
            builder.Entity<Review>()
                .HasOne(r => r.Transaction)
                .WithOne(t => t.Review)
                .HasForeignKey<Review>(r => r.TransactionId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Review>()
                .HasOne(r => r.ReviewedUser)
                .WithMany(u => u.ReviewsReceived)
                .HasForeignKey(r => r.ReviewedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review>()
                .HasOne(r => r.ReviewerUser)
                .WithMany(u => u.ReviewsGiven)
                .HasForeignKey(r => r.ReviewerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure WantedAd relationships
            builder.Entity<WantedAd>()
                .HasOne(w => w.User)
                .WithMany(u => u.WantedAds)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}