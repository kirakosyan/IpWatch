using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LocalDbRepo.Migrations
{
	[DbContext(typeof(ListRepoContext))]
	[Migration("20200220_InitialCreate")]
	public class _20200220 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"
CREATE TABLE [dbo].[WatchEntities](
	[WatchId] [uniqueidentifier] NOT NULL,
	[Host] [nvarchar](max) NULL,
	[PingIntervalSeconds] [int] NOT NULL,
	[Emails] [nvarchar](max) NULL,
	[Note] [nvarchar](max) NULL,
	[IsOnline] [bit] NOT NULL,
	[TimeSinceLastStatusChange] [datetime2](7) NOT NULL,
	[IsEnabled] [bit] NOT NULL,
 CONSTRAINT [PK_WatchEntities] PRIMARY KEY CLUSTERED 
(
	[WatchId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
");
		}
	}
}
