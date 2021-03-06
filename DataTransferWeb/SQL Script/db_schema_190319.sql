USE [TransferDB]
GO
/****** Object:  Table [dbo].[tblAdmin]    Script Date: 2019/3/19 下午 06:41:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblAdmin](
	[PersonalID] [nvarchar](20) NOT NULL,
	[NameChi] [nvarchar](50) NULL,
	[Password] [nvarchar](255) NULL,
	[UseStatus] [bit] NULL,
 CONSTRAINT [PK_tblAdmin] PRIMARY KEY CLUSTERED 
(
	[PersonalID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblExcelMapping]    Script Date: 2019/3/19 下午 06:41:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblExcelMapping](
	[ExcelName] [nvarchar](100) NOT NULL,
	[ColumnName] [nvarchar](255) NOT NULL,
	[FieldName] [nvarchar](50) NULL,
	[DefaultValue] [nvarchar](max) NULL,
	[DataType] [nvarchar](255) NOT NULL,
	[SheetName] [nvarchar](255) NOT NULL,
	[X] [int] NOT NULL,
	[NewLineChar] [nvarchar](2) NULL,
 CONSTRAINT [PK_tblExcelMapping] PRIMARY KEY CLUSTERED 
(
	[ExcelName] ASC,
	[ColumnName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblExcelSetting]    Script Date: 2019/3/19 下午 06:41:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblExcelSetting](
	[ExcelName] [nvarchar](100) NOT NULL,
	[SQLName] [nvarchar](100) NOT NULL,
	[CustomerName] [nvarchar](200) NOT NULL,
	[FileName] [nvarchar](255) NULL,
	[FileNameDateFormat] [varchar](20) NULL,
	[UserId] [varchar](max) NULL,
	[CreateTime] [datetime] NOT NULL,
	[Creator] [nvarchar](50) NULL,
	[UpdateTime] [datetime] NULL,
	[Updator] [nvarchar](50) NULL,
 CONSTRAINT [PK_tbExcelSetting] PRIMARY KEY CLUSTERED 
(
	[ExcelName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblLog]    Script Date: 2019/3/19 下午 06:41:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblLog](
	[LogID] [bigint] IDENTITY(1,1) NOT NULL,
	[Type] [nvarchar](10) NOT NULL,
	[UserID] [nvarchar](20) NOT NULL,
	[CustomerName] [nvarchar](200) NOT NULL,
	[Format] [varchar](10) NOT NULL,
	[ExecuteTime] [datetime] NOT NULL,
	[Destination] [varchar](10) NOT NULL,
	[Path] [nvarchar](max) NULL,
	[FileName] [nvarchar](max) NOT NULL,
	[Status] [varchar](10) NOT NULL,
	[Message] [nvarchar](max) NULL,
 CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
(
	[LogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblSQLColumns]    Script Date: 2019/3/19 下午 06:41:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblSQLColumns](
	[SQLName] [nvarchar](100) NOT NULL,
	[ColumnName] [nvarchar](50) NOT NULL,
	[Idx] [int] NOT NULL,
 CONSTRAINT [PK_SQLColumns] PRIMARY KEY CLUSTERED 
(
	[SQLName] ASC,
	[ColumnName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblSQLSetting]    Script Date: 2019/3/19 下午 06:41:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblSQLSetting](
	[SQLName] [nvarchar](100) NOT NULL,
	[SQLStatement] [ntext] NULL,
	[DataRow] [int] NOT NULL,
	[SQLType] [nvarchar](50) NULL,
	[CreateTime] [datetime] NOT NULL,
	[Creator] [nvarchar](50) NULL,
	[UpdateTime] [datetime] NULL,
	[Updator] [nvarchar](50) NULL,
 CONSTRAINT [PK_SQLSetting] PRIMARY KEY CLUSTERED 
(
	[SQLName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblXMLMapping]    Script Date: 2019/3/19 下午 06:41:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblXMLMapping](
	[XMLName] [nvarchar](100) NOT NULL,
	[TagName] [nvarchar](255) NOT NULL,
	[FieldName] [nvarchar](50) NULL,
	[DefaultValue] [nvarchar](max) NULL,
	[FatherTag] [nvarchar](255) NOT NULL,
	[CanRepeat] [bit] NOT NULL,
	[Idx] [int] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[Creator] [nvarchar](50) NULL,
	[UpdateTime] [datetime] NULL,
	[Updator] [nvarchar](50) NULL,
 CONSTRAINT [PK_tblXMLMapping] PRIMARY KEY CLUSTERED 
(
	[XMLName] ASC,
	[TagName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblXMLSetting]    Script Date: 2019/3/19 下午 06:41:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblXMLSetting](
	[XMLName] [nvarchar](100) NOT NULL,
	[SQLName] [nvarchar](100) NOT NULL,
	[CustomerName] [nvarchar](200) NOT NULL,
	[FileName] [nvarchar](255) NULL,
	[FileNameDateFormat] [varchar](20) NULL,
	[UserId] [varchar](max) NULL,
	[CreateTime] [datetime] NOT NULL,
	[Creator] [nvarchar](50) NULL,
	[UpdateTime] [datetime] NULL,
	[Updator] [nvarchar](50) NULL,
 CONSTRAINT [PK_tblXMLSetting] PRIMARY KEY CLUSTERED 
(
	[XMLName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[tblExcelMapping]  WITH CHECK ADD  CONSTRAINT [FK_tblExcelMapping_tblExcelSetting] FOREIGN KEY([ExcelName])
REFERENCES [dbo].[tblExcelSetting] ([ExcelName])
GO
ALTER TABLE [dbo].[tblExcelMapping] CHECK CONSTRAINT [FK_tblExcelMapping_tblExcelSetting]
GO
ALTER TABLE [dbo].[tblExcelSetting]  WITH CHECK ADD  CONSTRAINT [FK_tblExcelSetting_tblSQLSetting] FOREIGN KEY([SQLName])
REFERENCES [dbo].[tblSQLSetting] ([SQLName])
GO
ALTER TABLE [dbo].[tblExcelSetting] CHECK CONSTRAINT [FK_tblExcelSetting_tblSQLSetting]
GO
ALTER TABLE [dbo].[tblSQLColumns]  WITH CHECK ADD  CONSTRAINT [FK_SQLColumns_SQLSetting] FOREIGN KEY([SQLName])
REFERENCES [dbo].[tblSQLSetting] ([SQLName])
GO
ALTER TABLE [dbo].[tblSQLColumns] CHECK CONSTRAINT [FK_SQLColumns_SQLSetting]
GO
ALTER TABLE [dbo].[tblSQLSetting]  WITH CHECK ADD  CONSTRAINT [FK_SQLSetting_SQLSetting] FOREIGN KEY([SQLName])
REFERENCES [dbo].[tblSQLSetting] ([SQLName])
GO
ALTER TABLE [dbo].[tblSQLSetting] CHECK CONSTRAINT [FK_SQLSetting_SQLSetting]
GO
ALTER TABLE [dbo].[tblXMLMapping]  WITH CHECK ADD  CONSTRAINT [FK_tblXMLMapping_tblXMLSetting] FOREIGN KEY([XMLName])
REFERENCES [dbo].[tblXMLSetting] ([XMLName])
GO
ALTER TABLE [dbo].[tblXMLMapping] CHECK CONSTRAINT [FK_tblXMLMapping_tblXMLSetting]
GO
ALTER TABLE [dbo].[tblXMLSetting]  WITH CHECK ADD  CONSTRAINT [FK_tblXMLSetting_tblSQLSetting] FOREIGN KEY([XMLName])
REFERENCES [dbo].[tblXMLSetting] ([XMLName])
GO
ALTER TABLE [dbo].[tblXMLSetting] CHECK CONSTRAINT [FK_tblXMLSetting_tblSQLSetting]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'欄位順序' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tblExcelMapping', @level2type=N'COLUMN',@level2name=N'X'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'記錄 FTP/Mail位址' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tblLog', @level2type=N'COLUMN',@level2name=N'Path'
GO
