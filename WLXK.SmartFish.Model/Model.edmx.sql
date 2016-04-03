
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 08/10/2015 08:45:03
-- Generated from EDMX file: H:\赛佰特\决赛资料\服务器源码\2.WLXK.SmartFish(wifi版) 加导出excel版本\WLXK.SmartFish.Model\Model.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [SmartFish];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[ReceiveDatas]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ReceiveDatas];
GO
IF OBJECT_ID(N'[dbo].[ControlData]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ControlData];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'ReceiveDatas'
CREATE TABLE [dbo].[ReceiveDatas] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EnviroTemperate] float  NULL,
    [EnviroHumidity] float  NULL,
    [SunValue] float  NULL,
    [FishTemperate] float  NULL,
    [Oxygen] float  NULL,
    [PhValues] float  NULL,
    [EnviroTemperate2] float  NOT NULL,
    [EnviroHumidity2] float  NOT NULL,
    [SunValue2] float  NOT NULL,
    [FishTemperate2] float  NOT NULL,
    [SubTime] datetime  NOT NULL
);
GO

-- Creating table 'ControlData'
CREATE TABLE [dbo].[ControlData] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [PhValues] float  NOT NULL,
    [Oxygen] float  NOT NULL,
    [MinTemperate] float  NOT NULL,
    [MaxTemperate] float  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'ReceiveDatas'
ALTER TABLE [dbo].[ReceiveDatas]
ADD CONSTRAINT [PK_ReceiveDatas]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ControlData'
ALTER TABLE [dbo].[ControlData]
ADD CONSTRAINT [PK_ControlData]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------