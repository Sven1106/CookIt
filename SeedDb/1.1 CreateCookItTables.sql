CREATE TABLE [Host] (
	Id UNIQUEIDENTIFIER,
	Name varchar(MAX) NOT NULL,
	Url varchar(MAX) NOT NULL,
	LogoUrl varchar(MAX)
)
GO
CREATE TABLE [Ingredient] (
	Id UNIQUEIDENTIFIER,
	Name varchar(MAX) NOT NULL
)
GO
CREATE TABLE [Recipe] (
	Id UNIQUEIDENTIFIER,
	Title varchar(MAX) NOT NULL,
	HostId UNIQUEIDENTIFIER NOT NULL,
	Url varchar(MAX) NOT NULL,
	ImageUrl varchar(MAX)
)
GO
CREATE TABLE [RecipeSentence] (
	Id UNIQUEIDENTIFIER,
	RecipeId UNIQUEIDENTIFIER NOT NULL,
	DerivedFrom varchar(MAX) NOT NULL
)
GO
CREATE TABLE [RecipeSentenceIngredient] (
  Id UNIQUEIDENTIFIER,
  RecipeSentenceId UNIQUEIDENTIFIER NOT NULL,
  IngredientId UNIQUEIDENTIFIER NOT NULL
)
GO

CREATE TABLE [User] (
  Id UNIQUEIDENTIFIER,
  Username varchar(MAX) NOT NULL,
  PasswordHash varbinary(MAX) NOT NULL,
  PasswordSalt varbinary(MAX) NOT NULL
)
GO
