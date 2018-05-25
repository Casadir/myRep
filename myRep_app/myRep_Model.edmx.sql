
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 05/25/2018 21:50:04
-- Generated from EDMX file: E:\Praca_Inzynierska\myRep\myRep_app\myRep_Model.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [myRep_ODS];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_HCOHCP_HCO]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[HCOHCP] DROP CONSTRAINT [FK_HCOHCP_HCO];
GO
IF OBJECT_ID(N'[dbo].[FK_HCOHCP_HCP]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[HCOHCP] DROP CONSTRAINT [FK_HCOHCP_HCP];
GO
IF OBJECT_ID(N'[dbo].[FK_HCOAddress]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[HCOSet] DROP CONSTRAINT [FK_HCOAddress];
GO
IF OBJECT_ID(N'[dbo].[FK_HCPMeeting_HCP]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[HCPMeeting] DROP CONSTRAINT [FK_HCPMeeting_HCP];
GO
IF OBJECT_ID(N'[dbo].[FK_HCPMeeting_Meeting]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[HCPMeeting] DROP CONSTRAINT [FK_HCPMeeting_Meeting];
GO
IF OBJECT_ID(N'[dbo].[FK_MedicalEnquiryMeeting]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MedicalEnquirySet] DROP CONSTRAINT [FK_MedicalEnquiryMeeting];
GO
IF OBJECT_ID(N'[dbo].[FK_MeetingProduct]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MeetingSet] DROP CONSTRAINT [FK_MeetingProduct];
GO
IF OBJECT_ID(N'[dbo].[FK_AddressHCP]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[HCPSet] DROP CONSTRAINT [FK_AddressHCP];
GO
IF OBJECT_ID(N'[dbo].[FK_MeetingUser_Meeting]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MeetingUser] DROP CONSTRAINT [FK_MeetingUser_Meeting];
GO
IF OBJECT_ID(N'[dbo].[FK_MeetingUser_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MeetingUser] DROP CONSTRAINT [FK_MeetingUser_User];
GO
IF OBJECT_ID(N'[dbo].[FK__UserCrede__UserI__0C85DE4D]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserCredentialsSet] DROP CONSTRAINT [FK__UserCrede__UserI__0C85DE4D];
GO
IF OBJECT_ID(N'[dbo].[FK_UserUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserSet] DROP CONSTRAINT [FK_UserUser];
GO
IF OBJECT_ID(N'[dbo].[FK_ProductSample]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SampleSet] DROP CONSTRAINT [FK_ProductSample];
GO
IF OBJECT_ID(N'[dbo].[FK_UserSampleWarehouse]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SampleWarehouseSet] DROP CONSTRAINT [FK_UserSampleWarehouse];
GO
IF OBJECT_ID(N'[dbo].[FK_SampleSampleWarehouse]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SampleWarehouseSet] DROP CONSTRAINT [FK_SampleSampleWarehouse];
GO
IF OBJECT_ID(N'[dbo].[FK_SuggestionUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SuggestionSet] DROP CONSTRAINT [FK_SuggestionUser];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[HCOSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[HCOSet];
GO
IF OBJECT_ID(N'[dbo].[HCPSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[HCPSet];
GO
IF OBJECT_ID(N'[dbo].[AddressSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AddressSet];
GO
IF OBJECT_ID(N'[dbo].[MeetingSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MeetingSet];
GO
IF OBJECT_ID(N'[dbo].[MedicalEnquirySet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MedicalEnquirySet];
GO
IF OBJECT_ID(N'[dbo].[ProductSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProductSet];
GO
IF OBJECT_ID(N'[dbo].[UserSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserSet];
GO
IF OBJECT_ID(N'[dbo].[UserCredentialsSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserCredentialsSet];
GO
IF OBJECT_ID(N'[dbo].[SampleSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SampleSet];
GO
IF OBJECT_ID(N'[dbo].[SampleWarehouseSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SampleWarehouseSet];
GO
IF OBJECT_ID(N'[dbo].[SuggestionSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SuggestionSet];
GO
IF OBJECT_ID(N'[dbo].[HCOHCP]', 'U') IS NOT NULL
    DROP TABLE [dbo].[HCOHCP];
GO
IF OBJECT_ID(N'[dbo].[HCPMeeting]', 'U') IS NOT NULL
    DROP TABLE [dbo].[HCPMeeting];
GO
IF OBJECT_ID(N'[dbo].[MeetingUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MeetingUser];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'HCOSet'
CREATE TABLE [dbo].[HCOSet] (
    [hcoID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [PhoneNumber] int  NULL,
    [Email] nvarchar(max)  NULL,
    [Website] nvarchar(max)  NULL,
    [AddressID] int  NOT NULL,
    [Range] nvarchar(max)  NULL,
    [Level] int  NULL,
    [SpecialType] nvarchar(max)  NULL,
    [BedsAmount] nvarchar(max)  NULL,
    [EmployeesAmount] nvarchar(max)  NULL
);
GO

-- Creating table 'HCPSet'
CREATE TABLE [dbo].[HCPSet] (
    [hcpID] int IDENTITY(1,1) NOT NULL,
    [FirstName] nvarchar(max)  NOT NULL,
    [MiddleName] nvarchar(max)  NULL,
    [LastName] nvarchar(max)  NOT NULL,
    [Gender] nvarchar(max)  NOT NULL,
    [AcademicTitle] nvarchar(max)  NOT NULL,
    [Specialty] nvarchar(max)  NOT NULL,
    [Birthdate] datetime  NULL,
    [PhoneNumber] int  NULL,
    [Email] nvarchar(max)  NULL,
    [KOL] bit  NULL,
    [LanguageSpoken] nvarchar(max)  NULL,
    [AddressID] int  NOT NULL
);
GO

-- Creating table 'AddressSet'
CREATE TABLE [dbo].[AddressSet] (
    [addressID] int IDENTITY(1,1) NOT NULL,
    [Street] nvarchar(max)  NOT NULL,
    [City] nvarchar(max)  NOT NULL,
    [Territory] nvarchar(max)  NOT NULL,
    [Country] nvarchar(max)  NOT NULL,
    [ZipCode] int  NOT NULL
);
GO

-- Creating table 'MeetingSet'
CREATE TABLE [dbo].[MeetingSet] (
    [meetingID] int IDENTITY(1,1) NOT NULL,
    [HCPID] int  NOT NULL,
    [Date] datetime  NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [Topic] nvarchar(max)  NOT NULL,
    [ProductID] int  NOT NULL,
    [NextMtgNote] nvarchar(max)  NULL,
    [MedicalEnquiryID] int  NULL,
    [SampleDrop] int  NULL,
    [SampleDropQty] int  NULL
);
GO

-- Creating table 'MedicalEnquirySet'
CREATE TABLE [dbo].[MedicalEnquirySet] (
    [meID] int IDENTITY(1,1) NOT NULL,
    [Question] nvarchar(max)  NOT NULL,
    [Answer] nvarchar(max)  NOT NULL,
    [ExpectedAnswerDate] datetime  NOT NULL,
    [MeetingID] int  NOT NULL
);
GO

-- Creating table 'ProductSet'
CREATE TABLE [dbo].[ProductSet] (
    [productID] int IDENTITY(1,1) NOT NULL,
    [ProductName] nvarchar(max)  NOT NULL,
    [AntiDisease] nvarchar(max)  NULL,
    [Manufacturer] nvarchar(max)  NULL,
    [MainIngredient] nvarchar(max)  NULL
);
GO

-- Creating table 'UserSet'
CREATE TABLE [dbo].[UserSet] (
    [userID] int IDENTITY(1,1) NOT NULL,
    [FirstName] nvarchar(max)  NOT NULL,
    [MiddleName] nvarchar(max)  NULL,
    [LastName] nvarchar(max)  NOT NULL,
    [Email] nvarchar(max)  NOT NULL,
    [JobTitle] nvarchar(max)  NOT NULL,
    [PhoneNumber] int  NULL,
    [HireDate] datetime  NULL,
    [ManagerID] int  NULL,
    [Territory] nvarchar(max)  NULL,
    [Username] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'UserCredentialsSet'
CREATE TABLE [dbo].[UserCredentialsSet] (
    [UCID] int IDENTITY(1,1) NOT NULL,
    [UserID] int  NOT NULL,
    [PWD] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'SampleSet'
CREATE TABLE [dbo].[SampleSet] (
    [sampleID] int IDENTITY(1,1) NOT NULL,
    [SampleName] nvarchar(max)  NOT NULL,
    [ProductID] int  NOT NULL,
    [QtyPerBox] int  NOT NULL,
    [Value] int  NOT NULL
);
GO

-- Creating table 'SampleWarehouseSet'
CREATE TABLE [dbo].[SampleWarehouseSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserID] int  NOT NULL,
    [SampleID] int  NOT NULL,
    [Qty] int  NOT NULL
);
GO

-- Creating table 'SuggestionSet'
CREATE TABLE [dbo].[SuggestionSet] (
    [suggestionID] int IDENTITY(1,1) NOT NULL,
    [UserID] int  NOT NULL,
    [HCPID] int  NULL,
    [HCOID] int  NULL,
    [Message] nvarchar(max)  NOT NULL,
    [User_userID] int  NOT NULL
);
GO

-- Creating table 'HCOHCP'
CREATE TABLE [dbo].[HCOHCP] (
    [HCO_hcoID] int  NOT NULL,
    [HCP_hcpID] int  NOT NULL
);
GO

-- Creating table 'HCPMeeting'
CREATE TABLE [dbo].[HCPMeeting] (
    [HCP_hcpID] int  NOT NULL,
    [Meeting_meetingID] int  NOT NULL
);
GO

-- Creating table 'MeetingUser'
CREATE TABLE [dbo].[MeetingUser] (
    [Meeting_meetingID] int  NOT NULL,
    [User_userID] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [hcoID] in table 'HCOSet'
ALTER TABLE [dbo].[HCOSet]
ADD CONSTRAINT [PK_HCOSet]
    PRIMARY KEY CLUSTERED ([hcoID] ASC);
GO

-- Creating primary key on [hcpID] in table 'HCPSet'
ALTER TABLE [dbo].[HCPSet]
ADD CONSTRAINT [PK_HCPSet]
    PRIMARY KEY CLUSTERED ([hcpID] ASC);
GO

-- Creating primary key on [addressID] in table 'AddressSet'
ALTER TABLE [dbo].[AddressSet]
ADD CONSTRAINT [PK_AddressSet]
    PRIMARY KEY CLUSTERED ([addressID] ASC);
GO

-- Creating primary key on [meetingID] in table 'MeetingSet'
ALTER TABLE [dbo].[MeetingSet]
ADD CONSTRAINT [PK_MeetingSet]
    PRIMARY KEY CLUSTERED ([meetingID] ASC);
GO

-- Creating primary key on [meID] in table 'MedicalEnquirySet'
ALTER TABLE [dbo].[MedicalEnquirySet]
ADD CONSTRAINT [PK_MedicalEnquirySet]
    PRIMARY KEY CLUSTERED ([meID] ASC);
GO

-- Creating primary key on [productID] in table 'ProductSet'
ALTER TABLE [dbo].[ProductSet]
ADD CONSTRAINT [PK_ProductSet]
    PRIMARY KEY CLUSTERED ([productID] ASC);
GO

-- Creating primary key on [userID] in table 'UserSet'
ALTER TABLE [dbo].[UserSet]
ADD CONSTRAINT [PK_UserSet]
    PRIMARY KEY CLUSTERED ([userID] ASC);
GO

-- Creating primary key on [UCID] in table 'UserCredentialsSet'
ALTER TABLE [dbo].[UserCredentialsSet]
ADD CONSTRAINT [PK_UserCredentialsSet]
    PRIMARY KEY CLUSTERED ([UCID] ASC);
GO

-- Creating primary key on [sampleID] in table 'SampleSet'
ALTER TABLE [dbo].[SampleSet]
ADD CONSTRAINT [PK_SampleSet]
    PRIMARY KEY CLUSTERED ([sampleID] ASC);
GO

-- Creating primary key on [Id] in table 'SampleWarehouseSet'
ALTER TABLE [dbo].[SampleWarehouseSet]
ADD CONSTRAINT [PK_SampleWarehouseSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [suggestionID] in table 'SuggestionSet'
ALTER TABLE [dbo].[SuggestionSet]
ADD CONSTRAINT [PK_SuggestionSet]
    PRIMARY KEY CLUSTERED ([suggestionID] ASC);
GO

-- Creating primary key on [HCO_hcoID], [HCP_hcpID] in table 'HCOHCP'
ALTER TABLE [dbo].[HCOHCP]
ADD CONSTRAINT [PK_HCOHCP]
    PRIMARY KEY CLUSTERED ([HCO_hcoID], [HCP_hcpID] ASC);
GO

-- Creating primary key on [HCP_hcpID], [Meeting_meetingID] in table 'HCPMeeting'
ALTER TABLE [dbo].[HCPMeeting]
ADD CONSTRAINT [PK_HCPMeeting]
    PRIMARY KEY CLUSTERED ([HCP_hcpID], [Meeting_meetingID] ASC);
GO

-- Creating primary key on [Meeting_meetingID], [User_userID] in table 'MeetingUser'
ALTER TABLE [dbo].[MeetingUser]
ADD CONSTRAINT [PK_MeetingUser]
    PRIMARY KEY CLUSTERED ([Meeting_meetingID], [User_userID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [HCO_hcoID] in table 'HCOHCP'
ALTER TABLE [dbo].[HCOHCP]
ADD CONSTRAINT [FK_HCOHCP_HCO]
    FOREIGN KEY ([HCO_hcoID])
    REFERENCES [dbo].[HCOSet]
        ([hcoID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [HCP_hcpID] in table 'HCOHCP'
ALTER TABLE [dbo].[HCOHCP]
ADD CONSTRAINT [FK_HCOHCP_HCP]
    FOREIGN KEY ([HCP_hcpID])
    REFERENCES [dbo].[HCPSet]
        ([hcpID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_HCOHCP_HCP'
CREATE INDEX [IX_FK_HCOHCP_HCP]
ON [dbo].[HCOHCP]
    ([HCP_hcpID]);
GO

-- Creating foreign key on [AddressID] in table 'HCOSet'
ALTER TABLE [dbo].[HCOSet]
ADD CONSTRAINT [FK_HCOAddress]
    FOREIGN KEY ([AddressID])
    REFERENCES [dbo].[AddressSet]
        ([addressID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_HCOAddress'
CREATE INDEX [IX_FK_HCOAddress]
ON [dbo].[HCOSet]
    ([AddressID]);
GO

-- Creating foreign key on [HCP_hcpID] in table 'HCPMeeting'
ALTER TABLE [dbo].[HCPMeeting]
ADD CONSTRAINT [FK_HCPMeeting_HCP]
    FOREIGN KEY ([HCP_hcpID])
    REFERENCES [dbo].[HCPSet]
        ([hcpID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Meeting_meetingID] in table 'HCPMeeting'
ALTER TABLE [dbo].[HCPMeeting]
ADD CONSTRAINT [FK_HCPMeeting_Meeting]
    FOREIGN KEY ([Meeting_meetingID])
    REFERENCES [dbo].[MeetingSet]
        ([meetingID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_HCPMeeting_Meeting'
CREATE INDEX [IX_FK_HCPMeeting_Meeting]
ON [dbo].[HCPMeeting]
    ([Meeting_meetingID]);
GO

-- Creating foreign key on [MeetingID] in table 'MedicalEnquirySet'
ALTER TABLE [dbo].[MedicalEnquirySet]
ADD CONSTRAINT [FK_MedicalEnquiryMeeting]
    FOREIGN KEY ([MeetingID])
    REFERENCES [dbo].[MeetingSet]
        ([meetingID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MedicalEnquiryMeeting'
CREATE INDEX [IX_FK_MedicalEnquiryMeeting]
ON [dbo].[MedicalEnquirySet]
    ([MeetingID]);
GO

-- Creating foreign key on [ProductID] in table 'MeetingSet'
ALTER TABLE [dbo].[MeetingSet]
ADD CONSTRAINT [FK_MeetingProduct]
    FOREIGN KEY ([ProductID])
    REFERENCES [dbo].[ProductSet]
        ([productID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MeetingProduct'
CREATE INDEX [IX_FK_MeetingProduct]
ON [dbo].[MeetingSet]
    ([ProductID]);
GO

-- Creating foreign key on [AddressID] in table 'HCPSet'
ALTER TABLE [dbo].[HCPSet]
ADD CONSTRAINT [FK_AddressHCP]
    FOREIGN KEY ([AddressID])
    REFERENCES [dbo].[AddressSet]
        ([addressID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AddressHCP'
CREATE INDEX [IX_FK_AddressHCP]
ON [dbo].[HCPSet]
    ([AddressID]);
GO

-- Creating foreign key on [Meeting_meetingID] in table 'MeetingUser'
ALTER TABLE [dbo].[MeetingUser]
ADD CONSTRAINT [FK_MeetingUser_Meeting]
    FOREIGN KEY ([Meeting_meetingID])
    REFERENCES [dbo].[MeetingSet]
        ([meetingID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [User_userID] in table 'MeetingUser'
ALTER TABLE [dbo].[MeetingUser]
ADD CONSTRAINT [FK_MeetingUser_User]
    FOREIGN KEY ([User_userID])
    REFERENCES [dbo].[UserSet]
        ([userID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MeetingUser_User'
CREATE INDEX [IX_FK_MeetingUser_User]
ON [dbo].[MeetingUser]
    ([User_userID]);
GO

-- Creating foreign key on [UserID] in table 'UserCredentialsSet'
ALTER TABLE [dbo].[UserCredentialsSet]
ADD CONSTRAINT [FK__UserCrede__UserI__0C85DE4D]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[UserSet]
        ([userID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK__UserCrede__UserI__0C85DE4D'
CREATE INDEX [IX_FK__UserCrede__UserI__0C85DE4D]
ON [dbo].[UserCredentialsSet]
    ([UserID]);
GO

-- Creating foreign key on [ManagerID] in table 'UserSet'
ALTER TABLE [dbo].[UserSet]
ADD CONSTRAINT [FK_UserUser]
    FOREIGN KEY ([ManagerID])
    REFERENCES [dbo].[UserSet]
        ([userID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserUser'
CREATE INDEX [IX_FK_UserUser]
ON [dbo].[UserSet]
    ([ManagerID]);
GO

-- Creating foreign key on [ProductID] in table 'SampleSet'
ALTER TABLE [dbo].[SampleSet]
ADD CONSTRAINT [FK_ProductSample]
    FOREIGN KEY ([ProductID])
    REFERENCES [dbo].[ProductSet]
        ([productID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductSample'
CREATE INDEX [IX_FK_ProductSample]
ON [dbo].[SampleSet]
    ([ProductID]);
GO

-- Creating foreign key on [UserID] in table 'SampleWarehouseSet'
ALTER TABLE [dbo].[SampleWarehouseSet]
ADD CONSTRAINT [FK_UserSampleWarehouse]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[UserSet]
        ([userID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserSampleWarehouse'
CREATE INDEX [IX_FK_UserSampleWarehouse]
ON [dbo].[SampleWarehouseSet]
    ([UserID]);
GO

-- Creating foreign key on [Qty] in table 'SampleWarehouseSet'
ALTER TABLE [dbo].[SampleWarehouseSet]
ADD CONSTRAINT [FK_SampleSampleWarehouse]
    FOREIGN KEY ([Qty])
    REFERENCES [dbo].[SampleSet]
        ([sampleID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SampleSampleWarehouse'
CREATE INDEX [IX_FK_SampleSampleWarehouse]
ON [dbo].[SampleWarehouseSet]
    ([Qty]);
GO

-- Creating foreign key on [User_userID] in table 'SuggestionSet'
ALTER TABLE [dbo].[SuggestionSet]
ADD CONSTRAINT [FK_SuggestionUser]
    FOREIGN KEY ([User_userID])
    REFERENCES [dbo].[UserSet]
        ([userID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SuggestionUser'
CREATE INDEX [IX_FK_SuggestionUser]
ON [dbo].[SuggestionSet]
    ([User_userID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------