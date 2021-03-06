USE [TransferDB]
GO
/****** Object:  Table [dbo].[bscode]    Script Date: 2019/3/22 下午 05:57:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[bscode](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[cd] [varchar](20) NULL,
	[cd_descp] [varchar](300) NULL,
	[cd_type] [varchar](20) NULL,
	[created_by] [varchar](50) NULL,
	[updated_by] [varchar](50) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[value1] [varchar](200) NULL,
	[value2] [varchar](50) NULL,
	[value3] [varchar](50) NULL,
 CONSTRAINT [PK_bscode] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[bscode_kind]    Script Date: 2019/3/22 下午 05:57:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[bscode_kind](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[cd_type] [varchar](20) NULL,
	[cd_descp] [varchar](300) NULL,
	[created_by] [varchar](50) NULL,
	[updated_by] [varchar](50) NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
 CONSTRAINT [PK_bscode_kind] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tblSMTP]    Script Date: 2019/3/22 下午 05:57:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tblSMTP](
	[Email] [varchar](255) NOT NULL,
	[DisplayName] [nvarchar](50) NULL,
	[Password] [varchar](255) NOT NULL,
	[SMTP] [varchar](max) NOT NULL,
	[Port] [int] NOT NULL,
	[SSL] [bit] NOT NULL,
 CONSTRAINT [PK_SMTPSet] PRIMARY KEY CLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[bscode] ON 

INSERT [dbo].[bscode] ([id], [cd], [cd_descp], [cd_type], [created_by], [updated_by], [created_at], [updated_at], [value1], [value2], [value3]) VALUES (1, N'提單', NULL, N'SQL_TYPE', NULL, NULL, NULL, NULL, N'提單轉出', NULL, NULL)
INSERT [dbo].[bscode] ([id], [cd], [cd_descp], [cd_type], [created_by], [updated_by], [created_at], [updated_at], [value1], [value2], [value3]) VALUES (2, N'帳單', NULL, N'SQL_TYPE', NULL, NULL, NULL, NULL, N'帳單轉出', NULL, NULL)
INSERT [dbo].[bscode] ([id], [cd], [cd_descp], [cd_type], [created_by], [updated_by], [created_at], [updated_at], [value1], [value2], [value3]) VALUES (3, N'報單', NULL, N'SQL_TYPE', NULL, NULL, NULL, NULL, N'報單轉出', NULL, NULL)
SET IDENTITY_INSERT [dbo].[bscode] OFF
SET IDENTITY_INSERT [dbo].[bscode_kind] ON 

INSERT [dbo].[bscode_kind] ([id], [cd_type], [cd_descp], [created_by], [updated_by], [created_at], [updated_at]) VALUES (1, N'SQL_TYPE', NULL, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[bscode_kind] OFF
INSERT [dbo].[tblSMTP] ([Email], [DisplayName], [Password], [SMTP], [Port], [SSL]) VALUES (N'chris@standard-info.com', N'standard-info.com', N'pfCkp!&1y4JF', N'mail.standard-info.com', 25, 0)
ALTER TABLE [dbo].[bscode]  WITH CHECK ADD  CONSTRAINT [FK_bscode_bscode] FOREIGN KEY([id])
REFERENCES [dbo].[bscode] ([id])
GO
ALTER TABLE [dbo].[bscode] CHECK CONSTRAINT [FK_bscode_bscode]
GO
