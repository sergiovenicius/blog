using blog.common.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace blog.common.Database
{
    public class DBContextBlog : DbContext
    {
        private readonly IMediator _mediator;

        public DBContextBlog(DbContextOptions<DBContextBlog> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<PostDB> Posts { get; set; }
        public virtual DbSet<CommentDB> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
            .HasKey(p => new { p.ID });

            modelBuilder.Entity<User>().Property(b => b.ID)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<User>().Property(b => b.Username)
                .IsRequired();
            modelBuilder.Entity<User>().Property(b => b.Name)
                .IsRequired();
            modelBuilder.Entity<User>().Property(b => b.Email)
                .IsRequired();
            modelBuilder.Entity<User>().Property(b => b.Password)
                .IsRequired();

            modelBuilder.Entity<User>().Property(e => e.Role)
                .HasConversion(
                  v => string.Join(",", v.Select(e => e.ToString("D")).ToArray()),
                  v => v.Split(new[] { ',' })
                    .Select(e => Enum.Parse(typeof(UserRole), e))
                    .Cast<UserRole>()
                    .ToList());

            modelBuilder.Entity<PostDB>()
            .HasKey(p => new { p.ID });

            modelBuilder.Entity<PostDB>().Property(b => b.ID)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<PostDB>()
                .Property(b => b.DatePublished)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<PostDB>().Property(b => b.Content)
                .IsRequired();
            modelBuilder.Entity<PostDB>().Property(b => b.Title)
                .IsRequired();
            modelBuilder.Entity<PostDB>().Property(b => b.OwnerId)
                .IsRequired();

            modelBuilder.Entity<PostDB>()
                .HasMany(e => e.Comments)
                .WithOne()
                .HasForeignKey(e => e.PostId)
                .HasPrincipalKey(e => e.ID);

            modelBuilder.Entity<PostDB>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.OwnerId)
                .IsRequired();


            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _mediator.DispatchDomainEvents(this);

            var entities = from e in ChangeTracker.Entries()
                           where e.State == EntityState.Added
                               || e.State == EntityState.Modified
                           select e.Entity;
            foreach (var entity in entities)
            {
                var validationContext = new ValidationContext(entity);
                Validator.ValidateObject(
                    entity,
                    validationContext,
                    validateAllProperties: true);
            }

            return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
}
    }
}
