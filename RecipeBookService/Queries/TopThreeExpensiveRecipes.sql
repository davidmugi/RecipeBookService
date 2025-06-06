SELECT TOP 3
    r.Id AS RecipeId, r.Title AS RecipeName,
       SUM(i.Price)            AS TotalCost,
       COUNT(ri.IngredientsId) AS IngredientCount
FROM Recipes r
         JOIN IngredientRecipe ri ON r.Id = ri.RecipesId
         JOIN Ingredients i ON ri.IngredientsId = i.Id
GROUP BY r.Id, r.Title
ORDER BY TotalCost DESC;