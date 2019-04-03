USE [TransferDB]
GO

/****** Object:  Table [dbo].[tblSchedule]    Script Date: 2019/4/3 下午 05:00:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblSchedule](
	[ScheduleName] [nvarchar](50) NOT NULL,
	[ModeType] [varchar](10) NOT NULL,
	[Format] [varchar](10) NOT NULL,
	[CustomerName] [nvarchar](200) NOT NULL,
	[SettingName] [nvarchar](100) NOT NULL,
	[Destination] [varchar](10) NULL,
	[Path] [nvarchar](max) NULL,
	[Email] [varchar](max) NULL,
	[FTPServer] [nvarchar](max) NULL,
	[FTPAccount] [nvarchar](255) NULL,
	[FTPPassword] [nvarchar](255) NULL,
	[WorkType] [varchar](1) NOT NULL,
	[Month] [varchar](255) NULL,
	[Date] [varchar](255) NULL,
	[Day] [varchar](255) NULL,
	[Hour] [varchar](255) NULL,
	[Min] [varchar](255) NULL,
	[Active] [bit] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[Creator] [nvarchar](50) NULL,
	[UpdateTime] [datetime] NULL,
	[Updator] [nvarchar](50) NULL,
 CONSTRAINT [PK_Schedule] PRIMARY KEY CLUSTERED 
(
	[ScheduleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'轉入/轉出(IMPORT/EXPORT)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tblSchedule', @level2type=N'COLUMN',@level2name=N'ModeType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'XML/EXCEL' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tblSchedule', @level2type=N'COLUMN',@level2name=N'Format'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'XML/EXCEL Name' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tblSchedule', @level2type=N'COLUMN',@level2name=N'SettingName'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1: Path 2: Email 3: FTP' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tblSchedule', @level2type=N'COLUMN',@level2name=N'Destination'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1: DATE 2: DAY' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tblSchedule', @level2type=N'COLUMN',@level2name=N'WorkType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'月' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tblSchedule', @level2type=N'COLUMN',@level2name=N'Month'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'日' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tblSchedule', @level2type=N'COLUMN',@level2name=N'Date'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'星期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tblSchedule', @level2type=N'COLUMN',@level2name=N'Day'
GO


