CREATE DATABASE [MyFirstBiml]
GO

USE [MyFirstBiml]
GO
-- This table will store the names of all of the source tables that we would like to stage.
CREATE TABLE [MyFirstBiml].[dbo].[MyBimlMeta_Tables](
	[TableName] [nvarchar](50) NULL
) ON [PRIMARY]
GO

-- We will just add a couple of rows here. In this case, we will be staging all AdventureWorks tables whose names begin with 'Person'. Feel free to play around with the choice of source tables to stage!
USE AdventureWorks2014
TRUNCATE TABLE MyFirstBiml.dbo.MyBimlMeta_Tables
INSERT INTO MyFirstBiml.dbo.MyBimlMeta_Tables
SELECT Distinct Name from sysobjects 
WHERE name like 'Person%'
