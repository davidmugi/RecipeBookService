using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipeBookService.DTOs;
using RecipeBookService.Entities;

namespace RecipeBookService.Configurations;

public class RecipeBookDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public RecipeBookDbContext(DbContextOptions<RecipeBookDbContext> options)
        : base(options)
    {
    }

    public DbSet<Recipe> Recipes { get; set; }

    public DbSet<Ingredient> Ingredients { get; set; }

    public DbSet<TopRecipeDTO> TopRecipe { get; set; }

    public DbSet<MostUsedIngredientDTO> MostUsedIngredient { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Recipe>()
            .HasMany(r => r.Ingredients)
            .WithMany(i => i.Recipes);

        builder.Entity<TopRecipeDTO>().HasNoKey().ToView(null);

        builder.Entity<MostUsedIngredientDTO>().HasNoKey().ToView(null);

        base.OnModelCreating(builder);
    }
}