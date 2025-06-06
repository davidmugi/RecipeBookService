SELECT TOP 1
    i.Name AS IngredientName, COUNT(DISTINCT ri.RecipesId) AS RecipeCount
FROM IngredientRecipe ri
         INNER JOIN Ingredients i ON ri.IngredientsId = i.Id
GROUP BY i.Name
ORDER BY RecipeCount DESC;